using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ProductAPI.Services;
using ProductAPI.Infrastructure;
using ProductAPI.Validation;
using System.Data.Entity.Infrastructure;

namespace ProductAPI.UnitTests
{
    [TestClass]
    public class ProductServiceTest
    {
        private const string VALID_PRODUCT_GUID = "8f2e9176-35ee-4f0a-ae55-83023d2db1a3";

        private Mock<ProductDBContext> _dbContextMock;
        private Mock<IProductOptionService> _productOptionServiceMock;

        private List<Product> source;

        [TestInitialize]
        public void Initialize()
        {
            AutoMappings.Configure();
        }

        [TestMethod]
        public void Get_should_return_data()
        {
            SetupTestData();

            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);
            var result = service.Get();

            Assert.AreEqual(result.Items.Count, 3);
        }

        [TestMethod]
        public void GetByName_should_return_data()
        {
            SetupTestData();

            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);
            var result = service.GetByName("Bbb");

            Assert.AreEqual(result.Items.Count, 1);
            Assert.AreEqual(result.Items[0].Id.ToString(), VALID_PRODUCT_GUID);
        }

        [TestMethod]
        public void GetById_should_return_data()
        {
            SetupTestData();

            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);
            var result = service.GetById(Guid.Parse(VALID_PRODUCT_GUID));

            Assert.AreEqual(result.Id.ToString(), VALID_PRODUCT_GUID);
        }

        [TestMethod]
        public void Create_should_execute_successfully()
        {
            SetupTestData();
            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);

            service.Create(GetNewProduct());
            Assert.AreEqual(source.Count, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAPIRequestException))]
        public void Create_should_throw_an_exception()
        {
            SetupTestData();
            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);

            try
            {
                service.Create(GetExistingProduct());
            }
            catch (InvalidAPIRequestException ex)
            {
                Assert.AreEqual(ex.Message, "Id is not unique");
                Assert.AreEqual(source.Count, 3);
                throw;
            }
        }

        [TestMethod]
        public void Update_should_execute_successfully()
        {
            SetupTestData();
            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);

            service.Update(GetExistingProduct());
            Assert.AreEqual(source.Count, 3);
            Assert.AreEqual(source.Find(x => x.Id.ToString() == VALID_PRODUCT_GUID).Name, "CCC");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAPIRequestException))]
        public void Update_should_throw_an_exception()
        {
            SetupTestData();
            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);

            try
            {
                service.Update(GetNewProduct());
            }
            catch (InvalidAPIRequestException ex)
            {
                Assert.AreEqual(ex.Message, "Product does not exist");
                Assert.AreEqual(source.Count, 3);
                throw;
            }
        }

        [TestMethod]
        public void Delete_should_execute_successfully()
        {
            SetupTestData();
            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);

            service.Delete(Guid.Parse(VALID_PRODUCT_GUID));
            Assert.AreEqual(source.Count, 2);
            _productOptionServiceMock.Verify(x => x.DeleteByProductId(It.Is<Guid>(id => id.ToString() == VALID_PRODUCT_GUID)));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAPIRequestException))]
        public void Delete_should_throw_an_exception()
        {
            SetupTestData();
            var service = new ProductService(_dbContextMock.Object, _productOptionServiceMock.Object);

            try
            {
                service.Delete(GetNewProduct().Id);
            }
            catch (InvalidAPIRequestException ex)
            {
                Assert.AreEqual(ex.Message, "Product does not exist");
                Assert.AreEqual(source.Count, 3);
                throw;
            }
        }


        private void SetupTestData()
        {
            source = new List<Product>
            {
                new Product { Id = Guid.Parse(VALID_PRODUCT_GUID),  Name = "BBB" },
                new Product { Id = Guid.NewGuid(), Name = "ZZZ" },
                new Product { Id = Guid.NewGuid(), Name = "AAA" }
            };
            var data = source.AsQueryable();

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns((object[] input) => data.FirstOrDefault(d => d.Id == Guid.Parse(input[0].ToString())));
            mockSet.Setup(x => x.Add(It.IsAny<Product>())).Callback((Product product) => source.Add(product));
            mockSet.Setup(x => x.Remove(It.IsAny<Product>())).Callback((Product product) => source.Remove(product));

            _dbContextMock = new Mock<ProductDBContext>();
            _dbContextMock.Setup(c => c.Products).Returns(mockSet.Object);
            _dbContextMock.Setup(c => c.UpdateEntry(It.IsAny<Product>(), It.IsAny<Product>())).Callback((Product original, Product updated) =>
            {
                source.Remove(original);
                source.Add(updated);
            });

            _productOptionServiceMock = new Mock<IProductOptionService>();
            _productOptionServiceMock.Setup(x => x.DeleteByProductId(It.IsAny<Guid>())).Verifiable();
        }

        private Models.Product GetNewProduct()
        {
            return new Models.Product { Id = Guid.NewGuid(), Name = "CCC" };
        }

        private Models.Product GetExistingProduct()
        {
            return new Models.Product { Id = Guid.Parse(VALID_PRODUCT_GUID), Name = "CCC" };
        }
    }
}
