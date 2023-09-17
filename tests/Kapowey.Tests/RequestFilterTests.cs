using System.Linq.Dynamic.Core;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Tests
{
    public class RequestFilterTests
    {
        [Fact]
        public void RequestFilterCleanProp()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "Equals",
                Prop = "; DROP ALL TABLES; --",
                Value = 15
            };
            Assert.Throws<RequestException>(() => request.FilterSql());
        }

        [Fact]
        public void RequestFilterCleanValue()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "Equals",
                Prop = "UserName",
                Value = "; DROP ALL TABLES; --"
            };
            Assert.Throws<RequestException>(() => request.FilterSql());

            request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "Equals",
                Prop = "UserName",
                Value = ";DROP ALL TABLES; --"
            };
            Assert.Throws<RequestException>(() => request.FilterSql());

            request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "Equals",
                Prop = "UserName",
                Value = ";drop table 'batman'; --"
            };
            Assert.Throws<RequestException>(() => request.FilterSql());

            request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "Equals",
                Prop = "UserName",
                Value = ";update table set 'isactive' = 0; --"
            };
            Assert.Throws<RequestException>(() => request.FilterSql());
        }

        [Fact]
        public void RequestFilterEqualToSingleNumber()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "Equals",
                Prop = "UserId",
                Value = 15
            };
            Assert.Equal("UserId == 15", request.FilterSql());
        }

        [Fact]
        public void RequestFilterGreaterThanToSingleNumber()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "GreaterThan",
                Prop = "UserId",
                Value = 15
            };
            Assert.Equal("UserId > 15", request.FilterSql());
        }

        [Fact]
        public void RequestFilterLessThanToSingleNumber()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "LessThan",
                Prop = "UserId",
                Value = 15
            };
            Assert.Equal("UserId < 15", request.FilterSql());
        }

        [Fact]
        public void RequestBetweenNumbers()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "LessThan",
                Prop = "UserId",
                Value = 15,
                Value2 = 17
            };
            Assert.Equal("UserId >= 15 AND UserId <= 17", request.FilterSql());
        }

        [Fact]
        public void RequestFilterEqualToSingleString()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "Equals",
                Prop = "UserName",
                Value = "steven"
            };
            Assert.Equal("UserName == \"steven\"", request.FilterSql());
        }

        [Fact]
        public void RequestFilterStartsWithSingleString()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "StartsWith",
                Prop = "UserName",
                Value = "ste"
            };
            Assert.Equal("UserName.ToUpper().StartsWith(\"STE\")", request.FilterSql());
        }

        [Fact]
        public void RequestFilterEndsWithSingleString()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "EndsWith",
                Prop = "UserName",
                Value = "ste"
            };
            Assert.Equal("UserName.ToUpper().EndsWith(\"STE\")", request.FilterSql());
        }

        [Fact]
        public void RequestFilterLikeSingleString()
        {
            var request = new RequestFilter
            {
                AndOr = RequestFilterAndOr.And,
                Operation = "contains",
                Prop = "UserName",
                Value = "ste"
            };
            Assert.Equal("UserName.ToUpper().Contains(\"STE\")", request.FilterSql());
        }

        public DateTime RequestFilterCreatedDateValue()
        {
            return DateTime.Parse("08/18/2018 07:22:16Z").ToUniversalTime();
        }

        public IQueryable<User> RequestFilterUserTestData()
        {
            return new List<User>
            {
                new User
                {
                    UserId = 1,
                    UserName = "cute_katty",
                    CreatedDate = NodaTime.Instant.FromDateTimeUtc(RequestFilterCreatedDateValue()),
                    IsPublic = true,
                    NormalizedUserName = "CUTE_KATTY"
                },
                new User
                {
                    UserId = 2,
                    UserName = "Alexaxoxo",
                    CreatedDate = NodaTime.Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(2)),
                    NormalizedUserName = "ALEXAXOXO"
                },
                new User
                {
                    UserId = 3,
                    UserName = "Queenevamari",
                    CreatedDate = NodaTime.Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(2)),
                    NormalizedUserName = "QUEENEVAMARI"
                },
                new User
                {
                    UserId = 4,
                    UserName = "AllieBay",
                    IsPublic = true,
                    CreatedDate = NodaTime.Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(3)),
                    NormalizedUserName = "ALLIEBAY"
                },
                new User
                {
                    UserId = 5,
                    UserName = "Amber_Fry",
                    CreatedDate = NodaTime.Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(3)),
                    NormalizedUserName = "AMBER_FRY"
                }
            }.AsQueryable();
        }

        [Fact]
        public void QueryableEqualToBooleanTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "equals",
                        Prop = "IsPublic",
                        Value = true
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(2, data.Count());
        }

        [Fact]
        public void QueryableEqualToIntTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "equals",
                        Prop = "UserId",
                        Value = 1
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(1, data.Count());
        }

        [Fact]
        public void QueryableEqualToStringTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "equals",
                        Prop = "UserName",
                        Value = "cute_katty"
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(1, data.Count());
        }

        [Fact]
        public void QueryableNotEqualToStringTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "notequals",
                        Prop = "UserName",
                        Value = "cute_katty"
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(4, data.Count());
        }

        [Fact]
        public void QueryableLikeStringTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "like",
                        Prop = "UserName",
                        Value = "cute"
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(1, data.Count());
        }

        [Fact]
        public void QueryableStartsWithStringTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "startsWith",
                        Prop = "UserName",
                        Value = "cute"
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(1, data.Count());
        }

        [Fact]
        public void QueryableEndsWithStringTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "endsWith",
                        Prop = "UserName",
                        Value = "katty"
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(1, data.Count());
        }

        [Fact]
        public void QueryableEqualToDateTimeTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "equals",
                        Prop = "CreatedDate",
                        Value = NodaTime.Instant.FromDateTimeUtc(RequestFilterCreatedDateValue())
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(1, data.Count());
        }

        [Fact]
        public void QueryableGreaterThanDateTimeTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "greater",
                        Prop = "CreatedDate",
                        Value = NodaTime.Instant.FromDateTimeUtc(RequestFilterCreatedDateValue())
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(4, data.Count());
        }

        [Fact]
        public void QueryableIntGreaterThanTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "greater",
                        Prop = "UserId",
                        Value = 2
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(3, data.Count());
        }

        [Fact]
        public void QueryableIntLessThanTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "greater",
                        Prop = "UserId",
                        Value = 3
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(2, data.Count());
        }

        [Fact]
        public void QueryableIntBetweenThanTest()
        {
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<RequestFilter>
                {
                    new RequestFilter
                    {
                        AndOr = RequestFilterAndOr.And,
                        Operation = "between",
                        Prop = "UserId",
                        Value = 3,
                        Value2 = 5,
                    }
                }
            };
            var filter = pagedRequest.FilterSql();
            var data = RequestFilterUserTestData().Where(filter);
            Assert.NotNull(data);
            Assert.Equal(3, data.Count());
        }
    }
}