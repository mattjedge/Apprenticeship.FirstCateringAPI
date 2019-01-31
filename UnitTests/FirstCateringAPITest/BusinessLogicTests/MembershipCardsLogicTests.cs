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
    public class MembershipCardsLogicTests
    {
        [SetUp]
        public void Setup()
        {
            _mockCardsRepo = new Mock<IMembershipCardsRepo>();
            _mockEncryption = new Mock<IEncryption>();
            _mockMapper = new Mock<IMapper>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockCardsRepo = new Mock<IMembershipCardsRepo>();
            _classUnderTest = new MembershipCardLogic(_mockCardsRepo.Object, _mockEncryption.Object, _mockMapper.Object, 
                                        _mockUrlHelper.Object, _mockConfiguration.Object);
        }

        public MembershipCardLogic _classUnderTest;
        public Mock<IMembershipCardsRepo> _mockCardsRepo;
        public Mock<IEncryption> _mockEncryption;
        public Mock<IMapper> _mockMapper;
        public Mock<IUrlHelper> _mockUrlHelper;
        public Mock<IConfiguration> _mockConfiguration;



    }
}
