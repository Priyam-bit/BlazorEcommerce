using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared
{
    public class ProductVariant
    {
        //eg for books -> paperback, ebook

        //composite key
        //i.e. the key of ProductVariant will consist of product Id and product type Id
        [JsonIgnore] //since product will reference ProductVariant and ProductVariant references 
                     //Product, we ignore it in serialization to prevent circular reference

        public Product Product { get; set; }
        public int ProductId { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }

        //sale price
        [Column(TypeName="decimal(18,2)")]
        public decimal Price { get; set; }

        //original price
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }
    }
}
