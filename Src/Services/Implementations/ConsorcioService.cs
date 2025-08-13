using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Interfaces;

namespace rian_p01_back.src.Services.Implementations
{
    public class ConsorcioService : GenericService<Consorcio>, IConsorcioService
    {
        private readonly IConsorcioRepository _consorcioRepository;

        public ConsorcioService(IConsorcioRepository consorcioRepository) : base(consorcioRepository)
        {
            _consorcioRepository = consorcioRepository;
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

            return await base.CreateAsync(entity, cancellationToken);
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
        // TODO ativa caso precise
        // private bool ValidateEndDate(Consorcio consorcio)
        // {
        //     if (consorcio == null)
        //         throw new ArgumentNullException(nameof(consorcio));

        //     var dataTerminoCalculada = consorcio.DataInicio.AddMonths(consorcio.PrazoMeses);

        //     return consorcio.DataTermino.Date == dataTerminoCalculada.Date;
        // }
    }
}