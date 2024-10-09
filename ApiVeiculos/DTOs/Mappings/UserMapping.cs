using ApiVeiculos.Models;
using AutoMapper;

namespace ApiVeiculos.DTOs.Mappings;

public class UserMapping : Profile
{
    public UserMapping() 
    { 
        CreateMap<ApplicationUser, UserModel>().ReverseMap();
    }
}
