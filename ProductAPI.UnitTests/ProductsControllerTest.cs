using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductAPI.Infrastructure;
using ProductAPI.Services;
using System;
using ProductAPI.Controllers;
using ProductAPI.Models;
using ProductAPI.Validation;
using System.Web.Http;
using System.Net;

namespace ProductAPI.UnitTests
{
    [TestClass]
    public class ProductsControllerTest
    {
        private const string VALID_PRODUCT_GUID = "8f2e9176-35ee-4f0a-ae55-83023d2db1a3";
        private const string INVALID_PRODUCT_GUID = "8f2e9176-35ee-0000-ae55-83023d2db1a3";
        private const string VALID_PRODUCT_OPTION_GUID = "de1287c0-4b15-4a7b-9d8a-dd21b3cafec3";
        private const string INVALID_PRODUCT_OPTION_GUID = "de1287c0-4b15-0000-9d8a-dd21b3cafec3";

        private Mock<IProductService> _productService;
        private Mock<IProductOptionService> _productOptionService;

        [TestInitialize]
        public void Setup()
        {
            AutoMappings.Configure();
        }

        [TestMethod]
        public void GetAll_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.Get()).Returns(GetTestProducts());
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            var result = controller.GetAll();

            Assert.AreEqual(result.Items.Count, 1);
            Assert.AreEqual(result.Items[0].Id.ToString(), VALID_PRODUCT_GUID);
        }

        [TestMethod]
        public void SearchByName_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.GetByName(It.IsAny<string>())).Returns(GetTestProducts());
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            var result = controller.SearchByName("testName");

            Assert.AreEqual(result.Items.Count, 1);
            Assert.AreEqual(result.Items[0].Id.ToString(), VALID_PRODUCT_GUID);
        }

        [TestMethod]
        public void GetById_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(GetTestProduct());
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            var result = controller.GetProduct(Guid.Parse(VALID_PRODUCT_GUID));

            Assert.AreEqual(result.Id.ToString(), VALID_PRODUCT_GUID);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void GetById_Should_return_404()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns((Models.Product)null);
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.GetProduct(Guid.Parse(VALID_PRODUCT_GUID));
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
                throw;
            }
        }

        [TestMethod]
        public void Create_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.Create(It.IsAny<Models.Product>())).Verifiable();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            controller.Create(GetTestProduct());

            _productService.Verify(x => x.Create(It.Is<Models.Product>(p => p.Id.ToString() == VALID_PRODUCT_GUID)));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Create_Should_return_400()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.Create(It.IsAny<Models.Product>())).Throws<InvalidAPIRequestException>();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.Create(GetTestProduct());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        public void Update_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.Update(It.IsAny<Models.Product>())).Verifiable();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            controller.Update(Guid.Parse(VALID_PRODUCT_GUID), GetTestProduct());

            _productService.Verify(x => x.Update(It.Is<Models.Product>(p => p.Id.ToString() == VALID_PRODUCT_GUID)));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Update_Should_return_400_if_guid_doesnt_match()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.Update(Guid.Parse(INVALID_PRODUCT_GUID), GetTestProduct());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Update_Should_return_400_if_service_throws_an_error()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.Update(It.IsAny<Models.Product>())).Throws<InvalidAPIRequestException>();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.Update(Guid.Parse(VALID_PRODUCT_GUID), GetTestProduct());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        public void Delete_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.Delete(It.IsAny<Guid>())).Verifiable();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            controller.Delete(Guid.Parse(VALID_PRODUCT_GUID));

            _productService.Verify(x => x.Delete(It.Is<Guid>(id => id.ToString() == VALID_PRODUCT_GUID)));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Delete_Should_return_400()
        {
            _productService = new Mock<IProductService>();
            _productService.Setup(x => x.Delete(It.IsAny<Guid>())).Throws<InvalidAPIRequestException>();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.Delete(Guid.Parse(VALID_PRODUCT_GUID));
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        public void GetOptions_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.GetByProductId(It.IsAny<Guid>())).Returns(GetTestOptions());

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            var result = controller.GetOptions(Guid.Parse(VALID_PRODUCT_GUID));

            Assert.AreEqual(result.Items.Count, 1);
            Assert.AreEqual(result.Items[0].Id.ToString(), VALID_PRODUCT_OPTION_GUID);
            Assert.AreEqual(result.Items[0].ProductId.ToString(), VALID_PRODUCT_GUID);
        }

        [TestMethod]
        public void GetOption_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.GetByProductIdAndId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(GetTestOption());

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            var result = controller.GetOption(Guid.Parse(VALID_PRODUCT_GUID), Guid.Parse(VALID_PRODUCT_OPTION_GUID));

            Assert.AreEqual(result.Id.ToString(), VALID_PRODUCT_OPTION_GUID);
            Assert.AreEqual(result.ProductId.ToString(), VALID_PRODUCT_GUID);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void GetOption_Should_return_404()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.GetByProductIdAndId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((Models.ProductOption)null);

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.GetOption(Guid.Parse(VALID_PRODUCT_GUID), Guid.Parse(VALID_PRODUCT_OPTION_GUID));
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
                throw;
            }
        }

        [TestMethod]
        public void CreateOption_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.Create(It.IsAny<Models.ProductOption>())).Verifiable();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            controller.CreateOption(Guid.Parse(VALID_PRODUCT_GUID), GetTestOption());

            _productOptionService.Verify(x => x.Create(It.Is<Models.ProductOption>(p => p.Id.ToString() == VALID_PRODUCT_OPTION_GUID)));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void CreateOption_Should_return_400_if_guid_doesnt_match()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.CreateOption(Guid.Parse(INVALID_PRODUCT_GUID), GetTestOption());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void CreateOption_Should_return_400_if_service_throws_an_error()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.Create(It.IsAny<Models.ProductOption>())).Throws<InvalidAPIRequestException>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.CreateOption(Guid.Parse(VALID_PRODUCT_GUID), GetTestOption());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        public void UpdateOption_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.Update(It.IsAny<Models.ProductOption>())).Verifiable();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            controller.UpdateOption(Guid.Parse(VALID_PRODUCT_GUID), Guid.Parse(VALID_PRODUCT_OPTION_GUID), GetTestOption());

            _productOptionService.Verify(x => x.Update(It.Is<Models.ProductOption>(p => p.Id.ToString() == VALID_PRODUCT_OPTION_GUID)));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void UpdateOption_Should_return_400_if_guid_doesnt_match()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.UpdateOption(Guid.Parse(INVALID_PRODUCT_GUID), Guid.Parse(VALID_PRODUCT_OPTION_GUID), GetTestOption());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void UpdateOption_Should_return_400_if_service_throws_an_error()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.Update(It.IsAny<Models.ProductOption>())).Throws<InvalidAPIRequestException>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.UpdateOption(Guid.Parse(VALID_PRODUCT_GUID), Guid.Parse(VALID_PRODUCT_OPTION_GUID), GetTestOption());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        [TestMethod]
        public void DeleteOption_Should_return_200()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.DeleteById(It.IsAny<Guid>())).Verifiable();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            controller.DeleteOption(Guid.Parse(VALID_PRODUCT_OPTION_GUID));

            _productOptionService.Verify(x => x.DeleteById(It.Is<Guid>(id => id.ToString() == VALID_PRODUCT_OPTION_GUID)));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void DeleteOption_Should_return_400()
        {
            _productService = new Mock<IProductService>();
            _productOptionService = new Mock<IProductOptionService>();
            _productOptionService.Setup(x => x.DeleteById(It.IsAny<Guid>())).Throws<InvalidAPIRequestException>();

            var controller = new ProductsController(_productService.Object, _productOptionService.Object);

            try
            {
                controller.DeleteOption(Guid.Parse(VALID_PRODUCT_OPTION_GUID));
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
                throw;
            }
        }

        private Products GetTestProducts()
        {
            return new Products()
            {
                Items = new List<Models.Product> { GetTestProduct() }
            };
        }

        private Models.Product GetTestProduct()
        {
            return new Models.Product() { Id = Guid.Parse(VALID_PRODUCT_GUID), Name = "Test product" };
        }

        private ProductOptions GetTestOptions()
        {
            return new ProductOptions()
            {
                Items = new List<Models.ProductOption> { GetTestOption() }
            };
        }

        private Models.ProductOption GetTestOption()
        {
            return new Models.ProductOption()
            {
                Id = Guid.Parse(VALID_PRODUCT_OPTION_GUID),
                Name = "Test option",
                ProductId = Guid.Parse(VALID_PRODUCT_GUID)
            };
        }
    }
}
