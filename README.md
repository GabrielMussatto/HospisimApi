# HospisimApi

![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-69207D?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework-69207D?style=for-the-badge&logo=dotnet)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger)

O **HospisimApi** é um sistema de API Web construído com ASP.NET Core que visa modernizar a gestão clínica do Hospital Vida Plena. Ele garante a segurança, rastreabilidade de atendimentos e controle completo das informações dos pacientes, através de uma arquitetura robusta e bem definida.

## 🚀 Como Executar a API

Siga os passos abaixo para configurar e executar a API localmente em seu ambiente de desenvolvimento.

### Pré-requisitos

Certifique-se de ter os seguintes softwares instalados:

* **Visual Studio (versão 2022 ou superior):** O IDE principal para desenvolvimento .NET.
* **.NET SDK (versão 8.0 ou superior):** Compatível com a versão do projeto.
* **SQL Server:** Para o banco de dados foi utilizado o SQL Server.

### Passos de Execução

1.  **Clone ou Baixe o Repositório:**
    Se você estiver usando Git:
    ```bash
    git clone https://github.com/GabrielMussatto/HospisimApi.git
    cd HospisimApi
    ```
    Ou baixe o ZIP e extraia para uma pasta de sua preferência.

2.  **Abra o Projeto no Visual Studio:**
    * No Visual Studio, vá em `Arquivo` > `Abrir` > `Projeto/Solução`.
    * Navegue até a pasta onde você salvou o projeto (`HospisimApi`) e selecione o arquivo de solução (`.sln`).

3.  **Configurar a String de Conexão (opcional, se não usar LocalDB):**
    * Abra o arquivo `appsettings.json` na raiz do projeto `HospisimApi`.
    * A string de conexão padrão já deve estar configurada para o SQL Server LocalDB:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Coloque aqui o caminho pro seu banco de dados"
        }
        ```
    * Se você estiver usando outra instância do SQL Server, atualize esta string com suas credenciais e servidor.

4.  **Criar/Atualizar o Banco de Dados e Popular com Dados Iniciais (Seed):**
    * No Visual Studio, vá em `Ferramentas` > `Gerenciador de Pacotes NuGet` > `Console do Gerenciador de Pacotes`.
    * **Recomendado (para garantir um banco de dados limpo com o schema mais recente):**
        ```powershell
        Drop-Database
        ```
        *Confirme com 'Y' quando solicitado. Este comando apagará o banco de dados `HospisimDb` se ele existir.*
    * **Adicione a Migration Inicial (se a pasta `Migrations` estiver vazia ou para recriar o schema):**
        ```powerspowershell
        Add-Migration InitialFullSchema
        ```
        *Se você já tiver migrations e apenas fez pequenas alterações, pode ser `Add-Migration NomeDaSuaNovaMigration`.*
    * **Aplique as Migrations e execute o Seed de Dados:**
        ```powershell
        Update-Database
        ```
        *Este comando criará o banco de dados e todas as tabelas, além de popular com os dados iniciais configurados no `HospisimDbContext.SeedData`.*

5.  **Executar a API:**
    * Pressione `F5` no Visual Studio ou clique no botão `IIS Express` (ou o nome do seu projeto, ex: `HospisimApi`) na barra de ferramentas.
    * A API será compilada e executada. Seu navegador padrão abrirá automaticamente na interface do **Swagger UI** (geralmente `https://localhost:PORTA/swagger`).

### Utilizando o Swagger UI

A interface do Swagger UI permite que você explore e teste todos os endpoints da API interativamente:

* **Ver Endpoints:** Cada entidade (Paciente, Prontuário, etc.) terá uma seção expandível mostrando os métodos HTTP (`GET`, `POST`, `PUT`, `DELETE`).
* **Testar Requisições:**
    * Expanda um método (ex: `GET /api/Pacientes`).
    * Clique em `Try it out`.
    * Clique em `Execute`.
    * A resposta da API (código de status, corpo da resposta) será exibida.
* **Criar/Atualizar Dados:**
    * Para `POST` ou `PUT`, clique em `Try it out`.
    * O Swagger fornecerá um modelo JSON de exemplo no campo `Request body`. Edite este JSON com os dados que deseja enviar.
    * Para campos de `ID` em `POST`, você não precisa fornecer um GUID, ele será gerado automaticamente.
    * Para campos de `ID` em `PUT` ou `DELETE`, use o ID que você obteve de uma requisição `GET`.
    * Clique em `Execute`.

## 🤝 Entidades e Relacionamentos

