using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.ViewModels
{
    public class BasketViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Image { get; set; }

        public int Count { get; set; } = 1;
        public double TotalAmount { get; set; } = 0;

    }
}
