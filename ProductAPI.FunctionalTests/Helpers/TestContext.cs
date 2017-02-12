using ProductAPI.FunctionalTests.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.FunctionalTests.Helpers
{
    public class ProductApiTestContext
    {
        public string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["products_api_base_endpoint"]; }
        }

        public RestSharp.RestRequest Request { get; set; }
        public RestSharp.RestResponse Response { get; set; }

        public Product CreatedProduct { get; set; }
        public Product UpdatedProduct { get; set; }
        public ProductOption CreatedOption { get; set; }
        public ProductOption UpdatedOption { get; set; }
    }
}
