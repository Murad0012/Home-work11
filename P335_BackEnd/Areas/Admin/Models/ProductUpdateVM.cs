using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using P335_BackEnd.Entities;
using System.ComponentModel.DataAnnotations;

namespace P335_BackEnd.Areas.Admin.Models
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        public int CategoryId { get; set; }
        [ValidateNever]
        public List<Category> Categories { get; set; }

        [ValidateNever]
        public int? ProductTypeId { get; set; }
        [ValidateNever]
        public List<ProductType> ProductTypes { get; set; }

        public string ImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}
