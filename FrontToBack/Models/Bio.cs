using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Models
{
    public class Bio
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string FacebookUrl { get; set; }
        [StringLength(100)]
        public string LinkedinUrl { get; set; } 
    }
}
