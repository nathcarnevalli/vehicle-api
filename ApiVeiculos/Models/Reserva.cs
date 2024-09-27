using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiVeiculos.Models;

[Table("Reservas")]
public class Reserva
{
    [Key]
    public int ReservaId {  get; set; }
    [Required(ErrorMessage = "Informe uma data de início para a reserva")]
    public DateTime DataInicio { get; set; }
    [Required(ErrorMessage = "Informe uma data de fim para a reserva")]
    public DateTime DataFim { get; set; }
    public EstadoReserva Estado {  get; set; }
    public enum EstadoReserva
    {
        Provisorio,
        Confirmado,
        Cancelado
    }
    public int VeiculoId { get; set; }
    [JsonIgnore]
    public Veiculo? Veiculo { get; set; }
}

