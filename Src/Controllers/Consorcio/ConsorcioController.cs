using Microsoft.AspNetCore.Mvc;
using rian_p01_back.src.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace rian_p01_back.src.Controllers.Consorcio
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConsorcioController : ControllerBase
    {
        private readonly IConsorcioService _consorcioService;

        public ConsorcioController(IConsorcioService consorcioService)
        {
            _consorcioService = consorcioService;
        }

        [HttpPost]
        public async Task<ActionResult<ConsorcioResponseDto>> Create([FromBody] CreateConsorcioRequestDto request, CancellationToken cancellationToken)
        {
            var consorcioEntity = request.ToEntity();

            var consorcioCreated = await _consorcioService.CreateAsync(consorcioEntity, cancellationToken);
            var response = ConsorcioResponseDto.FromEntity(consorcioCreated);
            return CreatedAtAction(nameof(GetById), new { id = consorcioCreated.Id }, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsorcioResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var consorcio = await _consorcioService.GetByIdAsync(id, cancellationToken);
            if (consorcio == null)
                return NotFound(new { message = "Consórcio não encontrado" });

            var response = ConsorcioResponseDto.FromEntity(consorcio);
            return Ok(response);
        }

        [HttpGet("{id}/with-cotas")]
        public async Task<ActionResult<ConsorcioResponseDto>> GetWithCotas(int id, CancellationToken cancellationToken)
        {
            var consorcio = await _consorcioService.GetWithCotasAsync(id, cancellationToken);
            if (consorcio == null)
                return NotFound(new { message = "Consórcio não encontrado" });

            var response = ConsorcioResponseDto.FromEntity(consorcio);
            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ConsorcioResumoResponseDto>>> GetActive(CancellationToken cancellationToken)
        {
            var consorcios = await _consorcioService.GetActiveAsync(cancellationToken);
            var response = ConsorcioResumoResponseDto.FromEntities(consorcios);
            return Ok(response);
        }

        [HttpGet("{id}/available-cotas")]
        public async Task<ActionResult<IEnumerable<CotasResponseDto>>> GetAvailableCotas(int id, CancellationToken cancellationToken)
        {
            var cotasDisponiveis = await _consorcioService.GetAvailableCotasAsync(id, cancellationToken);
            var response = CotasResponseDto.FromEntities(cotasDisponiveis);
            return Ok(response);
        }

        [HttpPost("{id}/cotas")]
        public async Task<ActionResult> AddCotas(int id, [FromBody] AddCotasRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null || request.Quantidade <= 0)
                return BadRequest(new { message = "Quantidade deve ser maior que zero." });

            var consorcio = await _consorcioService.GetWithCotasAsync(id, cancellationToken);
            if (consorcio == null)
                return NotFound(new { message = "Consórcio não encontrado" });

            var created = await _consorcioService.AddCotasAsync(id, request.Quantidade, cancellationToken);
            var response = CotasResponseDto.FromEntities(created);
            return Ok(response);
        }

        [HttpPost("assign-cota")]
        public async Task<ActionResult> AssignCotaToUser([FromBody] AssignCotaRequestDto request, CancellationToken cancellationToken)
        {
            var success = await _consorcioService.AssignCotaToUsuarioAsync(request.CotaId, request.UsuarioId, cancellationToken);
            if (success)
                return Ok(new { message = "Cota atribuída com sucesso" });

            return BadRequest(new { message = "Não foi possível atribuir a cota" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ConsorcioResponseDto>> Update(int id, [FromBody] UpdateConsorcioRequestDto request, CancellationToken cancellationToken)
        {
            var consorcioExistente = await _consorcioService.GetByIdAsync(id, cancellationToken);
            if (consorcioExistente == null)
                return NotFound(new { message = "Consórcio não encontrado" });

            var consorcioEntity = request.ToEntity(consorcioExistente);
            var consorcioUpdated = await _consorcioService.UpdateAsync(consorcioEntity, cancellationToken);
            var response = ConsorcioResponseDto.FromEntity(consorcioUpdated);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _consorcioService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
