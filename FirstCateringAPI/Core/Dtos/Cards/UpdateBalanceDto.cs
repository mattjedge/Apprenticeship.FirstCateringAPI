using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCateringAPI.Core.Dtos.Cards
{
    public class UpdateBalanceDto
    {
        [Required]
        public Guid CardId { get; set; }

        [Required]
        [RegularExpression(@"^\d+.?\d{0,2}$")]
        public decimal Credit { get; set; }

        [RegularExpression(@"\d{4}")]
        public int PINNumber { get; set; }
    }
}
