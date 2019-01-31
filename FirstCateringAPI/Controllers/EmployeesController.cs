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
        public IActionResult Login(int employeeId, [FromBody]EmployeeCredentialsDto credentials)
        {          
            if (employeeId != credentials.EmployeeId)
            {
                return BadRequest(new { message = "Credentials and ID don't match." });
            }

            var employee = _employeeLogic.GetEmployee(employeeId);
            
            if (employee == null)
            {
                return NotFound(new { message = $"No employee with Employee Id {employeeId} was found." });
            }

            if (_employeeLogic.AuthorizedEmployee(credentials))
            {
                var employeeName = employee.Forename;
                var welcomeString = $"Welcome, {employeeName}";
                
                return Ok(new { welcome = welcomeString });
            }
            else
            {
                return BadRequest(new { message = "Unable to authenticate employee" });
            }
        }
        

        [HttpPost("{employeeId}/Logout")]
        public IActionResult Logout(int employeeId)
        {
            var employee = _employeeLogic.GetEmployee(employeeId);

            if (employee == null)
            {
                return NotFound(new { message = $"No employee with Employee Id {employeeId} was found." });
            }

            var employeeName = employee.Forename;
            var farewellString = $"Goodbye, {employeeName}.";

            return Ok(new { farewell = farewellString });
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

            //if (contentType != "application/json")
            //{
            //    return StatusCode(415); // unsupported mediatype, forcing consumer to use json (for simplicity)
            //}

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid model state."});
            }

            if (_employeeLogic.EmployeeIdExists(employeeToRegister.EmployeeId))
            {
                return BadRequest(new { message = "Provided EmployeeId already exists in the system." });
            }

            _employeeLogic.RegisterEmployee(employeeToRegister);

            if (!_employeeLogic.Save())
            {
                throw new Exception("Registering user failed on save");
            }
                       
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
                return NotFound(new { message = $"No employee with Employee Id {employeeId} was found." });
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

        [HttpDelete("{employeeId}", Name = "DeleteEmployee")]
        public IActionResult DeleteEmployee(int employeeId)
        {
            var employeeToDelete = _employeeLogic.GetEmployee(employeeId);

            if (employeeToDelete == null)
            {
                return NotFound(new { message = $"EmployeeId {employeeId} not found in the system." });
            }

            _employeeLogic.DeleteEmployee(employeeId);

            if (!_employeeLogic.Save())
            {
                throw new Exception($"Deleting employee with Id {employeeId} failed on save.");
            }

            return NoContent();
        }
    }
}