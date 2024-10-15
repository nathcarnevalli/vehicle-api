using Microsoft.AspNetCore.Identity;
using ApiVeiculos.Models;
using ApiVeiculos.DTOs;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace ApiVeiculos.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(ApplicationUser? user, string message)> AlteraUsuarioAsync(UserModel newUser, ApplicationUser oldUser)
        {
            if (
                oldUser.UserName == newUser.UserName &&
                oldUser.Email == newUser.Email &&
                oldUser.PasswordHash == newUser.PasswordHash &&
                oldUser.Name == newUser.Name &&
                oldUser.Estado == newUser.Estado)
            {
                return (oldUser, "");
            }

            if (oldUser.Email != newUser.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(newUser.Email!);

                if (emailExists is not null)
                {
                    return (null, "E-mail já está em uso.");
                }

                await _userManager.SetEmailAsync(oldUser, newUser.Email);
            }

            if (oldUser.PasswordHash != newUser.PasswordHash)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(oldUser);
                await _userManager.ResetPasswordAsync(oldUser, resetToken, newUser.PasswordHash!);
            }

            if (!oldUser.Estado.Equals(newUser.Estado))
            {
                oldUser.Estado = newUser.Estado;
            }

            if (oldUser.UserName != newUser.UserName)
            {
                var userNameExists = await _userManager.FindByNameAsync(newUser.UserName!);

                if (userNameExists is not null)
                {
                    return (null, "Nome de usuário já está em uso.");
                }

                await _userManager.SetUserNameAsync(oldUser, newUser.UserName);
            }

            return (oldUser, "Usuário atualizado com sucesso");
        }


        public async Task<bool> VerificaFormsAsync(string email, string cpf, string username)
        {
            var emailExists = await _userManager.FindByEmailAsync(email);

            if (emailExists != null)
            {
                return false;
            }

            var cpfLimpo = cpf.Replace(".", "").Replace("-", "").Trim();
            var cpfExists = await _userManager.Users.AnyAsync(u => u.CPF == cpfLimpo);

            if (cpfExists)
            {
                return false; 
            }

            var userNameExists = await _userManager.FindByNameAsync(username);

            if (userNameExists != null)
            {
                return false;
            }

            return true;
        }

        public bool VerificaCpf(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
            {
                return false;
            }

            int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += (tempCpf[i] - '0') * multiplicadores1[i];
            }

            int resto = (soma % 11);
            int digito1 = (resto < 2) ? 0 : 11 - resto;

            tempCpf += digito1;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += (tempCpf[i] - '0') * multiplicadores2[i];
            }

            resto = (soma % 11);
            int digito2 = (resto < 2) ? 0 : 11 - resto;

            return cpf.EndsWith(digito1.ToString() + digito2.ToString());
        }


        public bool VerificaPassword(string password)
        {
            var regexPassword = new Regex(@"^[A-Z].*[!@#$%^&*(),.?""{}|<>].*$");
            return regexPassword.IsMatch(password);
        }

        public bool VerificaEmail(string email)
        {
            var regexEmail = new Regex(@"^[a-zA-Z0-9_]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,}$");
            return regexEmail.IsMatch(email);
        }
    }
}
