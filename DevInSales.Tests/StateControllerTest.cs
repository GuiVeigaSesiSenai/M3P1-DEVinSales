using DevInSales.Context;
using DevInSales.Controllers;
using DevInSales.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevInSales.Tests
{
    public class StateControllerTest
    {
        private readonly DbContextOptions<SqlContext> _sqlContext;
        public StateControllerTest()
        {
            _sqlContext = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase("StateControllerTest")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new SqlContext(_sqlContext);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetAllStates()
        {
            var context = new SqlContext(_sqlContext);
            var stateCount = context.State.Count();

            var controller = new StateController(context);

            var result = await controller.GetState(null);

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<State>;

            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.AreEqual(content.Count(), stateCount);
        }

        [Test]
        public async Task GetStateById()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new StateController(context);

            var result = await controller.GetStateId(33);

            var expected = (result.Result as ObjectResult);

            Assert.That(expected.Value.ToString().Contains("State_Id encontrado com sucesso"));
            Assert.AreEqual(expected.StatusCode.ToString(), "200");
        }
    }
}
