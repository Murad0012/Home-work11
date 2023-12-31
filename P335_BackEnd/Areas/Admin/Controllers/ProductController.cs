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
            newProduct.Price = (decimal)model.Price;

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

        public IActionResult Update(int? id)
        {
            if (id is null) return NotFound();

            var product = _dbContext.Products.Include(x => x.ProductTypeProducts).FirstOrDefault(x => x.Id == id);

            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var productTypes = _dbContext.ProductTypes.AsNoTracking().ToList();

            if (product is null) return NotFound();

            var model = new ProductUpdateVM
            {
                Name = product.Name,
                Price = product.Price,
                Categories = categories,
                ProductTypes = productTypes,
                CategoryId = product.CategoryId,
                ProductTypeId = product.ProductTypeProducts.FirstOrDefault()?.ProductTypeId,
                ImageUrl = product.ImageUrl,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(ProductUpdateVM model)
        {
            var product = _dbContext.Products.Include(x => x.ProductTypeProducts).FirstOrDefault(x => x.Id == model.Id);
            if (product is null) return NotFound();

            if (model.ImageUrl != product.ImageUrl && model.Image is null)
            {
                _fileService.DeleteFile(product.ImageUrl && Path.Combine("img", "featured"));
                product.ImageUrl = null;
            }
            else if (model.Image != null)
            {
                _fileService.DeleteFile(product.ImageUrl && Path.Combine("img", "featured"));
                product.ImageUrl = _fileService.AddFile(model.Image, Path.Combine("img", "featured"));
            }

            product.Name = model.Name;
            product.Price = (decimal)model.Price;
            product.CategoryId = model.CategoryId;
            product.ProductTypeProducts = new List<ProductTypeProduct>()
            {
                new()
                {
                    ProductTypeId = (int)model.ProductTypeId
                }
            };

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}