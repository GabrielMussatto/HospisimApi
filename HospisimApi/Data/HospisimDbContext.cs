using Microsoft.EntityFrameworkCore;
using HospisimApi.Models;
using HospisimApi.Enums;
using System.Linq;
using System;

namespace HospisimApi.Data
{
    public class HospisimDbContext : DbContext
    {
        public HospisimDbContext(DbContextOptions<HospisimDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Prontuario> Prontuarios { get; set; }
        // public DbSet<ProfissionalSaude> ProfissionaisSaude { get; set; } // Descomentar quando criar a entidade
        // public DbSet<Especialidade> Especialidades { get; set; }         // Descomentar quando criar a entidade
        // public DbSet<Atendimento> Atendimentos { get; set; }             // Descomentar quando criar a entidade
        // public DbSet<Prescricao> Prescricoes { get; set; }               // Descomentar quando criar a entidade
        // public DbSet<Exame> Exames { get; set; }                         // Descomentar quando criar a entidade
        // public DbSet<Internacao> Internacoes { get; set; }               // Descomentar quando criar a entidade
        // public DbSet<AltaHospitalar> AltasHospitalares { get; set; }     // Descomentar quando criar a entidade

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações para Paciente
            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.CPF)
                .IsUnique();

            // Configurações para Prontuário
            modelBuilder.Entity<Prontuario>()
                .HasIndex(p => p.NumeroProntuario)
                .IsUnique();

            // Relacionamento Paciente 1:N Prontuário
            modelBuilder.Entity<Prontuario>()
                .HasOne(p => p.Paciente)
                .WithMany(pa => pa.Prontuarios)
                .HasForeignKey(p => p.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quando adicionar as outras entidades, as configurações de relacionamento virão aqui.
            // Exemplo (manter comentado por enquanto):
            /*
            // Atendimento 0..1 : 1 Internacao
            modelBuilder.Entity<Internacao>()
                .HasOne(i => i.Atendimento)
                .WithOne(a => a.Internacao)
                .HasForeignKey<Internacao>(i => i.AtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Internacao 0..1 : 1 AltaHospitalar
            modelBuilder.Entity<AltaHospitalar>()
                .HasOne(a => a.Internacao)
                .WithOne(i => i.AltaHospitalar)
                .HasForeignKey<AltaHospitalar>(a => a.InternacaoId);

            // Relacionamento Profissional 1:N Atendimento
            modelBuilder.Entity<Atendimento>()
                .HasOne(a => a.ProfissionalSaude)
                .WithMany(ps => ps.Atendimentos)
                .HasForeignKey(a => a.ProfissionalSaudeId);

            // Relacionamento Atendimento 1:N Prescrição
            modelBuilder.Entity<Prescricao>()
                .HasOne(p => p.Atendimento)
                .WithMany(a => a.Prescricoes)
                .HasForeignKey(p => p.AtendimentoId);

            // Relacionamento Atendimento 1:N Exame
            modelBuilder.Entity<Exame>()
                .HasOne(e => e.Atendimento)
                .WithMany(a => a.Exames)
                .HasForeignKey(e => e.AtendimentoId);

            // Relacionamento Profissional N:1 Especialidade
            modelBuilder.Entity<ProfissionalSaude>()
                .HasOne(ps => ps.Especialidade)
                .WithMany(e => e.ProfissionaisSaude)
                .HasForeignKey(ps => ps.EspecialidadeId);
            */

            base.OnModelCreating(modelBuilder);
        }

        // O método SeedData
        public static void SeedData(HospisimDbContext context)
        {
            if (context.Pacientes.Any())
            {
                return;
            }

            // --- PACIENTES ---
            var pacientes = new List<Paciente>();

            var paciente1 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Alice da Silva",
                CPF = "12345678901",
                DataNascimento = new DateTime(1990, 05, 10),
                Sexo = SexoEnum.Feminino,
                TipoSanguineo = TipoSanguineoEnum.APositivo,
                Telefone = "988881111",
                Email = "alice.silva@email.com",
                EnderecoCompleto = "Rua das Flores, 123, Centro, Cidade",
                NumeroCartaoSUS = "987654321012345",
                EstadoCivil = EstadoCivilEnum.Solteiro,
                PossuiPlanoSaude = true
            };
            pacientes.Add(paciente1);

