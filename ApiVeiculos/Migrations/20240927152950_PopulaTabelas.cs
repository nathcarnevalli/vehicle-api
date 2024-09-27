using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiVeiculos.Migrations
{
    public partial class PopulaTabelas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Veiculos",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql("Insert into Veiculos(Modelo, Placa, Marca, Ano, Estado) Values('Lancer EVO', 'AAA3A22', 'Mitsubishi' , '2013', 0)");
            migrationBuilder.Sql("Insert into Veiculos(Modelo, Placa, Marca, Ano, Estado) Values('Cruze LTZ', 'BBB3A22', 'Chevrolet', '2023', 1)");
            migrationBuilder.Sql("Insert into Veiculos(Modelo, Placa, Marca, Ano, Estado) Values('Rx-7', 'CCC3A22', 'Mazda', '2002', 0)");
            migrationBuilder.Sql("Insert into Veiculos(Modelo, Placa, Marca, Ano, Estado) Values('370Z', 'DDD3A22', 'Nissan', '2008', 2)");

            migrationBuilder.Sql("Insert into Reservas(DataInicio, DataFim, Estado, VeiculoId) Values('2024-09-27', '2024-09-29', 2, 1)");
            migrationBuilder.Sql("Insert into Reservas(DataInicio, DataFim, Estado, VeiculoId) Values('2024-09-27', '2024-09-29', 3, 2)");
            migrationBuilder.Sql("Insert into Reservas(DataInicio, DataFim, Estado, VeiculoId) Values('2024-09-27', '2024-09-29', 2, 3)");
            migrationBuilder.Sql("Insert into Reservas(DataInicio, DataFim, Estado, VeiculoId) Values('2024-09-27', '2024-09-29', 3, 4)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Veiculos");
        }
    }
}
