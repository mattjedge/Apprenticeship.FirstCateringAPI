﻿using FirstCateringAPI.Core.Entities;
using System;

namespace FirstCateringAPI.DataAccess.Contracts
{
    public interface IEmployeesRepo : IRepository
    {
        bool EmployeeIdExists(int employeeId);

        void AddNewEmployee(Employee employee);

        Employee GetEmployee(int employeeId);

        bool AuthorizedEmployee(int employeeId, string PINNumber, Guid cardId);

        void DeleteEmployee(Employee employee);
    }
}