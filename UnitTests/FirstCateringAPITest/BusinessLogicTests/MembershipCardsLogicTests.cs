using AutoMapper;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.BusinessLogic.Implementations;
using FirstCateringAPI.Core.Dtos.Cards;
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
            _mockConfiguration = new Mock<IConfiguration>();
            _classUnderTest = new MembershipCardLogic(_mockCardsRepo.Object, _mockEncryption.Object, _mockMapper.Object, 
                                        _mockUrlHelper.Object, _mockConfiguration.Object);
        }

        public MembershipCardLogic _classUnderTest;
        public Mock<IMembershipCardsRepo> _mockCardsRepo;
        public Mock<IEncryption> _mockEncryption;
        public Mock<IMapper> _mockMapper;
        public Mock<IUrlHelper> _mockUrlHelper;
        public Mock<IConfiguration> _mockConfiguration;

        [Test]
        public void CardLogic_Should_Call_UrlHelperLink()
        {
            _mockMapper.Setup(x => x.Map<MembershipCardLinksDto>(new MembershipCardOwnerDto() { EmployeeId = 10101 }));//.Returns(new MembershipCardLinksDto() { CurrentBalance = 1});
            _mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), new { }));
            
            _classUnderTest.AddHateoasLinks(new MembershipCardDto() {EmployeeId = 10101 });

            _mockUrlHelper.Verify(x => x.Link(It.IsAny<string>(), new { }), Times.Once);
        }
    }
}
