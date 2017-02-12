using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using ProductAPI.Services;


namespace ProductAPI.Models
{
    public class Products
    {
        public List<Product> Items { get; set; }

        public Products()
        {
        }

        public Products(string name)
        {
            LoadProducts($"where lower(name) like '%{name.ToLower()}%'");
        }

        private void LoadProducts(string where)
        {
            Items = new List<Product>();
            var conn = Helpers.NewConnection();
            var cmd = new SqlCommand($"select id from product {where}", conn);
            conn.Open();

            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var id = Guid.Parse(rdr["id"].ToString());
                Items.Add(new Product(id));
            }
        }
    }
}