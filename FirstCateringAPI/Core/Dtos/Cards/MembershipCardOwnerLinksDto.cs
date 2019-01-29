using FirstCateringAPI.Core.Dtos.LinksAndWrappers;
using System;

namespace FirstCateringAPI.Core.Dtos.Cards
{
    public class MembershipCardOwnerLinksDto : ResourceLinksBaseDto
    {
        public Guid CardId { get; set; }
        public int EmployeeId { get; set; }
    }
}