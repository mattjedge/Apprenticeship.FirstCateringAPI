using FirstCateringAPI.Core.Dtos.LinksAndWrappers;
using System;

namespace FirstCateringAPI.Core.Dtos.Employees
{
    public class EmployeeLinksDto : ResourceLinksBaseDto
    {
        public int EmployeeId { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public string MobileNumber { get; set; }

        public Guid CardId { get; set; }
    }
}