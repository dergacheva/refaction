using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductAPI.FunctionalTests.Helpers;
using ProductAPI.FunctionalTests.Models;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Net;
using TechTalk.SpecFlow;

namespace ProductAPI.FunctionalTests.StepDefinitions
{
    [Binding]
    public class ProductSteps : Steps
    {
        private readonly ProductApiTestContext _testContext;

        public ProductSteps(ProductApiTestContext testContext)
        {
            _testContext = testContext;
        }

        [Given(@"an http request with ""(.*)"" verb and ""(.*)"" body")]
        public void GivenAnHttpRequestWithVerbAndBody(string verb, string body)
        {
            var requestVerb = (Method)Enum.Parse(typeof(Method), verb);
            _testContext.Request = new RestRequest(requestVerb);

            switch (body)
            {
                case "CreatedProduct":
                    _testContext.CreatedProduct = new Product()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Automated test product",
                        Description = "Created for testing",
                        Price = 5,
                        DeliveryPrice = 1
                    };
                    _testContext.Request.AddJsonBody(_testContext.CreatedProduct);
                    break;
                case "UpdatedProduct":
                    _testContext.UpdatedProduct = new Product()
                    {
                        Id = _testContext.CreatedProduct.Id,
                        Name = "Automated test product",
                        Description = "Updated description",
                        Price = 100,
                        DeliveryPrice = 2
                    };
                    _testContext.Request.AddJsonBody(_testContext.UpdatedProduct);
                    break;
                case "CreatedOption":
                    _testContext.CreatedOption = new ProductOption()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Automated test option",
                        Description = "Created option description",
                        ProductId = _testContext.CreatedProduct.Id
                    };
                    _testContext.Request.AddJsonBody(_testContext.CreatedOption);
                    break;
                case "UpdatedOption":
                    _testContext.UpdatedOption = new ProductOption()
                    {
                        Id = _testContext.CreatedOption.Id,
                        Name = "Automated test option",
                        Description = "Updated option description",
                        ProductId = _testContext.CreatedProduct.Id
                    };
                    _testContext.Request.AddJsonBody(_testContext.UpdatedOption);
                    break;
                default: break;
            }
        }

        [Given(@"I have created a new product")]
        public void GivenIHaveCreatedANewProduct()
        {
            Given("an http request with \"POST\" verb and \"CreatedProduct\" body");
            When("I call \"Products\" endpoint with \"no\" query");
        }

        [Given(@"I have created a new product option")]
        public void GivenIHaveCreatedANewProductOption()
        {
            Given("an http request with \"POST\" verb and \"CreatedOption\" body");
            When("I call \"Options\" endpoint with \"no\" query");
        }

        [When(@"I call ""(.*)"" endpoint with ""(.*)"" query")]
        public void WhenICallEndpoint(string endpointName, string queryName)
        {
            string endpoint;
            switch (endpointName)
            {
                case "Products": endpoint = Endpoints.Products(); break;
                case "ProductsById": endpoint = Endpoints.ProductsById(_testContext.CreatedProduct.Id); break;
                case "Options": endpoint = Endpoints.Options(_testContext.CreatedProduct.Id); break;
                case "OptionsById": endpoint = Endpoints.OptionsById(_testContext.CreatedProduct.Id, _testContext.CreatedOption.Id); break;
                default: endpoint = Endpoints.BaseUrl(); break;
            }

            switch (queryName)
            {
                case "name": _testContext.Request.AddQueryParameter("name", "Automated test product"); break;
                default: break;
            }

            var restClient = new RestClient(endpoint);
            _testContext.Response = (RestResponse)restClient.Execute(_testContext.Request);

            if (_testContext.Response.ErrorException == null)
                return;

            const string message = "Error retrieving response.  Check inner details for more info.";
            var applicationException = new ApplicationException(message, _testContext.Response.ErrorException);
            throw applicationException;
        }

        [Then(@"the result should include product items")]
        public void ThenTheResultShouldIncludeProductItems()
        {
            var jsonConvert = new JsonDeserializer();
            var data = jsonConvert.Deserialize<Products>(_testContext.Response);

            Assert.IsTrue(data.Items.Count > 0);
            Assert.IsTrue(data.Items.Find(x => x.Id.ToString() == _testContext.CreatedProduct.Id.ToString()) != null);
        }

        [Then(@"the result should include product")]
        public void ThenTheResultShouldIncludeProduct()
        {
            var jsonConvert = new JsonDeserializer();
            var data = jsonConvert.Deserialize<Product>(_testContext.Response);

            Assert.AreEqual(data.Id.ToString(), _testContext.CreatedProduct.Id.ToString());
        }

        [Then(@"the result should include product options item")]
        public void ThenTheResultShouldIncludeProductOptionsItem()
        {
            var jsonConvert = new JsonDeserializer();
            var data = jsonConvert.Deserialize<ProductOptions>(_testContext.Response);

            Assert.IsTrue(data.Items.Count > 0);
            Assert.IsTrue(data.Items.Find(x => x.Id.ToString() == _testContext.CreatedOption.Id.ToString()) != null);
        }

        [Then(@"the result should include product option")]
        public void ThenTheResultShouldIncludeProductOption()
        {
            var jsonConvert = new JsonDeserializer();
            var data = jsonConvert.Deserialize<ProductOption>(_testContext.Response);

            Assert.AreEqual(data.Id.ToString(), _testContext.CreatedOption.Id.ToString());
        }

        [Then(@"I get OK response")]
        public void ThenIGetAOKResponse()
        {
            Assert.AreEqual(HttpStatusCode.OK, _testContext.Response.StatusCode);
        }

        [Then(@"I get NoContent response")]
        public void ThenIGetNoContentResponse()
        {
            Assert.AreEqual(HttpStatusCode.NoContent, _testContext.Response.StatusCode);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Given("an http request with \"GET\" verb and \"no\" body");
            When("I call \"Products\" endpoint with \"name\" query");
            var jsonConvert = new JsonDeserializer();
            var data = jsonConvert.Deserialize<Products>(_testContext.Response);

            foreach (Product product in data.Items)
            {
                Given("an http request with \"DELETE\" verb and \"no\" body");
                var restClient = new RestClient(Endpoints.ProductsById(product.Id));
                _testContext.Response = (RestResponse)restClient.Execute(_testContext.Request);
            }
        }
    }
}
