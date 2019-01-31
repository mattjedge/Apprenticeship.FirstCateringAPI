using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Controllers;
using FirstCateringAPI.Core.Dtos.Cards;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;

namespace UnitTests.FirstCateringAPITest.ControllerTests
{
    [TestFixture]
    public class MembershipCardsControllerTests
    {
        [SetUp]
        public void Setup()
        {
            _mockCardLogic = new Mock<IMembershipCardLogic>();
            _mockConfiguration = new Mock<IConfiguration>();
            _classUnderTest = new MembershipCardsController(_mockCardLogic.Object, _mockConfiguration.Object);
        }

        public MembershipCardsController _classUnderTest;
        public Mock<IMembershipCardLogic> _mockCardLogic;
        public Mock<IConfiguration> _mockConfiguration;


        // VERIFY CARD TESTS
        [Test]
        public void MembershipCards_VerifyCard_Should_Call_MembershipCardExists()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            _classUnderTest.VerifyCard("", It.IsAny<Guid>());

            _mockCardLogic.Verify(x => x.MembershipCardExists(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void MembershipCards_VerifyCard_Should_Return_NotFound_IfCardDoesntExist()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(false);

            var result = _classUnderTest.VerifyCard("", It.IsAny<Guid>());
            var objResult = result as NotFoundObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(404, objResult.StatusCode);
        }

        [Test]
        public void MembershipCards_VerifyCard_Should_Call_GetCardAndOwnerId()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);

            _classUnderTest.VerifyCard("", It.IsAny<Guid>());

            _mockCardLogic.Verify(x => x.GetCardAndOwnerId(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void MembershipCards_VerifyCard_Should_Return_OkResult()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.GetCardAndOwnerId(It.IsAny<Guid>())).Returns(new MembershipCardOwnerDto() { EmployeeId = 1 });

            var result = _classUnderTest.VerifyCard("", It.IsAny<Guid>());
            var objResult = result as OkObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(200, objResult.StatusCode);
        }



        // ADD CREDIT TESTS
        [Test]
        public void MembershipCards_AddCredit_Should_Return_BadRequest_If_ModelStateNotValid()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _classUnderTest.ModelState.AddModelError("", "");

            var result = _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId});
            var objResult = result as BadRequestObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(400, objResult.StatusCode);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_Return_NotFound_IfCardDoesntExist()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(false);

            var result = _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId });
            var objResult = result as NotFoundObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(404, objResult.StatusCode);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_Call_Authenticated()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);

            _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId });

            _mockCardLogic.Verify(x => x.Authenticated(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_Return_Unauthorized_IfAuthorizedFails()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);

            var result = _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId });

            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_Call_AddCredit()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Authenticated(It.IsAny<string>(), It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Save()).Returns(true);

            var result = _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId, PINNumber = "0000" });

            _mockCardLogic.Verify(x => x.AddCredit(It.IsAny<UpdateBalanceDto>()), Times.Once);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_Call_Save()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Authenticated(It.IsAny<string>(), It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Save()).Returns(true);

            var result = _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId, PINNumber = "0000"});

            _mockCardLogic.Verify(x => x.Save(), Times.Once);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_ThrowException_IfSaveFails()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Authenticated(It.IsAny<string>(), It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Save()).Returns(false);

            Assert.That(() => _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId, PINNumber = "0000" }),Throws.Exception);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_CallGetMembershipCard()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Authenticated(It.IsAny<string>(), It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Save()).Returns(true);

            _classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId, PINNumber = "0000" });

            _mockCardLogic.Verify(x => x.GetMembershipCard(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_Return_OkResult()
        {
            var cardId = Guid.NewGuid();
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.MembershipCardExists(It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Authenticated(It.IsAny<string>(), It.IsAny<Guid>())).Returns(true);
            _mockCardLogic.Setup(x => x.Save()).Returns(true);            

            var result =_classUnderTest.AddCredit(cardId, "", new UpdateBalanceDto() { CardId = cardId, PINNumber = "0000" });
            var objResult = result as OkObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(200, objResult.StatusCode);
        }

        [Test]
        public void MembershipCards_AddCredit_Should_Return_BadRequest_IfURICardIdAndUpdateBalanceCardIdDontMatch()
        {
            var result = _classUnderTest.AddCredit(Guid.NewGuid(), "", new UpdateBalanceDto() { CardId = Guid.NewGuid(), PINNumber = "0000" });
            var objResult = result as BadRequestObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(400, objResult.StatusCode);
        }


        // GET MEMBERSHIP CARD TESTS
        [Test]
        public void MembershipCards_GetMembershipCard_Should_Call_GetMembershipCard()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            _classUnderTest.GetMembershipCard("", It.IsAny<Guid>());

            _mockCardLogic.Verify(x => x.GetMembershipCard(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void MembershipCards_GetMembershipCard_Should_Return_NotFound_IfCardDoesntExist()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.GetMembershipCard(It.IsAny<Guid>())).Returns(It.IsAny<MembershipCardDto>());

            var result = _classUnderTest.GetMembershipCard("", It.IsAny<Guid>());
            var objResult = result as NotFoundObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(404, objResult.StatusCode);
        }

        [Test]
        public void MembershipCards_GetMembershipCard_Should_Return_Ok()
        {
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            _mockCardLogic.Setup(x => x.GetMembershipCard(It.IsAny<Guid>())).Returns(new MembershipCardDto() { CurrentBalance = 50 });

            var result = _classUnderTest.GetMembershipCard("", It.IsAny<Guid>());
            var objResult = result as OkObjectResult;

            Assert.IsNotNull(objResult);
            Assert.AreEqual(200, objResult.StatusCode);
        }
    }
}
