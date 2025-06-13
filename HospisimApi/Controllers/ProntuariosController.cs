using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospisimApi.Data;
using HospisimApi.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HospisimApi.DTOs;

namespace HospisimApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProntuariosController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<ProntuariosController> _logger;

        public ProntuariosController(HospisimDbContext context, ILogger<ProntuariosController> logger)
        {
            _context = context;
            _logger = logger; 
        }

        // GET: api/Prontuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prontuario>>> GetProntuarios()
        {
            try
            {
                var prontuarios = await _context.Prontuarios
                                                .Include(p => p.Paciente)
                                                .ToListAsync();

                if (!prontuarios.Any())
                {
                    return NotFound("Nenhum prontuário encontrado.");
                }
                return prontuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de prontuários.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter prontuários.");
            }
        }

        // GET: api/Prontuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prontuario>> GetProntuario(Guid id)
        {
            try
            {
                var prontuario = await _context.Prontuarios
                                               .Include(p => p.Paciente)
                                               .FirstOrDefaultAsync(p => p.Id == id);
                if (prontuario == null)
                {
                    return NotFound();
                }
                return prontuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter prontuário com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter prontuário com ID: {id}.");
            }
        }

        // PUT: api/Prontuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProntuario(Guid id, UpdateProntuarioDto prontuarioDto) // Alterado para receber o DTO de atualização
        {
            // Valida se o ID na URL corresponde ao ID no DTO
            if (id != prontuarioDto.Id)
            {
                return BadRequest("O ID na URL não corresponde ao ID do prontuário no corpo da requisição.");
            }

            // Validação de modelo (Data Annotations do DTO)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Busca o prontuário existente no banco de dados
                var prontuarioToUpdate = await _context.Prontuarios.FindAsync(id);

                if (prontuarioToUpdate == null)
                {
                    return NotFound("Prontuário não encontrado para atualização.");
                }

                // Verifica se o PacienteId fornecido realmente existe no banco de dados
                if (!await _context.Pacientes.AnyAsync(p => p.Id == prontuarioDto.PacienteId))
                {
                    return BadRequest("O PacienteId fornecido não existe. Por favor, insira um ID de paciente válido.");
                }

                // Atualiza as propriedades da entidade existente com os dados do DTO
                prontuarioToUpdate.NumeroProntuario = prontuarioDto.NumeroProntuario;
                prontuarioToUpdate.DataAbertura = prontuarioDto.DataAbertura;
                prontuarioToUpdate.ObservacoesGerais = prontuarioDto.ObservacoesGerais;
                prontuarioToUpdate.PacienteId = prontuarioDto.PacienteId; // Atualiza a chave estrangeira

                // Marca a entidade como modificada para que o EF Core a atualize no banco
                _context.Entry(prontuarioToUpdate).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return NoContent(); // Retorna 204 No Content para PUT bem-sucedido
            }
            catch (DbUpdateConcurrencyException ex) // Exceção específica para problemas de concorrência
            {
                if (!ProntuarioExists(id))
                {
                    return NotFound("Prontuário não encontrado para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao tentar atualizar prontuário com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. O prontuário pode ter sido modificado ou excluído por outro usuário.");
                }
            }
            catch (DbUpdateException ex) // Exceção para outros erros de banco de dados (ex: violação de unique constraint)
            {
                // Tenta verificar se é uma violação de unique constraint para o NumeroProntuario
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de atualizar prontuário com NumeroProntuario duplicado: {prontuarioDto.NumeroProntuario}.");
                    return Conflict($"Já existe um prontuário com o número '{prontuarioDto.NumeroProntuario}'.");
                }
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar prontuário com ID: {id}. Dados do prontuário: {JsonConvert.SerializeObject(prontuarioDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar prontuário. Verifique os dados fornecidos.");
            }
            catch (Exception ex) // Exceção genérica para qualquer outro erro inesperado
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar prontuário com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar prontuário.");
            }
        }

        // POST: api/Prontuarios
        [HttpPost]
        public async Task<ActionResult<Prontuario>> PostProntuario(CreateProntuarioDto prontuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _context.Pacientes.AnyAsync(p => p.Id == prontuarioDto.PacienteId))
            {
                return BadRequest("O PacienteId fornecido não existe. Por favor, insira um ID de paciente válido.");
            }

            var prontuario = new Prontuario
            {
                Id = Guid.NewGuid(),
                NumeroProntuario = prontuarioDto.NumeroProntuario,
                DataAbertura = prontuarioDto.DataAbertura,
                ObservacoesGerais = prontuarioDto.ObservacoesGerais,
                PacienteId = prontuarioDto.PacienteId
            };

            try
            {
                _context.Prontuarios.Add(prontuario);
                await _context.SaveChangesAsync();

                await _context.Entry(prontuario).Reference(p => p.Paciente).LoadAsync();

                return CreatedAtAction("GetProntuario", new { id = prontuario.Id }, prontuario);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de inserir prontuário com NúmeroProntuario duplicado: {prontuario.NumeroProntuario}.");
                    return Conflict($"Já existe um prontuário com o número '{prontuario.NumeroProntuario}'.");
                }

                _logger.LogError(ex, $"Erro de banco de dados ao criar prontuário. Dados do prontuário: {JsonConvert.SerializeObject(prontuario)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar prontuário. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar prontuário. Dados do prontuário: {JsonConvert.SerializeObject(prontuario)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar prontuário.");
            }
        }

        // DELETE: api/Prontuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProntuario(Guid id)
        {
            try
            {
                var prontuario = await _context.Prontuarios.FindAsync(id);
                if (prontuario == null)
                {
                    return NotFound("Prontuário não encontrado para exclusão.");
                }

                _context.Prontuarios.Remove(prontuario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("conflicted with the REFERENCE constraint") == true ||
                    ex.InnerException?.Message.Contains("FOREIGN KEY constraint") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de excluir prontuário com ID: {id}, mas existem atendimentos vinculados.");
                    return Conflict("Não é possível excluir o prontuário, pois existem atendimentos vinculados a ele.");
                }
                _logger.LogError(ex, $"Erro de banco de dados ao excluir prontuário com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir prontuário.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir prontuário com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir prontuário.");
            }
        }

        private bool ProntuarioExists(Guid id)
        {
            return _context.Prontuarios.Any(e => e.Id == id);
        }
    }
}