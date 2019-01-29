using FirstCateringAPI.Core.Context;
using FirstCateringAPI.DataAccess.Contracts;

namespace FirstCateringAPI.DataAccess.Implementations
{
    public class Repository : IRepository
    {
        private readonly FirstCateringContext _dbContext;

        public Repository(FirstCateringContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Save()
        {
            return (_dbContext.SaveChanges() >= 0);
        }
    }
}