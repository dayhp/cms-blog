namespace CMSBlog.Core.Models
{
    public abstract class PageResultBase
    {
        public int CurrentPage { get; set; }
        public int PageCount
        {
            get
            {
                var pageCount = (double)RowCount / PageSize;
                return (int)Math.Ceiling(pageCount);
            }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(PageCount));
                PageCount = value;
            }
        }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {
            get
            {
                if (RowCount == 0) return 0;
                return (CurrentPage - 1) * PageSize + 1;
            }
        }
        public int LastRowOnPage
        {
            get
            {
                var lastRowOnPage = CurrentPage * PageSize;
                return lastRowOnPage > RowCount ? RowCount : lastRowOnPage;
            }
        }

        public string? AdditionalData { get; set; }
    }
}
