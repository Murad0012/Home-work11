using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using P335_BackEnd.Entities;

namespace P335_BackEnd.Areas.Admin.Models
{
    public class ProductAddVM
    {
        public int ProductID { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        public int CategoryId { get; set; }
        [ValidateNever]
        public List<Category> Categories { get; set; }
        public int SelectedCategoryId { get; set; }

        [ValidateNever]
        public int? ProductTypeId { get; set; }
        [ValidateNever]
        public List<ProductType> ProductTypes { get; set; }
        public int SelectedProductTypeId { get; set; }

        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}