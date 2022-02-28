using FrontToBack.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class ProductsController : Controller    
    {
        private readonly AppDbContext _dbContext;
        private readonly int _productsCount;
        public ProductsController(AppDbContext dbContext)
        {   
            _dbContext = dbContext;
            _productsCount = _dbContext.Products.Count();
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.ProductsCount = _productsCount;
           // var products = await _dbContext.Products.Include(x => x.Category).Take(4).ToListAsync();
            return View(/*products*/);
        }

        public async Task<IActionResult> Scroll(int skip)   
        {
            if (skip >= _productsCount)
            {
                return BadRequest();
            }
            var product = await _dbContext.Products.Include(x => x.Category).Skip(skip).Take(4).ToListAsync();

            return PartialView("_ProductPartial", product);
        }
    }
}
