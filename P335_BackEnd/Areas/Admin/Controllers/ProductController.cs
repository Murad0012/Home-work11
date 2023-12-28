using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P335_BackEnd.Areas.Admin.Models;
using P335_BackEnd.Data;
using P335_BackEnd.Entities;
using P335_BackEnd.Services;

namespace P335_BackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly FileService _fileService;

        public ProductController(AppDbContext dbContext, FileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            var products = _dbContext.Products.AsNoTracking().ToList();

            var model = new ProductIndexVM
            {
                Products = products
            };

            return View(model);
        }

        public IActionResult Add()
        {
            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var productTypes = _dbContext.ProductTypes.AsNoTracking().ToList();

            var model = new ProductAddVM
            {
                Categories = categories,
                ProductTypes = productTypes
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ProductAddVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newProduct = new Product();

            newProduct.Name = model.Name;
            newProduct.Price = (decimal)model.Price!;

            var foundCategory = _dbContext.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
            if (foundCategory is null) return View(model);

            newProduct.Category = foundCategory;

            if (model.ProductTypeId != null)
            {
                var foundProductType = _dbContext.ProductTypes.FirstOrDefault(x => x.Id == model.ProductTypeId);
                if (foundProductType is null) return View(model);

                newProduct.ProductTypeProducts = new()
                {
                    new ProductTypeProduct
                    {
                        ProductType = foundProductType
                    }
                };
            }

            newProduct.ImageUrl = _fileService.AddFile(model.Image, Path.Combine("img", "featured"));

            _dbContext.Add(newProduct);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(x => x.Id == id);

            if (product is null) return NotFound();

            _fileService.DeleteFile(product.ImageUrl, Path.Combine("img", "featured"));

            _dbContext.Remove(product);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(x => x.Id == id);

            var categories = _dbContext.Categories.AsNoTracking().ToList();

            var productTypes = _dbContext.ProductTypes.AsNoTracking().ToList();

            var productTypeProduct = _dbContext.ProductTypeProducts.
                FirstOrDefault(x => x.ProductId == product!.Id);

            ViewData["imageUrl"] = product!.ImageUrl;

            var model = new ProductAddVM
            {
                Name = product.Name,
                Price = product.Price,
                ProductID = product.Id,
                ImageUrl = product.ImageUrl,
                Categories = categories,
                ProductTypes = productTypes,
                SelectedCategoryId = product.CategoryId,
                SelectedProductTypeId = productTypeProduct!.ProductTypeId
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(ProductAddVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var productTypes = _dbContext.ProductTypes.AsNoTracking().ToList();

            var updatedProduct = _dbContext.Products.
                FirstOrDefault(x => x.Id == model.ProductID);

         
            updatedProduct!.Name = model.Name;
            updatedProduct.ImageUrl = model.ImageUrl;
            updatedProduct.Price = (decimal)model.Price!;
            updatedProduct.CategoryId = model.SelectedCategoryId;
            updatedProduct.ImageUrl = model.ImageUrl;

            if (model.SelectedProductTypeId is not 0)
            {
                var foundProductType = _dbContext.ProductTypes.
                    FirstOrDefault(x => x.Id == model.SelectedProductTypeId);

                if (foundProductType is null) return View(model);

                var foundProductTypeProduct = _dbContext.ProductTypeProducts.
                    FirstOrDefault(x => x.ProductTypeId == foundProductType.Id);

                if (foundProductTypeProduct is null) return View(model);

                foreach (var product in updatedProduct.ProductTypeProducts)
                {
                    product.ProductTypeId = foundProductType.Id;
                }
            }

            _dbContext.Update(updatedProduct);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteImage(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var newPath = Path.Combine
                    (Directory.GetCurrentDirectory(), "wwwroot//img/featured", path);

                if (System.IO.File.Exists(newPath)) System.IO.File.Delete(newPath);

            }

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Update));
        }

    }
}