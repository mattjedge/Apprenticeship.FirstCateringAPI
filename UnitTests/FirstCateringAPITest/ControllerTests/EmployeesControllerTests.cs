using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Controllers;
using FirstCateringAPI.Core.Dtos.Employees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace UnitTests.FirstCateringAPITest.ControllerTests
{
    [TestFixture]
    public class EmployeesControllerTests
    {
        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockEmployeeLogic = new Mock<IEmployeesLogic>();
            _mockCardLogic = new Mock<IMembershipCardLogic>();
            _classUnderTest = new EmployeesController(_mockEmployeeLogic.Object, _mockCardLogic.Object, _mockConfiguration.Object);
        }

        public EmployeesController _classUnderTest;
        public Mock<IConfiguration> _mockConfiguration;
        public Mock<IEmployeesLogic> _mockEmployeeLogic;
        public Mock<IMembershipCardLogic> _mockCardLogic;

        // LOGIN TESTS
        [Test]
        public void Employees_Login_Should_Call_GetEmployee()
        {
            var employeeId = 10101;
            _classUnderTest.Login(employeeId, new EmployeeCredentialsDto() { EmployeeId = employeeId});

            _mockEmployeeLogic.Verify(x => x.GetEmployee(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Employees_Login_Should_Call_AuthorizedEmployee()
        {
            var employeeId = 10101;
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "Test" });

            _classUnderTest.Login(employeeId, new EmployeeCredentialsDto() { EmployeeId = employeeId });

            _mockEmployeeLogic.Verify(x => x.AuthorizedEmployee(It.IsAny<EmployeeCredentialsDto>()), Times.Once);
        }



        [Test]
        public void Employees_Login_Should_Return_OkResult()
        {
            var employeeId = 10101;
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "Test" });
            _mockEmployeeLogic.Setup(x => x.AuthorizedEmployee(It.IsAny<EmployeeCredentialsDto>())).Returns(true);

            var result = _classUnderTest.Login(employeeId, new EmployeeCredentialsDto() { EmployeeId = employeeId });
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public void Employees_Login_Should_Return_BadRequestResult_IfAuthFails()
        {
            var employeeId = 10101;
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "Test" });

            var result = _classUnderTest.Login(employeeId, new EmployeeCredentialsDto() { EmployeeId = employeeId });
            var badResult = result as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);
        }

        [Test]
        public void Employees_Login_Should_Return_NotFoundResult_IfEmployeeNull()
        {
            var employeeId = 10101;
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(It.IsAny<EmployeeDto>());
            _mockEmployeeLogic.Setup(x => x.AuthorizedEmployee(It.IsAny<EmployeeCredentialsDto>())).Returns(false);

            var result = _classUnderTest.Login(employeeId, new EmployeeCredentialsDto() { EmployeeId = employeeId });
            var notFound = result as NotFoundObjectResult;

            Assert.IsNotNull(notFound);
            Assert.AreEqual(404, notFound.StatusCode);
        }


        // LOG OUT TESTS
        [Test]
        public void Employees_Logout_Should_Call_GetEmployee()
        {
            _classUnderTest.Logout(It.IsAny<int>());

            _mockEmployeeLogic.Verify(x => x.GetEmployee(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Employees_Logout_Should_Return_OkResult()
        {
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "Test" });

            var result = _classUnderTest.Logout(It.IsAny<int>());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public void Employees_Logout_Should_Return_NotFoundResult_IfEmployeeNull()
        {
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(It.IsAny<EmployeeDto>());

            var result = _classUnderTest.Logout(It.IsAny<int>());
            var notFound = result as NotFoundObjectResult;

            Assert.IsNotNull(notFound);
            Assert.AreEqual(404, notFound.StatusCode);
        }


        // REGISTER TESTS
        [Test]
        public void Employees_Register_Should_Return_BadRequestResult_IfModelNotValid()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _classUnderTest.ModelState.AddModelError("details required", "details required");

            var result = _classUnderTest.Register(It.IsAny<string>(), "test/test", It.IsAny<RegisterEmployeeDto>());
            var badRequest = result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

        }
        [Test]
        public void Employees_Register_Should_Call_EmployeeIdExists()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(It.IsAny<int>())).Returns(true);
            
            var result = _classUnderTest.Register(It.IsAny<string>(), "test/test", new RegisterEmployeeDto() { Forename = "test" });

            _mockEmployeeLogic.Verify(x => x.EmployeeIdExists(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Employees_Register_Should_Return_BadRequestResult_IfEmployeeExists()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(It.IsAny<int>())).Returns(true);
           
            var result = _classUnderTest.Register(It.IsAny<string>(), "test/test", new RegisterEmployeeDto() { EmployeeId = 1 });
            var badRequest = result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);
        }

        [Test]
        public void Employee_Register_Should_Call_RegisterEmployee()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(It.IsAny<int>())).Returns(false);
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "test" });

            var result = _classUnderTest.Register(It.IsAny<string>(), "test/test", new RegisterEmployeeDto() { Forename="test"});

            _mockEmployeeLogic.Verify(x => x.RegisterEmployee(It.IsAny<RegisterEmployeeDto>()), Times.Once);
        }

        [Test]
        public void Employee_Register_Should_Call_Save()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(1)).Returns(false);
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);
            _mockEmployeeLogic.Setup(x => x.GetEmployee(1)).Returns(new EmployeeDto() { Forename = "test" });
           
            var result = _classUnderTest.Register(It.IsAny<string>(), "test/test", new RegisterEmployeeDto() { EmployeeId = 1 });

            _mockEmployeeLogic.Verify(x => x.Save(), Times.Once);
        }

        [Test]
        public void Employee_Register_Should_Throw_Exception_IfSaveFails()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(1)).Returns(false);
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(false);                    

            Assert.That(() => _classUnderTest.Register(It.IsAny<string>(), "test/test", new RegisterEmployeeDto() { EmployeeId = 1 }), Throws.Exception);
        }

        [Test]
        public void Employee_Register_Should_Call_GetEmployee()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(1)).Returns(false);
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);
            _mockEmployeeLogic.Setup(x => x.GetEmployee(1)).Returns(new EmployeeDto() { Forename = "test" });

            _classUnderTest.Register(It.IsAny<string>(), "test/test", new RegisterEmployeeDto() { EmployeeId = 1 });

            _mockEmployeeLogic.Verify(x => x.GetEmployee(1), Times.Once);            
        }


        //[Test]
        //public void Employee_Register_Should_Call_AddHateoasLinks_IfHateoasHeader()
        //{
        //    _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
        //    _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(1)).Returns(false);
        //    _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);

        //    _classUnderTest.Register(It.IsAny<string>(), "application/vnd.catering.hateoas+json", new RegisterEmployeeDto() { EmployeeId = 1 });

        //    _mockEmployeeLogic.Verify(x => x.AddHateoasLinks(It.IsAny<EmployeeDto>()), Times.Once);
        //}

        [Test]
        public void Employee_Register_Should_Return_CreatedAtRouteResult()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.EmployeeIdExists(1)).Returns(false);
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);
            _mockEmployeeLogic.Setup(x => x.GetEmployee(1)).Returns(new EmployeeDto() { Forename = "test" });

            var result = _classUnderTest.Register(It.IsAny<string>(), "application/vnd.catering.hateoas+json", new RegisterEmployeeDto() { EmployeeId = 1 });
            var objResult = result as ObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(201, objResult.StatusCode);
        }



        // GET EMPLOYEE TESTS
        [Test]
        public void Employees_GetEmployee_Should_Call_GetEmployee()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            _classUnderTest.GetEmployee(It.IsAny<string>(), It.IsAny<int>());

            _mockEmployeeLogic.Verify(x => x.GetEmployee(It.IsAny<int>()), Times.Once);
        }
        [Test]
        public void Employees_GetEmployee_Should_Return_NotFoundResult_IfEmployeeNull()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            var result = _classUnderTest.GetEmployee(It.IsAny<string>(), It.IsAny<int>());
            var objResult = result as NotFoundObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(404, objResult.StatusCode);
            
        }
        [Test]
        public void Employees_GetEmployee_Should_Return_OkResult()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "test" });

            var result = _classUnderTest.GetEmployee("test", It.IsAny<int>());
            var objResult = result as OkObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(200, objResult.StatusCode);
        }


        // DELETE EMPLOYEE TESTS
        [Test]
        public void Employees_DeleteEmployee_Should_Call_GetEmployee()
        {
            _classUnderTest.DeleteEmployee(It.IsAny<int>());

            _mockEmployeeLogic.Verify(x => x.GetEmployee(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Employees_DeleteEmployee_Should_Return_NotFoundResult_IfEmployeeNull()
        {
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(It.IsAny<EmployeeDto>());

            var result = _classUnderTest.DeleteEmployee(It.IsAny<int>());
            var objResult = result as NotFoundObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(404, objResult.StatusCode);
        }

        [Test]
        public void Employees_DeleteEmployee_Should_Call_DeleteEmployee()
        {
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename="test"});
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);

            _classUnderTest.DeleteEmployee(1);

            _mockEmployeeLogic.Verify(x => x.DeleteEmployee(1), Times.Once);
        }

        [Test]
        public void Employees_DeleteEmployee_Should_Call_Save()
        {
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "test" });
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);

            _classUnderTest.DeleteEmployee(1);

            _mockEmployeeLogic.Verify(x => x.Save(), Times.Once);
        }

        [Test]
        public void Employees_DeleteEmployee_Should_ThrowException_IfSaveFails()
        {
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "test" });
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(false);

            Assert.That(() => _classUnderTest.DeleteEmployee(It.IsAny<int>()), Throws.Exception);
        }

        [Test]
        public void Employees_DeleteEmployee_Should_Return_NoContent()
        {
            _mockEmployeeLogic.Setup(x => x.GetEmployee(It.IsAny<int>())).Returns(new EmployeeDto() { Forename = "test" });
            _mockEmployeeLogic.Setup(x => x.Save()).Returns(true);

            var result = _classUnderTest.DeleteEmployee(1);

            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}