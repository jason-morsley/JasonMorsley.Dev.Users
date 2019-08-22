namespace Users.API.Helpers
{
    public class UsersResourceParameters
    {
        const int maxPageSize = 100;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 25;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        /// <summary>
        /// 
        /// </summary>
        public string SearchQuery { get; set; }
        /// <summary>
        /// The way in which you wish to sort the result, eg by Id, name.
        /// </summary>
        public string OrderBy { get; set; } = "Id";
        /// <summary>
        /// Search fields you wish to include, eg name, id.
        /// </summary>
        public string Fields { get; set; }

    }
}