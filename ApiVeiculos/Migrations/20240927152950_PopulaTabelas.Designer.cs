﻿// <auto-generated />
using System;
using ApiVeiculos.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiVeiculos.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240927152950_PopulaTabelas")]
    partial class PopulaTabelas
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("ApiVeiculos.Models.Reserva", b =>
                {
                    b.Property<int>("ReservaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ReservaId"));

                    b.Property<DateTime>("DataFim")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DataInicio")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<int>("VeiculoId")
                        .HasColumnType("int");

                    b.HasKey("ReservaId");

                    b.HasIndex("VeiculoId");

                    b.ToTable("Reservas");
                });

            modelBuilder.Entity("ApiVeiculos.Models.Veiculo", b =>
                {
                    b.Property<int>("VeiculoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("VeiculoId"));

                    b.Property<string>("Ano")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("varchar(4)");

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<string>("Marca")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)");

                    b.Property<string>("Modelo")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)");

                    b.Property<string>("Placa")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("varchar(7)");

                    b.HasKey("VeiculoId");

                    b.ToTable("Veiculos");
                });

            modelBuilder.Entity("ApiVeiculos.Models.Reserva", b =>
                {
                    b.HasOne("ApiVeiculos.Models.Veiculo", "Veiculo")
                        .WithMany("Reservas")
                        .HasForeignKey("VeiculoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Veiculo");
                });

            modelBuilder.Entity("ApiVeiculos.Models.Veiculo", b =>
                {
                    b.Navigation("Reservas");
                });
#pragma warning restore 612, 618
        }
    }
}
