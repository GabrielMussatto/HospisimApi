using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospisimApi.Data;
using HospisimApi.Models;

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

        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
            try
            {
                if (!_context.Pacientes.Any())
                {
                    return NotFound("Nenhum paciente encontrado.");
                }
                return await _context.Pacientes.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de pacientes.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter pacientes.");
            }
        }

        // GET: api/Pacientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> GetPaciente(Guid id)
        {
            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);

                if (paciente == null)
                {
                    return NotFound();
                }

                return paciente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter paciente com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter paciente com ID: {id}.");
            }
        }

        // PUT: api/Pacientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(Guid id, Paciente paciente)
        {
            if (id != paciente.Id)
            {
                return BadRequest("O ID na URL não corresponde ao ID do paciente no corpo da requisição.");
            }

            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
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
                    _logger.LogWarning(ex, $"Tentativa de atualizar paciente com CPF duplicado: {paciente.CPF}.");
                    return Conflict($"Já existe um paciente com o CPF '{paciente.CPF}'.");
                }

                _logger.LogError(ex, $"Erro de banco de dados ao atualizar paciente com ID: {id}. Dados do paciente: {Newtonsoft.Json.JsonConvert.SerializeObject(paciente)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar paciente. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar paciente com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar paciente.");
            }
        }

        // POST: api/Pacientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Paciente>> PostPaciente(Paciente paciente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            paciente.Id = Guid.NewGuid();

            try
            {
                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPaciente", new { id = paciente.Id }, paciente);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de inserir paciente com CPF duplicado: {paciente.CPF}.");
                    return Conflict($"Já existe um paciente com o CPF '{paciente.CPF}'.");
                }

                _logger.LogError(ex, $"Erro de banco de dados ao criar paciente. Dados do paciente: {Newtonsoft.Json.JsonConvert.SerializeObject(paciente)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar paciente. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar paciente. Dados do paciente: {Newtonsoft.Json.JsonConvert.SerializeObject(paciente)}");
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
