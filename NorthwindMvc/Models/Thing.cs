using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindMvc.Models
{
    public class Thing
    {
        [Range(1,10)]
        public int? ID { get; set; }
        [Required]
        public string Color { get; set; }
    }
}
