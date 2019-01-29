using FirstCateringAPI.Core.Entities;
using System;

namespace FirstCateringAPI.DataAccess.Contracts
{
    public interface IMembershipCardsRepo : IRepository
    {
        bool MembershipCardExists(Guid cardId);

        MembershipCard GetMembershipCard(Guid cardId);

        Employee GetCardOwner(Guid cardId);
    }
}
