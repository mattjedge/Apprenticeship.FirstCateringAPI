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


        /// <summary>
        /// Method used to return a login welcome object - { "welcome": "Welcome, EmployeeForename." }
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <param name="credentials">A json object containing EmployeeId, PINNumber, and CardID </param>
        /// <response code="200"> Successful login</response>
        /// <response code="404"> No employee with matching ID found</response>
        /// <response code="400"> Unable to authenticate employee (PIN / ID don't match) || EmployeeId parameter and EmployeeId in credentials don't match.</response>
        /// <returns></returns>
        [HttpPost("{employeeId}/Login",Name ="Login")]       
        [Produces("application/json")]
        [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(404)]
        public IActionResult Login(int employeeId, [FromBody]EmployeeCredentialsDto credentials)
        {          
            if (employeeId != credentials.EmployeeId)
            {
                return BadRequest(new { message = "Credentials EmployeeId and URI ID don't match." });
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
        
        /// <summary>
        /// This method returns a personalised Goodbye object- { "farewell": "Goodbye, EmployeeForename" }.
        /// </summary>
        /// <param name="employeeId">The ID of the employee that wishes to log out.</param>
        /// <response code="404"> No employee with matching ID found.</response>
        /// <response code="200"> Logout message successful</response>
        /// <returns></returns>
        [HttpPost("{employeeId}/Logout")]        
        [Produces("application/json")]
        [ProducesResponseType(404), ProducesResponseType(200)]
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
        

        /// <summary>
        /// This method 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="acceptMediaTypes"> application/vnd.catering.hateoas+json will return an Employee object including links to
        /// their membership card, self links, and delete links.</param>
        /// <param name="employeeToRegister">This Json object must contain all employee details. See RegisterEmployeeDto object at the bottom of this page.</param>
        /// <response code="201"></response>
        /// <returns></returns>
        [HttpPost("Register",Name ="Register")]
        [Produces("application/json", "application/vnd.catering.hateoas+json")]
        [ProducesResponseType(typeof(EmployeeDto), 201), ProducesResponseType(400), ProducesResponseType(401)]
        public IActionResult Register([FromHeader(Name ="Content-Type")]string contentType, [FromHeader(Name ="Accept")]string acceptMediaTypes, [FromBody]RegisterEmployeeDto employeeToRegister)
        {
            var hateoasHeader = _config.GetValue<string>("AppSettings:HateoasAcceptType");
            string[] acceptTypes = acceptMediaTypes.Split(", ");
            
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
        [ProducesResponseType(typeof(EmployeeDto),200), ProducesResponseType(404)]
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
        [ProducesResponseType(204), ProducesResponseType(404)]
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