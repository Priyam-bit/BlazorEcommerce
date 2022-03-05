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
        [Column(TypeName = "decimal(18,2)")]  //define decimal places (total digits, digits after .)
        public decimal Price { get; set; }

        //many to one relation with category
        //? after Category allows this field to be nullable
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
    }
}
