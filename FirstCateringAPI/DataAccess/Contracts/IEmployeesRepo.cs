using FirstCateringAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCateringAPI.DataAccess.Contracts
{
    public interface IEmployeesRepo : IRepository
    {
        bool EmployeeIdExists(int employeeId);

        bool AddNewEmployee(Employee employee);

        Employee GetEmployee(int employeeId);

        bool AuthorizedEmployee(int employeeId, int pinNumber);
    }
}
