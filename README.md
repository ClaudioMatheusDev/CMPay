# CMPay

Gateway de pagamentos v1 — API REST em ASP.NET Core para gerenciar clientes, endereços, cartões e pagamentos (Pix, cartão de crédito/débito), com suporte a idempotência na criação de pagamentos.

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

Cobertura atual: `ClienteService` e `PagamentoService` (incluindo cenários de replay e conflito de idempotência). Ainda sem testes para `CartaoService`, `EnderecoService`, `Transacao` e para os controllers.

## Gaps conhecidos

- Sem autenticação/autorização — API totalmente aberta.
- Sem validação de formato nas DTOs (CPF/CNPJ, e-mail, dados de cartão) além das checagens de negócio nos services.
- Sem paginação nas listagens (`GET` de coleção retorna tudo).
- Sem CI configurado (`.github/workflows` vazio).
