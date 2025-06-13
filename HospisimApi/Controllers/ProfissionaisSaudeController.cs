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
using HospisimApi.DTOs;

namespace HospisimApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfissionaisSaudeController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<ProfissionaisSaudeController> _logger;

        public ProfissionaisSaudeController(HospisimDbContext context, ILogger<ProfissionaisSaudeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ProfissionaisSaude
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfissionalSaudeResponseDto>>> GetProfissionaisSaude()
        {
            try
            {
                var profissionais = await _context.ProfissionaisSaude
                                                .Include(ps => ps.Especialidade)
                                                .Select(ps => new ProfissionalSaudeResponseDto
                                                {
                                                    Id = ps.Id,
                                                    NomeCompleto = ps.NomeCompleto,
                                                    CPFFormatado = ps.CPFFormatado,
                                                    Email = ps.Email,
                                                    TelefoneFormatado = ps.TelefoneFormatado,
                                                    RegistroConselho = ps.RegistroConselho,
                                                    TipoRegistro = ps.TipoRegistro,
                                                    Especialidade = ps.Especialidade != null ? new EspecialidadeResumidoDto
                                                    {
                                                        Id = ps.Especialidade.Id,
                                                        Nome = ps.Especialidade.Nome
                                                    } : null,
                                                    DataAdmissao = ps.DataAdmissao,
                                                    CargaHorariaSemanal = ps.CargaHorariaSemanal,
                                                    Turno = ps.Turno,
                                                    Ativo = ps.Ativo
                                                })
                                                .ToListAsync();

                if (!profissionais.Any())
                {
                    return NotFound("Nenhum profissional de saúde encontrado.");
                }
                return profissionais;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de profissionais de saúde.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter profissionais de saúde.");
            }
        }

        // GET: api/ProfissionaisSaude/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfissionalSaudeResponseDto>> GetProfissionalSaude(Guid id)
        {
            try
            {
                var profissionalSaude = await _context.ProfissionaisSaude
                                                    .Include(ps => ps.Especialidade)
                                                    .FirstOrDefaultAsync(ps => ps.Id == id);

                if (profissionalSaude == null)
                {
                    return NotFound("Profissional de saúde não encontrado.");
                }

                var profissionalSaudeDto = new ProfissionalSaudeResponseDto
                {
                    Id = profissionalSaude.Id,
                    NomeCompleto = profissionalSaude.NomeCompleto,
                    CPFFormatado = profissionalSaude.CPFFormatado,
                    Email = profissionalSaude.Email,
                    TelefoneFormatado = profissionalSaude.TelefoneFormatado,
                    RegistroConselho = profissionalSaude.RegistroConselho,
                    TipoRegistro = profissionalSaude.TipoRegistro,
                    Especialidade = profissionalSaude.Especialidade != null ? new EspecialidadeResumidoDto
                    {
                        Id = profissionalSaude.Especialidade.Id,
                        Nome = profissionalSaude.Especialidade.Nome
                    } : null,
                    DataAdmissao = profissionalSaude.DataAdmissao,
                    CargaHorariaSemanal = profissionalSaude.CargaHorariaSemanal,
                    Turno = profissionalSaude.Turno,
                    Ativo = profissionalSaude.Ativo
                };

                return profissionalSaudeDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter profissional de saúde com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter profissional de saúde com ID: {id}.");
            }
        }

        // PUT: api/ProfissionaisSaude/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfissionalSaude(Guid id, [FromBody] UpdateProfissionalSaudeDto profissionalSaudeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var profissionalToUpdate = await _context.ProfissionaisSaude.FindAsync(id);
                if (profissionalToUpdate == null)
                {
                    return NotFound("Profissional de saúde não encontrado para atualização.");
                }

                if (!await _context.Especialidades.AnyAsync(e => e.Id == profissionalSaudeDto.EspecialidadeId))
                {
                    return BadRequest("O EspecialidadeId fornecido não existe. Por favor, insira um ID de especialidade válido.");
                }

                profissionalToUpdate.NomeCompleto = profissionalSaudeDto.NomeCompleto;
                profissionalToUpdate.CPF = new string(profissionalSaudeDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                profissionalToUpdate.Email = profissionalSaudeDto.Email;
                profissionalToUpdate.Telefone = new string(profissionalSaudeDto.Telefone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                profissionalToUpdate.RegistroConselho = profissionalSaudeDto.RegistroConselho;
                profissionalToUpdate.TipoRegistro = profissionalSaudeDto.TipoRegistro;
                profissionalToUpdate.EspecialidadeId = profissionalSaudeDto.EspecialidadeId;
                profissionalToUpdate.DataAdmissao = profissionalSaudeDto.DataAdmissao;
                profissionalToUpdate.CargaHorariaSemanal = profissionalSaudeDto.CargaHorariaSemanal;
                profissionalToUpdate.Turno = profissionalSaudeDto.Turno;
                profissionalToUpdate.Ativo = profissionalSaudeDto.Ativo;

                _context.Entry(profissionalToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _context.Entry(profissionalToUpdate).Reference(ps => ps.Especialidade).LoadAsync();

                var updatedProfissionalDto = new ProfissionalSaudeResponseDto
                {
                    Id = profissionalToUpdate.Id,
                    NomeCompleto = profissionalToUpdate.NomeCompleto,
                    CPFFormatado = profissionalToUpdate.CPFFormatado,
                    Email = profissionalToUpdate.Email,
                    TelefoneFormatado = profissionalToUpdate.TelefoneFormatado,
                    RegistroConselho = profissionalToUpdate.RegistroConselho,
                    TipoRegistro = profissionalToUpdate.TipoRegistro,
                    Especialidade = profissionalToUpdate.Especialidade != null ? new EspecialidadeResumidoDto
                    {
                        Id = profissionalToUpdate.Especialidade.Id,
                        Nome = profissionalToUpdate.Especialidade.Nome
                    } : null,
                    DataAdmissao = profissionalToUpdate.DataAdmissao,
                    CargaHorariaSemanal = profissionalToUpdate.CargaHorariaSemanal,
                    Turno = profissionalToUpdate.Turno,
                    Ativo = profissionalToUpdate.Ativo
                };
                return Ok(updatedProfissionalDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProfissionalSaudeExists(id))
                {
                    return NotFound("Profissional de saúde não encontrado para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar profissional de saúde com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. O profissional de saúde foi modificado ou excluído por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    string errorMessage = "Violação de restrição de unicidade: ";
                    if (ex.InnerException.Message.Contains("CPF"))
                        errorMessage += $"Já existe um profissional com o CPF '{new string(profissionalSaudeDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>())}'.";
                    else if (ex.InnerException.Message.Contains("RegistroConselho"))
                        errorMessage += $"Já existe um profissional com o Registro do Conselho '{profissionalSaudeDto.RegistroConselho}'.";
                    else if (ex.InnerException.Message.Contains("Email"))
                        errorMessage += $"Já existe um profissional com o e-mail '{profissionalSaudeDto.Email}'.";
                    else
                        errorMessage += "Verifique os dados únicos (CPF, Registro do Conselho, Email).";

                    _logger.LogWarning(ex, $"Tentativa de atualizar profissional com dados duplicados: {JsonConvert.SerializeObject(profissionalSaudeDto)}");
                    return Conflict(errorMessage);
                }
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar profissional de saúde com ID: {id}. Dados do profissional: {JsonConvert.SerializeObject(profissionalSaudeDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar profissional de saúde. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar profissional de saúde com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar profissional de saúde.");
            }
        }

        // POST: api/ProfissionaisSaude
        [HttpPost]
        public async Task<ActionResult<ProfissionalSaudeResponseDto>> PostProfissionalSaude(CreateProfissionalSaudeDto profissionalSaudeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _context.Especialidades.AnyAsync(e => e.Id == profissionalSaudeDto.EspecialidadeId))
            {
                return BadRequest("O EspecialidadeId fornecido não existe. Por favor, insira um ID de especialidade válido.");
            }

            var profissionalSaude = new ProfissionalSaude
            {
                Id = Guid.NewGuid(),
                NomeCompleto = profissionalSaudeDto.NomeCompleto,
                CPF = new string(profissionalSaudeDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()),
                Email = profissionalSaudeDto.Email,
                Telefone = new string(profissionalSaudeDto.Telefone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()),
                RegistroConselho = profissionalSaudeDto.RegistroConselho,
                TipoRegistro = profissionalSaudeDto.TipoRegistro,
                EspecialidadeId = profissionalSaudeDto.EspecialidadeId,
                DataAdmissao = profissionalSaudeDto.DataAdmissao,
                CargaHorariaSemanal = profissionalSaudeDto.CargaHorariaSemanal,
                Turno = profissionalSaudeDto.Turno,
                Ativo = profissionalSaudeDto.Ativo
            };

            try
            {
                _context.ProfissionaisSaude.Add(profissionalSaude);
                await _context.SaveChangesAsync();

                await _context.Entry(profissionalSaude).Reference(ps => ps.Especialidade).LoadAsync();

                var responseDto = new ProfissionalSaudeResponseDto
                {
                    Id = profissionalSaude.Id,
                    NomeCompleto = profissionalSaude.NomeCompleto,
                    CPFFormatado = profissionalSaude.CPFFormatado,
                    Email = profissionalSaude.Email,
                    TelefoneFormatado = profissionalSaude.TelefoneFormatado,
                    RegistroConselho = profissionalSaude.RegistroConselho,
                    TipoRegistro = profissionalSaude.TipoRegistro,
                    Especialidade = profissionalSaude.Especialidade != null ? new EspecialidadeResumidoDto
                    {
                        Id = profissionalSaude.Especialidade.Id,
                        Nome = profissionalSaude.Especialidade.Nome
                    } : null,
                    DataAdmissao = profissionalSaude.DataAdmissao,
                    CargaHorariaSemanal = profissionalSaude.CargaHorariaSemanal,
                    Turno = profissionalSaude.Turno,
                    Ativo = profissionalSaude.Ativo
                };

                return CreatedAtAction("GetProfissionalSaude", new { id = responseDto.Id }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    string errorMessage = "Violação de restrição de unicidade: ";
                    if (ex.InnerException.Message.Contains("CPF"))
                        errorMessage += $"Já existe um profissional com o CPF '{new string(profissionalSaudeDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>())}'.";
                    else if (ex.InnerException.Message.Contains("RegistroConselho"))
                        errorMessage += $"Já existe um profissional com o Registro do Conselho '{profissionalSaudeDto.RegistroConselho}'.";
                    else if (ex.InnerException.Message.Contains("Email"))
                        errorMessage += $"Já existe um profissional com o e-mail '{profissionalSaudeDto.Email}'.";
                    else
                        errorMessage += "Verifique os dados únicos (CPF, Registro do Conselho, Email).";

                    _logger.LogWarning(ex, $"Tentativa de inserir profissional com dados duplicados: {JsonConvert.SerializeObject(profissionalSaudeDto)}");
                    return Conflict(errorMessage);
                }
                _logger.LogError(ex, $"Erro de banco de dados ao criar profissional de saúde. Dados do profissional: {JsonConvert.SerializeObject(profissionalSaudeDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar profissional de saúde. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar profissional de saúde. Dados do profissional: {JsonConvert.SerializeObject(profissionalSaudeDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar profissional de saúde.");
            }
        }

        // DELETE: api/ProfissionaisSaude/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfissionalSaude(Guid id)
        {
            try
            {
                var profissionalSaude = await _context.ProfissionaisSaude.FindAsync(id);
                if (profissionalSaude == null)
                {
                    return NotFound("Profissional de saúde não encontrado para exclusão.");
                }

                if (await _context.Atendimentos.AnyAsync(a => a.ProfissionalSaudeId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir profissional de saúde com ID: {id}, mas existem atendimentos vinculados.");
                    return Conflict("Não é possível excluir este profissional, pois existem atendimentos vinculados a ele.");
                }
                if (await _context.Prescricoes.AnyAsync(p => p.ProfissionalId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir profissional de saúde com ID: {id}, mas existem prescrições vinculadas.");
                    return Conflict("Não é possível excluir este profissional, pois existem prescrições vinculadas a ele.");
                }

                _context.ProfissionaisSaude.Remove(profissionalSaude);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir profissional de saúde com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir profissional de saúde.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir profissional de saúde com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir profissional de saúde.");
            }
        }

        private bool ProfissionalSaudeExists(Guid id)
        {
            return _context.ProfissionaisSaude.Any(e => e.Id == id);
        }
    }
}