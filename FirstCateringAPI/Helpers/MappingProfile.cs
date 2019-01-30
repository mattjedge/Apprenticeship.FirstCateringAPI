using AutoMapper;
using FirstCateringAPI.Core.Dtos.Cards;
using FirstCateringAPI.Core.Dtos.Employees;
using FirstCateringAPI.Core.Entities;

namespace FirstCateringAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterEmployeeDto,Employee>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, EmployeeLinksDto>();

            CreateMap<MembershipCard, MembershipCardOwnerDto>();
            CreateMap<MembershipCardOwnerDto, MembershipCardOwnerLinksDto>();
            CreateMap<MembershipCard, MembershipCardDto>();
            CreateMap<MembershipCardDto, MembershipCardLinksDto>();

        }
    }
}
