using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Core.Context;
using FirstCateringAPI.Core.Entities;
using FirstCateringAPI.DataAccess.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FirstCateringAPI.DataAccess.Implementations
{
    public class EmployeesRepo : Repository, IEmployeesRepo
    {
        private readonly FirstCateringContext _dbContext;

        public EmployeesRepo(FirstCateringContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddNewEmployee(Employee employee)
        {
            
            _dbContext.Employees.Add(employee);
            _dbContext.Entry(employee).State = EntityState.Added;

            var memberCard = new MembershipCard()
            {
                CardId = employee.CardId,
                CurrentBalance = (decimal)0.00
            };

            _dbContext.MembershipCards.Add(memberCard);
            _dbContext.Entry(memberCard).State = EntityState.Added;
        }


        public bool AuthorizedEmployee(int employeeId, string pinNumber, Guid cardId)
        {
            return _dbContext.Employees.Where(x => x.EmployeeId == employeeId && x.PINNumber == pinNumber && x.CardId == cardId).Any();
        }


        public bool EmployeeIdExists(int employeeId)
        {
            return _dbContext.Employees.Where(x => x.EmployeeId == employeeId).Any();
        }


        public Employee GetEmployee(int employeeId)
        {
            return _dbContext.Employees.AsNoTracking().SingleOrDefault(x => x.EmployeeId == employeeId);
        }

        public void DeleteEmployee(Employee employee)
        {
            _dbContext.Employees.Remove(employee);
            _dbContext.Entry(employee).State = EntityState.Deleted;
        }
    }
}