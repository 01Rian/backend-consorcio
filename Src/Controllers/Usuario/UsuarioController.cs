using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using rian_p01_back.src.Services.Interfaces;

namespace rian_p01_back.Src.Controllers.Usuario
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<AuthUsuarioResponseDto>> Create([FromBody] CreateUsuarioRequestDto request, CancellationToken cancellationToken = default)
        {
            var usuario = request.ToEntity();
            var usuarioCriado = await _usuarioService.CreateAsync(usuario, cancellationToken);

            var token = Factories.JwtTokenFactory.Create(usuarioCriado);

            var response = new AuthUsuarioResponseDto(usuarioCriado, token);

            return CreatedAtAction(nameof(GetById), new { id = usuarioCriado.Id }, response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthUsuarioResponseDto>> Login([FromBody] LoginUsuarioRequestDto request, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioService.GetByEmailAsync(request.Email, cancellationToken);
            if (usuario == null || !_usuarioService.ValidatePassword(usuario, request.Senha))
                return Unauthorized(new { message = "Email ou senha inválidos" });

            var token = Factories.JwtTokenFactory.Create(usuario);

            var response = new AuthUsuarioResponseDto(usuario, token);

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResumoResponseDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var usuarios = await _usuarioService.GetAllAsync(cancellationToken);
            var usuariosDto = UsuarioResumoResponseDto.FromEntities(usuarios);

            return Ok(usuariosDto);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetById(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            var usuario = await _usuarioService.GetByIdAsync(id, cancellationToken);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado" });

            var usuarioDto = UsuarioResponseDto.FromEntity(usuario);

            return Ok(usuarioDto);
        }

        [Authorize]
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetByEmail(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email é obrigatório" });

            var usuario = await _usuarioService.GetByEmailAsync(email, cancellationToken);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado" });

            var usuarioDto = UsuarioResponseDto.FromEntity(usuario);

            return Ok(usuarioDto);
        }

        [Authorize]
        [HttpGet("cpf/{cpf}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetByCpf(string cpf, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest(new { message = "CPF é obrigatório" });

            var usuario = await _usuarioService.GetByCpfAsync(cpf, cancellationToken);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado" });

            var usuarioDto = UsuarioResponseDto.FromEntity(usuario);

            return Ok(usuarioDto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> Update(int id, [FromBody] UpdateUsuarioRequestDto request, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            var usuarioExistente = await _usuarioService.GetByIdAsync(id, cancellationToken);
            if (usuarioExistente == null)
                return NotFound(new { message = "Usuário não encontrado" });

            var usuario = request.ToEntity(usuarioExistente);

            var usuarioAtualizado = await _usuarioService.UpdateAsync(usuario, cancellationToken);

            var usuarioDto = UsuarioResponseDto.FromEntity(usuarioAtualizado);

            return Ok(usuarioDto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            var usuario = await _usuarioService.GetByIdAsync(id, cancellationToken);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado" });

            await _usuarioService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}