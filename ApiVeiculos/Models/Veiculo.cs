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
    [MinLength(5)]
    [StringLength(80)]
    public string? Modelo { get; set; }
    [Required(ErrorMessage = "Informe a placa do veículo")]
    [MinLength(7)]
    [MaxLength(7)]
    public string? Placa { get; set; }
    [Required(ErrorMessage = "Informe a marca do veículo")]
    [MinLength(5)]
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
        Indisponivel /* Esse estado serve para caso se queira "deletar" um veículo */
    }
    public ICollection<Reserva>? Reservas { get; set; }

}

