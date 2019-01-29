using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.DataAccess.Contracts;

namespace FirstCateringAPI.BusinessLogic.Implementations
{
    public class BaseLogic : IBaseLogic
    {
        private readonly IRepository _repo;

        public BaseLogic(IRepository repo)
        {
            _repo = repo;
        }

        public bool Save()
        {
            return _repo.Save();
        }
    }
}