# ApiStudent

API REST em .NET 8 para gestão acadêmica: cadastro de usuários, alunos, instrutores e cursos, além de matrículas. Os recursos seguem o ciclo CRUD (criar, listar, consultar, atualizar e remover), com autenticação JWT e autorização por papéis.

## Tecnologias

- **.NET 8.0** — ASP.NET Core Web API
- **Entity Framework Core 8** — acesso a dados e migrations
- **Pomelo.EntityFrameworkCore.MySql** — provider MySQL
- **ASP.NET Core Identity** — usuários, senhas e papéis
- **JWT Bearer** — autenticação stateless
- **AutoMapper** — conversão entre entidades e DTOs
- **User Secrets** — credenciais fora do repositório
- **Swagger / OpenAPI** (Swashbuckle) — documentação interativa

## Arquitetura e boas práticas

A solução separa a entrada HTTP da regra de negócio e do banco:

| Camada | Responsabilidade |
|---|---|
| `Controllers` | Rotas, códigos HTTP e autorização |
| `Service` | Regras de negócio |
| `Models/Dtos` | Contratos de entrada e saída, sem expor a entidade |
| `Profiles` | Mapas do AutoMapper |
| `Data` | `DbContext`, relacionamentos e filtros |
| `Program.cs` | Composição da aplicação e injeção de dependência |

Práticas aplicadas:

- **Injeção de dependência.** Serviços, `DbContext`, Identity e geração de token são registrados no container e recebidos pelos construtores.
- **DTOs.** Create, update e read usam records próprios. O cliente não envia nem recebe a entidade de persistência.
- **User Secrets.** A string de conexão e a chave de assinatura do JWT não ficam em `appsettings`. Cada máquina configura os próprios segredos.
- **Autorização por política.** `AdminOnly`, `IsStudent` e `IsInstructor` concentram as regras de acesso. O token carrega papéis e o claim `UserType`.

## Pré-requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) em execução (8.x recomendado)
- Um cliente SQL (MySQL Workbench, linha de comando ou equivalente) para criar o banco

Opcional, para aplicar as migrations:

```bash
dotnet tool install --global dotnet-ef
```

## Como executar

### 1. Clonar o repositório

```bash
git clone https://github.com/<seu-usuario>/ApiStudent.git
cd ApiStudent
```

### 2. Criar o banco

```sql
CREATE DATABASE ApiStudent CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 3. Configurar os User Secrets

Na pasta do projeto (onde está o `ApiStudent.csproj`):

```bash
dotnet user-secrets set "ConnectionStrings:Connection" "Server=localhost;Port=3306;Database=ApiStudent;User=root;Password=SUA_SENHA;"
dotnet user-secrets set "SymmetricSecurityKey" "troque-por-uma-chave-com-no-minimo-32-caracteres"
```

`ConnectionStrings:Connection` é a chave lida na inicialização. `SymmetricSecurityKey` assina o JWT (HMAC-SHA256) e precisa ter pelo menos 32 caracteres. Substitua usuário, senha, host e o texto da chave pelos valores da sua máquina.

Para conferir o que foi gravado, sem precisar abrir arquivo de configuração:

```bash
dotnet user-secrets list
```

### 4. Aplicar as migrations

```bash
dotnet ef database update
```

Esse comando cria as tabelas de alunos, cursos, instrutores, matrículas e do ASP.NET Identity.

### 5. Executar

```bash
dotnet run
```

No perfil HTTPS a API sobe em:

- `https://localhost:7190`
- `http://localhost:5012`

Na primeira execução a aplicação garante as roles `Admin` e `SuperAdmin` e o usuário administrador abaixo.

| Campo | Valor |
|---|---|
| E-mail | `superAdmin@email.com` |
| Senha | `SenhaDoAdmin@123` |

## Documentação interativa (Swagger)

Com a API em execução e o ambiente `Development`, abra:

- `https://localhost:7190/swagger`
- `http://localhost:5012/swagger`

O Swagger lista os grupos `User`, `Student`, `Instructor`, `Course` e `Enrollment`. Em cada operação, **Try it out** libera o corpo JSON e os parâmetros.

Rotas protegidas exigem o token JWT:

1. Execute **POST** `/User/Login` com o e-mail e a senha do SuperAdmin (ou de um usuário que você cadastrou).
2. Copie o valor de `token` na resposta `200`. Cole apenas o JWT, sem o prefixo `Bearer`.
3. Clique em **Authorize**, cole o token e confirme.

O token expira em 10 minutos. Depois disso, faça login de novo e atualize o **Authorize**.

Consulta pública de cursos (**GET** `/Course` e **GET** `/Course/{id}`) responde sem token. As demais rotas de escrita e as consultas de usuários, alunos, instrutores e matrículas exigem o papel correspondente.
