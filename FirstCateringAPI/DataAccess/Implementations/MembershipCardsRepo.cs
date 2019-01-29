using FirstCateringAPI.Core.Context;
using FirstCateringAPI.Core.Entities;
using FirstCateringAPI.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCateringAPI.DataAccess.Implementations
{
    public class MembershipCardsRepo : Repository, IMembershipCardsRepo
    {
        private readonly FirstCateringContext _dbContext;

        public MembershipCardsRepo(FirstCateringContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public MembershipCard GetMembershipCard(Guid cardId)
        {
            return _dbContext.MembershipCards.Single(x => x.CardId == cardId);
        }

        public bool MembershipCardExists(Guid cardId)
        {
            return _dbContext.MembershipCards.Where(x => x.CardId == cardId).Any();
        }

        public Employee GetCardOwner(Guid cardId)
        {
            return _dbContext.Employees.Single(x => x.CardId == cardId);
        }
    }
}
