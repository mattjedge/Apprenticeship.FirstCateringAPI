using FirstCateringAPI.Core.Dtos.Cards;
using System;

namespace FirstCateringAPI.BusinessLogic.Contracts
{
    public interface IMembershipCardLogic
    {
        bool MembershipCardExists(Guid cardId);

        MembershipCardOwnerDto GetCardAndOwnerId(Guid cardId);

        MembershipCardOwnerLinksDto AddHateoasLinks(MembershipCardOwnerDto cardAndOwner);

        MembershipCardLinksDto AddHateoasLinks(MembershipCardDto membershipCard);

        MembershipCardDto GetMembershipCard(Guid cardId);
    }
}