using FrontToBack.Areas.AdminPanel.Data;
using FrontToBack.Areas.AdminPanel.Data.NewFolder;
using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize]
    public class SliderImagesController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public SliderImagesController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        public async Task<IActionResult>  Index()
        {
            var sliderImages = await _dbContext.SliderImages.ToListAsync();
            ViewBag.Count = _dbContext.SliderImages.Count();
         
            return View(sliderImages);
          
        }
        public IActionResult Create()
        {
            ViewBag.Count = _dbContext.SliderImages.Count();
            if (ViewBag.Count >= 5)
            {
                return Content("max lenght 5");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderImage sliderImage)
        {
            if (!ModelState.IsValid)
                return View();
            var imageCount = _dbContext.SliderImages.Count();
           
            if (sliderImage.Photos.Count()+imageCount>5)
            {
                ModelState.AddModelError("Photos", $"{5- imageCount}can be selected");
                return View();
            }

            foreach (var photo in sliderImage.Photos)
            {
                if (!photo.isImage())
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} - Yuklediyiniz shekil olmalidir.");
                    return View();
                }

                if (!photo.IsAllowedSize(1))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} - shekil 1 mb-dan az olmalidir.");
                    return View();
                }

                var fileName = await photo.GenerateFile(Constants.ImageFolderPath);

                var newSliderImage = new SliderImage { Name = fileName };
                await _dbContext.SliderImages.AddAsync(newSliderImage);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var sliderImage = await _dbContext.SliderImages.FindAsync(id);
            if (sliderImage == null)
                return NotFound();

            return View(sliderImage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSlider(int? id)  
        {
            if (id == null)
                return BadRequest();

            var sliderImage = await _dbContext.SliderImages.FindAsync(id);
            if (sliderImage == null)
                return NotFound();

            ViewBag.Count = _dbContext.SliderImages.Count();

            var imageCount = _dbContext.SliderImages.Count();
            if (imageCount==1)
            {

                return Content("You Cant Deleted");
            }

            string pathDelete = Path.Combine(Constants.ImageFolderPath, sliderImage.Name);
            if (System.IO.File.Exists(pathDelete))
            {
                System.IO.File.Delete(pathDelete);
            }

            _dbContext.SliderImages.Remove(sliderImage);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
