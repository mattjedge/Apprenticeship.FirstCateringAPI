using System.Collections.Generic;

namespace FirstCateringAPI.Core.Dtos.LinksAndWrappers
{
    public class ResourceLinksBaseDto
    {
        public List<LinkDto> Links { get; set; } =
            new List<LinkDto>();
    }
}