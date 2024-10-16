﻿using System.ComponentModel.DataAnnotations;

namespace ApiVeiculos.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(5)]
    public string? Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatório")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password é obrigatória")]
    [MinLength(5)]
    [MaxLength(255)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    [StringLength(14)]
    public string? CPF { get; set; }
}
