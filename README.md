# 🚀 Eficientiza - Sistema de Gerenciamento de Motos e Estações

API RESTful desenvolvida em **.NET 8** para gerenciamento de motos, estações de recarga e usuários, com recursos de **Machine Learning** para previsão de demanda.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Funcionalidades](#funcionalidades)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Arquitetura](#arquitetura)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Executando o Projeto](#executando-o-projeto)
- [Testes Unitários](#testes-unitários)
- [Endpoints da API](#endpoints-da-api)
- [Machine Learning](#machine-learning)
- [Autenticação JWT](#autenticação-jwt)
- [Swagger](#swagger)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Licença](#licença)

---

## 📖 Sobre o Projeto

O **Eficientiza ** é um sistema completo para gerenciar frotas de motos compartilhadas, estações de recarga e usuários. O projeto utiliza **Machine Learning (ML.NET)** para prever a demanda de motos em diferentes estações, otimizando a distribuição da frota.

### Principais Características:

✅ **CRUD Completo** - Usuários, Motos e Estações  
✅ **Autenticação JWT** - Sistema seguro de login e autorização por roles  
✅ **Machine Learning** - Previsão inteligente de demanda de motos  
✅ **Clean Architecture** - Separação em camadas (Controller → UseCase → Repository)  
✅ **HATEOAS** - Links para navegação entre recursos  
✅ **Paginação** - Endpoints otimizados com paginação  
✅ **Testes Unitários** - 111 testes cobrindo todas as camadas  
✅ **Documentação Swagger** - API totalmente documentada

---

## 🎯 Funcionalidades

### 👤 Gestão de Usuários

- Cadastro, edição, listagem e exclusão de usuários
- Autenticação com JWT
- Controle de acesso por roles (admin/user)

### 🏍️ Gestão de Motos

- Gerenciamento completo de motos
- Controle de status (Disponível, Em Uso, Em Manutenção)
- Informações detalhadas (Placa, Modelo, Cor, Ano)

### 🏢 Gestão de Estações

- Cadastro de estações de recarga
- Controle de capacidade e localização
- Gestão de responsáveis

### 🤖 Machine Learning

- **Previsão de demanda** de motos por estação
- Análise baseada em:
  - Capacidade da estação
  - Dia da semana
  - Horário
  - Histórico de uso
- Recomendações automáticas de reabastecimento

---

## 🛠️ Tecnologias Utilizadas

### Backend

- **.NET 8** - Framework principal
- **C#** - Linguagem de programação
- **Entity Framework Core** - ORM
- **SQL Server / In-Memory Database** - Banco de dados

### Machine Learning

- **ML.NET** - Framework de Machine Learning da Microsoft
- **FastTree Algorithm** - Algoritmo de regressão

### Autenticação

- **JWT (JSON Web Tokens)** - Autenticação stateless
- **ASP.NET Core Identity** - Gerenciamento de usuários

### Testes

- **xUnit** - Framework de testes
- **Moq** - Biblioteca de mocking
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração

### Documentação

- **Swagger/OpenAPI** - Documentação interativa da API
- **Swashbuckle** - Geração automática de documentação

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture** com separação clara de responsabilidades:

```
┌─────────────────────────────────────────┐
│          Presentation Layer             │
│         (Controllers / API)             │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         Application Layer               │
│  (UseCases / DTOs / Interfaces)         │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│          Domain Layer                   │
│    (Entities / Domain Logic)            │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Infrastructure Layer              │
│  (Repositories / Database / ML)         │
└─────────────────────────────────────────┘
```

### Camadas:

**Controllers** → Recebem requisições HTTP  
**UseCases** → Regras de negócio e orquestração  
**Repositories** → Acesso a dados  
**Entities** → Modelos de domínio

---

## 📦 Pré-requisitos

Antes de começar, você precisa ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) ou SQL Server Express (opcional - pode usar InMemory)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

---

## 🚀 Instalação

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/eficientiza-s3.git
cd eficientiza-s3
```

### 2. Instale as dependências

```bash
dotnet restore
```

### 3. Instale os pacotes NuGet necessários

```bash
# Pacotes principais
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.InMemory

# Autenticação JWT
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Machine Learning
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.FastTree

# Swagger
dotnet add package Swashbuckle.AspNetCore
dotnet add package Swashbuckle.AspNetCore.Annotations
dotnet add package Swashbuckle.AspNetCore.Filters

# Testes (apenas no projeto de testes)
cd Eficientiza-s3.Tests
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

---

## ⚙️ Configuração

### 1. Configure o `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EficientizaDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Secretkey": "sua-chave-secreta-super-segura-com-pelo-menos-32-caracteres",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 2. Configure o banco de dados

```bash
# Criar migrations
dotnet ef migrations add InitialCreate

# Aplicar migrations
dotnet ef database update
```

### 3. Configure o Machine Learning

O modelo de ML será treinado automaticamente na primeira execução. Para treinar manualmente:

```bash
# Via API (requer autenticação como admin)
POST /api/v1/previsao/treinar-modelo
```

---

## ▶️ Executando o Projeto

### Desenvolvimento

```bash
dotnet run
```

A aplicação estará disponível em:

- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `https://localhost:5001/swagger`

### Produção

```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet Eficientiza-s3.dll
```

---

## 🧪 Testes Unitários

O projeto possui **111 testes unitários** cobrindo todas as camadas da aplicação.

### Estrutura dos Testes

```
Eficientiza-s3.Tests/
└── App/
    ├── UsuarioRepositoryTest.cs   (11 testes)
    ├── UsuarioUseCaseTest.cs      (11 testes)
    ├── UsuarioControllerTest.cs   (9 testes)
    ├── MotoRepositoryTest.cs      (10 testes)
    ├── MotoUseCaseTest.cs         (9 testes)
    ├── MotoControllerTest.cs      (7 testes)
    ├── EstacaoRepositoryTest.cs   (10 testes)
    ├── EstacaoUseCaseTest.cs      (9 testes)
    └── EstacaoControllerTest.cs   (7 testes)
```

### Executar Todos os Testes

```bash
dotnet test
```

### Executar Testes por Categoria

```bash
# Apenas testes de Entity
dotnet test --filter "Category=Entity"

# Apenas testes de Repository
dotnet test --filter "Category=Repository"

# Apenas testes de UseCase
dotnet test --filter "Category=UseCase"

# Apenas testes de Controller
dotnet test --filter "Category=Controller"
```

````

### Executar Testes Específicos

```bash
# Testes de Repository de Usuario
dotnet test --filter "Repository&Usuario"

# Testes de UseCase de Moto
dotnet test --filter "UseCase&Moto"

# Testes de Controller de Estacao
dotnet test --filter "Controller&Estacao"
````

### Resultado Esperado

```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:   79, Skipped:     0, Total:   111
```

---

## 📡 Endpoints da API

### 🔐 Autenticação

#### Login de ADMIN

```http
POST /api/v1/usuario/autenticar
Content-Type: application/json

{
  "nome": "Teste Admin",
  "email": "admin@eficientiza.com",
  "senha": "123123",
  "tipoUsuario": "Admin"
}
```

**Resposta:**

```json
{
  "nome": "Admin Teste",
  "email": "admin@eficientiza.com",
  "senha": "123123",
  "tipoUsuario": "Admin"
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Login de OPERADOR

```http
POST /api/v1/usuario/autenticar
Content-Type: application/json

{
  "nome": "Teste Op",
  "email": "operador@eficientiza.com",
  "senha": "123123",
  "tipoUsuario": "Operador"
}
```

**Resposta:**

```json
{
  "nome": "Operador Teste",
  "email": "operador@eficientiza.com",
  "senha": "123123",
  "tipoUsuario": "Operador"
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 👤 Usuários

| Método | Endpoint               | Descrição                 | Autenticação |
| ------ | ---------------------- | ------------------------- | ------------ |
| GET    | `/api/v1/usuario`      | Lista usuários (paginado) | ✅           |
| GET    | `/api/v1/usuario/{id}` | Busca usuário por ID      | ✅           |
| POST   | `/api/v1/usuario`      | Cria novo usuário         | ✅ Admin     |
| PUT    | `/api/v1/usuario/{id}` | Atualiza usuário          | ✅ Admin     |
| DELETE | `/api/v1/usuario/{id}` | Deleta usuário            | ✅ Admin     |

### 🏍️ Motos

| Método | Endpoint            | Descrição              | Autenticação |
| ------ | ------------------- | ---------------------- | ------------ |
| GET    | `/api/v1/moto`      | Lista motos (paginado) | ✅           |
| GET    | `/api/v1/moto/{id}` | Busca moto por ID      | ✅           |
| POST   | `/api/v1/moto`      | Cria nova moto         | ✅ Admin     |
| PUT    | `/api/v1/moto/{id}` | Atualiza moto          | ✅ Admin     |
| DELETE | `/api/v1/moto/{id}` | Deleta moto            | ✅ Admin     |

### 🏢 Estações

| Método | Endpoint               | Descrição                 | Autenticação |
| ------ | ---------------------- | ------------------------- | ------------ |
| GET    | `/api/v1/estacao`      | Lista estações (paginado) | ✅           |
| GET    | `/api/v1/estacao/{id}` | Busca estação por ID      | ✅           |
| POST   | `/api/v1/estacao`      | Cria nova estação         | ✅ Admin     |
| PUT    | `/api/v1/estacao/{id}` | Atualiza estação          | ✅ Admin     |
| DELETE | `/api/v1/estacao/{id}` | Deleta estação            | ✅ Admin     |

### 🤖 Machine Learning

| Método | Endpoint                          | Descrição              | Autenticação |
| ------ | --------------------------------- | ---------------------- | ------------ |
| POST   | `/api/v1/previsao/demanda-motos`  | Prevê demanda de motos | ✅           |
| POST   | `/api/v1/previsao/treinar-modelo` | Treina o modelo ML     | ✅ Admin     |
| GET    | `/api/v1/previsao/exemplo`        | Exemplo de previsão    | ❌           |

---

## 🤖 Machine Learning

### Como Funciona

O sistema utiliza **ML.NET** com o algoritmo **FastTree** para prever a demanda de motos em uma estação específica.

### Features Utilizadas:

1. **Capacidade da estação**
2. **Dia da semana** (0-6)
3. **Hora do dia** (0-23)
4. **Média de uso dos últimos 7 dias**

### Fazer uma Previsão

```http
POST /api/v1/previsao/demanda-motos
Authorization: Bearer {token}
Content-Type: application/json

{
  "estacaoId": 2,
  "capacidade": 50,
  "dataHoraPrevista": "2025-11-10T08:00:00",
  "mediaUso7Dias": 35
}
```

**Resposta:**

```json
{
  "estacaoId": 1,
  "nomeEstacao": "Estação Centro",
  "dataHoraPrevista": "2025-11-10T08:00:00",
  "motosNecessariasPrevistas": 42,
  "recomendacao": "⚠️ Demanda média (84% da capacidade). Monitorar estoque."
}
```

### Treinar o Modelo

```http
POST /api/v1/previsao/treinar-modelo
Authorization: Bearer {token_admin}
```

O modelo é salvo automaticamente em: `ML/demanda_motos_model.zip`

---

## 🔐 Autenticação JWT

### Como Funciona

1. **Login**: Envie email e senha para `/api/v1/usuario/autenticar`
2. **Token**: Receba um token JWT válido por 8 horas
3. **Autorização**: Inclua o token no header de todas as requisições protegidas

### Usar o Token

```http
GET /api/v1/usuario
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Roles

- **admin**: Acesso total (CRUD completo)
- **user**: Acesso de leitura

---

## 📚 Swagger

Acesse a documentação interativa da API:

```
https://localhost:5001/swagger
```

### Autenticar no Swagger:

1. Faça login no endpoint `/api/v1/usuario/autenticar`
2. Copie o token retornado
3. Clique no botão **"Authorize"** 🔒 no topo do Swagger
4. Cole o token (sem "Bearer")
5. Clique em **"Authorize"**
6. Agora todos os endpoints protegidos funcionarão! ✅

---

## 📁 Estrutura do Projeto

```
Eficientiza-s3/
├── Controllers/                 # Endpoints da API
│   ├── UsuariosController.cs
│   ├── MotosController.cs
│   ├── EstacoesController.cs
│   └── PrevisaoDemandaController.cs
│
├── UseCases/                    # Regras de negócio
│   ├── UsuarioUseCase.cs
│   ├── MotoUseCase.cs
│   ├── EstacaoUseCase.cs
│   └── PrevisaoDemandaUseCase.cs
│
├── Data/
│   ├── Repositories/            # Acesso a dados
│   │   ├── UsuarioRepository.cs
│   │   ├── MotoRepository.cs
│   │   └── EstacaoRepository.cs
│   └── AppData/
│       └── ApplicationContext.cs
│
├── Models/                      # Entidades de domínio
│   ├── UsuarioEntity.cs
│   ├── MotoEntity.cs
│   ├── EstacaoEntity.cs
│   ├── PageData.cs
│   └── OperationResult.cs
│
├── ML/                          # Machine Learning
│   ├── Models/
│   │   └── DemandaMotosModels.cs
│   ├── Services/
│   │   └── DemandaMotosMLService.cs
│   └── demanda_motos_model.zip  (gerado automaticamente)
│
├── Dtos/                        # Data Transfer Objects
│   ├── UsuarioDto.cs
│   ├── MotoDto.cs
│   └── EstacaoDto.cs
│
├── Interfaces/                  # Contratos
│   ├── IUsuarioUseCase.cs
│   ├── IMotoUseCase.cs
│   └── IEstacaoUseCase.cs
│
├── Mappers/                     # Conversões DTO ↔ Entity
│
└── Program.cs                   # Configuração da aplicação

Eficientiza-s3.Tests/           # Projeto de testes
└── App/
    ├── UsuarioRepositoryTest.cs
    ├── UsuarioUseCaseTest.cs
    ├── UsuarioControllerTest.cs
    ├── MotoRepositoryTest.cs
    ├── MotoUseCaseTest.cs
    ├── MotoControllerTest.cs
    ├── EstacaoRepositoryTest.cs
    ├── EstacaoUseCaseTest.cs
    └── EstacaoControllerTest.cs
```

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
