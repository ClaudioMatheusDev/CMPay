using CMPay.Application.Interfaces;
using CMPay.Application.Services;
using CMPay.Applicatios.Interfaces;
using CMPay.Infrastructure.Data;
using CMPay.Infrastructure.Pagamentos;
using CMPay.Infrastructure.Repositories;
using CMPay.Infrastructure.Repositories.Cartao;
using CMPay.Infrastructure.Repositories.Cliente;
using CMPay.Infrastructure.Repositories.Endereco;
using CMPay.Infrastructure.Repositories.Pagamento;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

builder.Services.AddScoped<IEnderecoService, EnderecoService>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();

builder.Services.AddScoped<ICartaoService, CartaoService>();
builder.Services.AddScoped<ICartaoRepository, CartaoRepository>();

builder.Services.AddScoped<IPagamentoService, PagamentoService>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();

builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

builder.Services.AddScoped<IProcessadorPagamento, ProcessadorPagamento>();



builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();


if (app.Configuration.GetValue<bool>("APPLY_MIGRATIONS"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
