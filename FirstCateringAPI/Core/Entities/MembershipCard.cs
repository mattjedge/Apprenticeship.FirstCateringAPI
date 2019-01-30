using System;
using System.ComponentModel.DataAnnotations;

namespace FirstCateringAPI.Core.Entities
{
    public class MembershipCard
    {
        [Key]
        public int pkCardAutoId { get; set; }

        [Required]
        public Guid CardId { get; set; }

        [Required]
        public decimal CurrentBalance { get; set; }
        
        public Employee Employee { get; set; }
    }
}