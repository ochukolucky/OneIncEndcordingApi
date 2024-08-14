using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class EncodeRequest
    {
        [Required(ErrorMessage = "text is required")]
        public string Input { get; set; }
    }
}
