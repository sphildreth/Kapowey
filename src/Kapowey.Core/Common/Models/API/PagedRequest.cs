using System.Text;

namespace Kapowey.Core.Common.Models.API
{
    [Serializable]
    public sealed class PagedRequest
    {
        public bool IsValid => Page > 0 && PageSize > 0;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        public string OrderBy { get; set; } = nameof(Entities.UserInfo.CreatedDate);

        public string OrderDir { get; set; } = "asc";

        public OrderByDirection OrderByDirection => (OrderByDirection)Enum.Parse(typeof(OrderByDirection), OrderDir, true);

        public IEnumerable<RequestFilter> Filters { get; set; }

        public int Skip => PageSize * (Page - 1);

        public PagedRequest()
        {
        }

        public string FilterSql()
        {
            if(!(Filters?.Any() ?? false))
            {
                return "1 == 1";
            }
            if(Filters.Count() == 1)
            {
                return Filters.First().FilterSql();
            }
            var stringBuilder = new StringBuilder();
            foreach (var filter in Filters)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(' ').Append(filter.AndOr.ToString().ToUpper()).Append(' ');
                }
                stringBuilder.Append(filter.FilterSql());
            }
            return stringBuilder.ToString();
        }

        public string Ordering() => $"{ RequestFilter.CleanValue(OrderBy, false) } { (OrderByDirection == OrderByDirection.Asc ? "asc" : "desc") }";
    }
}