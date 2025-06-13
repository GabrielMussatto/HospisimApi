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
    public class ExamesController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<ExamesController> _logger;

        public ExamesController(HospisimDbContext context, ILogger<ExamesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Helper para mapear Entidade para DTO de Resposta
        private ExameResponseDto MapToResponseDto(Exame exame)
        {
            return new ExameResponseDto
            {
                Id = exame.Id,
                Tipo = exame.Tipo,
                DataSolicitacao = exame.DataSolicitacao,
                DataRealizacao = exame.DataRealizacao,
                Resultado = exame.Resultado,
                Atendimento = exame.Atendimento != null ? new AtendimentoResumidoDto
                {
                    Id = exame.Atendimento.Id,
                    Data = exame.Atendimento.Data,
                    Hora = exame.Atendimento.Hora,
                    Tipo = exame.Atendimento.Tipo
                } : null
            };
        }

        // GET: api/Exames
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExameResponseDto>>> GetExames()
        {
            try
            {
                var exames = await _context.Exames
                                            .Include(e => e.Atendimento)
                                            .ToListAsync();

                if (!exames.Any())
                {
                    return NotFound("Nenhum exame encontrado.");
                }
                return exames.Select(e => MapToResponseDto(e)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de exames.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter exames.");
            }
        }

        // GET: api/Exames/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExameResponseDto>> GetExame(Guid id)
        {
            try
            {
                var exame = await _context.Exames
                                            .Include(e => e.Atendimento)
                                            .FirstOrDefaultAsync(e => e.Id == id);

                if (exame == null)
                {
                    return NotFound("Exame não encontrado.");
                }

                return MapToResponseDto(exame);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter exame com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter exame com ID: {id}.");
            }
        }

        // PUT: api/Exames/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExame(Guid id, [FromBody] UpdateExameDto exameDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var exameToUpdate = await _context.Exames.FindAsync(id);
                if (exameToUpdate == null)
                {
                    return NotFound("Exame não encontrado para atualização.");
                }

                // Verifica se o AtendimentoId fornecido existe
                if (!await _context.Atendimentos.AnyAsync(a => a.Id == exameDto.AtendimentoId))
                {
                    return BadRequest("O AtendimentoId fornecido não existe.");
                }

                exameToUpdate.Tipo = exameDto.Tipo;
                exameToUpdate.DataSolicitacao = exameDto.DataSolicitacao;
                exameToUpdate.DataRealizacao = exameDto.DataRealizacao;
                exameToUpdate.Resultado = exameDto.Resultado;
                exameToUpdate.AtendimentoId = exameDto.AtendimentoId;

                _context.Entry(exameToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _context.Entry(exameToUpdate).Reference(e => e.Atendimento).LoadAsync();

                return Ok(MapToResponseDto(exameToUpdate));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ExameExists(id))
                {
                    return NotFound("Exame não encontrado para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar exame com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. O exame foi modificado ou excluído por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar exame com ID: {id}. Dados do exame: {JsonConvert.SerializeObject(exameDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar exame. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar exame com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar exame.");
            }
        }

        // POST: api/Exames
        [HttpPost]
        public async Task<ActionResult<ExameResponseDto>> PostExame(CreateExameDto exameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _context.Atendimentos.AnyAsync(a => a.Id == exameDto.AtendimentoId))
            {
                return BadRequest("O AtendimentoId fornecido não existe.");
            }

            var exame = new Exame
            {
                Id = Guid.NewGuid(),
                Tipo = exameDto.Tipo,
                DataSolicitacao = exameDto.DataSolicitacao,
                DataRealizacao = exameDto.DataRealizacao,
                Resultado = exameDto.Resultado,
                AtendimentoId = exameDto.AtendimentoId
            };

            try
            {
                _context.Exames.Add(exame);
                await _context.SaveChangesAsync();

                await _context.Entry(exame).Reference(e => e.Atendimento).LoadAsync();

                var responseDto = MapToResponseDto(exame);
                return CreatedAtAction("GetExame", new { id = responseDto.Id }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao criar exame. Dados do exame: {JsonConvert.SerializeObject(exameDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar exame. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar exame. Dados do exame: {JsonConvert.SerializeObject(exameDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar exame.");
            }
        }

        // DELETE: api/Exames/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExame(Guid id)
        {
            try
            {
                var exame = await _context.Exames.FindAsync(id);
                if (exame == null)
                {
                    return NotFound("Exame não encontrado para exclusão.");
                }

                _context.Exames.Remove(exame);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir exame com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir exame.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir exame com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir exame.");
            }
        }

        private bool ExameExists(Guid id)
        {
            return _context.Exames.Any(e => e.Id == id);
        }
    }
}