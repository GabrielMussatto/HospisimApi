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
using HospisimApi.DTOs;

namespace HospisimApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspecialidadesController : ControllerBase
    {
        private readonly HospisimDbContext _context;
        private readonly ILogger<EspecialidadesController> _logger;

        public EspecialidadesController(HospisimDbContext context, ILogger<EspecialidadesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Especialidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Especialidade>>> GetEspecialidades()
        {
            try
            {
                if (!_context.Especialidades.Any())
                {
                    return NotFound("Nenhuma especialidade encontrada.");
                }
                return await _context.Especialidades.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter a lista de especialidades.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao obter especialidades.");
            }
        }

        // GET: api/Especialidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Especialidade>> GetEspecialidade(Guid id)
        {
            try
            {
                var especialidade = await _context.Especialidades.FindAsync(id);

                if (especialidade == null)
                {
                    return NotFound("Especialidade não encontrada.");
                }

                return especialidade;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar obter especialidade com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno do servidor ao obter especialidade com ID: {id}.");
            }
        }

        // PUT: api/Especialidades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEspecialidade(Guid id, [FromBody] UpdateEspecialidadeDto especialidadeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var especialidadeToUpdate = await _context.Especialidades.FindAsync(id);
                if (especialidadeToUpdate == null)
                {
                    return NotFound("Especialidade não encontrada para atualização.");
                }

                especialidadeToUpdate.Nome = especialidadeDto.Nome;

                _context.Entry(especialidadeToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!EspecialidadeExists(id))
                {
                    return NotFound("Especialidade não encontrada para atualização.");
                }
                else
                {
                    _logger.LogError(ex, $"Erro de concorrência ao atualizar especialidade com ID: {id}.");
                    return StatusCode(StatusCodes.Status409Conflict, "Conflito de concorrência. A especialidade foi modificada ou excluída por outro usuário.");
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de atualizar especialidade com nome duplicado: {especialidadeDto.Nome}.");
                    return Conflict($"Já existe uma especialidade com o nome '{especialidadeDto.Nome}'.");
                }
                _logger.LogError(ex, $"Erro de banco de dados ao atualizar especialidade com ID: {id}. Dados da especialidade: {JsonConvert.SerializeObject(especialidadeDto)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar especialidade. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar atualizar especialidade com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao atualizar especialidade.");
            }
        }

        // POST: api/Especialidades
        [HttpPost]
        public async Task<ActionResult<Especialidade>> PostEspecialidade(CreateEspecialidadeDto especialidadeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var especialidade = new Especialidade
            {
                Id = Guid.NewGuid(),
                Nome = especialidadeDto.Nome
            };

            try
            {
                _context.Especialidades.Add(especialidade);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetEspecialidade", new { id = especialidade.Id }, especialidade);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("duplicate key value is unique index") == true)
                {
                    _logger.LogWarning(ex, $"Tentativa de inserir especialidade com nome duplicado: {especialidade.Nome}.");
                    return Conflict($"Já existe uma especialidade com o nome '{especialidade.Nome}'.");
                }
                _logger.LogError(ex, $"Erro de banco de dados ao criar especialidade. Dados da especialidade: {JsonConvert.SerializeObject(especialidade)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar especialidade. Verifique os dados fornecidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar criar especialidade. Dados da especialidade: {JsonConvert.SerializeObject(especialidade)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao criar especialidade.");
            }
        }

        // DELETE: api/Especialidades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEspecialidade(Guid id)
        {
            try
            {
                var especialidade = await _context.Especialidades.FindAsync(id);
                if (especialidade == null)
                {
                    return NotFound("Especialidade não encontrada para exclusão.");
                }

                if (await _context.ProfissionaisSaude.AnyAsync(ps => ps.EspecialidadeId == id))
                {
                    _logger.LogWarning($"Tentativa de excluir especialidade com ID: {id}, mas existem profissionais vinculados.");
                    return Conflict("Não é possível excluir esta especialidade, pois existem profissionais de saúde vinculados a ela.");
                }

                _context.Especialidades.Remove(especialidade);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erro de banco de dados ao excluir especialidade com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir especialidade.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar excluir especialidade com ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor ao excluir especialidade.");
            }
        }

        private bool EspecialidadeExists(Guid id)
        {
            return _context.Especialidades.Any(e => e.Id == id);
        }
    }
}