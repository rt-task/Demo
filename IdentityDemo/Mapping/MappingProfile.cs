using AutoMapper;
using IdentityDemo.Dal.Entities;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SignUpRequest, CompanyEntity>();
            CreateMap<SignUpRequest, IdentityUser<int>>();
        }
    }
}
