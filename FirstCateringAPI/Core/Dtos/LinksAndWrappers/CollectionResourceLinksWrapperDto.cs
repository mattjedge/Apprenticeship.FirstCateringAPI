using System.Collections.Generic;

namespace FirstCateringAPI.Core.Dtos.LinksAndWrappers
{
    public class CollectionResourceLinksWrapperDto<T> : ResourceLinksBaseDto where T : ResourceLinksBaseDto
    {
        public IEnumerable<T> Value { get; set; }

        public CollectionResourceLinksWrapperDto(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
