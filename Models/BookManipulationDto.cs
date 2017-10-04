using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreRest.Models
{
    public class BookManipulationDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }
}