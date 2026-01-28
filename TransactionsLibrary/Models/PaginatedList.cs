using TransactionsLibrary.Constants;

namespace TransactionsLibrary.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginatedList()
        {
            Items = new List<T>();
            TotalCount = 0;
            PageIndex = Pagination.DefaultPageIndex;
            TotalPages = Pagination.DefaultPageSize;
        }

        public PaginatedList(
            List<T> items,
            int count,
            int pageIndex,
            int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
