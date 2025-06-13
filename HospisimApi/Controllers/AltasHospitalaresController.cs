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
    public class AltasHospitalaresController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<AltasHospitalaresController> _logger;

        public AltasHospitalaresController(HospisimDbContext context, ILogger<AltasHospitalaresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private AltaHospitalarResponseDto MapToResponseDto(AltaHospitalar alta)
        {
            return new AltaHospitalarResponseDto
            {
                InternacaoId = alta.InternacaoId,
                DataAlta = alta.DataAlta,
                CondicaoPaciente = alta.CondicaoPaciente,
                InstrucoesPosAlta = alta.InstrucoesPosAlta,
                Internacao = alta.Internacao != null ? new InternacaoResumidoDto
                {
                    Id = alta.Internacao.Id,
                    MotivoInternacao = alta.Internacao.MotivoInternacao,
                    Leito = alta.Internacao.Leito,
                    Quarto = alta.Internacao.Quarto
                } : null
            };
        }

        // GET: api/AltasHospitalares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AltaHospitalarResponseDto>>> GetAltasHospitalares()
        {
            try
            {
                var altas = await _context.AltasHospitalares
                                            .Include(a => a.Internacao)
                                            .ToListAsync();

                if (!altas.Any())
                {
                    return NotFound("Nenhuma alta hospitalar encontrada.");
                }
                return altas.Select(a => MapToResponseDto(a)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de altas hospitalares.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter altas hospitalares.");
            }
        }

        // GET: api/AltasHospitalares/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AltaHospitalarResponseDto>> GetAltaHospitalar(Guid id)
        {
            try
            {
                var alta = await _context.AltasHospitalares
                                            .Include(a => a.Internacao)
                                            .FirstOrDefaultAsync(a => a.InternacaoId == id);

                if (alta == null)
                {
                    return NotFound("Alta hospitalar não encontrada para a internação fornecida.");
                }

                return MapToResponseDto(alta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter alta hospitalar com InternacaoId: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter alta hospitalar com InternacaoId: {id}.");
            }
        }

        // PUT: api/AltasHospitalares/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAltaHospitalar(Guid id, [FromBody] UpdateAltaHospitalarDto altaDto)
        {
            if (id != altaDto.InternacaoId)
            {
                return BadRequest("O ID na URL não corresponde ao InternacaoId no corpo da requisição.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var altaToUpdate = await _context.AltasHospitalares.FindAsync(id);
                if (altaToUpdate == null)
                {
                    return NotFound("Alta hospitalar não encontrada para atualização.");
                }

                if (!await _context.Internacoes.AnyAsync(i => i.Id == altaDto.InternacaoId))
                {
                    return BadRequest("A Internação associada a esta alta hospitalar não existe.");
                }

                altaToUpdate.DataAlta = altaDto.DataAlta;
                altaToUpdate.CondicaoPaciente = altaDto.CondicaoPaciente;
                altaToUpdate.InstrucoesPosAlta = altaDto.InstrucoesPosAlta;

                _context.Entry(altaToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _context.Entry(altaToUpdate).Reference(a => a.Internacao).LoadAsync();

                return Ok(MapToResponseDto(altaToUpdate));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AltaHospitalarExists(id))
                {
                    return NotFound("Alta hospitalar não encontrada para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar alta hospitalar com InternacaoId: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. A alta hospitalar foi modificada ou excluída por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar alta hospitalar com InternacaoId: {id}. Dados da alta: {JsonConvert.SerializeObject(altaDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar alta hospitalar. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar alta hospitalar com InternacaoId: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar alta hospitalar.");
            }
        }

        // POST: api/AltasHospitalares
        [HttpPost]
        public async Task<ActionResult<AltaHospitalarResponseDto>> PostAltaHospitalar(CreateAltaHospitalarDto altaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var internacaoExistente = await _context.Internacoes.FindAsync(altaDto.InternacaoId);
            if (internacaoExistente == null)
            {
                return BadRequest("A InternacaoId fornecida não existe. Por favor, insira um ID de internação válido.");
            }

            if (await _context.AltasHospitalares.AnyAsync(a => a.InternacaoId == altaDto.InternacaoId))
            {
                _logger.LogWarning($"Tentativa de criar alta para InternacaoId: {altaDto.InternacaoId}, mas já existe uma alta associada.");
                return Conflict($"Já existe uma alta hospitalar para a internação com ID '{altaDto.InternacaoId}'. Cada internação pode ter apenas uma alta.");
            }

            var altaHospitalar = new AltaHospitalar
            {
                InternacaoId = altaDto.InternacaoId,
                DataAlta = altaDto.DataAlta,
                CondicaoPaciente = altaDto.CondicaoPaciente,
                InstrucoesPosAlta = altaDto.InstrucoesPosAlta
            };

            try
            {
                _context.AltasHospitalares.Add(altaHospitalar);
                await _context.SaveChangesAsync();

                // Carrega a Internação para o DTO de retorno
                await _context.Entry(altaHospitalar).Reference(a => a.Internacao).LoadAsync();

                var responseDto = MapToResponseDto(altaHospitalar);
                return CreatedAtAction("GetAltaHospitalar", new { id = responseDto.InternacaoId }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row in object 'dbo.AltasHospitalares'") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de criar alta para InternacaoId: {altaDto.InternacaoId}, mas já existe uma alta associada.");
                    return Conflict($"Já existe uma alta hospitalar para a internação com ID '{altaDto.InternacaoId}'. Cada internação pode ter apenas uma alta.");
                }
                _logger.LogError(ex, $"Erro de banco de dados ao criar alta hospitalar. Dados da alta: {JsonConvert.SerializeObject(altaDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar alta hospitalar. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar alta hospitalar. Dados da alta: {JsonConvert.SerializeObject(altaDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar alta hospitalar.");
            }
        }

        // DELETE: api/AltasHospitalares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAltaHospitalar(Guid id)
        {
            try
            {
                var alta = await _context.AltasHospitalares.FindAsync(id);
                if (alta == null)
                {
                    return NotFound("Alta hospitalar não encontrada para exclusão.");
                }

                _context.AltasHospitalares.Remove(alta);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir alta hospitalar com InternacaoId: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir alta hospitalar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir alta hospitalar com InternacaoId: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir alta hospitalar.");
            }
        }

        private bool AltaHospitalarExists(Guid id)
        {
            return _context.AltasHospitalares.Any(e => e.InternacaoId == id);
        }
    }
}