using FrontToBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderImage> SliderImage { get; set; }
        public Slider Slider { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public AboutSection AboutSections { get; set; }
        public AboutSectionImage AboutSectionImages { get; set; }
        public List<AboutSectionList> AboutSectionLists { get; set; }
        public ExpertSection ExpertSections { get; set; }
        public List<ExpertSectionList> ExpertSectionLists { get; set; }
        public Blog Blogs { get; set; } 
        public List<BlogList> BlogLists { get; set; }
        public List<SliderSay> SliderSays { get; set; }
        public Subscribe Subscribes { get; set; }
        public List<Instagram> Instagrams { get; set; }   
    }
}
