using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.Models
{
    /// <summary>
    /// Generic cursor-based pagination wrapper.
    /// SPs return PageSize + 1 rows — if we got the extra row, there's more data.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Data { get; }
        public long? NextCursor { get; }
        public bool HasMore { get; }
        public int PageSize { get; }

        private PagedResult(List<T> data, long? nextCursor, bool hasMore, int pageSize)
        {
            Data = data;
            NextCursor = nextCursor;
            HasMore = hasMore;
            PageSize = pageSize;
        }

        /// <summary>
        /// How this works:
        /// 
        /// The SP always fetches PageSize + 1 rows (e.g., 21 when you ask for 20).
        /// 
        ///   - If we get 21 back → there's more data → trim to 20, HasMore = true
        ///   - If we get 20 or less → that's everything → HasMore = false
        /// 
        /// This avoids running a separate COUNT(*) on millions of rows just to check
        /// if another page exists. The extra row is never returned to the client.
        /// </summary>
        public static PagedResult<T> Create(List<T> items, int pageSize, Func<T, long> idSelector)
        {
            // Did the SP return more rows than we asked for? If yes, more pages exist.
            var hasMore = items.Count > pageSize;

            // Trim the extra row — the client only sees PageSize items
            if (hasMore)
                items = items.Take(pageSize).ToList();

            // Grab the Id of the last item in the trimmed list.
            // The client sends this back as ?cursor=12345 to fetch the next page.
            // The SP then does WHERE Id > 12345 to continue from where we left off.
            var nextCursor = hasMore && items.Count > 0
                ? idSelector(items[^1])   // items[^1] = last item in the list
                : (long?)null;

            return new PagedResult<T>(items, nextCursor, hasMore, pageSize);
        }
    }
}