using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiVeiculos.Models;

[Table("Veiculos")]
public class Veiculo
{
    public Veiculo()
    {
        Reservas = new Collection<Reserva>();
    }

    [Key]
    public int VeiculoId { get; set; }
    [Required(ErrorMessage ="Informe o modelo do veículo")]
    [StringLength(80)]
    public string? Modelo { get; set; }
    [Required(ErrorMessage = "Informe a placa do veículo")]
    [StringLength(7)]
    public string? Placa { get; set; }
    [Required(ErrorMessage = "Informe a marca do veículo")]
    [StringLength(80)]
    public string? Marca { get; set; }
    [Required(ErrorMessage = "Informe o ano do veículo")]
    [StringLength(4)]
    public string? Ano { get; set; }
    public EstadoVeiculo Estado { get; set; }
    public enum EstadoVeiculo
    {
        Disponivel,
        Manutencao,
        Indisponivel
    }
    [JsonIgnore]
    public ICollection<Reserva>? Reservas { get; set; }

}

