using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace ProductAPI.Infrastructure
{
    public static class AutoMappings
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Product, Models.Product>();
                cfg.CreateMap<Models.Product, Product>();

                cfg.CreateMap<ProductOption, Models.ProductOption>();
                cfg.CreateMap<Models.ProductOption, ProductOption>();

                cfg.CreateMap<Models.Products, List<Product>>()
                    .ConstructUsing(x => Mapper.Map<List<Product>>(x.Items));
                cfg.CreateMap<List<Product>, Models.Products>()
                    .ConstructUsing(x => new Models.Products() { Items = Mapper.Map<List<Models.Product>>(x) });

                cfg.CreateMap<Models.ProductOptions, List<ProductOption>>()
                    .ConstructUsing(x => Mapper.Map<List<ProductOption>>(x.Items));
                cfg.CreateMap<List<ProductOption>, Models.ProductOptions>()
                    .ConstructUsing(x => new Models.ProductOptions() { Items = Mapper.Map<List<Models.ProductOption>>(x) });
            });
        }
    }
}