using System;
using System.Data.SqlClient;
using Newtonsoft.Json;
using ProductAPI.Services;
using System.Collections.Generic;

namespace ProductAPI.Models
{
    public class ProductOptions
    {
        public List<ProductOption> Items { get; set; }

        public ProductOptions()
        {
        }

        public ProductOptions(Guid productId)
        {
        }

        private void LoadProductOptions(string where)
        {
            Items = new List<ProductOption>();
            var conn = Helpers.NewConnection();
            var cmd = new SqlCommand($"select id from productoption {where}", conn);
            conn.Open();

            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var id = Guid.Parse(rdr["id"].ToString());
                Items.Add(new ProductOption(id));
            }
        }
    }
}