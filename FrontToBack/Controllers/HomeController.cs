using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {

            //Response.Cookies.Append("cookie", "Hello", new CookieOptions { Expires = System.DateTimeOffset.Now.AddHours(1) });


            var sliderImage = await _dbContext.SliderImages.ToListAsync();
            var slider = await _dbContext.Sliders.SingleOrDefaultAsync();
            var category = await _dbContext.Categories.ToListAsync();
           // var product = _dbContext.Products.Include(x=>x.Category).Take(4).ToListAsync();
            var aboutsection = await _dbContext.AboutSections.SingleOrDefaultAsync();
            var aboutsectionimage = await _dbContext.AboutSectionImages.SingleOrDefaultAsync();
            var aboutsectionlist = await _dbContext.AboutSectionLists.ToListAsync();
            var expertsection = await _dbContext.ExpertSections.SingleOrDefaultAsync();
            var expertsectionlist = await _dbContext.ExpertSectionLists.ToListAsync();
            var blog = await _dbContext.Blogs.SingleOrDefaultAsync();
            var bloglist = await _dbContext.BlogLists.ToListAsync();
            var slidersay = await _dbContext.SliderSays.ToListAsync();
            var subscribe = await _dbContext.Subscribes.SingleOrDefaultAsync();
            var instagram = await _dbContext.Instagrams.ToListAsync();

            return View(new HomeViewModel { 
                SliderImage=  sliderImage,
                Slider=  slider,
                Categories=  category,
               //Products= await product,
                AboutSections= aboutsection,
                AboutSectionImages=  aboutsectionimage,
                AboutSectionLists=  aboutsectionlist,
                ExpertSections =  expertsection,
                ExpertSectionLists=  expertsectionlist,
                Blogs=  blog,
                BlogLists=  bloglist,
                SliderSays=  slidersay,
                Subscribes=  subscribe,
                Instagrams=  instagram
            });
        }

        public async Task<IActionResult> Basket()
        {
            var basket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basket))
            {
                return Content("empty");
            }

            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
            var newBasket = new List<BasketViewModel>();
            foreach (var basketViewModel in basketViewModels)
            {
                var product = await _dbContext.Products.FindAsync(basketViewModel.Id);
                if (product==null)
                    continue;

                newBasket.Add(new BasketViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    Count = basketViewModel.Count,
                    TotalAmount=product.Price*basketViewModel.Count
                });
            }

            basket = JsonConvert.SerializeObject(newBasket);
            Response.Cookies.Append("basket", basket);
            return View(newBasket);

        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null)
                return BadRequest();

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            List<BasketViewModel> basketViewModels; 
            var existbasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existbasket))
            {
                basketViewModels = new List<BasketViewModel>();
            }
            else
            {
                basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(existbasket);
            }

            var existBasketViewModel = basketViewModels.FirstOrDefault(x => x.Id == id);
            if (existBasketViewModel==null)
            {
                existBasketViewModel = new BasketViewModel
                {
                    Id = product.Id,
                    Price=product.Price
                };
                basketViewModels.Add(existBasketViewModel);
            }
            else
            {
                existBasketViewModel.Count++;   
            }


            var basket = JsonConvert.SerializeObject(basketViewModels);
            Response.Cookies.Append("basket", basket);
            return RedirectToAction(nameof(Index)); 

        }

        public IActionResult Delete(int? id)
        {
            if (id==null)
                return BadRequest();

            var existbasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existbasket))
                return BadRequest();

            var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(existbasket).Where(x=>x.Id!=id).ToList();
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products));
            return RedirectToAction(nameof(Basket));
        }

        public IActionResult Minus(int? id)
        {
            if (id == null)
                return BadRequest();
            var basket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basket))
                return BadRequest();
            var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
            foreach (var item in products)
            {
                if (item.Id == id)
                {
                    item.Count--;
                    if (item.Count == 0)
                        products = products.Where(x => x.Id != id).ToList();
                }
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products));
            return RedirectToAction(nameof(Basket));
        }

        public IActionResult Plus(int? id)
        {
            if (id == null)
                return BadRequest();

            var basket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basket))
                return BadRequest();

            var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
            foreach (var item in products)
            {
                if (item.Id == id)
                {
                    item.Count++;
                }
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products));
            return RedirectToAction(nameof(Basket));
        }

    }
}
