using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Interfaces;

namespace rian_p01_back.src.Services.Implementations
{
    public class CotasService : GenericService<Cotas>, ICotasService
    {
        private readonly ICotasRepository _cotasRepository;

        public CotasService(ICotasRepository cotasRepository) : base(cotasRepository)
        {
            _cotasRepository = cotasRepository;
        }

        public async Task<IEnumerable<Cotas>> GetByConsorcioAsync(int consorcioId, CancellationToken cancellationToken = default)
        {
            if (consorcioId <= 0)
                throw new ArgumentException("O ConsorcioId deve ser maior que zero", nameof(consorcioId));

            return await _cotasRepository.GetByConsorcioAsync(consorcioId, cancellationToken);
        }

        public async Task<IEnumerable<Cotas>> GetByUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("O UsuarioId deve ser maior que zero", nameof(usuarioId));

            return await _cotasRepository.GetByUsuarioAsync(usuarioId, cancellationToken);
        }

        public async Task<Cotas?> GetDetailedAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _cotasRepository.GetDetalhadaAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Cotas>> GetContemplatedAsync(CancellationToken cancellationToken = default)
        {
            return await FindAsync(c => c.Contemplada, cancellationToken);
        }

        public async Task<IEnumerable<Cotas>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await FindAsync(c => c.Ativo && c.Status == StatusCota.Ativo, cancellationToken);
        }

        public async Task<bool> ContemplateAsync(int cotaId, CancellationToken cancellationToken = default)
        {
            var cota = await _cotasRepository.GetByIdAsync(cotaId, cancellationToken);
            if (cota == null)
                return false;

            if (cota.Contemplada)
                throw new InvalidOperationException("A cota já está contemplada");

            if (!cota.Ativo || cota.Status != StatusCota.Ativo)
                throw new InvalidOperationException("A cota precisa estar ativa para ser contemplada");

            cota.Contemplada = true;
            cota.DataContemplacao = DateTime.Now;
            cota.Status = StatusCota.Contemplado;
            cota.DataAtualizacao = DateTime.Now;

            await _cotasRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RegisterPaymentAsync(int cotaId, decimal valorPago, CancellationToken cancellationToken = default)
        {
            if (valorPago <= 0)
                throw new ArgumentException("O valor pago deve ser maior que zero", nameof(valorPago));

            var cota = await _cotasRepository.GetByIdAsync(cotaId, cancellationToken);
            if (cota == null)
                return false;

            if (!cota.Ativo)
                throw new InvalidOperationException("Não é possível registrar pagamento para uma cota inativa");

            cota.ValorPago += valorPago;
            cota.ParcelasPagas++;
            cota.DataAtualizacao = DateTime.Now;

            var valorTotalEsperado = cota.ValorParcela * cota.Consorcio?.PrazoMeses ?? 0;
            if (cota.ValorPago >= valorTotalEsperado && cota.Status != StatusCota.Quitado)
            {
                cota.Status = StatusCota.Quitado;
            }

            await _cotasRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public override async Task<Cotas> CreateAsync(Cotas entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.ValorParcela <= 0)
                throw new ArgumentException("O valor da parcela deve ser maior que zero");

            if (entity.ConsorcioId <= 0)
                throw new ArgumentException("O ConsorcioId deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(entity.NumeroCota))
                throw new ArgumentException("O número da cota é obrigatório");

            entity.DataCadastro = DateTime.Now;
            entity.Status = StatusCota.Ativo;
            entity.Ativo = true;
            entity.ValorPago = 0;
            entity.ParcelasPagas = 0;
            entity.Contemplada = false;

            return await base.CreateAsync(entity, cancellationToken);
        }

        public override async Task<Cotas> UpdateAsync(Cotas entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.ValorParcela <= 0)
                throw new ArgumentException("O valor da parcela deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(entity.NumeroCota))
                throw new ArgumentException("O número da cota é obrigatório");

            entity.DataAtualizacao = DateTime.Now;
            return await base.UpdateAsync(entity, cancellationToken);
        }

        public async Task<IEnumerable<Cotas>> GetAvailableCotasAsync(int consorcioId, CancellationToken cancellationToken = default)
        {
            if (consorcioId <= 0)
                throw new ArgumentException("O ConsorcioId deve ser maior que zero", nameof(consorcioId));

            var cotasDoConsorcio = await _cotasRepository.GetByConsorcioAsync(consorcioId, cancellationToken);
            return cotasDoConsorcio.Where(c => c.UsuarioId == null && c.Ativo && c.Status == StatusCota.Ativo);
        }

        public async Task<bool> RemoveUsuarioFromCotaAsync(int cotaId, CancellationToken cancellationToken = default)
        {
            if (cotaId <= 0)
                throw new ArgumentException("O ID da cota deve ser maior que zero", nameof(cotaId));

            var cota = await _cotasRepository.GetByIdAsync(cotaId, cancellationToken);
            if (cota == null)
                return false;

            if (cota.UsuarioId == null)
                throw new InvalidOperationException("Esta cota não está atribuída a nenhum usuário");

            if (cota.ValorPago > 0 || cota.ParcelasPagas > 0)
                throw new InvalidOperationException("Não é possível remover usuário de uma cota com pagamentos registrados");

            cota.UsuarioId = null;
            cota.DataAtualizacao = DateTime.Now;

            await _cotasRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

    }
}
