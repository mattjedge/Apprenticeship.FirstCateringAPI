using System;

namespace FirstCateringAPI.Core.Dtos.Cards
{
    public class MembershipCardOwnerDto
    {
        public Guid CardId { get; set; }
        public int EmployeeId { get; set; }
    }
}