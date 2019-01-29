using FirstCateringAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCateringAPI.Core.Dtos.Cards
{
    public class MembershipCardDto
    {
        public Guid CardId { get; set; }
               
        public decimal CurrentBalance { get; set; }
    }
}
