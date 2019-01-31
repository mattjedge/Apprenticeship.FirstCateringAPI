using FirstCateringAPI.Core.Dtos.Cards;
using System;

namespace FirstCateringAPI.BusinessLogic.Contracts
{
    public interface IMembershipCardLogic : IBaseLogic
    {
        bool MembershipCardExists(Guid cardId);

        MembershipCardOwnerDto GetCardAndOwnerId(Guid cardId);

        MembershipCardOwnerLinksDto AddHateoasLinks(MembershipCardOwnerDto cardAndOwner);

        MembershipCardLinksDto AddHateoasLinks(MembershipCardDto membershipCard);

        MembershipCardDto GetMembershipCard(Guid cardId);

        void AddCredit(UpdateBalanceDto membershipCard);

        bool Authenticated(string pinNumber, Guid cardId);
    }
}