            var paciente2 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Bruno Costa",
                CPF = "01234567890",
                DataNascimento = new DateTime(1985, 11, 20),
                Sexo = SexoEnum.Masculino,
                TipoSanguineo = TipoSanguineoEnum.ONegativo,
                Telefone = "977772222",
                Email = "bruno.costa@email.com",
                EnderecoCompleto = "Av. Principal, 456, Bairro Novo, Cidade",
                NumeroCartaoSUS = "123456789012345",
                EstadoCivil = EstadoCivilEnum.Casado,
                PossuiPlanoSaude = false
            };
            pacientes.Add(paciente2);

            var paciente3 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Carla Pereira",
                CPF = "12121212121",
                DataNascimento = new DateTime(2000, 01, 15),
                Sexo = SexoEnum.Feminino,
                TipoSanguineo = TipoSanguineoEnum.BPositivo,
                Telefone = "966663333",
                Email = "carla.pereira@email.com",
                EnderecoCompleto = "Rua do Sol, 789, Vila Feliz, Cidade",
                NumeroCartaoSUS = "234567890123456",
                EstadoCivil = EstadoCivilEnum.Solteiro,
                PossuiPlanoSaude = true
            };
            pacientes.Add(paciente3);

            var paciente4 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Daniela Santos",
                CPF = "34343434343",
                DataNascimento = new DateTime(1975, 07, 07),
                Sexo = SexoEnum.Feminino,
                TipoSanguineo = TipoSanguineoEnum.ABNegativo,
                Telefone = "955554444",
                Email = "daniela.santos@email.com",
                EnderecoCompleto = "Travessa da Lua, 101, Jardim Alto, Cidade",
                NumeroCartaoSUS = "345678901234567",
                EstadoCivil = EstadoCivilEnum.Divorciado,
                PossuiPlanoSaude = false
            };
            pacientes.Add(paciente4);

            var paciente5 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Eduardo Rocha",
                CPF = "56565656565",
                DataNascimento = new DateTime(1960, 03, 03),
                Sexo = SexoEnum.Masculino,
                TipoSanguineo = TipoSanguineoEnum.OPositivo,
                Telefone = "944445555",
                Email = "eduardo.rocha@email.com",
                EnderecoCompleto = "Av. Brasil, 2020, Centro, Cidade",
                NumeroCartaoSUS = "456789012345678",
                EstadoCivil = EstadoCivilEnum.Casado,
                PossuiPlanoSaude = true
            };
            pacientes.Add(paciente5);

            var paciente6 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Fernanda Gomes",
                CPF = "78787878787",
                DataNascimento = new DateTime(1995, 12, 25),
                Sexo = SexoEnum.Feminino,
                TipoSanguineo = TipoSanguineoEnum.ANegativo,
                Telefone = "933336666",
                Email = "fernanda.gomes@email.com",
                EnderecoCompleto = "Rua Nova, 303, Vila Bela, Cidade",
                NumeroCartaoSUS = "567890123456789",
                EstadoCivil = EstadoCivilEnum.Solteiro,
                PossuiPlanoSaude = false
            };
            pacientes.Add(paciente6);

            var paciente7 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Gustavo Martins",
                CPF = "90909090909",
                DataNascimento = new DateTime(1980, 02, 14),
                Sexo = SexoEnum.Masculino,
                TipoSanguineo = TipoSanguineoEnum.BNegativo,
                Telefone = "922227777",
                Email = "gustavo.martins@email.com",
                EnderecoCompleto = "Estrada Velha, 404, Zona Rural, Cidade",
                NumeroCartaoSUS = "678901234567890",
                EstadoCivil = EstadoCivilEnum.Casado,
                PossuiPlanoSaude = true
            };
            pacientes.Add(paciente7);

            var paciente8 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Heloisa Ribeiro",
                CPF = "02020202020",
                DataNascimento = new DateTime(2010, 09, 01),
                Sexo = SexoEnum.Feminino,
                TipoSanguineo = TipoSanguineoEnum.ABPositivo,
                Telefone = "911118888",
                Email = "heloisa.ribeiro@email.com",
                EnderecoCompleto = "Praça Central, 505, Centro, Cidade",
                NumeroCartaoSUS = "789012345678901",
                EstadoCivil = EstadoCivilEnum.Solteiro,
                PossuiPlanoSaude = true
            };
            pacientes.Add(paciente8);

            var paciente9 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Igor Silveira",
                CPF = "45454545454",
                DataNascimento = new DateTime(1970, 04, 22),
                Sexo = SexoEnum.Masculino,
                TipoSanguineo = TipoSanguineoEnum.APositivo,
                Telefone = "900009999",
                Email = "igor.silveira@email.com",
                EnderecoCompleto = "Rua A, 606, Bairro Industrial, Cidade",
                NumeroCartaoSUS = "890123456789012",
                EstadoCivil = EstadoCivilEnum.Casado,
                PossuiPlanoSaude = false
            };
            pacientes.Add(paciente9);

            var paciente10 = new Paciente
            {
                Id = Guid.NewGuid(),
                NomeCompleto = "Joana Nogueira",
                CPF = "67676767676",
                DataNascimento = new DateTime(1988, 08, 18),
                Sexo = SexoEnum.Feminino,
                TipoSanguineo = TipoSanguineoEnum.ONegativo,
                Telefone = "987654321",
                Email = "joana.nogueira@email.com",
                EnderecoCompleto = "Av. do Contorno, 707, Cidade Velha, Cidade",
                NumeroCartaoSUS = "901234567890123",
                EstadoCivil = EstadoCivilEnum.Solteiro,
                PossuiPlanoSaude = true
            };
            pacientes.Add(paciente10);

            context.Pacientes.AddRange(pacientes);
            context.SaveChanges();

            // --- PRONTUÁRIOS ---
            var prontuario1 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT001", DataAbertura = DateTime.Now.AddDays(-30), ObservacoesGerais = "Paciente com histórico de asma.", PacienteId = paciente1.Id };
            var prontuario2 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT002", DataAbertura = DateTime.Now.AddDays(-25), ObservacoesGerais = "Nenhuma observação relevante.", PacienteId = paciente2.Id };
            var prontuario3 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT003", DataAbertura = DateTime.Now.AddDays(-20), ObservacoesGerais = "Histórico familiar de diabetes.", PacienteId = paciente3.Id };
            var prontuario4 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT004", DataAbertura = DateTime.Now.AddDays(-18), ObservacoesGerais = "Paciente em acompanhamento nutricional.", PacienteId = paciente4.Id };
            var prontuario5 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT005", DataAbertura = DateTime.Now.AddDays(-15), ObservacoesGerais = "Alerta para alergia a penicilina.", PacienteId = paciente5.Id };
            var prontuario6 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT006", DataAbertura = DateTime.Now.AddDays(-12), ObservacoesGerais = "Paciente atleta, sem histórico de doenças crônicas.", PacienteId = paciente6.Id };
            var prontuario7 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT007", DataAbertura = DateTime.Now.AddDays(-10), ObservacoesGerais = "Criança com calendário vacinal em dia.", PacienteId = paciente7.Id };
            var prontuario8 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT008", DataAbertura = DateTime.Now.AddDays(-8), ObservacoesGerais = "Criança com calendário vacinal em dia.", PacienteId = paciente8.Id };
            var prontuario9 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT009", DataAbertura = DateTime.Now.AddDays(-5), ObservacoesGerais = "Paciente idoso, atenção à polifarmácia.", PacienteId = paciente9.Id };
            var prontuario10 = new Prontuario { Id = Guid.NewGuid(), NumeroProntuario = "PRONT010", DataAbertura = DateTime.Now.AddDays(-3), ObservacoesGerais = "Realizou exames de rotina recentemente.", PacienteId = paciente10.Id };
           

            context.Prontuarios.AddRange(
                prontuario1, prontuario2, prontuario3, prontuario4, prontuario5,
                prontuario6, prontuario7, prontuario8, prontuario9, prontuario10
            );
            context.SaveChanges();
        }
    }
}