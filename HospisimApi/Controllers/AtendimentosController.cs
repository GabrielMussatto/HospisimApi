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
using HospisimApi.DTO.ResponseDto;
using HospisimApi.DTO;

namespace HospisimApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtendimentosController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<AtendimentosController> _logger;

        public AtendimentosController(HospisimDbContext context, ILogger<AtendimentosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private AtendimentoResponseDto MapToResponseDto(Atendimento atendimento)
        {
            return new AtendimentoResponseDto
            {
                Id = atendimento.Id,
                Data = atendimento.Data,
                Hora = atendimento.Hora,
                Tipo = atendimento.Tipo,
                Status = atendimento.Status,
                Local = atendimento.Local,
                Paciente = atendimento.Paciente != null ? new PacienteResumidoDto
                {
                    Id = atendimento.Paciente.Id,
                    NomeCompleto = atendimento.Paciente.NomeCompleto,
                    CPFFormatado = atendimento.Paciente.CPFFormatado
                } : null,
                ProfissionalSaude = atendimento.ProfissionalSaude != null ? new ProfissionalSaudeResumidoDto
                {
                    Id = atendimento.ProfissionalSaude.Id,
                    NomeCompleto = atendimento.ProfissionalSaude.NomeCompleto,
                    RegistroConselho = atendimento.ProfissionalSaude.RegistroConselho
                } : null,
                Prontuario = atendimento.Prontuario != null ? new ProntuarioResumidoDto
                {
                    Id = atendimento.Prontuario.Id,
                    NumeroProntuario = atendimento.Prontuario.NumeroProntuario
                } : null
            };
        }

        // GET: api/Atendimentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtendimentoResponseDto>>> GetAtendimentos()
        {
            try
            {
                var atendimentos = await _context.Atendimentos
                                                .Include(a => a.Paciente)
                                                .Include(a => a.ProfissionalSaude)
                                                .Include(a => a.Prontuario)
                                                .ToListAsync();

                if (!atendimentos.Any())
                {
                    return NotFound("Nenhum atendimento encontrado.");
                }
                return atendimentos.Select(a => MapToResponseDto(a)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de atendimentos.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter atendimentos.");
            }
        }

        // GET: api/Atendimentos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AtendimentoResponseDto>> GetAtendimento(Guid id)
        {
            try
            {
                var atendimento = await _context.Atendimentos
                                                .Include(a => a.Paciente)
                                                .Include(a => a.ProfissionalSaude)
                                                .Include(a => a.Prontuario)
                                                .FirstOrDefaultAsync(a => a.Id == id);

                if (atendimento == null)
                {
                    return NotFound("Atendimento não encontrado.");
                }

                return MapToResponseDto(atendimento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter atendimento com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter atendimento com ID: {id}.");
            }
        }

        // PUT: api/Atendimentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAtendimento(Guid id, [FromBody] UpdateAtendimentoDto atendimentoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var atendimentoToUpdate = await _context.Atendimentos.FindAsync(id);
                if (atendimentoToUpdate == null)
                {
                    return NotFound("Atendimento não encontrado para atualização.");
                }

                // Verifica se os IDs de FKs existem
                if (!await _context.Pacientes.AnyAsync(p => p.Id == atendimentoDto.PacienteId))
                {
                    return BadRequest("O PacienteId fornecido não existe.");
                }
                if (!await _context.ProfissionaisSaude.AnyAsync(ps => ps.Id == atendimentoDto.ProfissionalSaudeId))
                {
                    return BadRequest("O ProfissionalSaudeId fornecido não existe.");
                }
                if (!await _context.Prontuarios.AnyAsync(p => p.Id == atendimentoDto.ProntuarioId))
                {
                    return BadRequest("O ProntuarioId fornecido não existe.");
                }

                atendimentoToUpdate.Data = atendimentoDto.Data;
                atendimentoToUpdate.Hora = atendimentoDto.Hora;
                atendimentoToUpdate.Tipo = atendimentoDto.Tipo;
                atendimentoToUpdate.Status = atendimentoDto.Status;
                atendimentoToUpdate.Local = atendimentoDto.Local;
                atendimentoToUpdate.PacienteId = atendimentoDto.PacienteId;
                atendimentoToUpdate.ProfissionalSaudeId = atendimentoDto.ProfissionalSaudeId;
                atendimentoToUpdate.ProntuarioId = atendimentoDto.ProntuarioId;

                _context.Entry(atendimentoToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _context.Entry(atendimentoToUpdate).Reference(a => a.Paciente).LoadAsync();
                await _context.Entry(atendimentoToUpdate).Reference(a => a.ProfissionalSaude).LoadAsync();
                await _context.Entry(atendimentoToUpdate).Reference(a => a.Prontuario).LoadAsync();

                return Ok(MapToResponseDto(atendimentoToUpdate));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AtendimentoExists(id))
                {
                    return NotFound("Atendimento não encontrado para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar atendimento com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. O atendimento foi modificado ou excluído por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar atendimento com ID: {id}. Dados do atendimento: {JsonConvert.SerializeObject(atendimentoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar atendimento. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar atendimento com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar atendimento.");
            }
        }

        // POST: api/Atendimentos
        [HttpPost]
        public async Task<ActionResult<AtendimentoResponseDto>> PostAtendimento(CreateAtendimentoDto atendimentoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _context.Pacientes.AnyAsync(p => p.Id == atendimentoDto.PacienteId))
            {
                return BadRequest("O PacienteId fornecido não existe.");
            }
            if (!await _context.ProfissionaisSaude.AnyAsync(ps => ps.Id == atendimentoDto.ProfissionalSaudeId))
            {
                return BadRequest("O ProfissionalSaudeId fornecido não existe.");
            }
            if (!await _context.Prontuarios.AnyAsync(p => p.Id == atendimentoDto.ProntuarioId))
            {
                return BadRequest("O ProntuarioId fornecido não existe.");
            }

            var atendimento = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = atendimentoDto.Data,
                Hora = atendimentoDto.Hora,
                Tipo = atendimentoDto.Tipo,
                Status = atendimentoDto.Status,
                Local = atendimentoDto.Local,
                PacienteId = atendimentoDto.PacienteId,
                ProfissionalSaudeId = atendimentoDto.ProfissionalSaudeId,
                ProntuarioId = atendimentoDto.ProntuarioId
            };

            try
            {
                _context.Atendimentos.Add(atendimento);
                await _context.SaveChangesAsync();

                await _context.Entry(atendimento).Reference(a => a.Paciente).LoadAsync();
                await _context.Entry(atendimento).Reference(a => a.ProfissionalSaude).LoadAsync();
                await _context.Entry(atendimento).Reference(a => a.Prontuario).LoadAsync();

                var responseDto = MapToResponseDto(atendimento);
                return CreatedAtAction("GetAtendimento", new { id = responseDto.Id }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao criar atendimento. Dados do atendimento: {JsonConvert.SerializeObject(atendimentoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar atendimento. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar atendimento. Dados do atendimento: {JsonConvert.SerializeObject(atendimentoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar atendimento.");
            }
        }

        // DELETE: api/Atendimentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAtendimento(Guid id)
        {
            try
            {
                var atendimento = await _context.Atendimentos.FindAsync(id);
                if (atendimento == null)
                {
                    return NotFound("Atendimento não encontrado para exclusão.");
                }

                if (await _context.Prescricoes.AnyAsync(p => p.AtendimentoId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir atendimento com ID: {id}, mas existem prescrições vinculadas.");
                    return Conflict("Não é possível excluir este atendimento, pois existem prescrições vinculadas a ele.");
                }
                if (await _context.Exames.AnyAsync(e => e.AtendimentoId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir atendimento com ID: {id}, mas existem exames vinculados.");
                    return Conflict("Não é possível excluir este atendimento, pois existem exames vinculados a ele.");
                }
                if (await _context.Internacoes.AnyAsync(i => i.AtendimentoId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir atendimento com ID: {id}, mas existe uma internação vinculada.");
                    return Conflict("Não é possível excluir este atendimento, pois existe uma internação vinculada a ele.");
                }

                _context.Atendimentos.Remove(atendimento);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir atendimento com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir atendimento.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir atendimento com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir atendimento.");
            }
        }

        private bool AtendimentoExists(Guid id)
        {
            return _context.Atendimentos.Any(e => e.Id == id);
        }
    }
}