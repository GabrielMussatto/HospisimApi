using Microsoft.EntityFrameworkCore;
using HospisimApi.Models;
using HospisimApi.Enums;
using System.Linq;
using System;
using System.Collections.Generic;

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
        public DbSet<Especialidade> Especialidades { get; set; }
        public DbSet<ProfissionalSaude> ProfissionaisSaude { get; set; }
        public DbSet<Atendimento> Atendimentos { get; set; }
        public DbSet<Prescricao> Prescricoes { get; set; }
        public DbSet<Exame> Exames { get; set; }
        public DbSet<Internacao> Internacoes { get; set; }
        public DbSet<AltaHospitalar> AltasHospitalares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.CPF)
                .IsUnique();
            modelBuilder.Entity<Prontuario>()
                .HasIndex(p => p.NumeroProntuario)
                .IsUnique();
            modelBuilder.Entity<ProfissionalSaude>()
                .HasIndex(ps => ps.CPF)
                .IsUnique();
            modelBuilder.Entity<ProfissionalSaude>()
                .HasIndex(ps => ps.RegistroConselho)
                .IsUnique();
            modelBuilder.Entity<ProfissionalSaude>()
                .HasIndex(ps => ps.Email)
                .IsUnique();
            modelBuilder.Entity<Especialidade>()
                .HasIndex(e => e.Nome)
                .IsUnique();


            // --- RELACIONAMENTOS ---

            // Paciente 1:N Prontuário
            modelBuilder.Entity<Prontuario>()
                .HasOne(p => p.Paciente)
                .WithMany(pa => pa.Prontuarios)
                .HasForeignKey(p => p.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Especialidade 1:N ProfissionalSaude
            modelBuilder.Entity<ProfissionalSaude>()
                .HasOne(ps => ps.Especialidade)
                .WithMany(e => e.ProfissionaisSaude)
                .HasForeignKey(ps => ps.EspecialidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prontuário 1:N Atendimento
            modelBuilder.Entity<Atendimento>()
                .HasOne(a => a.Prontuario)
                .WithMany(p => p.Atendimentos)
                .HasForeignKey(a => a.ProntuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paciente 1:N Atendimento
            modelBuilder.Entity<Atendimento>()
                .HasOne(a => a.Paciente)
                .WithMany(p => p.Atendimentos)
                .HasForeignKey(a => a.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProfissionalSaude 1:N Atendimento
            modelBuilder.Entity<Atendimento>()
                .HasOne(a => a.ProfissionalSaude)
                .WithMany(ps => ps.Atendimentos)
                .HasForeignKey(a => a.ProfissionalSaudeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Atendimento 1:N Prescrição
            modelBuilder.Entity<Prescricao>()
                .HasOne(p => p.Atendimento)
                .WithMany(a => a.Prescricoes)
                .HasForeignKey(p => p.AtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProfissionalSaude 1:N Prescrição
            modelBuilder.Entity<Prescricao>()
                .HasOne(p => p.Profissional)
                .WithMany(ps => ps.Prescricoes)
                .HasForeignKey(p => p.ProfissionalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Atendimento 1:N Exame
            modelBuilder.Entity<Exame>()
                .HasOne(e => e.Atendimento)
                .WithMany(a => a.Exames)
                .HasForeignKey(e => e.AtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Atendimento 0..1:1 Internacao
            modelBuilder.Entity<Internacao>()
                .HasOne(i => i.Atendimento)
                .WithOne(a => a.Internacao)
                .HasForeignKey<Internacao>(i => i.AtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paciente 1:N Internacao
            modelBuilder.Entity<Internacao>()
                .HasOne(i => i.Paciente)
                .WithMany(p => p.Internacoes)
                .HasForeignKey(i => i.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Internacao 0..1:1 Alta Hospitalar
            modelBuilder.Entity<AltaHospitalar>()
                .HasOne(a => a.Internacao)
                .WithOne(i => i.AltaHospitalar)
                .HasForeignKey<AltaHospitalar>(a => a.InternacaoId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
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
                Telefone = "63988881111",
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
                Telefone = "63977772222",
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
                Telefone = "63966663333", 
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
                Telefone = "63955554444",
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
                Telefone = "63944445555",
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
                Telefone = "63933336666",
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
                Telefone = "63922227777",
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
                Telefone = "63911118888",
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
                Telefone = "63900009999",
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
                Telefone = "63987654321",
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
            var prontuarios = new List<Prontuario>();
            var prontuario1 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT001", 
                DataAbertura = DateTime.Now.AddDays(-30), 
                ObservacoesGerais = "Paciente com histórico de asma.", 
                PacienteId = paciente1.Id 
            };
            prontuarios.Add(prontuario1);

            var prontuario2 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT002", 
                DataAbertura = DateTime.Now.AddDays(-25), 
                ObservacoesGerais = "Nenhuma observação relevante.", 
                PacienteId = paciente2.Id 
            };
            prontuarios.Add(prontuario2);

            var prontuario3 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT003", 
                DataAbertura = DateTime.Now.AddDays(-20), 
                ObservacoesGerais = "Histórico familiar de diabetes.", 
                PacienteId = paciente3.Id 
            };
            prontuarios.Add(prontuario3);

            var prontuario4 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT004", 
                DataAbertura = DateTime.Now.AddDays(-18), 
                ObservacoesGerais = "Paciente em acompanhamento nutricional.", 
                PacienteId = paciente4.Id 
            };
            prontuarios.Add(prontuario4);

            var prontuario5 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT005", 
                DataAbertura = DateTime.Now.AddDays(-15), 
                ObservacoesGerais = "Alerta para alergia a penicilina.", 
                PacienteId = paciente5.Id 
            };
            prontuarios.Add(prontuario5);

            var prontuario6 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT006", 
                DataAbertura = DateTime.Now.AddDays(-12), 
                ObservacoesGerais = "Paciente atleta, sem histórico de doenças crônicas.", 
                PacienteId = paciente6.Id 
            };
            prontuarios.Add(prontuario6);

            var prontuario7 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT007", 
                DataAbertura = DateTime.Now.AddDays(-10), 
                ObservacoesGerais = "Criança com calendário vacinal em dia.", 
                PacienteId = paciente7.Id 
            };
            prontuarios.Add(prontuario7);

            var prontuario8 = new Prontuario 
            { 
                Id = Guid.NewGuid(),
                NumeroProntuario = "PRONT008",
                DataAbertura = DateTime.Now.AddDays(-8), 
                ObservacoesGerais = "Criança com calendário vacinal em dia.", 
                PacienteId = paciente8.Id 
            };
            prontuarios.Add(prontuario8);

            var prontuario9 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT009", 
                DataAbertura = DateTime.Now.AddDays(-5), 
                ObservacoesGerais = "Paciente idoso, atenção à polifarmácia.", 
                PacienteId = paciente9.Id 
            };
            prontuarios.Add(prontuario9);

            var prontuario10 = new Prontuario 
            { 
                Id = Guid.NewGuid(), 
                NumeroProntuario = "PRONT010", 
                DataAbertura = DateTime.Now.AddDays(-3), 
                ObservacoesGerais = "Realizou exames de rotina recentemente.", 
                PacienteId = paciente10.Id 
            };
            prontuarios.Add(prontuario10);

            context.Prontuarios.AddRange(prontuarios);
            context.SaveChanges();

            // --- ESPECIALIDADES ---
            var especialidades = new List<Especialidade>();
            var cardiologia = new Especialidade { Id = Guid.NewGuid(), Nome = "Cardiologia" };
            especialidades.Add(cardiologia);
            var pediatria = new Especialidade { Id = Guid.NewGuid(), Nome = "Pediatria" };
            especialidades.Add(pediatria);
            var ortopedia = new Especialidade { Id = Guid.NewGuid(), Nome = "Ortopedia" };
            especialidades.Add(ortopedia);
            var clinicaGeral = new Especialidade { Id = Guid.NewGuid(), Nome = "Clínica Geral" };
            especialidades.Add(clinicaGeral);
            var dermatologia = new Especialidade { Id = Guid.NewGuid(), Nome = "Dermatologia" };
            especialidades.Add(dermatologia);
            var neurologia = new Especialidade { Id = Guid.NewGuid(), Nome = "Neurologia" };
            especialidades.Add(neurologia);
            var oftalmologia = new Especialidade { Id = Guid.NewGuid(), Nome = "Oftalmologia" };
            especialidades.Add(oftalmologia);
            var ginecologia = new Especialidade { Id = Guid.NewGuid(), Nome = "Ginecologia" };
            especialidades.Add(ginecologia);
            var urologia = new Especialidade { Id = Guid.NewGuid(), Nome = "Urologia" };
            especialidades.Add(urologia);
            var psiquiatria = new Especialidade { Id = Guid.NewGuid(), Nome = "Psiquiatria" };
            especialidades.Add(psiquiatria);

            context.Especialidades.AddRange(especialidades);
            context.SaveChanges();

            // --- PROFISSIONAIS DE SAÚDE ---
            var profissionaisSaude = new List<ProfissionalSaude>();
            var medico1 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(),
                NomeCompleto = "Dr. João Silva", 
                CPF = "11111111111", 
                Email = "joao.silva@hospisim.com", 
                Telefone = "63999911111", 
                RegistroConselho = "CRM/SP 12345", 
                TipoRegistro = "CRM", 
                EspecialidadeId = cardiologia.Id, 
                DataAdmissao = new DateTime(2018, 01, 15), 
                CargaHorariaSemanal = 40, 
                Turno = "Manhã", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico1);

            var medico2 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Dra. Maria Souza", 
                CPF = "22222222222", 
                Email = "maria.souza@hospisim.com", 
                Telefone = "63999922222", 
                RegistroConselho = "CRM/RJ 54321",
                TipoRegistro = "CRM",
                EspecialidadeId = pediatria.Id,
                DataAdmissao = new DateTime(2019, 03, 10), 
                CargaHorariaSemanal = 30, 
                Turno = "Tarde", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico2);

            var enfermeira1 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Enf. Ana Paula", 
                CPF = "33333333333", 
                Email = "ana.paula@hospisim.com", 
                Telefone = "63999933333", 
                RegistroConselho = "COREN/MG 67890", 
                TipoRegistro = "COREN", 
                EspecialidadeId = clinicaGeral.Id, 
                DataAdmissao = new DateTime(2020, 05, 20),
                CargaHorariaSemanal = 36, 
                Turno = "Noite", 
                Ativo = true 
            };
            profissionaisSaude.Add(enfermeira1);

            var medico3 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Dr. Pedro Costa", 
                CPF = "44444444444", 
                Email = "pedro.costa@hospisim.com", 
                Telefone = "63999944444", 
                RegistroConselho = "CRM/SP 98765", 
                TipoRegistro = "CRM", 
                EspecialidadeId = ortopedia.Id, 
                DataAdmissao = new DateTime(2017, 02, 01), 
                CargaHorariaSemanal = 40, 
                Turno = "Manhã", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico3);

            var medico4 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Dra. Juliana Lima", 
                CPF = "55555555555", 
                Email = "juliana.lima@hospisim.com", 
                Telefone = "63999955555", 
                RegistroConselho = "CRM/RJ 11223", 
                TipoRegistro = "CRM", 
                EspecialidadeId = dermatologia.Id, 
                DataAdmissao = new DateTime(2021, 07, 01), 
                CargaHorariaSemanal = 30, 
                Turno = "Tarde", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico4);

            var enfermeira2 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Enf. Carlos Reis", 
                CPF = "66666666666", 
                Email = "carlos.reis@hospisim.com", 
                Telefone = "63999966666", 
                RegistroConselho = "COREN/SP 22334", 
                TipoRegistro = "COREN", 
                EspecialidadeId = clinicaGeral.Id, 
                DataAdmissao = new DateTime(2019, 11, 01), 
                CargaHorariaSemanal = 36,
                Turno = "Manhã", 
                Ativo = true 
            };
            profissionaisSaude.Add(enfermeira2);

            var medico5 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Dr. Fernanda Santos", 
                CPF = "77777777777", 
                Email = "fernanda.santos@hospisim.com", 
                Telefone = "63999977777", 
                RegistroConselho = "CRM/MG 33445", 
                TipoRegistro = "CRM", 
                EspecialidadeId = neurologia.Id, 
                DataAdmissao = new DateTime(2022, 01, 01), 
                CargaHorariaSemanal = 40, 
                Turno = "Tarde", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico5);

            var medico6 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Dra. Gabriel Oliveira", 
                CPF = "88888888888", 
                Email = "gabriel.oliveira@hospisim.com", 
                Telefone = "63999988888", 
                RegistroConselho = "CRM/SP 44556", 
                TipoRegistro = "CRM", 
                EspecialidadeId = oftalmologia.Id, 
                DataAdmissao = new DateTime(2016, 06, 10), 
                CargaHorariaSemanal = 30, 
                Turno = "Noite", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico6);

            var medico7 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Dr. Patricia Almeida", 
                CPF = "99999999999", 
                Email = "patricia.almeida@hospisim.com", 
                Telefone = "63999999999", 
                RegistroConselho = "CRM/RJ 55667", 
                TipoRegistro = "CRM", 
                EspecialidadeId = ginecologia.Id, 
                DataAdmissao = new DateTime(2023, 03, 15), 
                CargaHorariaSemanal = 40, 
                Turno = "Manhã", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico7);

            var medico8 = new ProfissionalSaude
            { 
                Id = Guid.NewGuid(), 
                NomeCompleto = "Dr. Roberto Pereira", 
                CPF = "00000000000", 
                Email = "roberto.pereira@hospisim.com", 
                Telefone = "63999900000", 
                RegistroConselho = "CRM/MG 66778", 
                TipoRegistro = "CRM", 
                EspecialidadeId = urologia.Id, 
                DataAdmissao = new DateTime(2015, 09, 20), 
                CargaHorariaSemanal = 30, 
                Turno = "Tarde", 
                Ativo = true 
            };
            profissionaisSaude.Add(medico8);

            context.ProfissionaisSaude.AddRange(profissionaisSaude);
            context.SaveChanges();

            // --- ATENDIMENTOS ---
            var atendimentos = new List<Atendimento>();
            var atendimento1 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddHours(-5).Date,
                Hora = new TimeSpan(10, 0, 0), // Ex: 10:00
                Tipo = TipoAtendimentoEnum.Consulta,
                Status = StatusAtendimentoEnum.Realizado,
                Local = "Consultório 01",
                PacienteId = paciente1.Id,
                ProfissionalSaudeId = medico1.Id,
                ProntuarioId = prontuario1.Id
            };
            atendimentos.Add(atendimento1);
            var atendimento2 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddHours(-3).Date,
                Hora = new TimeSpan(14, 30, 0), // Ex: 14:30
                Tipo = TipoAtendimentoEnum.Emergencia,
                Status = StatusAtendimentoEnum.EmAndamento,
                Local = "Emergência 02",
                PacienteId = paciente2.Id,
                ProfissionalSaudeId = medico2.Id,
                ProntuarioId = prontuario2.Id
            };
            atendimentos.Add(atendimento2);
            var atendimento3 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddHours(-2).Date,
                Hora = new TimeSpan(9, 15, 0),
                Tipo = TipoAtendimentoEnum.Consulta,
                Status = StatusAtendimentoEnum.Realizado,
                Local = "Consultório 03",
                PacienteId = paciente3.Id,
                ProfissionalSaudeId = medico3.Id,
                ProntuarioId = prontuario3.Id
            };
            atendimentos.Add(atendimento3);
            var atendimento4 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddHours(-1).Date,
                Hora = new TimeSpan(11, 45, 0),
                Tipo = TipoAtendimentoEnum.Emergencia,
                Status = StatusAtendimentoEnum.Realizado,
                Local = "Emergência 01",
                PacienteId = paciente4.Id,
                ProfissionalSaudeId = medico4.Id,
                ProntuarioId = prontuario4.Id
            };
            atendimentos.Add(atendimento4);
            var atendimento5 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddMinutes(-30).Date,
                Hora = new TimeSpan(16, 0, 0),
                Tipo = TipoAtendimentoEnum.Internacao,
                Status = StatusAtendimentoEnum.EmAndamento,
                Local = "UTI 01",
                PacienteId = paciente5.Id,
                ProfissionalSaudeId = medico5.Id,
                ProntuarioId = prontuario5.Id
            };
            atendimentos.Add(atendimento5);
            var atendimento6 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddDays(-1).Date,
                Hora = new TimeSpan(8, 0, 0),
                Tipo = TipoAtendimentoEnum.Consulta,
                Status = StatusAtendimentoEnum.Realizado,
                Local = "Consultório 02",
                PacienteId = paciente6.Id,
                ProfissionalSaudeId = medico6.Id,
                ProntuarioId = prontuario6.Id
            };
            atendimentos.Add(atendimento6);
            var atendimento7 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddDays(-2).Date,
                Hora = new TimeSpan(18, 5, 0),
                Tipo = TipoAtendimentoEnum.Emergencia,
                Status = StatusAtendimentoEnum.Realizado,
                Local = "Emergência 03",
                PacienteId = paciente7.Id,
                ProfissionalSaudeId = medico7.Id,
                ProntuarioId = prontuario7.Id
            };
            atendimentos.Add(atendimento7);
            var atendimento8 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddDays(-3).Date,
                Hora = new TimeSpan(7, 30, 0),
                Tipo = TipoAtendimentoEnum.Consulta,
                Status = StatusAtendimentoEnum.Cancelado,
                Local = "Consultório 04",
                PacienteId = paciente8.Id,
                ProfissionalSaudeId = medico8.Id,
                ProntuarioId = prontuario8.Id
            };
            atendimentos.Add(atendimento8);
            var atendimento9 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddDays(-4).Date,
                Hora = new TimeSpan(10, 40, 0),
                Tipo = TipoAtendimentoEnum.Consulta,
                Status = StatusAtendimentoEnum.Realizado,
                Local = "Consultório 05",
                PacienteId = paciente9.Id,
                ProfissionalSaudeId = medico1.Id,
                ProntuarioId = prontuario9.Id
            };
            atendimentos.Add(atendimento9);
            var atendimento10 = new Atendimento
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now.AddDays(-5).Date,
                Hora = new TimeSpan(21, 0, 0),
                Tipo = TipoAtendimentoEnum.Emergencia,
                Status = StatusAtendimentoEnum.Realizado,
                Local = "Emergência 04",
                PacienteId = paciente10.Id,
                ProfissionalSaudeId = medico2.Id,
                ProntuarioId = prontuario10.Id
            };
            atendimentos.Add(atendimento10);
            context.Atendimentos.AddRange(atendimentos);
            context.SaveChanges();

            // --- PRESCRIÇÕES ---
            var prescricoes = new List<Prescricao>();
            var prescricao1 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento1.Id, 
                ProfissionalId = medico1.Id, 
                Medicamento = "Paracetamol", 
                Dosagem = "500mg", 
                Frequencia = "8 em 8h", 
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now,
                Observacoes = "Dor de cabeça", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao1);

            var prescricao2 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento2.Id, 
                ProfissionalId = medico2.Id, 
                Medicamento = "Dipirona", 
                Dosagem = "1g", 
                Frequencia = "6 em 6h", 
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now, 
                Observacoes = "Febre alta", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa,
            };
            prescricoes.Add(prescricao2);

            var prescricao3 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento3.Id, 
                ProfissionalId = medico3.Id, 
                Medicamento = "Amoxicilina", 
                Dosagem = "250mg", 
                Frequencia = "12 em 12h", 
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now, 
                Observacoes = "Infecção bacteriana", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao3);

            var prescricao4 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento4.Id, 
                ProfissionalId = medico4.Id, 
                Medicamento = "Ibuprofeno", 
                Dosagem = "400mg", 
                Frequencia = "8 em 8h", 
                ViaAdministracao = "Oral",
                DataInicio = DateTime.Now, 
                Observacoes = "Inflamação", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao4);

            var prescricao5 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento5.Id, 
                ProfissionalId = medico5.Id, 
                Medicamento = "Morfina", 
                Dosagem = "2mg", 
                Frequencia = "4 em 4h", 
                ViaAdministracao = "Intravenosa", 
                DataInicio = DateTime.Now, 
                Observacoes = "Controle de dor severa", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao5);

            var prescricao6 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento6.Id, 
                ProfissionalId = medico6.Id, 
                Medicamento = "Ciprofloxacino", 
                Dosagem = "500mg", 
                Frequencia = "12 em 12h", 
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now, 
                Observacoes = "Infecção urinária", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao6);

            var prescricao7 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento7.Id, 
                ProfissionalId = medico7.Id, 
                Medicamento = "Prednisona", 
                Dosagem = "20mg", 
                Frequencia = "1 vez ao dia",
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now, 
                Observacoes = "Reação alérgica", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao7);

            var prescricao8 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento9.Id, 
                ProfissionalId = medico1.Id, 
                Medicamento = "Atenolol", 
                Dosagem = "25mg", 
                Frequencia = "1 vez ao dia", 
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now, 
                Observacoes = "Hipertensão", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao8);

            var prescricao9 = new Prescricao
            { 
                Id = Guid.NewGuid(),
                AtendimentoId = atendimento10.Id, 
                ProfissionalId = medico2.Id, 
                Medicamento = "Omeprazol", 
                Dosagem = "20mg", 
                Frequencia = "1 vez ao dia", 
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now, 
                Observacoes = "Queimação estomacal", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao9);

            var prescricao10 = new Prescricao
            { 
                Id = Guid.NewGuid(), 
                AtendimentoId = atendimento1.Id, 
                ProfissionalId = medico1.Id, 
                Medicamento = "Buscopan Composto", 
                Dosagem = "1 comprimido", 
                Frequencia = "6 em 6h", 
                ViaAdministracao = "Oral", 
                DataInicio = DateTime.Now, 
                Observacoes = "Cólicas abdominais", 
                StatusPrescricao = StatusPrescricaoEnum.Ativa 
            };
            prescricoes.Add(prescricao10);

            context.Prescricoes.AddRange(prescricoes);
            context.SaveChanges();

            // --- EXAMES ---
            var exames = new List<Exame>();
            var exame1 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Hemograma Completo", 
                DataSolicitacao = DateTime.Now.AddDays(-1), 
                DataRealizacao = DateTime.Now, 
                Resultado = "Resultados normais", 
                AtendimentoId = atendimento1.Id 
            };
            exames.Add(exame1);

            var exame2 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Raio-X de Tórax", 
                DataSolicitacao = DateTime.Now.AddDays(-0.5), 
                DataRealizacao = DateTime.Now, 
                Resultado = "Fratura de costela", 
                AtendimentoId = atendimento2.Id 
            };
            exames.Add(exame2);

            var exame3 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Glicemia de Jejum", 
                DataSolicitacao = DateTime.Now.AddDays(-2), 
                DataRealizacao = DateTime.Now.AddDays(-1), 
                Resultado = "Glicemia alta", 
                AtendimentoId = atendimento3.Id 
            };
            exames.Add(exame3);

            var exame4 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Urina Tipo I", 
                DataSolicitacao = DateTime.Now.AddDays(-1), 
                DataRealizacao = DateTime.Now, 
                Resultado = "Infecção urinária confirmada", 
                AtendimentoId = atendimento4.Id 
            };
            exames.Add(exame4);

            var exame5 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Tomografia Computadorizada", 
                DataSolicitacao = DateTime.Now.AddHours(-1), 
                DataRealizacao = null, 
                Resultado = null, 
                AtendimentoId = atendimento5.Id 
            };
            exames.Add(exame5);

            var exame6 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Colesterol Total", 
                DataSolicitacao = DateTime.Now.AddDays(-3), 
                DataRealizacao = DateTime.Now.AddDays(-2), 
                Resultado = "Colesterol levemente elevado", 
                AtendimentoId = atendimento6.Id 
            };
            exames.Add(exame6);

            var exame7 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Eletrocardiograma",
                DataSolicitacao = DateTime.Now.AddDays(-2), 
                DataRealizacao = DateTime.Now.AddDays(-1), 
                Resultado = "Ritmo cardíaco normal", 
                AtendimentoId = atendimento7.Id 
            };
            exames.Add(exame7);

            var exame8 = new Exame
            {
                Id = Guid.NewGuid(),
                Tipo = "Ultrassonografia Abdominal", 
                DataSolicitacao = DateTime.Now.AddDays(-4), 
                DataRealizacao = DateTime.Now.AddDays(-3),
                Resultado = "Cistos renais", 
                AtendimentoId = atendimento9.Id 
            };
            exames.Add(exame8);

            var exame9 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Teste de Covid-19", 
                DataSolicitacao = DateTime.Now.AddDays(-5), 
                DataRealizacao = DateTime.Now.AddDays(-4), 
                Resultado = "Positivo", 
                AtendimentoId = atendimento10.Id 
            };
            exames.Add(exame9);

            var exame10 = new Exame 
            { 
                Id = Guid.NewGuid(), 
                Tipo = "Ressonância Magnética", 
                DataSolicitacao = DateTime.Now.AddDays(-1), 
                DataRealizacao = null, 
                Resultado = null, 
                AtendimentoId = atendimento1.Id 
            };
            exames.Add(exame10);

            context.Exames.AddRange(exames);
            context.SaveChanges();

            // --- INTERNAÇÕES ---
            var internacoes = new List<Internacao>();
            var internacao1 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente5.Id,
                AtendimentoId = atendimento5.Id,
                DataEntrada = DateTime.Now.AddHours(-1),
                PrevisaoAlta = DateTime.Now.AddDays(5),
                MotivoInternacao = "Crise respiratória aguda",
                Leito = "UTI-01",
                Quarto = "101",
                Setor = "UTI",
                PlanoSaudeUtilizado = "Unimed",
                ObservacoesClinicas = "Paciente com histórico de DPOC, necessita de suporte ventilatório.",
                StatusInternacao = StatusInternacaoEnum.Ativa
            };
            internacoes.Add(internacao1);
            var internacao2 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente1.Id,
                AtendimentoId = atendimento1.Id,
                DataEntrada = DateTime.Now.AddDays(-7),
                PrevisaoAlta = DateTime.Now.AddDays(-2),
                MotivoInternacao = "Pneumonia",
                Leito = "CL-05",
                Quarto = "205",
                Setor = "Clínica Geral",
                PlanoSaudeUtilizado = "SUS",
                ObservacoesClinicas = "Paciente respondeu bem ao tratamento.",
                StatusInternacao = StatusInternacaoEnum.AltaConcedida
            };
            internacoes.Add(internacao2);
            var internacao3 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente3.Id,
                AtendimentoId = atendimento3.Id,
                DataEntrada = DateTime.Now.AddDays(-10),
                PrevisaoAlta = DateTime.Now.AddDays(-5),
                MotivoInternacao = "Fratura de fêmur",
                Leito = "ORT-02",
                Quarto = "302",
                Setor = "Ortopedia",
                PlanoSaudeUtilizado = "Bradesco Saúde",
                ObservacoesClinicas = "Passou por cirurgia e está em recuperação.",
                StatusInternacao = StatusInternacaoEnum.AltaConcedida
            };
            internacoes.Add(internacao3);
            var internacao4 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente4.Id,
                AtendimentoId = atendimento4.Id,
                DataEntrada = DateTime.Now.AddDays(-1),
                PrevisaoAlta = DateTime.Now.AddDays(3),
                MotivoInternacao = "Infecção renal",
                Leito = "CL-01",
                Quarto = "201",
                Setor = "Clínica Geral",
                PlanoSaudeUtilizado = "SUS",
                ObservacoesClinicas = "Iniciou antibióticos, monitorar função renal.",
                StatusInternacao = StatusInternacaoEnum.Ativa
            };
            internacoes.Add(internacao4);
            var internacao5 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente6.Id,
                AtendimentoId = atendimento6.Id,
                DataEntrada = DateTime.Now.AddDays(-15),
                PrevisaoAlta = DateTime.Now.AddDays(-10),
                MotivoInternacao = "Apendicite",
                Leito = "CIR-03",
                Quarto = "403",
                Setor = "Cirurgia",
                PlanoSaudeUtilizado = "Amil",
                ObservacoesClinicas = "Apendicectomia realizada com sucesso.",
                StatusInternacao = StatusInternacaoEnum.AltaConcedida
            };
            internacoes.Add(internacao5);
            var internacao6 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente7.Id,
                AtendimentoId = atendimento7.Id,
                DataEntrada = DateTime.Now.AddDays(-8),
                PrevisaoAlta = DateTime.Now.AddDays(-4),
                MotivoInternacao = "Crise asmática",
                Leito = "EMG-01",
                Quarto = "105",
                Setor = "Emergência",
                PlanoSaudeUtilizado = "SUS",
                ObservacoesClinicas = "Estabilizado, aguarda alta.",
                StatusInternacao = StatusInternacaoEnum.AltaConcedida
            };
            internacoes.Add(internacao6);
            var internacao7 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente8.Id,
                AtendimentoId = atendimento8.Id,
                DataEntrada = DateTime.Now.AddDays(-2),
                PrevisaoAlta = DateTime.Now.AddDays(4),
                MotivoInternacao = "Febre persistente",
                Leito = "PED-02",
                Quarto = "502",
                Setor = "Pediatria",
                PlanoSaudeUtilizado = "Golden Cross",
                ObservacoesClinicas = "Investigando causa da febre em criança.",
                StatusInternacao = StatusInternacaoEnum.Ativa
            };
            internacoes.Add(internacao7);
            var internacao8 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente9.Id,
                AtendimentoId = atendimento9.Id,
                DataEntrada = DateTime.Now.AddDays(-20),
                PrevisaoAlta = DateTime.Now.AddDays(-18),
                MotivoInternacao = "Infarto Agudo do Miocárdio",
                Leito = "UTI-03",
                Quarto = "103",
                Setor = "UTI",
                PlanoSaudeUtilizado = "Unimed",
                ObservacoesClinicas = "Recuperação lenta mas progressiva.",
                StatusInternacao = StatusInternacaoEnum.AltaConcedida
            };
            internacoes.Add(internacao8);
            var internacao9 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente10.Id,
                AtendimentoId = atendimento10.Id,
                DataEntrada = DateTime.Now.AddDays(-12),
                PrevisaoAlta = DateTime.Now.AddDays(-7),
                MotivoInternacao = "AVC Isquêmico",
                Leito = "NEU-01",
                Quarto = "601",
                Setor = "Neurologia",
                PlanoSaudeUtilizado = "SUS",
                ObservacoesClinicas = "Iniciou fisioterapia e fonoaudiologia.",
                StatusInternacao = StatusInternacaoEnum.AltaConcedida
            };
            internacoes.Add(internacao9);
            var internacao10 = new Internacao
            {
                Id = Guid.NewGuid(),
                PacienteId = paciente2.Id,
                AtendimentoId = atendimento2.Id,
                DataEntrada = DateTime.Now.AddHours(-2),
                PrevisaoAlta = DateTime.Now.AddDays(7),
                MotivoInternacao = "Crise convulsiva",
                Leito = "CL-02",
                Quarto = "202",
                Setor = "Clínica Geral",
                PlanoSaudeUtilizado = "Particular",
                ObservacoesClinicas = "Monitoramento neurológico.",
                StatusInternacao = StatusInternacaoEnum.Ativa
            };
            internacoes.Add(internacao10);

            context.Internacoes.AddRange(internacoes);
            context.SaveChanges();

            // --- ALTAS HOSPITALARES ---
            var altasHospitalares = new List<AltaHospitalar>();
            var alta1 = new AltaHospitalar 
            { 
                InternacaoId = internacao2.Id, 
                DataAlta = internacao2.PrevisaoAlta.Value, 
                CondicaoPaciente = "Estável, recuperado", 
                InstrucoesPosAlta = "Repouso, retorno em 7 dias." 
            };
            altasHospitalares.Add(alta1);

            var alta2 = new AltaHospitalar 
            { 
                InternacaoId = internacao3.Id, 
                DataAlta = internacao3.PrevisaoAlta.Value, 
                CondicaoPaciente = "Estável, andando com auxílio", 
                InstrucoesPosAlta = "Fisioterapia ambulatorial." 
            };
            altasHospitalares.Add(alta2);

            var alta3 = new AltaHospitalar 
            { 
                InternacaoId = internacao5.Id, 
                DataAlta = internacao5.PrevisaoAlta.Value, 
                CondicaoPaciente = "Estável, sem dor", 
                InstrucoesPosAlta = "Retirar pontos em 5 dias." 
            };
            altasHospitalares.Add(alta3);

            var alta4 = new AltaHospitalar 
            { 
                InternacaoId = internacao6.Id, 
                DataAlta = internacao6.PrevisaoAlta.Value, 
                CondicaoPaciente = "Estável, sem dispneia", 
                InstrucoesPosAlta = "Seguir uso de bombinha." 
            };
            altasHospitalares.Add(alta4);

            var alta5 = new AltaHospitalar 
            { 
                InternacaoId = internacao8.Id, 
                DataAlta = internacao8.PrevisaoAlta.Value, 
                CondicaoPaciente = "Estável, em recuperação", 
                InstrucoesPosAlta = "Manter medicações." 
            };
            altasHospitalares.Add(alta5);

            var alta6 = new AltaHospitalar 
            { 
                InternacaoId = internacao9.Id, 
                DataAlta = internacao9.PrevisaoAlta.Value, 
                CondicaoPaciente = "Parcialmente recuperado, com sequelas", 
                InstrucoesPosAlta = "Continuar terapia." 
            };
            altasHospitalares.Add(alta6);

            var alta7 = new AltaHospitalar 
            { 
                InternacaoId = internacao1.Id, 
                DataAlta = DateTime.Now.AddDays(5), 
                CondicaoPaciente = "Em recuperação, aguardando exames", 
                InstrucoesPosAlta = "Acompanhamento médico" 
            };
            altasHospitalares.Add(alta7);

            var alta8 = new AltaHospitalar 
            { 
                InternacaoId = internacao4.Id, 
                DataAlta = DateTime.Now.AddDays(3), 
                CondicaoPaciente = "Melhorando da infecção", 
                InstrucoesPosAlta = "Reavaliação em 3 dias" 
            };
            altasHospitalares.Add(alta8);

            var alta9 = new AltaHospitalar 
            { 
                InternacaoId = internacao7.Id, 
                DataAlta = DateTime.Now.AddDays(4), 
                CondicaoPaciente = "Estável, febre controlada", 
                InstrucoesPosAlta = "Retorno em 5 dias." 
            };
            altasHospitalares.Add(alta9);

            var alta10 = new AltaHospitalar 
            { 
                InternacaoId = internacao10.Id, 
                DataAlta = DateTime.Now.AddDays(7), 
                CondicaoPaciente = "Estável, sem crises", 
                InstrucoesPosAlta = "Acompanhamento neurológico." 
            };
            altasHospitalares.Add(alta10);


            context.AltasHospitalares.AddRange(altasHospitalares);
            context.SaveChanges();
        }
    }
}