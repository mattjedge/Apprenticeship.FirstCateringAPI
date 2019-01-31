using AutoMapper;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.BusinessLogic.Implementations;
using FirstCateringAPI.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.FirstCateringAPITest.BusinessLogicTests
{
    [TestFixture]
    public class EmployeesLogicTests
    {
        [SetUp]
        public void Setup()
        {
            _mockEmployeeRepo = new Mock<IEmployeesRepo>();
            _mockEncryption = new Mock<IEncryption>();
            _mockMapper = new Mock<IMapper>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockConfiguration = new Mock<IConfiguration>();
            _classUnderTest = new EmployeesLogic(_mockEmployeeRepo.Object, _mockEncryption.Object, 
                _mockMapper.Object, _mockUrlHelper.Object, _mockConfiguration.Object);
        }

        public EmployeesLogic _classUnderTest;
        public Mock<IEmployeesRepo> _mockEmployeeRepo;
        public Mock<IMapper> _mockMapper;
        public Mock<IUrlHelper> _mockUrlHelper;
        public Mock<IEncryption> _mockEncryption;
        public Mock<IConfiguration> _mockConfiguration;
    }
}
