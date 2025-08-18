using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using rian_p01_back.src.Services.Interfaces;

namespace rian_p01_back.Src.Controllers.Cotas
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CotasController : ControllerBase
    {
        private readonly ICotasService _cotasService;

        public CotasController(ICotasService cotasService)
        {
            _cotasService = cotasService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CotasResumoResponseDto>> GetById(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            var cota = await _cotasService.GetByIdAsync(id, cancellationToken);
            if (cota == null)
                return NotFound(new { message = "Cota não encontrada" });

            var response = CotasResumoResponseDto.FromEntity(cota);
            return Ok(response);
        }

        [HttpGet("{id}/detailed")]
        public async Task<ActionResult<CotasResponseDto>> GetDetailed(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            var cota = await _cotasService.GetDetailedAsync(id, cancellationToken);
            if (cota == null)
                return NotFound(new { message = "Cota não encontrada" });

            var response = CotasResponseDto.FromEntity(cota);
            return Ok(response);
        }

        [HttpGet("by-consorcio/{consorcioId}")]
        public async Task<ActionResult<IEnumerable<CotasResponseDto>>> GetByConsorcio(int consorcioId, CancellationToken cancellationToken = default)
        {
            if (consorcioId <= 0)
                return BadRequest(new { message = "ID do consórcio deve ser maior que zero" });

            var cotas = await _cotasService.GetByConsorcioAsync(consorcioId, cancellationToken);
            var response = CotasResponseDto.FromEntities(cotas);
            return Ok(response);
        }

        [HttpGet("by-user/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<CotasResponseDto>>> GetByUsuario(int usuarioId, CancellationToken cancellationToken = default)
        {
            if (usuarioId <= 0)
                return BadRequest(new { message = "ID do usuário deve ser maior que zero" });

            var cotas = await _cotasService.GetByUsuarioAsync(usuarioId, cancellationToken);
            var response = CotasResponseDto.FromEntities(cotas);
            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<CotasResumoResponseDto>>> GetActive(CancellationToken cancellationToken = default)
        {
            var cotas = await _cotasService.GetActiveAsync(cancellationToken);
            var response = CotasResumoResponseDto.FromEntities(cotas);
            return Ok(response);
        }

        [HttpGet("contemplated")]
        public async Task<ActionResult<IEnumerable<CotasResponseDto>>> GetContemplated(CancellationToken cancellationToken = default)
        {
            var cotas = await _cotasService.GetContemplatedAsync(cancellationToken);
            var response = CotasResponseDto.FromEntities(cotas);
            return Ok(response);
        }

        [HttpPost("{id}/contemplate")]
        public async Task<ActionResult> Contemplate(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });
            var success = await _cotasService.ContemplateAsync(id, cancellationToken);
            if (!success)
                return NotFound(new { message = "Cota não encontrada" });

            return Ok(new { message = "Cota contemplada com sucesso" });
        }

        [HttpPost("{id}/register-payment")]
        public async Task<ActionResult> RegisterPayment(int id, [FromBody] RegisterPaymentRequestDto request, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            var success = await _cotasService.RegisterPaymentAsync(id, request.ValorPago, cancellationToken);
            if (success)
                return Ok(new { message = "Pagamento registrado com sucesso" });

            return NotFound(new { message = "Cota não encontrada" });
        }

        [HttpDelete("{id}/remove-user")]
        public async Task<ActionResult> RemoveUser(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            var success = await _cotasService.RemoveUsuarioFromCotaAsync(id, cancellationToken);
            if (success)
                return Ok(new { message = "Usuário removido da cota com sucesso" });

            return NotFound(new { message = "Cota não encontrada" });
        }

        [HttpPut("{id}/update-status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateCotaStatusRequestDto request, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            if (request.Status != StatusCota.Ativo && request.Status != StatusCota.Suspenso)
                return BadRequest(new { message = "Status inválido. Apenas os status 'Ativo' e 'Suspenso' são permitidos para esta operação." });

            var cota = await _cotasService.GetByIdAsync(id, cancellationToken);
            if (cota == null)
                return NotFound(new { message = "Cota não encontrada" });

            if (cota.Status == StatusCota.Contemplado)
                return BadRequest(new { message = "Não é possível alterar o status de uma cota contemplada." });

            if (cota.Status == StatusCota.Quitado)
                return BadRequest(new { message = "Não é possível alterar o status de uma cota quitada." });

            cota.Status = request.Status;
            cota.Ativo = request.Status == StatusCota.Ativo;

            await _cotasService.UpdateAsync(cota, cancellationToken);
            
            return Ok(new { message = $"Status da cota atualizado para '{request.Status.ToString()}' com sucesso." });
        }
    }
}
