using System;
using System.ComponentModel.DataAnnotations;

namespace FirstCateringAPI.Core.Dtos.Employees
{
    public class RegisterEmployeeDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required,MaxLength(25)]
        public string Forename { get; set; }

        [Required,MaxLength(25)]
        public string Surname { get; set; }

        [Required,EmailAddress,MaxLength(50)]
        public string EmailAddress { get; set; }

        [Required,MaxLength(20)]
        public string MobileNumber { get; set; }

        [Required, MaxLength(4), RegularExpression("^[0-9]*$", ErrorMessage ="PIN Number must be four digits long")]
        public string PINNumber { get; set; }

        [Required]
        public Guid CardId { get; set; }
    }
}