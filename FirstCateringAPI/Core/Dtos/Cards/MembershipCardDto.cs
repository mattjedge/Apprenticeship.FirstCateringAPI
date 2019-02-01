using System;

namespace FirstCateringAPI.Core.Dtos.Cards
{
    public class MembershipCardDto
    {
        public Guid CardId { get; set; }
               
        public decimal CurrentBalance { get; set; }

        public int EmployeeId { get; set; }        
    }
}