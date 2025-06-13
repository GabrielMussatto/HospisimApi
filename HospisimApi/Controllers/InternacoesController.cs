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
using HospisimApi.DTO;
using Newtonsoft.Json;

namespace HospisimApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternacoesController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<InternacoesController> _logger;

        public InternacoesController(HospisimDbContext context, ILogger<InternacoesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private InternacaoResponseDto MapToResponseDto(Internacao internacao)
        {
            return new InternacaoResponseDto
            {
                Id = internacao.Id,
                DataEntrada = internacao.DataEntrada,
                PrevisaoAlta = internacao.PrevisaoAlta,
                MotivoInternacao = internacao.MotivoInternacao,
                Leito = internacao.Leito,
                Quarto = internacao.Quarto,
                Setor = internacao.Setor,
                PlanoSaudeUtilizado = internacao.PlanoSaudeUtilizado,
                ObservacoesClinicas = internacao.ObservacoesClinicas,
                StatusInternacao = internacao.StatusInternacao,
                Paciente = internacao.Paciente != null ? new PacienteResumidoDto
                {
                    Id = internacao.Paciente.Id,
                    NomeCompleto = internacao.Paciente.NomeCompleto,
                    CPFFormatado = internacao.Paciente.CPFFormatado
                } : null,
                Atendimento = internacao.Atendimento != null ? new AtendimentoResumidoDto
                {
                    Id = internacao.Atendimento.Id,
                    Data = internacao.Atendimento.Data,
                    Hora = internacao.Atendimento.Hora,
                    Tipo = internacao.Atendimento.Tipo
                } : null
            };
        }

        // GET: api/Internacoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InternacaoResponseDto>>> GetInternacoes()
        {
            try
            {
                var internacoes = await _context.Internacoes
                                                .Include(i => i.Paciente)
                                                .Include(i => i.Atendimento)
                                                .ToListAsync();

                if (!internacoes.Any())
                {
                    return NotFound("Nenhuma internação encontrada.");
                }
                return internacoes.Select(i => MapToResponseDto(i)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de internações.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter internações.");
            }
        }

        // GET: api/Internacoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InternacaoResponseDto>> GetInternacao(Guid id)
        {
            try
            {
                var internacao = await _context.Internacoes
                                                .Include(i => i.Paciente)
                                                .Include(i => i.Atendimento)
                                                .FirstOrDefaultAsync(i => i.Id == id);

                if (internacao == null)
                {
                    return NotFound("Internação não encontrada.");
                }

                return MapToResponseDto(internacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter internação com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter internação com ID: {id}.");
            }
        }

        // PUT: api/Internacoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInternacao(Guid id, [FromBody] UpdateInternacaoDto internacaoDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var internacaoToUpdate = await _context.Internacoes.FindAsync(id);
                if (internacaoToUpdate == null)
                {
                    return NotFound("Internação não encontrada para atualização.");
                }

                if (!await _context.Pacientes.AnyAsync(p => p.Id == internacaoDto.PacienteId))
                {
                    return BadRequest("O PacienteId fornecido não existe.");
                }

                if (internacaoToUpdate.AtendimentoId != internacaoDto.AtendimentoId)
                {
                    if (await _context.Internacoes.AnyAsync(i => i.AtendimentoId == internacaoDto.AtendimentoId))
                    {
                        return Conflict($"O AtendimentoId '{internacaoDto.AtendimentoId}' já está associado a outra internação. Um atendimento só pode originar uma internação.");
                    }
                }
                if (!await _context.Atendimentos.AnyAsync(a => a.Id == internacaoDto.AtendimentoId))
                {
                    return BadRequest("O AtendimentoId fornecido não existe.");
                }

                internacaoToUpdate.PacienteId = internacaoDto.PacienteId;
                internacaoToUpdate.AtendimentoId = internacaoDto.AtendimentoId;
                internacaoToUpdate.DataEntrada = internacaoDto.DataEntrada;
                internacaoToUpdate.PrevisaoAlta = internacaoDto.PrevisaoAlta;
                internacaoToUpdate.MotivoInternacao = internacaoDto.MotivoInternacao;
                internacaoToUpdate.Leito = internacaoDto.Leito;
                internacaoToUpdate.Quarto = internacaoDto.Quarto;
                internacaoToUpdate.Setor = internacaoDto.Setor;
                internacaoToUpdate.PlanoSaudeUtilizado = internacaoDto.PlanoSaudeUtilizado;
                internacaoToUpdate.ObservacoesClinicas = internacaoDto.ObservacoesClinicas;
                internacaoToUpdate.StatusInternacao = internacaoDto.StatusInternacao;

                _context.Entry(internacaoToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _context.Entry(internacaoToUpdate).Reference(i => i.Paciente).LoadAsync();
                await _context.Entry(internacaoToUpdate).Reference(i => i.Atendimento).LoadAsync();

                return Ok(MapToResponseDto(internacaoToUpdate));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!InternacaoExists(id))
                {
                    return NotFound("Internação não encontrada para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar internação com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. A internação foi modificada ou excluída por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row in object 'dbo.Internacoes'") == true &&
                    ex.InnerException?.Message.Contains("IX_Internacoes_AtendimentoId") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de atualizar internação com AtendimentoId duplicado: {internacaoDto.AtendimentoId}.");
                    return Conflict($"O AtendimentoId '{internacaoDto.AtendimentoId}' já está associado a outra internação. Um atendimento só pode originar uma internação.");
                }
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar internação com ID: {id}. Dados da internação: {JsonConvert.SerializeObject(internacaoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar internação. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar internação com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar internação.");
            }
        }

        // POST: api/Internacoes
        [HttpPost]
        public async Task<ActionResult<InternacaoResponseDto>> PostInternacao(CreateInternacaoDto internacaoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _context.Pacientes.AnyAsync(p => p.Id == internacaoDto.PacienteId))
            {
                return BadRequest("O PacienteId fornecido não existe.");
            }

            if (await _context.Internacoes.AnyAsync(i => i.AtendimentoId == internacaoDto.AtendimentoId))
            {
                return Conflict($"O AtendimentoId '{internacaoDto.AtendimentoId}' já está associado a uma internação existente. Um atendimento só pode originar uma internação.");
            }

            if (!await _context.Atendimentos.AnyAsync(a => a.Id == internacaoDto.AtendimentoId))
            {
                return BadRequest("O AtendimentoId fornecido não existe.");
            }

            var internacao = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = internacaoDto.PacienteId,
                AtendimentoId = internacaoDto.AtendimentoId,
                DataEntrada = internacaoDto.DataEntrada,
                PrevisaoAlta = internacaoDto.PrevisaoAlta,
                MotivoInternacao = internacaoDto.MotivoInternacao,
                Leito = internacaoDto.Leito,
                Quarto = internacaoDto.Quarto,
                Setor = internacaoDto.Setor,
                PlanoSaudeUtilizado = internacaoDto.PlanoSaudeUtilizado,
                ObservacoesClinicas = internacaoDto.ObservacoesClinicas,
                StatusInternacao = internacaoDto.StatusInternacao
            };

            try
            {
                _context.Internacoes.Add(internacao);
                await _context.SaveChangesAsync();

                await _context.Entry(internacao).Reference(i => i.Paciente).LoadAsync();
                await _context.Entry(internacao).Reference(i => i.Atendimento).LoadAsync();

                var responseDto = MapToResponseDto(internacao);
                return CreatedAtAction("GetInternacao", new { id = responseDto.Id }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row in object 'dbo.Internacoes'") == true &&
                    ex.InnerException?.Message.Contains("IX_Internacoes_AtendimentoId") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de criar internação com AtendimentoId duplicado: {internacaoDto.AtendimentoId}.");
                    return Conflict($"O AtendimentoId '{internacaoDto.AtendimentoId}' já está associado a uma internação existente. Um atendimento só pode originar uma internação.");
                }
                _logger.LogError(ex, $"Erro de banco de dados ao criar internação. Dados da internação: {JsonConvert.SerializeObject(internacaoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar internação. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar internação. Dados da internação: {JsonConvert.SerializeObject(internacaoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar internação.");
            }
        }

        // DELETE: api/Internacoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInternacao(Guid id)
        {
            try
            {
                var internacao = await _context.Internacoes.FindAsync(id);
                if (internacao == null)
                {
                    return NotFound("Internação não encontrada para exclusão.");
                }

                if (await _context.AltasHospitalares.AnyAsync(a => a.InternacaoId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir internação com ID: {id}, mas existe uma alta hospitalar vinculada.");
                    return Conflict("Não é possível excluir esta internação, pois existe uma alta hospitalar vinculada a ela.");
                }

                _context.Internacoes.Remove(internacao);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir internação com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir internação.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir internação com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir internação.");
            }
        }

        private bool InternacaoExists(Guid id)
        {
            return _context.Internacoes.Any(e => e.Id == id);
        }
    }
}