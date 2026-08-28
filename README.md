# CMPay

Gateway de pagamentos v1 — API REST em ASP.NET Core para gerenciar clientes, endereços, cartões e pagamentos (Pix, cartão de crédito/débito), com suporte a idempotência na criação de pagamentos, autenticação por API Key e validação de entrada.

Projeto de estudo, focado em praticar decisões de arquitetura de um gateway de pagamentos real (idempotência, observabilidade, autenticação) além do CRUD básico.

## Stack

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 (SQL Server)
- Serilog (log estruturado em console e arquivo)
- xUnit + Moq (testes unitários)
- DotNetEnv (variáveis de ambiente via `.env`)

## Estrutura do projeto

Arquitetura em camadas:

```text
CMPay.API             # Controllers, middlewares, Program.cs (composition root)
CMPay.Application      # DTOs, interfaces, services (regras de negócio)
CMPay.Domain           # Entidades e enums
CMPay.Infrastructure    # DbContext, migrations, repositórios
CMPay.Tests            # Testes unitários (xUnit + Moq)
```

## Domínio e endpoints

| Recurso   | Rota base       | Operações |
| --------- | --------------- | --------- |
| Cliente   | `api/clientes`  | criar, buscar por ID, listar, atualizar, excluir |
| Endereço  | `api/endereco`  | criar, buscar por ID, listar, atualizar, excluir |
| Cartão    | `api/Cartao`    | criar, buscar por ID, listar, atualizar, excluir |
| Pagamento | `api/pagamento` | criar (idempotente), buscar por ID, listar, detalhes (com transações), processar, cancelar, estornar |

## Autenticação

Toda a API exige o header `X-Api-Key`, exceto `POST /api/clientes` (cadastro público, já que é o único jeito de um cliente novo obter sua própria chave).

- `POST /api/clientes` gera uma API Key aleatória para o cliente recém-criado e a devolve **uma única vez** na resposta (`{ idCliente, apiKey }`) — só o hash (SHA-256) fica persistido no banco.
- Requisições sem a chave, ou com uma chave inválida, recebem `401`.
- Implementado como um `AuthenticationHandler` customizado (`ApiKeyAuthenticationHandler`), integrado ao pipeline padrão do ASP.NET Core (`[Authorize]`/`[AllowAnonymous]`).

## Validação de entrada

- **CPF/CNPJ** (`Cliente.Documento`): validação de dígito verificador via `ValidationAttribute` customizada.
- **E-mail** e **telefone**: `[EmailAddress]`/`[Phone]`.
- **Cartão**: mês/ano de expiração validados via `IValidatableObject` (rejeita cartão já vencido), além do formato do mês (`[Range(1,12)]`).
- **Enums** (`Moeda`, `TipoMetodoPagamento`): `[EnumDataType]`, pra rejeitar valores numéricos fora do enum (o `System.Text.Json` aceita qualquer inteiro em um enum por padrão).

## Idempotency-Key na criação de pagamento

`POST /api/pagamento` exige o header `Idempotency-Key` (não vazio, até 100 caracteres):

- **Mesma key + mesmo payload** → devolve o pagamento já criado, sem duplicar.
- **Mesma key + payload diferente** → `409 Conflict`.
- A key é escopada por cliente (`IDCliente` + `Idempotency-Key` formam um índice único), e requisições concorrentes com a mesma key são resolvidas via captura de violação do índice único no banco.

## Como rodar localmente

**Pré-requisitos:** .NET 9 SDK, SQL Server (ou LocalDB).

1. Crie um arquivo `.env` na raiz de `CMPay.API` com a connection string:

   ```text
   ConnectionStrings__DefaultConnection="Server=(localdb)\MinhaInstancia;Database=PayCM;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

2. Aplique as migrations (ou defina `APPLY_MIGRATIONS=true` no ambiente para aplicar automaticamente na subida da aplicação):

   ```bash
   dotnet ef database update --project CMPay.Infrastructure --startup-project CMPay.API
   ```

3. Rode a API:

   ```bash
   dotnet run --project CMPay.API
   ```

## Observabilidade

- **Logs**: Serilog grava em console e em `CMPay.API/logs/log-{data}.txt` (configurável em `appsettings.json`, seção `Serilog`).
- **Health check**: `GET /health` verifica a conectividade com o banco via `AddDbContextCheck<AppDbContext>`.
- **Correlation ID**: toda requisição recebe/propaga um `X-Correlation-Id` (gerado se não vier no header), disponível nos logs daquela requisição.
- **Tratamento de erros**: middleware global mapeia `NotFoundException` → 404, `BusinessException` → 400, `ConflictException` → 409; qualquer outra exceção não tratada → 500.

## Testes

```bash
dotnet test
```

Cobertura atual: `ClienteService`, `PagamentoService` (incluindo cenários de replay e conflito de idempotência), `CartaoService` e `EnderecoService`. Ainda sem testes para `Transacao`, para os controllers, nem testes de integração (`WebApplicationFactory`).
