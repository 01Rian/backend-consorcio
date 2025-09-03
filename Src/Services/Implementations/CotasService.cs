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

        public async Task<bool> RegisterPaymentAsync(int cotaId, decimal valorPago, CancellationToken cancellationToken = default)
        {
            if (valorPago <= 0)
                throw new ArgumentException("O valor pago deve ser maior que zero", nameof(valorPago));

            var cota = await _cotasRepository.GetByIdAsync(cotaId, cancellationToken);

            if (cota == null)
                return false;

            if (!cota.Ativo)
                throw new InvalidOperationException("Não é possível registrar pagamento para uma cota inativa");

            if (cota.Status == StatusCota.Quitado)
                throw new InvalidOperationException("Não é possível registrar pagamento para uma cota já quitada");

            var valorParcela = decimal.Round(cota.ValorParcela, 2);
            var pagamento = decimal.Round(valorPago, 2);
            var parcelasAdicionadas = CalculateParcelToAdd(pagamento, valorParcela);

            var cotaConsorcio = await _cotasRepository.GetByConsorcioAsync(cota.ConsorcioId, cancellationToken);
            var prazoConsorcio = cotaConsorcio.FirstOrDefault()?.Consorcio?.PrazoMeses;

            ValidatePaymentTerm(cota, parcelasAdicionadas, prazoConsorcio);

            UpdateCotaWithPayment(cota, parcelasAdicionadas, pagamento, prazoConsorcio);

            await _cotasRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ContemplateAsync(int cotaId, CancellationToken cancellationToken = default)
        {
            if (cotaId <= 0)
                throw new ArgumentException("ID deve ser maior que zero", nameof(cotaId));

            var cota = await _cotasRepository.GetByIdAsync(cotaId, cancellationToken);
            if (cota == null)
                return false;

            if (cota.Contemplada)
                throw new InvalidOperationException("A cota já está contemplada");

            if (!cota.Ativo && cota.Status != StatusCota.Quitado)
                throw new InvalidOperationException("A cota precisa estar ativa ou quitada para ser contemplada");

            cota.Contemplada = true;
            cota.DataContemplacao = DateTime.Now;
            cota.Status = StatusCota.Contemplado;
            cota.DataAtualizacao = DateTime.Now;

            _cotasRepository.Update(cota);
            await _cotasRepository.SaveChangesAsync(cancellationToken);

            return true;
        }



    private static int CalculateParcelToAdd(decimal pagamento, decimal valorParcela)
        {
            if (pagamento < valorParcela)
                throw new InvalidOperationException("O valor pago não pode ser menor que o valor da parcela");

            var parcelasDecimal = pagamento / valorParcela;
            if (decimal.Truncate(parcelasDecimal) != parcelasDecimal)
                throw new InvalidOperationException("Pagamento inválido: deve ser igual ao valor da(s) parcela(s) (múltiplo exato).");

            return (int)parcelasDecimal;
        }

    

    private static void ValidatePaymentTerm(Cotas cota, int parcelasAdicionadas, int? prazo)
        {
            if (!prazo.HasValue || prazo.Value <= 0)
                return;

            if (cota.ParcelasPagas >= prazo.Value)
                throw new InvalidOperationException("Esta cota já está quitada. Não é possível registrar mais pagamentos.");

            var parcelasRestantes = prazo.Value - cota.ParcelasPagas;
            if (parcelasAdicionadas > parcelasRestantes)
                throw new InvalidOperationException($"Não é possível pagar {parcelasAdicionadas} parcela(s). Restam apenas {parcelasRestantes} parcela(s) para quitar esta cota.");
        }

    private static void UpdateCotaWithPayment(Cotas cota, int parcelasAdicionadas, decimal pagamento, int? prazo)
        {
            cota.ParcelasPagas += parcelasAdicionadas;
            cota.ValorPago += pagamento;
            cota.DataAtualizacao = DateTime.Now;

            if (prazo.HasValue && cota.ParcelasPagas >= prazo.Value)
            {
                cota.Status = StatusCota.Quitado;
            }
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
