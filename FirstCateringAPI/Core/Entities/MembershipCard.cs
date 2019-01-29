using System;
using System.ComponentModel.DataAnnotations;

namespace FirstCateringAPI.Core.Entities
{
    public class MembershipCard
    {
        [Key]
        public Guid CardId { get; set; }

        [Required]
        [RegularExpression(@"^\d+.?\d{0,2}$")] // two decimal points
        public decimal CurrentBalance { get; set; }
        
        public Employee Employee { get; set; }
    }
}
