using Microsoft.AspNetCore.Identity;
using ApiVeiculos.Models;
using ApiVeiculos.DTOs;
using System.Text.RegularExpressions;

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

            var cpfExists = await _userManager.FindByNameAsync(cpf);

            if (cpfExists != null)
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
            cpf = cpf.ToString().Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11 || !Regex.IsMatch(cpf, @"^\d{11}$"))
            {
                return false;
            }

            return true;
        }

        public bool VerificaEmail(string email)
        {
            var regexEmail = new Regex(@"^[a-zA-Z0-9_]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,}$");
            return regexEmail.IsMatch(email);
        }
    }
}
