using CMPay.API.Middleware;
using CMPay.Application.Auth;
using CMPay.Application.Interfaces;
using CMPay.Application.Services;
using CMPay.Infrastructure.Data;
using CMPay.Infrastructure.Pagamentos;
using CMPay.Infrastructure.Repositories;
using CMPay.Infrastructure.Repositories.Cartao;
using CMPay.Infrastructure.Repositories.Cliente;
using CMPay.Infrastructure.Repositories.Endereco;
using CMPay.Infrastructure.Repositories.Pagamento;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;

DotNetEnv.Env.Load();

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();



var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext());

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

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

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddAuthentication("ApiKey")
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });


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

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseAuthentication();
app.UseAuthorization();



app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
