using System;

namespace DotNetCoreRest.Models
{
    public class AuthorDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }
    }
}