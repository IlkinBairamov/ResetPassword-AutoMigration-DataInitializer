using FrontToBack.DataAccessLayer;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
      private readonly AppDbContext _dbContext;

      public HeaderViewComponent(AppDbContext dbContext)
      {
         _dbContext = dbContext;
      }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            double Total = 0;
            var count = 0;
            var basket = Request.Cookies["basket"];
            if (!string.IsNullOrEmpty(basket))
            {
                var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
                count = products.Count;
                foreach (var item in products)
                {
                    Total += item.Count * item.Price;
                }
            }
            ViewBag.BasketCount = count;
            ViewBag.BasketTotal = Total;

            return View();
        }
    }
}
