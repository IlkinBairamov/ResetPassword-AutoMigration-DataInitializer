using FrontToBack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AboutSection> AboutSections { get; set; }
        public DbSet<AboutSectionImage> AboutSectionImages { get; set; }
        public DbSet<AboutSectionList> AboutSectionLists { get; set; }
        public DbSet<ExpertSection> ExpertSections { get; set; }
        public DbSet<ExpertSectionList> ExpertSectionLists { get; set; }
        public DbSet<Blog> Blogs { get; set; }  
        public DbSet<BlogList> BlogLists { get; set; }      
        public DbSet<SliderSay> SliderSays { get; set; }      
        public DbSet<Subscribe> Subscribes { get; set; }      
        public DbSet<Instagram> Instagrams { get; set; }     
        public DbSet<Bio> Bios { get; set; }   
    
    }
}
