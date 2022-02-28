using FrontToBack.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public ProductViewComponent(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(int take=8)
        {
            var product = await _dbContext.Products.Include(x => x.Category).Take(take).ToListAsync();
            return View(product);
        }
    }
}
