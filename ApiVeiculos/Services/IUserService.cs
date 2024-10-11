using ApiVeiculos.DTOs;
using ApiVeiculos.Models;

namespace ApiVeiculos.Services
{
    public interface IUserService
    {
        public Task<(ApplicationUser? user, string message)> AlteraUsuarioAsync(UserModel newUser, ApplicationUser oldUser);
        public Task<bool> VerificaFormsAsync(string email, string cpf, string username);
        public bool VerificaCpf(string cpf);
        public bool VerificaEmail(string email);
    }
}
