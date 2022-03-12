using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool Featured { get; set; } = false;
        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

        //many to one relation with category
        //? after Category allows this field to be nullable
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
    }
}
