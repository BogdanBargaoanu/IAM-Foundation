namespace TransactionsLibrary.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginatedList()
        {
            Items = new List<T>();
            PageIndex = 1;
            TotalPages = 0;
        }

        public PaginatedList(
            List<T> items,
            int count,
            int pageIndex,
            int pageSize)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
