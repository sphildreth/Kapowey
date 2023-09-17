using Kapowey.Core.Common.Models.API;

namespace Kapowey.Tests
{
    public class PagedRequestTests
    {

        [Fact]
        public void PageRequestNoFilters()
        {
            var r = new PagedRequest();
            Assert.NotNull(r.FilterSql());
        }

        [Fact]
        public void PageRequestSingleFilters()
        {
            var r = new PagedRequest
            {
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "Equals",
                        Prop = "UserId",
                        Value = 15
                    }
                }
            };
            Assert.Equal("UserId == 15", r.FilterSql());
        }

        [Fact]
        public void PageRequestTwoAndFilters()
        {
            var r = new PagedRequest
            {
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "Contains",
                        Prop = "UserName",
                        Value = "steven"
                    },
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "GreaterThan",
                        Prop = "CreatedDate",
                        Value = "2020-07-14T02:13:14.793801Z"
                    }
                }
            };
            Assert.Equal("UserName LIKE \"%steven%\" AND CreatedDate > \"2020-07-14T02:13:14.793801Z\"", r.FilterSql());
        }

        [Fact]
        public void PageRequestTwoOrFilters()
        {
            var r = new PagedRequest
            {
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        Operation = "Contains",
                        Prop = "UserName",
                        Value = "steven"
                    },
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.Or,
                        Operation = "Contains",
                        Prop = "UserName",
                        Value = "batman"
                    }
                }
            };
            Assert.Equal("UserName LIKE \"%steven%\" OR UserName LIKE \"%batman%\"", r.FilterSql());
        }
    }
}
