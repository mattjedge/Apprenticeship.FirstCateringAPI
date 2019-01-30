using AutoMapper;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Core.Dtos.Employees;
using FirstCateringAPI.Core.Dtos.LinksAndWrappers;
using FirstCateringAPI.Core.Entities;
using FirstCateringAPI.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FirstCateringAPI.BusinessLogic.Implementations
{
    public class EmployeesLogic : BaseLogic, IEmployeesLogic
    {
        private readonly IEmployeesRepo _repo;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public EmployeesLogic(IEmployeesRepo repo, IMapper mapper, IUrlHelper urlHelper) : base(repo)
        {
            _repo = repo;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        public bool EmployeeIdExists(int employeeId)
        {
            return _repo.EmployeeIdExists(employeeId);
        }

        public void RegisterEmployee(RegisterEmployeeDto employee)
        {
            var employeeToAdd = _mapper.Map<Employee>(employee);            

            _repo.AddNewEmployee(employeeToAdd);
        }

        public EmployeeDto GetEmployee(int employeeId)
        {
            var employeeFromRepo = _repo.GetEmployee(employeeId);
            var employeeDto = _mapper.Map<EmployeeDto>(employeeFromRepo);

            return employeeDto;
        }

        public EmployeeLinksDto AddHateoasLinks(EmployeeDto employee)
        {
            var employeeLinksWrapper = _mapper.Map<EmployeeLinksDto>(employee);
            employeeLinksWrapper.Links.Add(new LinkDto(_urlHelper.Link("GetEmployee", new { employeeId = employee.EmployeeId }), "self", "GET"));
            employeeLinksWrapper.Links.Add(new LinkDto(_urlHelper.Link("DeleteEmployee", new { employeeId = employee.EmployeeId }), "delete", "DELETE"));
            employeeLinksWrapper.Links.Add(new LinkDto(_urlHelper.Link("GetMembershipCard", new { cardId = employee.CardId }), "membershipCard", "GET"));

            return employeeLinksWrapper;
        }

        public bool AuthorizedEmployee(EmployeeCredentialsDto credentials)
        {
            var employeeId = credentials.EmployeeId;
            var pinNumber = credentials.PINNumber;
            var cardId = credentials.CardId;

            return _repo.AuthorizedEmployee(employeeId, pinNumber, cardId);
        }
    }
}
