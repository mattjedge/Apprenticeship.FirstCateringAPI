using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstCateringAPI.Core.Entities
{
    public class Employee
    {
        [Key]
        public int pkAutoId { get; set; }

        public int EmployeeId { get; set; }

        [Required,MaxLength(25)]
        public string Forename { get; set; }

        [Required,MaxLength(25)]
        public string Surname { get; set; }

        [Required,MaxLength(50)]
        public string EmailAddress { get; set; }

        [Required,MaxLength(20)]
        public string MobileNumber { get; set; }

        //[MaxLength(4),RegularExpression("^[0-9]*$", ErrorMessage ="PIN Number must be four digits long")]
        [Required]
        public string PINNumber { get; set; }

        [ForeignKey("CardId")]
        public MembershipCard MembershipCard { get; set; }

        public Guid CardId { get; set; }
    }
}