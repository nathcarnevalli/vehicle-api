using System.ComponentModel.DataAnnotations;
using static ApiVeiculos.Models.Reserva;

namespace ApiVeiculos.DTOs;

public class ReservaModel
{
    public int ReservaId { get; set; }
    [Required(ErrorMessage = "Informe uma data de início para a reserva")]
    public DateTime DataInicio { get; set; }
    [Required(ErrorMessage = "Informe uma data de fim para a reserva")]
    public DateTime DataFim { get; set; }
    public EstadoReserva Estado { get; set; }
    public string UserId { get; set; } = "0";
    public int VeiculoId { get; set; }
}
