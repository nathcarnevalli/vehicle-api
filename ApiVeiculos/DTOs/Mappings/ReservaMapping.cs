using ApiVeiculos.Models;
using AutoMapper;

namespace ApiVeiculos.DTOs.Mappings;

public class ReservaMapping : Profile
{
    public ReservaMapping() 
    {
        CreateMap<Reserva, ReservaModel>().ReverseMap();
    }
}