O sistema HospisimApi é construído com base nas seguintes entidades e seus relacionamentos:

### Modelos de Entidades

* **Paciente:** Informações cadastrais do paciente (nome, CPF, data de nascimento, contato, SUS, etc.). Inclui propriedades formatadas para `CPF` e `Telefone` na saída da API.
* **Prontuário:** Registros gerais do histórico clínico de um paciente (número, data de abertura, observações).
* **ProfissionalSaude:** Dados dos profissionais (nome, CPF, contato, registro de conselho, especialidade, carga horária). Inclui propriedades formatadas para `CPF` e `Telefone`.
* **Especialidade:** Categorias de especialidades médicas (Cardiologia, Pediatria, etc.).
* **Atendimento:** Registros de atendimentos (data, hora, tipo, status, local). Relaciona um paciente, um profissional e um prontuário. A data e hora são armazenadas e manipuladas como campos separados.
* **Prescrição:** Detalhes de medicamentos prescritos em um atendimento (medicamento, dosagem, frequência, via, datas, observações).
* **Exame:** Registros de exames solicitados em um atendimento (tipo, datas, resultado).
* **Internação:** Detalhes de internações de pacientes (motivo, leito, quarto, setor, plano de saúde). Relacionada a um paciente e um atendimento.
* **AltaHospitalar (Bônus):** Informações sobre a alta de uma internação (data, condição do paciente, instruções). Relacionada a uma internação específica.

### Relacionamentos entre Entidades

Os relacionamentos foram cuidadosamente modelados e implementados no Entity Framework Core para garantir a integridade referencial:

* **`Paciente` 1:N `Prontuário`**:
    * Um `Paciente` pode ter múltiplos `Prontuários`.
    * Um `Prontuário` pertence a um único `Paciente`.
    * **Regra de Negócio:** Não é possível excluir um `Paciente` se ele possuir `Prontuário`s vinculados.

* **`Prontuário` 1:N `Atendimento`**:
    * Um `Prontuário` pode ter múltiplos `Atendimento`s.
    * Um `Atendimento` está vinculado a um único `Prontuário`.
    * **Regra de Negócio:** Não é possível excluir um `Prontuário` se ele possuir `Atendimento`s vinculados.

* **`ProfissionalSaude` 1:N `Atendimento`**:
    * Um `ProfissionalSaude` pode realizar múltiplos `Atendimento`s.
    * Um `Atendimento` é realizado por um único `ProfissionalSaude`.
    * **Regra de Negócio:** Não é possível excluir um `ProfissionalSaude` se ele possuir `Atendimento`s vinculados.

* **`Especialidade` 1:N `ProfissionalSaude`**:
    * Uma `Especialidade` pode ser associada a múltiplos `ProfissionalSaude`s.
    * Um `ProfissionalSaude` possui uma única `Especialidade`.
    * **Regra de Negócio:** Não é possível excluir uma `Especialidade` se ela possuir `ProfissionalSaude`s vinculados.

* **`Atendimento` 1:N `Prescrição`**:
    * Um `Atendimento` pode ter múltiplas `Prescrição`s.
    * Uma `Prescrição` está vinculada a um único `Atendimento`.
    * **Regra de Negócio:** Não é possível excluir um `Atendimento` se ele possuir `Prescrição`s vinculadas.

* **`Atendimento` 1:N `Exame`**:
    * Um `Atendimento` pode ter múltiplos `Exame`s.
    * Um `Exame` está vinculado a um único `Atendimento`.
    * **Regra de Negócio:** Não é possível excluir um `Atendimento` se ele possuir `Exame`s vinculados.

* **`Atendimento` 0..1:1 `Internação`**:
    * Um `Atendimento` pode originar zero ou uma `Internação`.
    * Uma `Internação` sempre é originada por um único `Atendimento`.
    * **Regra de Negócio:** Um `Atendimento` só pode estar associado a uma única `Internação`. Não é possível criar uma segunda `Internação` para o mesmo `Atendimento`. Não é possível excluir um `Atendimento` se ele possuir uma `Internação` vinculada.

* **`Internação` 0..1:1 `AltaHospitalar`**:
    * Uma `Internação` pode ter zero ou uma `AltaHospitalar`.
    * Uma `AltaHospitalar` sempre se refere a uma única `Internação`.
    * **Regra de Negócio:** Uma `Internação` só pode ter uma única `AltaHospitalar`. Não é possível criar uma segunda `AltaHospitalar` para a mesma `Internação`. Não é possível excluir uma `Internação` se ela possuir uma `AltaHospitalar` vinculada.
