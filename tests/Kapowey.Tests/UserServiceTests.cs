
using Kapowey.Core.Services.Data;

namespace Kapowey.Tests
{
    public class UserServiceTests
    {
        private IUserService UserService { get; }

        public UserServiceTests()
        {

        }

        //[Fact]
        //public async Task GetUserList()
        //{

        //}

        //private IEnumerable<Person> GetFakeData()
        //{
        //    var i = 1;
        //    var persons = A.ListOf<Person>(26);
        //    persons.ForEach(x => x.Id = i++);
        //    return persons.Select(_ => _);
        //}

        //private Mock<ApplicationDbContext> CreateDbContext()
        //{
        //    var persons = GetFakeData().AsQueryable();

        //    var dbSet = new Mock<DbSet<Person>>();
        //    dbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(persons.Provider);
        //    dbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(persons.Expression);
        //    dbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(persons.ElementType);
        //    dbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(persons.GetEnumerator());

        //    var context = new Mock<ApplicationDbContext>();
        //    context.Setup(c => c.Persons).Returns(dbSet.Object);
        //    return context;
        //}

        [Theory]
        [InlineData(null)]
        [InlineData("0")]
        [InlineData("123")]
        [InlineData("!323")]
        public void PasswordsNotStrongEnough(string input)
        {
            Assert.False(Core.Services.Data.UserService.IsNewPasswordStrongEnough(input));
        }

        [Theory]
        [InlineData("S0mething99!")]
        [InlineData("Br549Dx4!3l")]
        [InlineData("8d7xz3l2x,f0.")]
        [InlineData("8d7xz3l2x,f0.#sxd329d45")]
        public void PasswordsStrongEnough(string input)
        {
            Assert.True(Core.Services.Data.UserService.IsNewPasswordStrongEnough(input));
        }

    }
}
