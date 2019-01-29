using FirstCateringAPI.Core.Dtos.LinksAndWrappers;
using System;

namespace FirstCateringAPI.Core.Dtos.Cards
{
    public class MembershipCardLinksDto : ResourceLinksBaseDto
    {
        public Guid CardId { get; set; }
               
        public decimal CurrentBalance { get; set; }
    }
}