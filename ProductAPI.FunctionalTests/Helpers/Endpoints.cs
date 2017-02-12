using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.FunctionalTests.Helpers
{
    public static class Endpoints
    {
        public static string BaseUrl()
        {
            return ConfigurationManager.AppSettings["products_api_base_endpoint"];
        }

        public static string Products()
        {
            return String.Format("{0}/products", BaseUrl());
        }

        public static string ProductsById(Guid Id)
        {
            return String.Format("{0}/products/{1}", BaseUrl(), Id);
        }

        public static string Options(Guid productId)
        {
            return String.Format("{0}/products/{1}/options", BaseUrl(), productId);
        }

        public static string OptionsById(Guid productId, Guid optionId)
        {
            return String.Format("{0}/products/{1}/options/{2}", BaseUrl(), productId, optionId);
        }
    }
}