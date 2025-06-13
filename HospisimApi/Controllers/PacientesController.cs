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
    public class PacientesController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<PacientesController> _logger;

        public PacientesController(HospisimDbContext context, ILogger<PacientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private PacienteResponseDto MapToResponseDto(Paciente paciente)
        {
            return new PacienteResponseDto
            {
                Id = paciente.Id,
                NomeCompleto = paciente.NomeCompleto,
                CPFFormatado = paciente.CPFFormatado,
                DataNascimento = paciente.DataNascimento,
                Sexo = paciente.Sexo,
                TipoSanguineo = paciente.TipoSanguineo,
                TelefoneFormatado = paciente.TelefoneFormatado,
                Email = paciente.Email,
                EnderecoCompleto = paciente.EnderecoCompleto,
                NumeroCartaoSUS = paciente.NumeroCartaoSUS,
                EstadoCivil = paciente.EstadoCivil,
                PossuiPlanoSaude = paciente.PossuiPlanoSaude
            };
        }


        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteResponseDto>>> GetPacientes()
        {
            try
            {
                var pacientes = await _context.Pacientes.ToListAsync();

                if (!pacientes.Any())
                {
                    return NotFound("Nenhum paciente encontrado.");
                }
                return pacientes.Select(p => MapToResponseDto(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de pacientes.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter pacientes.");
            }
        }

        // GET: api/Pacientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PacienteResponseDto>> GetPaciente(Guid id)
        {
            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);

                if (paciente == null)
                {
                    return NotFound("Paciente não encontrado.");
                }

                return MapToResponseDto(paciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter paciente com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter paciente com ID: {id}.");
            }
        }

        // PUT: api/Pacientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(Guid id, [FromBody] UpdatePacienteDto pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var pacienteToUpdate = await _context.Pacientes.FindAsync(id);
                if (pacienteToUpdate == null)
                {
                    return NotFound("Paciente não encontrado para atualização.");
                }

                pacienteToUpdate.NomeCompleto = pacienteDto.NomeCompleto;
                pacienteToUpdate.CPF = new string(pacienteDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                pacienteToUpdate.DataNascimento = pacienteDto.DataNascimento;
                pacienteToUpdate.Sexo = pacienteDto.Sexo;
                pacienteToUpdate.TipoSanguineo = pacienteDto.TipoSanguineo;
                pacienteToUpdate.Telefone = new string(pacienteDto.Telefone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                pacienteToUpdate.Email = pacienteDto.Email;
                pacienteToUpdate.EnderecoCompleto = pacienteDto.EnderecoCompleto;
                pacienteToUpdate.NumeroCartaoSUS = pacienteDto.NumeroCartaoSUS;
                pacienteToUpdate.EstadoCivil = pacienteDto.EstadoCivil;
                pacienteToUpdate.PossuiPlanoSaude = pacienteDto.PossuiPlanoSaude;

                _context.Entry(pacienteToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(MapToResponseDto(pacienteToUpdate));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PacienteExists(id))
                {
                    return NotFound("Paciente não encontrado para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar paciente com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. O paciente foi modificado ou excluído por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de atualizar paciente com CPF duplicado: {new string(pacienteDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>())}.");
                    return Conflict($"Já existe um paciente com o CPF '{new string(pacienteDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>())}'.");
                }

                _logger.LogError(ex, $"Erro de banco de dados ao atualizar paciente com ID: {id}. Dados do paciente: {JsonConvert.SerializeObject(pacienteDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar paciente. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar paciente com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar paciente.");
            }
        }

        // POST: api/Pacientes
        [HttpPost]
        public async Task<ActionResult<PacienteResponseDto>> PostPaciente(CreatePacienteDto pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paciente = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = pacienteDto.NomeCompleto,
                CPF = new string(pacienteDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()),
                DataNascimento = pacienteDto.DataNascimento,
                Sexo = pacienteDto.Sexo,
                TipoSanguineo = pacienteDto.TipoSanguineo,
                Telefone = new string(pacienteDto.Telefone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()),
                Email = pacienteDto.Email,
                EnderecoCompleto = pacienteDto.EnderecoCompleto,
                NumeroCartaoSUS = pacienteDto.NumeroCartaoSUS,
                EstadoCivil = pacienteDto.EstadoCivil,
                PossuiPlanoSaude = pacienteDto.PossuiPlanoSaude
            };

            try
            {
                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                var responseDto = MapToResponseDto(paciente);

                return CreatedAtAction("GetPaciente", new { id = responseDto.Id }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de inserir paciente com CPF duplicado: {new string(pacienteDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>())}.");
                    return Conflict($"Já existe um paciente com o CPF '{new string(pacienteDto.CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>())}'.");
                }

                _logger.LogError(ex, $"Erro de banco de dados ao criar paciente. Dados do paciente: {JsonConvert.SerializeObject(pacienteDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar paciente. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar paciente. Dados do paciente: {JsonConvert.SerializeObject(pacienteDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar paciente.");
            }
        }

        // DELETE: api/Pacientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(Guid id)
        {
            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);
                if (paciente == null)
                {
                    return NotFound("Paciente não encontrado para exclusão.");
                }

                if (await _context.Prontuarios.AnyAsync(p => p.PacienteId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir paciente com ID: {id}, mas existem prontuários vinculados.");
                    return Conflict("Não é possível excluir o paciente, pois existem prontuários vinculados a ele.");
                }
                if (await _context.Internacoes.AnyAsync(i => i.PacienteId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir paciente com ID: {id}, mas existem internações vinculadas.");
                    return Conflict("Não é possível excluir o paciente, pois existem internações vinculadas a ele.");
                }
                if (await _context.Atendimentos.AnyAsync(a => a.PacienteId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir paciente com ID: {id}, mas existem atendimentos vinculados.");
                    return Conflict("Não é possível excluir o paciente, pois existem atendimentos vinculados a ele.");
                }

                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir paciente com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir paciente. Pode haver registros relacionados que impedem a exclusão.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir paciente com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir paciente.");
            }
        }

        private bool PacienteExists(Guid id)
        {
            return _context.Pacientes.Any(e => e.Id == id);
        }
    }
}