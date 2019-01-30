using System.Collections.Generic;

namespace FirstCateringAPI.Core.Dtos.LinksAndWrappers
{
    // Adds HATEOAS links to a collection of resources (not implemented any collection requests)
    public class CollectionResourceLinksWrapperDto<T> : ResourceLinksBaseDto where T : ResourceLinksBaseDto
    {
        public IEnumerable<T> Value { get; set; }

        public CollectionResourceLinksWrapperDto(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
