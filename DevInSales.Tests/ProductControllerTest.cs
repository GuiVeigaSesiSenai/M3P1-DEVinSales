using DevInSales.Context;
using DevInSales.Controllers;
using DevInSales.DTOs;
using DevInSales.Models;
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
    public class ProductControllerTest
    {
        private readonly DbContextOptions<SqlContext> _sqlContext;
        public ProductControllerTest()
        {
            _sqlContext = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase("ProductControllerTest")
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
        public async Task GetAllProducts()
        {
            var context = new SqlContext(_sqlContext);
            var productCount = context.Product.Count();

            var controller = new ProductController(context);

            var result = await controller.GetProduct(null, null, null);

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<ProductGetDTO>;

            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.AreEqual(content.Count(), productCount);
        }

        [Test]
        public async Task GetProductByName()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.GetProduct("Curso de Kotlin", null, null);

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<ProductGetDTO>;

            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.AreEqual(content.Count(), 1);
            Assert.That(content[0].Name.Contains("Curso de Kotlin"));
        }

        [Test]
        public async Task GetProductByMinValue()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.GetProduct(null, 280, null);

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<ProductGetDTO>;

            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.AreEqual(content.Count(), 3);
        }

        [Test]
        public async Task GetProductByMaxValue()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.GetProduct(null, null, 200);

            var expected = (result.Result as ObjectResult);

            var content = expected.Value as List<ProductGetDTO>;

            Assert.AreEqual(expected.StatusCode.ToString(), "200");
            Assert.That(content.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetProductMinValueBiggerThanMaxValue()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.GetProduct("Curso de Kotlin", 666, 200);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
            Assert.That(expected.Value.ToString().Contains("O Preço Máximo (200) não pode ser menor que o Preço Mínimo (666)."));
        }

        [Test]
        public async Task PostProduct()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = "Curso de Tester",
                CategoryId = 1,
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);
            var productCount = context.Product.Count() + 1;

            var controller = new ProductController(context);

            var result = await controller.PostProduct(product);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "201");
            Assert.AreEqual(context.Product.Count(), productCount);
        }

        [Test]
        public async Task PostProductNameAlreadyExists()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = "Curso de Kotlin",
                CategoryId = 1,
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PostProduct(product);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
            Assert.AreEqual(expected.Value.ToString(), "Já existe um produto com este nome.");
        }

        [Test]
        public async Task PostProductInvalidPrice()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = "Curso de Tester Pro",
                CategoryId = 1,
                Suggested_Price = 0M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PostProduct(product);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
            Assert.AreEqual(expected.Value.ToString(), "O preço sugerido não pode ser menor ou igual a 0.");
        }

        [Test]
        public async Task PutProduct()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = "Curso de Tester Ultra",
                CategoryId = 1,
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PutProduct(1, product);

            var expected = (result.Result as StatusCodeResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "204");
        }

        [Test]
        public async Task PutProductId404()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = "Curso de Tester Ultra",
                CategoryId = 1,
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PutProduct(0, product);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "404");
            Assert.AreEqual(expected.Value.ToString(), "Não existe um produto com esta Id.");
        }

        [Test]
        public async Task PutProductNameAlreadyExists()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = "Curso de Kotlin",
                CategoryId = 1,
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PutProduct(9, product);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
            Assert.AreEqual(expected.Value.ToString(), "Já existe um produto com este nome.");
        }

        [Test]
        public async Task PutProductInvalidPrice()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = "Curso de Tester Omega",
                CategoryId = 1,
                Suggested_Price = 0M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PutProduct(1, product);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
            Assert.AreEqual(expected.Value.ToString(), "O preço sugerido não pode ser menor ou igual a 0.");
        }

        [Test]
        public async Task PutProductNullName()
        {
            var product = new ProductPostAndPutDTO
            {
                Name = null,
                CategoryId = 1,
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PutProduct(1, product);

            var expected = (result.Result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
            Assert.AreEqual(expected.Value.ToString(), "Nome ou Preço Sugerido são Nulos.");
        }

        [Test]
        public async Task PatchProductNullNameAndPrice()
        {
            var product = new ProductPatchDTO
            {
                Name = null,
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PatchProduct(2, product);

            var expected = (result.Result as StatusCodeResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
        }

        [Test]
        public async Task PatchProductNullName()
        {
            var product = new ProductPatchDTO
            {
                Name = null,
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PatchProduct(9, product);

            var expected = (result.Result as StatusCodeResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "204");
        }

        [Test]
        public async Task PatchProductNameExists()
        {
            var product = new ProductPatchDTO
            {
                Name = "Curso de React",
                Suggested_Price = 599.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PatchProduct(2, product);

            var expected = (result.Result as StatusCodeResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
        }

        [Test]
        public async Task PatchProducId404()
        {
            var product = new ProductPatchDTO
            {
                Name = "Curso de Angular PRO",
                Suggested_Price = 699.99M
            };

            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.PatchProduct(0, product);

            var expected = (result.Result as StatusCodeResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
        }

        [Test]
        public async Task DeleteProduct()
        {
            var context = new SqlContext(_sqlContext);
            var productCount = context.Product.Count() - 1;

            var controller = new ProductController(context);

            var result = await controller.DeleteProduct(10);

            var expected = (result as StatusCodeResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "204");
            Assert.AreEqual(context.Product.Count(), productCount);
        }

        [Test]
        public async Task DeleteProductId404()
        {
            var context = new SqlContext(_sqlContext);

            var controller = new ProductController(context);

            var result = await controller.DeleteProduct(0);

            var expected = (result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "404");
            Assert.AreEqual(expected.Value.ToString(), "O Id de Produto de número 0 não foi encontrado.");
        }

        [Test]
        public async Task DeleteProductWithOrder()
        {
            var context = new SqlContext(_sqlContext);

            var user = await context.User.FindAsync(1);
            var seller = await context.User.FindAsync(2);
            var order = new Order
            {
                Id = 1,
                User = user,
                UserId = 1,
                Seller = seller,
                SellerId = 2,
                Date_Order = DateTime.Now,
                Shipping_Company = "AJAX",
                Shipping_Company_Price = 299.99M,
            };
            var product = await context.Product.FindAsync(3);
            var orderProduct = new OrderProduct
            {
                Id = 1,
                Unit_Price = 599.99M,
                Amount = 1,
                Order = order,
                Product = product
            };
            await context.Order_Product.AddAsync(orderProduct);

            await context.SaveChangesAsync();

            var controller = new ProductController(context);

            var result = await controller.DeleteProduct(3);

            var expected = (result as ObjectResult);

            Assert.AreEqual(expected.StatusCode.ToString(), "400");
            Assert.AreEqual(expected.Value.ToString(), "O Id de Produto de número 3 possui uma Ordem de Produto vinculada, por este motivo não pode ser deletado.");
        }
    }
}
