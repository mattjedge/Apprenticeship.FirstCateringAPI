using AutoMapper;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Core.Dtos.Cards;
using FirstCateringAPI.Core.Dtos.LinksAndWrappers;
using FirstCateringAPI.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FirstCateringAPI.BusinessLogic.Implementations
{
    public class MembershipCardLogic : BaseLogic, IMembershipCardLogic
    {
        private readonly IMembershipCardsRepo _repo;
        private readonly IEmployeesRepo _employeesRepo;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public MembershipCardLogic(IMembershipCardsRepo repo, IEmployeesRepo empRepo, IMapper mapper, IUrlHelper urlHelper) : base(repo)
        {
            _repo = repo;
            _employeesRepo = empRepo;
            _mapper = mapper;
            _urlHelper = urlHelper;
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
    }
}
