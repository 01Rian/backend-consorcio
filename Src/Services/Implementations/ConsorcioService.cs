using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Interfaces;

namespace rian_p01_back.src.Services.Implementations
{
    public class ConsorcioService : GenericService<Consorcio>, IConsorcioService
    {
        private readonly IConsorcioRepository _consorcioRepository;
        private readonly ICotasService _cotasService;

        public ConsorcioService(IConsorcioRepository consorcioRepository, ICotasService cotasService) : base(consorcioRepository)
        {
            _consorcioRepository = consorcioRepository;
            _cotasService = cotasService;
        }

        public async Task<Consorcio?> GetWithCotasAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _consorcioRepository.GetWithCotasAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Consorcio>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await FindAsync(c => c.Ativo, cancellationToken);
        }

        public override async Task<Consorcio> CreateAsync(Consorcio entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateConsorcio(entity);

            entity.DataTermino = entity.DataInicio.AddMonths(entity.PrazoMeses);
            entity.DataCadastro = DateTime.Now;
            entity.Ativo = true;

            // Cria o consórcio primeiro
            var consorcioCreated = await base.CreateAsync(entity, cancellationToken);

            // Cria as cotas automaticamente baseado na quantidade definida
            await CreateCotasForConsorcioAsync(consorcioCreated, cancellationToken);

            return consorcioCreated;
        }

        public override async Task<Consorcio> UpdateAsync(Consorcio entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateConsorcio(entity);

            var dataTerminoCalculada = entity.DataInicio.AddMonths(entity.PrazoMeses);
            if (entity.DataTermino.Date != dataTerminoCalculada.Date)
            {
                entity.DataTermino = dataTerminoCalculada;
            }

            entity.DataAtualizacao = DateTime.Now;
            return await base.UpdateAsync(entity, cancellationToken);
        }

        public async Task<bool> AssignCotaToUsuarioAsync(int cotaId, int usuarioId, CancellationToken cancellationToken = default)
        {
            if (cotaId <= 0)
                throw new ArgumentException("O ID da cota deve ser maior que zero", nameof(cotaId));

            if (usuarioId <= 0)
                throw new ArgumentException("O ID do usuário deve ser maior que zero", nameof(usuarioId));

            var cota = await _cotasService.GetByIdAsync(cotaId, cancellationToken);
            if (cota == null)
                throw new ArgumentException("Cota não encontrada", nameof(cotaId));

            if (cota.UsuarioId != null)
                throw new InvalidOperationException("Esta cota já está atribuída a um usuário");

            cota.UsuarioId = usuarioId;
            cota.DataAtualizacao = DateTime.Now;

            await _cotasService.UpdateAsync(cota, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<Cotas>> GetAvailableCotasAsync(int consorcioId, CancellationToken cancellationToken = default)
        {
            if (consorcioId <= 0)
                throw new ArgumentException("O ID do consórcio deve ser maior que zero", nameof(consorcioId));

            var cotasDoConsorcio = await _cotasService.GetByConsorcioAsync(consorcioId, cancellationToken);
            return cotasDoConsorcio.Where(c => c.UsuarioId == null && c.Ativo);
        }


        private void ValidateConsorcio(Consorcio entity)
        {
            switch (entity)
            {
                case var e when e.ValorBem <= 0:
                    throw new ArgumentException("O valor do bem deve ser maior que zero");

                case var e when e.QuantidadeCotas <= 0:
                    throw new ArgumentException("A quantidade de cotas deve ser maior que zero");

                case var e when e.PrazoMeses <= 0:
                    throw new ArgumentException("O prazo em meses deve ser maior que zero");

                case var e when e.TaxaAdministracao < 0:
                    throw new ArgumentException("A taxa de administração não pode ser negativa");

                case var e when e.FundoReserva < 0:
                    throw new ArgumentException("O fundo de reserva não pode ser negativo");
            }
        }

        private async Task CreateCotasForConsorcioAsync(Consorcio consorcio, CancellationToken cancellationToken = default)
        {
            if (consorcio == null)
                throw new ArgumentNullException(nameof(consorcio));

            // Calcula o valor da parcela baseado no valor do bem dividido pelo prazo
            var valorParcela = consorcio.ValorBem / consorcio.PrazoMeses;

            for (int i = 1; i <= consorcio.QuantidadeCotas; i++)
            {
                var cota = new Cotas
                {
                    NumeroCota = $"{consorcio.Codigo}-{i:D4}", // Formato: CODIGO-0001, CODIGO-0002, etc.
                    ValorParcela = valorParcela,
                    ValorPago = 0,
                    ParcelasPagas = 0,
                    Contemplada = false,
                    DataContemplacao = null,
                    Status = StatusCota.Ativo,
                    Ativo = true,
                    DataCadastro = DateTime.Now,
                    ConsorcioId = consorcio.Id,
                    UsuarioId = null // Cotas são criadas sem usuário, serão atribuídas posteriormente
                };

                await _cotasService.CreateAsync(cota, cancellationToken);
            }
        }
    }
}