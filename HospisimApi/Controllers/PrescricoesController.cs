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
using HospisimApi.DTO.ResponseDto;

namespace HospisimApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescricoesController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<PrescricoesController> _logger;

        public PrescricoesController(HospisimDbContext context, ILogger<PrescricoesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private PrescricaoResponseDto MapToResponseDto(Prescricao prescricao)
        {
            return new PrescricaoResponseDto
            {
                Id = prescricao.Id,
                Medicamento = prescricao.Medicamento,
                Dosagem = prescricao.Dosagem,
                Frequencia = prescricao.Frequencia,
                ViaAdministracao = prescricao.ViaAdministracao,
                DataInicio = prescricao.DataInicio,
                DataFim = prescricao.DataFim,
                Observacoes = prescricao.Observacoes,
                StatusPrescricao = prescricao.StatusPrescricao,
                ReacoesAdversas = prescricao.ReacoesAdversas,
                Atendimento = prescricao.Atendimento != null ? new AtendimentoResumidoDto
                {
                    Id = prescricao.Atendimento.Id,
                    Data = prescricao.Atendimento.Data,
                    Hora = prescricao.Atendimento.Hora,
                    Tipo = prescricao.Atendimento.Tipo
                } : null,
                Profissional = prescricao.Profissional != null ? new ProfissionalSaudeResumidoDto
                {
                    Id = prescricao.Profissional.Id,
                    NomeCompleto = prescricao.Profissional.NomeCompleto,
                    RegistroConselho = prescricao.Profissional.RegistroConselho
                } : null
            };
        }

        // GET: api/Prescricoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescricaoResponseDto>>> GetPrescricoes()
        {
            try
            {
                var prescricoes = await _context.Prescricoes
                                                .Include(p => p.Atendimento)
                                                .Include(p => p.Profissional)
                                                .ToListAsync();

                if (!prescricoes.Any())
                {
                    return NotFound("Nenhuma prescrição encontrada.");
                }
                return prescricoes.Select(p => MapToResponseDto(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de prescrições.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter prescrições.");
            }
        }

        // GET: api/Prescricoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescricaoResponseDto>> GetPrescricao(Guid id)
        {
            try
            {
                var prescricao = await _context.Prescricoes
                                                .Include(p => p.Atendimento)
                                                .Include(p => p.Profissional)
                                                .FirstOrDefaultAsync(p => p.Id == id);

                if (prescricao == null)
                {
                    return NotFound("Prescrição não encontrada.");
                }

                return MapToResponseDto(prescricao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter prescrição com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter prescrição com ID: {id}.");
            }
        }

        // PUT: api/Prescricoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescricao(Guid id, [FromBody] UpdatePrescricaoDto prescricaoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var prescricaoToUpdate = await _context.Prescricoes.FindAsync(id);
                if (prescricaoToUpdate == null)
                {
                    return NotFound("Prescrição não encontrada para atualização.");
                }

                // Verifica se os IDs de FKs existem
                if (!await _context.Atendimentos.AnyAsync(a => a.Id == prescricaoDto.AtendimentoId))
                {
                    return BadRequest("O AtendimentoId fornecido não existe.");
                }
                if (!await _context.ProfissionaisSaude.AnyAsync(ps => ps.Id == prescricaoDto.ProfissionalId))
                {
                    return BadRequest("O ProfissionalId fornecido não existe.");
                }

                prescricaoToUpdate.AtendimentoId = prescricaoDto.AtendimentoId;
                prescricaoToUpdate.ProfissionalId = prescricaoDto.ProfissionalId;
                prescricaoToUpdate.Medicamento = prescricaoDto.Medicamento;
                prescricaoToUpdate.Dosagem = prescricaoDto.Dosagem;
                prescricaoToUpdate.Frequencia = prescricaoDto.Frequencia;
                prescricaoToUpdate.ViaAdministracao = prescricaoDto.ViaAdministracao;
                prescricaoToUpdate.DataInicio = prescricaoDto.DataInicio;
                prescricaoToUpdate.DataFim = prescricaoDto.DataFim;
                prescricaoToUpdate.Observacoes = prescricaoDto.Observacoes;
                prescricaoToUpdate.StatusPrescricao = prescricaoDto.StatusPrescricao;
                prescricaoToUpdate.ReacoesAdversas = prescricaoDto.ReacoesAdversas;

                _context.Entry(prescricaoToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _context.Entry(prescricaoToUpdate).Reference(p => p.Atendimento).LoadAsync();
                await _context.Entry(prescricaoToUpdate).Reference(p => p.Profissional).LoadAsync();

                return Ok(MapToResponseDto(prescricaoToUpdate));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PrescricaoExists(id))
                {
                    return NotFound("Prescrição não encontrada para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar prescrição com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. A prescrição foi modificada ou excluída por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar prescrição com ID: {id}. Dados da prescrição: {JsonConvert.SerializeObject(prescricaoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar prescrição. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar prescrição com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar prescrição.");
            }
        }

        // POST: api/Prescricoes
        [HttpPost]
        public async Task<ActionResult<PrescricaoResponseDto>> PostPrescricao(CreatePrescricaoDto prescricaoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _context.Atendimentos.AnyAsync(a => a.Id == prescricaoDto.AtendimentoId))
            {
                return BadRequest("O AtendimentoId fornecido não existe.");
            }
            if (!await _context.ProfissionaisSaude.AnyAsync(ps => ps.Id == prescricaoDto.ProfissionalId))
            {
                return BadRequest("O ProfissionalId fornecido não existe.");
            }

            var prescricao = new Prescricao
            {
                Id = Guid.NewGuid(),
                AtendimentoId = prescricaoDto.AtendimentoId,
                ProfissionalId = prescricaoDto.ProfissionalId,
                Medicamento = prescricaoDto.Medicamento,
                Dosagem = prescricaoDto.Dosagem,
                Frequencia = prescricaoDto.Frequencia,
                ViaAdministracao = prescricaoDto.ViaAdministracao,
                DataInicio = prescricaoDto.DataInicio,
                DataFim = prescricaoDto.DataFim,
                Observacoes = prescricaoDto.Observacoes,
                StatusPrescricao = prescricaoDto.StatusPrescricao,
                ReacoesAdversas = prescricaoDto.ReacoesAdversas
            };

            try
            {
                _context.Prescricoes.Add(prescricao);
                await _context.SaveChangesAsync();

                await _context.Entry(prescricao).Reference(p => p.Atendimento).LoadAsync();
                await _context.Entry(prescricao).Reference(p => p.Profissional).LoadAsync();

                var responseDto = MapToResponseDto(prescricao);
                return CreatedAtAction("GetPrescricao", new { id = responseDto.Id }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao criar prescrição. Dados da prescrição: {JsonConvert.SerializeObject(prescricaoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar prescrição. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar prescrição. Dados da prescrição: {JsonConvert.SerializeObject(prescricaoDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar prescrição.");
            }
        }

        // DELETE: api/Prescricoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescricao(Guid id)
        {
            try
            {
                var prescricao = await _context.Prescricoes.FindAsync(id);
                if (prescricao == null)
                {
                    return NotFound("Prescrição não encontrada para exclusão.");
                }

                _context.Prescricoes.Remove(prescricao);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir prescrição com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir prescrição. Verifique se não há outras entidades vinculadas (embora não haja, por enquanto).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir prescrição com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir prescrição.");
            }
        }

        private bool PrescricaoExists(Guid id)
        {
            return _context.Prescricoes.Any(e => e.Id == id);
        }
    }
}