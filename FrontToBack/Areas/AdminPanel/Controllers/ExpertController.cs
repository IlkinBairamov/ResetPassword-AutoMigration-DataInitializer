using FrontToBack.Areas.AdminPanel.Data;
using FrontToBack.Areas.AdminPanel.Data.NewFolder;
using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    public class ExpertController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ExpertController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            var expertPhoto = await _dbContext.ExpertSectionLists.ToListAsync();
            return View(expertPhoto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertSectionList expertSectionList)
        {
            if (!ModelState.IsValid)
                return View();

            if (!expertSectionList.Photo.isImage())
            {
                ModelState.AddModelError("Photo", "This is not a Photo");
                return View();
            }
            if (!expertSectionList.Photo.IsAllowedSize(1))
            {
                ModelState.AddModelError("Photo", "Choose size under 1 mb.");
                return View();
            }

            var webRootPath = _environment.WebRootPath;
            var fileName = $"{Guid.NewGuid()}-{expertSectionList.Photo.FileName}";
            var path = Path.Combine(webRootPath, "img", fileName);

            var fileStream = new FileStream(path, FileMode.CreateNew);
            await expertSectionList.Photo.CopyToAsync(fileStream);

            expertSectionList.Image = fileName;
            await _dbContext.ExpertSectionLists.AddAsync(expertSectionList);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var expertSectionList = await _dbContext.ExpertSectionLists.FindAsync(id);
            if (expertSectionList == null)
                return NotFound();

            return View(expertSectionList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteExpert(int? id)  
        {
            if (id == null)
                return BadRequest();

            var expertSectionList = await _dbContext.ExpertSectionLists.FindAsync(id);
            if (expertSectionList == null)
                return NotFound();

            string pathDelete = Path.Combine(Constants.ImageFolderPath, expertSectionList.Image);
            if (System.IO.File.Exists(pathDelete))
            {
                System.IO.File.Delete(pathDelete);
            }

            _dbContext.ExpertSectionLists.Remove(expertSectionList);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var expertSectionList = await _dbContext.ExpertSectionLists.FindAsync(id);
            if (expertSectionList == null)
                return NotFound();

            return View(expertSectionList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ExpertSectionList expertSectionList)
        {

            if (id == null)
                return NotFound();

            if (id != expertSectionList.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View();

            var exist = await _dbContext.ExpertSectionLists.FindAsync(id);
            if (exist == null)
                return NotFound();

            string pathDelete = Path.Combine(Constants.ImageFolderPath, exist.Image);
            if (System.IO.File.Exists(pathDelete))
            {
                System.IO.File.Delete(pathDelete);
            }

            if (expertSectionList.Photo!=null)
            {
                if (expertSectionList.Photo.Length > 1024 * 1000)
                {
                    ModelState.AddModelError("Photo", "Choose size under 1 mb.");
                    return View();
                }
                var webRootPath = _environment.WebRootPath;
                var fileName = $"{Guid.NewGuid()}-{expertSectionList.Photo.FileName}";
                var path = Path.Combine(webRootPath, "img", fileName);

                var fileStream = new FileStream(path, FileMode.CreateNew);
                await expertSectionList.Photo.CopyToAsync(fileStream);

                exist.Image = fileName;
            }
            exist.Title = expertSectionList.Title;
            exist.Subtitle = expertSectionList.Subtitle;
            
            await _dbContext.SaveChangesAsync();

          
         


            return RedirectToAction(nameof(Index));
        }
    }
}
