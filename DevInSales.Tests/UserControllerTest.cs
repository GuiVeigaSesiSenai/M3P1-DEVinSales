using DevInSales.Context;
using DevInSales.Controllers;
using DevInSales.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevInSales.Tests
{
    public class UserControllerTest
    {
        private readonly DbContextOptions<SqlContext> _sqlContext;
        public UserControllerTest()
        {
            _sqlContext = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase("UserControllerTest")
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
        public async Task GetUserByName()
        {
            var context = new SqlContext(_sqlContext);
            var user = context.User.FirstAsync(user => user.Name.Contains("Henrique"));

            var controller = new UserController(context);

            var result = await controller.Get("Henrique", null, null);

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<UserResponseDTO>;

            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.That(content[0].Name.Contains(user.Result.Name));
        }

        [Test]
        public async Task GetUserMinimumDate()
        {
            var context = new SqlContext(_sqlContext);
            var date = new DateTime(1994, 8, 11);
            var user = await context.User.FirstAsync(user => user.BirthDate >= date);

            var controller = new UserController(context);

            var result = await controller.Get(null, "11/08/1994", null);

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<UserResponseDTO>;

            Assert.That(content[0].BirthDate >= date);
            Assert.AreEqual(expected.StatusCode.ToString(), "200");
        }

        [Test]
        public async Task GetUserMaximumDate()
        {
            var context = new SqlContext(_sqlContext);
            var date = new DateTime(1994, 8, 11);
            var user = await context.User.FirstAsync(user => user.BirthDate <= date);

            var controller = new UserController(context);

            var result = await controller.Get(null, null, "11/08/1994");

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<UserResponseDTO>;

            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.That(content[0].BirthDate <= date);
        }

        [Test]
        public async Task GetUser404()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new UserController(context);

            var result = await controller.Get("Não Existe", "01/01/1994", "31/12/2022");

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "Nenhum usuário foi encontrado.");
            Assert.AreEqual(expected.StatusCode.ToString(), "404");
        }


        [Test]
        public async Task PostUser()
        {
            UserCreateDTO userDTO = new UserCreateDTO();
            userDTO.BirthDate = "11/08/1994";
            userDTO.Email = "testando@gmail.com";
            userDTO.Name = "Guilherme";
            userDTO.Password = "12345678910";
            userDTO.ProfileId = 1;

            var context = new SqlContext(_sqlContext);

            var userCount = context.User.Count();

            var controller = new UserController(context);

            var result = await controller.Create(userDTO);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "201");
            Assert.AreEqual(context.User.Count(), (userCount + 1));

        }

        [Test]
        public async Task PostUserMinor()
        {
            UserCreateDTO userDTO = new UserCreateDTO();
            userDTO.BirthDate = "11/08/2022";
            userDTO.Email = "testando@gmail.com";
            userDTO.Name = "Guilherme";
            userDTO.Password = "12345678910";
            userDTO.ProfileId = 1;

            var context = new SqlContext(_sqlContext);

            var controller = new UserController(context);

            var result = await controller.Create(userDTO);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "O usuário deve ser maior de 18 anos.");
            Assert.AreEqual(expected.StatusCode.ToString(), "400");
        }

        [Test]
        public async Task PostUserInvalidDate()
        {
            UserCreateDTO userDTO = new UserCreateDTO();
            userDTO.BirthDate = "13/32/1994";
            userDTO.Email = "testando@gmail.com";
            userDTO.Name = "Guilherme";
            userDTO.Password = "12345678910";
            userDTO.ProfileId = 1;

            var context = new SqlContext(_sqlContext);

            var controller = new UserController(context);

            var result = await controller.Create(userDTO);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "Data inválida.");
            Assert.AreEqual(expected.StatusCode.ToString(), "400");
        }

        [Test]
        public async Task PostUserInvalidPassword()
        {
            UserCreateDTO userDTO = new UserCreateDTO();
            userDTO.BirthDate = "11/08/1994";
            userDTO.Email = "testando@gmail.com";
            userDTO.Name = "Guilherme";
            userDTO.Password = "999";
            userDTO.ProfileId = 1;

            var context = new SqlContext(_sqlContext);

            var controller = new UserController(context);

            var result = await controller.Create(userDTO);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "Senha inválida. Deve-se ter pelo menos um caractere diferente dos demais.");
            Assert.AreEqual(expected.StatusCode.ToString(), "400");
        }

        [Test]
        public async Task PostUserEmailAlreadyExists()
        {
            UserCreateDTO userDTO = new UserCreateDTO();
            userDTO.BirthDate = "11/08/1994";
            userDTO.Email = "romeu@lenda.com";
            userDTO.Name = "Guilherme";
            userDTO.Password = "12345678910";
            userDTO.ProfileId = 1;

            var context = new SqlContext(_sqlContext);

            var controller = new UserController(context);

            var result = await controller.Create(userDTO);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "O email informado já existe.");
            Assert.AreEqual(expected.StatusCode.ToString(), "400");
        }

        [Test]
        public async Task PostUserInvalidProfileId()
        {
            UserCreateDTO userDTO = new UserCreateDTO();
            userDTO.BirthDate = "11/08/1994";
            userDTO.Email = "testando123@gmail.com";
            userDTO.Name = "Guilherme";
            userDTO.Password = "12345678910";
            userDTO.ProfileId = 0;

            var context = new SqlContext(_sqlContext);

            var controller = new UserController(context);

            var result = await controller.Create(userDTO);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "O perfil informado não foi encontrado.");
            Assert.AreEqual(expected.StatusCode.ToString(), "404");
        }

        [Test]
        public async Task DeleteUser()
        {
            var context = new SqlContext(_sqlContext);
            var userCount = context.User.Count();

            var controller = new UserController(context);

            var result = await controller.DeleteUser(5);

            var expected = (result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "5");
            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.AreEqual(context.User.Count(), (userCount - 1));
        }

        [Test]
        public async Task DeleteUserInvalidId()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new UserController(context);

            var result = await controller.DeleteUser(0);

            var expected = (result as ObjectResult);

            Assert.AreEqual(expected.Value.ToString(), "O Id de Usuário de número 0 não foi encontrado.");
            Assert.AreEqual(expected.StatusCode.ToString(), "404");
        }
    }
}
