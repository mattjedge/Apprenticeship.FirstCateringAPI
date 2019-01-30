using System;
using System.ComponentModel.DataAnnotations;

namespace FirstCateringAPI.Core.Dtos.Employees
{
    public class EmployeeCredentialsDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [RegularExpression(@"\d{4}")]
        public int PINNumber { get; set; }

        [Required]
        public Guid CardId { get; set; }
    }
}