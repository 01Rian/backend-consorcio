using Microsoft.AspNetCore.Identity;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Interfaces;

namespace rian_p01_back.src.Services.Implementations
{
    public class UsuarioService : GenericService<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail não pode ser nulo ou vazio", nameof(email));

            return await _usuarioRepository.GetByEmailAsync(email, cancellationToken);
        }

        public async Task<Usuario?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("O CPF não pode ser nulo ou vazio", nameof(cpf));

            return await _usuarioRepository.GetByCpfAsync(cpf, cancellationToken);
        }

        public override async Task<Usuario> CreateAsync(Usuario entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingByEmail = await _usuarioRepository.GetByEmailAsync(entity.Email, cancellationToken);
            if (existingByEmail != null)
                throw new InvalidOperationException("O e-mail já está em uso por outro usuário");

            var existingByCpf = await _usuarioRepository.GetByCpfAsync(entity.CPF, cancellationToken);
            if (existingByCpf != null)
                throw new InvalidOperationException("O CPF já está em uso por outro usuário");

            if (string.IsNullOrWhiteSpace(entity.Senha))
                throw new ArgumentException("A senha não pode ser nula ou vazia", nameof(entity.Senha));

            entity.Senha = _passwordHasher.HashPassword(entity, entity.Senha);
            entity.DataCadastro = DateTime.Now;
            return await base.CreateAsync(entity, cancellationToken);
        }

        public override async Task<Usuario> UpdateAsync(Usuario entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingUser = await _usuarioRepository.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new InvalidOperationException("Usuário não encontrado para atualizar");

            var existingByEmail = await _usuarioRepository.GetByEmailAsync(entity.Email, cancellationToken);
            if (existingByEmail != null && existingByEmail.Id != entity.Id)
                throw new InvalidOperationException("O e-mail já está em uso por outro usuário");

            var existingByCpf = await _usuarioRepository.GetByCpfAsync(entity.CPF, cancellationToken);
            if (existingByCpf != null && existingByCpf.Id != entity.Id)
                throw new InvalidOperationException("O CPF já está em uso por outro usuário");

            if (string.IsNullOrWhiteSpace(entity.Senha))
            {
                entity.Senha = existingUser.Senha;
            }
            else
            {
                entity.Senha = _passwordHasher.HashPassword(entity, entity.Senha);
            }

            entity.DataAtualizacao = DateTime.Now;
            return await base.UpdateAsync(entity, cancellationToken);
        }
    }
}
