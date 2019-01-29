using System;
using System.Linq;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Core.Dtos.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FirstCateringAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeesLogic _employeeLogic;
        private readonly IConfiguration _config;

        public EmployeesController(IEmployeesLogic employeeLogic, IConfiguration config)
        {
            _employeeLogic = employeeLogic;
            _config = config;
        }


       
        [HttpPost("{employeeId}/Login",Name ="Login")]
        public IActionResult Login([FromHeader(Name="Authorization")]string authHeader, int employeeId, [FromBody]EmployeeIdAndPINDto credentials)
        {
            var employee = _employeeLogic.GetEmployee(employeeId);
            
            if (employee == null)
            {
                return NotFound($"No employee with Employee Id {employeeId} was found.");
            }

            if (_employeeLogic.AuthorizedEmployee(credentials))
            {
                var employeeName = employee.Forename;
                var welcomeString = $"Welcome, {employeeName}";

                return Ok(welcomeString);
            }
            else
            {
                return BadRequest("Unable to authenticate employee");
            }
        }



        [HttpPost("{employeeId}/Logout")]
        public IActionResult Logout(int employeeId)
        {
            var employee = _employeeLogic.GetEmployee(employeeId);

            if (employee == null)
            {
                return NotFound($"No employee with Employee Id {employeeId} was found.");
            }

            var employeeName = employee.Forename;
            var farewellString = $"Goodbye, {employeeName}.";
            return Ok(farewellString);
        }



        [HttpPost("Register",Name ="Register")]
        [ProducesResponseType(typeof(EmployeeDto),201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Register([FromHeader(Name ="Content-Type")]string contentType, 
            [FromHeader(Name ="Accept")]string acceptMediaTypes, 
            [FromBody]RegisterEmployeeDto employeeToRegister)
        {
            var hateoasHeader = _config.GetValue<string>("AppSettings:HateoasAcceptType");
            string[] acceptTypes = acceptMediaTypes.Split(", ");

            if (contentType != "application/json")
            {
                return StatusCode(415); // unsupported mediatype, forcing consumer to use json (for simplicity)
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (_employeeLogic.EmployeeIdExists(employeeToRegister.EmployeeId))
            {
                return BadRequest("Provided EmployeeId already exists in the system.");
            }

            if (!_employeeLogic.RegisterEmployee(employeeToRegister))
            {
                throw new Exception("Registering user failed on save");
            }

            // return ok? return no content? return CreatedAtRoute?
            var employeeToReturn = _employeeLogic.GetEmployee(employeeToRegister.EmployeeId);

            if (acceptTypes.Contains(hateoasHeader))
            {
                var hateoasEmployee = _employeeLogic.AddHateoasLinks(employeeToReturn);
                return CreatedAtRoute("GetEmployee", new { employeeId = hateoasEmployee.EmployeeId }, hateoasEmployee);
            }

            return CreatedAtRoute("GetEmployee", new { employeeToReturn.EmployeeId }, employeeToReturn);
        }



        [HttpGet("{employeeId}",Name ="GetEmployee")]
        [Produces("application/vnd.catering.hateoas+json", "application/json")]
        public IActionResult GetEmployee([FromHeader(Name ="Accept")]string acceptMediaTypes, int employeeId)
        {
            var hateoasHeader = _config.GetValue<string>("application/vnd.catering.hateoas+json");
            var employeeDto = _employeeLogic.GetEmployee(employeeId);

            if (employeeDto == null)
            {
                return NotFound();
            }

            string[] acceptTypes = acceptMediaTypes.Split(", ");

            if (acceptTypes.Contains(hateoasHeader))
            {
                Response.Headers.Add("Content-Type", "application/vnd.catering.hateoas+json");
                var hateoasEmployee = _employeeLogic.AddHateoasLinks(employeeDto);

                return Ok(hateoasEmployee);
            }

            return Ok(employeeDto);
        }
    }
}