using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models
{
    public class MyFile
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "File Name")]
        [StringLength(60, MinimumLength = 3)]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "File")]
        public IFormFile File { get; set; }
    }
}
