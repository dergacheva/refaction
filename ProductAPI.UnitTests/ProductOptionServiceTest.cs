using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ProductAPI.Services;
using ProductAPI.Infrastructure;
using System.Linq.Expressions;
using ProductAPI.Validation;

namespace ProductAPI.UnitTests
{
    [TestClass]
    public class ProductOptionServiceTest
    {
        private const string VALID_PRODUCT_GUID = "8f2e9176-35ee-4f0a-ae55-83023d2db1a3";
        private const string VALID_PRODUCT_OPTION_GUID = "8f2e9176-35ee-9758-ae55-83023d2d0000";

        private Mock<ProductDBContext> _dbContextMock;

        private List<ProductOption> source;

        [TestInitialize]
        public void Initialize()
        {
            AutoMappings.Configure();
        }

        [TestMethod]
        public void GetByProductId_Should_return_data()
        {
            SetupTestData();

            var service = new ProductOptionService(_dbContextMock.Object);
            var result = service.GetByProductId(Guid.Parse(VALID_PRODUCT_GUID));

            Assert.AreEqual(result.Items.Count, 2);
        }

        [TestMethod]
        public void GetByProductIdAndId_Should_return_data()
        {
            SetupTestData();

            var service = new ProductOptionService(_dbContextMock.Object);
            var result = service.GetByProductIdAndId(Guid.Parse(VALID_PRODUCT_GUID), Guid.Parse(VALID_PRODUCT_OPTION_GUID));

            Assert.AreEqual(result.ProductId.ToString(), VALID_PRODUCT_GUID);
            Assert.AreEqual(result.Id.ToString(), VALID_PRODUCT_OPTION_GUID);
        }

        [TestMethod]
        public void Create_should_execute_successfully()
        {
            SetupTestData();
            var service = new ProductOptionService(_dbContextMock.Object);

            service.Create(GetNewOption());
            Assert.AreEqual(source.Count, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAPIRequestException))]
        public void Create_should_throw_an_exception()
        {
            SetupTestData();
            var service = new ProductOptionService(_dbContextMock.Object);

            try
            {
                service.Create(GetExistingOption());
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
            var service = new ProductOptionService(_dbContextMock.Object);

            service.Update(GetExistingOption());
            Assert.AreEqual(source.Count, 3);
            Assert.AreEqual(source.Find(x => x.Id.ToString() == VALID_PRODUCT_OPTION_GUID).Name, "DDD");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAPIRequestException))]
        public void Update_should_throw_an_exception()
        {
            SetupTestData();
            var service = new ProductOptionService(_dbContextMock.Object);

            try
            {
                service.Update(GetNewOption());
            }
            catch (InvalidAPIRequestException ex)
            {
                Assert.AreEqual(ex.Message, "Option does not exist");
                Assert.AreEqual(source.Count, 3);
                throw;
            }
        }

        [TestMethod]
        public void Delete_should_execute_successfully()
        {
            SetupTestData();
            var service = new ProductOptionService(_dbContextMock.Object);

            service.DeleteById(Guid.Parse(VALID_PRODUCT_OPTION_GUID));
            Assert.AreEqual(source.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAPIRequestException))]
        public void Delete_should_throw_an_exception()
        {
            SetupTestData();
            var service = new ProductOptionService(_dbContextMock.Object);

            try
            {
                service.DeleteById(GetNewOption().Id);
            }
            catch (InvalidAPIRequestException ex)
            {
                Assert.AreEqual(ex.Message, "Option does not exist");
                Assert.AreEqual(source.Count, 3);
                throw;
            }
        }

        [TestMethod]
        public void DeleteByProductId_should_execute_successfully()
        {
            SetupTestData();
            var service = new ProductOptionService(_dbContextMock.Object);

            service.DeleteByProductId(Guid.Parse(VALID_PRODUCT_GUID));
            Assert.AreEqual(source.Count, 1);
        }

        private void SetupTestData()
        {
            source = new List<ProductOption>()
            {
                new ProductOption { Id = Guid.Parse(VALID_PRODUCT_OPTION_GUID), ProductId = Guid.Parse(VALID_PRODUCT_GUID), Name = "AAA" },
                new ProductOption { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Name = "BBB" },
                new ProductOption { Id = Guid.NewGuid(), ProductId = Guid.Parse(VALID_PRODUCT_GUID), Name = "CCC" },
            };
            var data = source.AsQueryable();

            var mockSet = new Mock<DbSet<ProductOption>>();
            mockSet.As<IQueryable<ProductOption>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<ProductOption>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<ProductOption>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<ProductOption>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns((object[] input) => data.FirstOrDefault(d => d.Id == Guid.Parse(input[0].ToString())));
            mockSet.Setup(x => x.RemoveRange(It.IsAny<IEnumerable<ProductOption>>()))
                .Callback((IEnumerable<ProductOption> range) => range.ToList().ForEach(x => source.Remove(x)));

            mockSet.Setup(x => x.Add(It.IsAny<ProductOption>())).Callback((ProductOption product) => source.Add(product));
            mockSet.Setup(x => x.Remove(It.IsAny<ProductOption>())).Callback((ProductOption product) => source.Remove(product));

            _dbContextMock = new Mock<ProductDBContext>();
            _dbContextMock.Setup(c => c.ProductOptions).Returns(mockSet.Object);
            _dbContextMock.Setup(c => c.UpdateEntry(It.IsAny<ProductOption>(), It.IsAny<ProductOption>())).Callback((ProductOption original, ProductOption updated) =>
            {
                source.Remove(original);
                source.Add(updated);
            });
        }

        private Models.ProductOption GetNewOption()
        {
            return new Models.ProductOption { Id = Guid.NewGuid(), Name = "CCC" };
        }

        private Models.ProductOption GetExistingOption()
        {
            return new Models.ProductOption { Id = Guid.Parse(VALID_PRODUCT_OPTION_GUID), Name = "DDD" };
        }
    }
}
