using System.ComponentModel.DataAnnotations;

namespace DotNetCoreRest.Models
{
    public class BookCreationDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }
}