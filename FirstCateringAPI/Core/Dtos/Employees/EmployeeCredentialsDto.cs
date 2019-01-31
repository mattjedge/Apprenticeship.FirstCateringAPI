using System;
using System.ComponentModel.DataAnnotations;

namespace FirstCateringAPI.Core.Dtos.Employees
{
    public class EmployeeCredentialsDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(4),RegularExpression("^[0-9]*$", ErrorMessage ="PIN Number must be four digits long")]
        public string PINNumber { get; set; }

        [Required]
        public Guid CardId { get; set; }
    }
}