using ApolloStage.Data;
using ApolloStageFirst.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ApolloStageTest
{
    public class MarketControllerTest : IClassFixture<ApplicationDbContextFixture>
    {
        private readonly ApplicationDbContext _context;

        public MarketControllerTest(ApplicationDbContextFixture contextFixture)
        {
            _context = contextFixture.DbContext;
        }

        [Fact]
        public void Index()
        {
            var controller = new MarketController(null, _context, null);
            var result = controller.market();
            Assert.IsType<ViewResult>(result);
        }
    }
}
