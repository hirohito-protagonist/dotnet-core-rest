using System;
using System.Collections.Generic;

namespace DotNetCoreRest.Models
{
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}