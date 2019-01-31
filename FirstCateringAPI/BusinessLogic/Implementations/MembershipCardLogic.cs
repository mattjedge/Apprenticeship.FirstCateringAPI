using AutoMapper;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Core.Dtos.Cards;
using FirstCateringAPI.Core.Dtos.LinksAndWrappers;
using FirstCateringAPI.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace FirstCateringAPI.BusinessLogic.Implementations
{
    public class MembershipCardLogic : BaseLogic, IMembershipCardLogic
    {
        private readonly IMembershipCardsRepo _repo;
        private readonly IEncryption _encryption;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IConfiguration _configuration;

        public MembershipCardLogic(IMembershipCardsRepo repo, IEncryption encrypt, IMapper mapper, IUrlHelper urlHelper, IConfiguration configuration) : base(repo)
        {
            _repo = repo;
            _encryption = encrypt;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _configuration = configuration;
        }

        public MembershipCardOwnerLinksDto AddHateoasLinks(MembershipCardOwnerDto cardAndOwner)
        {
            var cardAndOwnerLinks = _mapper.Map<MembershipCardOwnerLinksDto>(cardAndOwner);

            cardAndOwnerLinks.Links.Add(new LinkDto(_urlHelper.Link("Login", new { employeeId = cardAndOwner.EmployeeId}),"login","POST"));
            
            return cardAndOwnerLinks;
            
        }


        public MembershipCardLinksDto AddHateoasLinks(MembershipCardDto membershipCard)
        {
            var membershipCardLinksDto = _mapper.Map<MembershipCardLinksDto>(membershipCard);

            return membershipCardLinksDto;
        }


        public MembershipCardOwnerDto GetCardAndOwnerId(Guid cardId)
        {
            var cardAndOwner = _repo.GetCardOwner(cardId);

            return _mapper.Map<MembershipCardOwnerDto>(cardAndOwner);
        }


        public MembershipCardDto GetMembershipCard(Guid cardId)
        {           
            var membershipCard = _repo.GetMembershipCard(cardId);

            return _mapper.Map<MembershipCardDto>(membershipCard);
        }


        public bool MembershipCardExists(Guid cardId)
        {
            return _repo.MembershipCardExists(cardId);
        }


        public void AddCredit(UpdateBalanceDto updateBalanceDto)
        {
            var membershipCard = _repo.GetMembershipCard(updateBalanceDto.CardId);

            membershipCard.CurrentBalance += updateBalanceDto.Credit;

            _repo.UpdateMembershipCard(membershipCard);
        }

        public bool Authenticated(string pinNumber, Guid cardId)
        {
            var pinEncryptKey = _configuration.GetValue<string>("AppSettings:Secrets:PinEncryptKey");

            var encryptedPin = _repo.GetCardOwner(cardId).PINNumber;
            var decryptedPin = _encryption.DecryptString(encryptedPin, pinEncryptKey);

            if (pinNumber == decryptedPin)
            {
                return true;
            }
            else return false;
        }
    }
}