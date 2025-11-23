# Integrantes

- Alexsandro Macedo: RM557068
- Leonardo Faria Salazar: RM557484

# RemoteReady API

Video demonstrativo: [https://www.youtube.com/watch?v=Tu6e6R4XNn4]

# Sobre o projeto

RemoteReady é uma plataforma moderna desenvolvida para preparar estudantes e profissionais para o modelo de trabalho remoto e híbrido, que hoje domina empresas de tecnologia, inovação e serviços digitais. O projeto oferece uma experiência completa composta por:

A solução permite que usuários consumam conteúdos curtos sobre produtividade, carreira e boas práticas de trabalho remoto. Conforme avançam na leitura das postagens, o sistema registra automaticamente o progresso e, ao atingir 10 posts lidos, o usuário se torna apto a gerar um certificado digital, comprovando sua jornada de aprendizado.

A administração de usuários, postagens, empresas e certificados é realizada por um painel web seguro, voltado para gestores ou equipe acadêmica.

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas (Clean Architecture):

```
RemoteReady/
├── Controllers/          # Endpoints da API
├── UseCases/            # Lógica de negócio
├── Data/
│   ├── AppData/         # DbContext
│   └── Repositories/    # Acesso a dados
│       └── Interfaces/
├── Models/              # Entidades do banco
├── Dtos/                # Data Transfer Objects
├── Mappers/             # Mapeamento entre DTOs e Entities
├── Interfaces/          # Contratos de Use Cases
├── Infrastructure/      # Health Checks, etc.
└── Doc/
    └── Samples/         # Exemplos para Swagger

```

## 📋 Entidades

### Usuario (TB_GS_USUARIO)
- Gerenciamento de usuários da plataforma
- Autenticação via JWT
- Roles: USER, ADMIN, OPERADOR

### BlogPost (TB_GS_BLOG_POST)
- Posts educacionais sobre trabalho remoto
- Tags para categorização
- Imagens e descrições

### Empresa (TB_GS_EMPRESA)
- Empresas remote-friendly
- Status de contratação
- Áreas de atuação

### UserPost (TB_GS_USER_POST)
- Rastreamento de leitura de posts
- Sistema de gamificação (certificados)
- Status de conclusão

## 🚀 Tecnologias

- **.NET 8.0**
- **Entity Framework Core**
- **Oracle Database**
- **JWT Authentication**
- **Swagger/OpenAPI**
- **Rate Limiting**
- **Health Checks**
- **HATEOAS**

## 📦 Instalação

1. Clone o repositório
2. Configure a connection string no `appsettings.json`
3. Configure a chave JWT no `appsettings.json`
4. Execute as migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. Execute o projeto:

```bash
dotnet run
```

## 🔐 Autenticação

### Cadastro de Usuário
```http
POST /api/v1/usuario
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "senha123",
  "tipoUsuario": "ADMIN"
}
```

### Login
```http
POST /api/v1/usuario/autenticar
Content-Type: application/json

{
  "email": "joao@email.com",
  "senha": "senha123"
}
```

Retorna um token JWT que deve ser usado nos headers das requisições:
```
Authorization: Bearer {seu_token_aqui}
```

## 📊 Endpoints

### Usuários
- `GET /api/v1/usuario` - Lista usuários (paginado)
- `GET /api/v1/usuario/{id}` - Obtém usuário por ID
- `POST /api/v1/usuario` - Cadastra usuário (público)
- `POST /api/v1/usuario/autenticar` - Autentica usuário (público)
- `PUT /api/v1/usuario/{id}` - Atualiza usuário (ADMIN)
- `DELETE /api/v1/usuario/{id}` - Deleta usuário (ADMIN)

### Health Checks
- `GET /health/live` - Verifica se a aplicação está rodando
- `GET /health/ready` - Verifica se está pronta para receber requests (testa Oracle)

## 🔒 Segurança

- Autenticação JWT
- Rate Limiting (5 requisições a cada 10 segundos)
- Autorização baseada em roles

## 📝 Swagger

Acesse a documentação interativa em: `http://localhost:5000/swagger`

## 🎯 Padrões Implementados

- ✅ **HATEOAS** - Links de navegação nas respostas
- ✅ **Result Pattern** - Tratamento consistente de erros
- ✅ **Repository Pattern** - Abstração de acesso a dados
- ✅ **Use Case Pattern** - Lógica de negócio isolada
- ✅ **DTO Pattern** - Transferência de dados otimizada
- ✅ **Paginação** - Todas as listagens são paginadas
- ✅ **Health Checks** - Monitoramento da aplicação

## 📐 Banco de Dados

O banco Oracle segue a 3ª Forma Normal (3FN) com as seguintes tabelas:

- **TB_GS_USUARIO** - Usuários do sistema
- **TB_GS_BLOG_POST** - Posts do blog
- **TB_GS_EMPRESA** - Empresas cadastradas
- **TB_GS_USER_POST** - Relação usuário-post (leitura)

### Migrations

```bash
# Criar migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations
dotnet ef database update

# Reverter migration
dotnet ef database update NomeMigrationAnterior

# Remover última migration (não aplicada)
dotnet ef migrations remove
```

## 🧪 Testes

Este projeto contém **testes automatizados** completos para a aplicação RemoteReady, cobrindo todas as camadas da arquitetura.

## 🚀 Como Executar

### Todos os testes
```bash
dotnet test
```

### Com detalhes
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Por categoria
```bash
dotnet test --filter "Trait=Repository"
dotnet test --filter "Trait=UseCase"
dotnet test --filter "Trait=Controller"
```

## 📊 Tecnologias

- **xUnit** - Framework de testes
- **Moq** - Biblioteca de mock
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração
- **InMemory Database** - Banco em memória para testes

## ✅ Padrões Aplicados

- **AAA Pattern** (Arrange, Act, Assert)
- **Traits** para organização
- **DisplayName** para descrições claras
- **Builder Pattern** para criar objetos de teste
- **Isolamento** de testes


## 📄 Licença

Este projeto é parte do trabalho acadêmico da FIAP.
