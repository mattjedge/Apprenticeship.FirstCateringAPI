using FirstCateringAPI.Core.Context;
using FirstCateringAPI.Core.Entities;
using FirstCateringAPI.DataAccess.Contracts;
using Microsoft.EntityFrameworkCore;
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

        public bool AddNewEmployee(Employee employee)
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

            _dbContext.Database.OpenConnection();
            try
            {
                _dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employees ON");
                _dbContext.SaveChanges();
                _dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employees OFF");
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _dbContext.Database.CloseConnection();
            }
            
            
        }

        public bool AuthorizedEmployee(int employeeId, int pinNumber)
        {
            return _dbContext.Employees.Where(x => x.EmployeeId == employeeId && x.PINNumber == pinNumber).Any();
        }

        public bool EmployeeIdExists(int employeeId)
        {
            return _dbContext.Employees.Where(x => x.EmployeeId == employeeId).Any();
        }

        public Employee GetEmployee(int employeeId)
        {
            return _dbContext.Employees.SingleOrDefault(x => x.EmployeeId == employeeId);
        }
    }
}
