using FirstCateringAPI.Core.Dtos.Employees;
using System;

namespace FirstCateringAPI.BusinessLogic.Contracts
{
    public interface IEmployeesLogic : IBaseLogic
    {
        void RegisterEmployee(RegisterEmployeeDto employeeToRegister);

        bool EmployeeIdExists(int employeeId);        

        EmployeeDto GetEmployee(int employeeId);

        EmployeeLinksDto AddHateoasLinks(EmployeeDto employee);

        bool AuthorizedEmployee(EmployeeCredentialsDto credentials);
    }
}
