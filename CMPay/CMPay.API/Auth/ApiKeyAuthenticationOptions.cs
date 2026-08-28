using CMPay.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace CMPay.Application.Auth
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions { }

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IClienteRepository _clienteRepository;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IClienteRepository clienteRepository)
            : base(options, logger, encoder)
        {
            _clienteRepository = clienteRepository;
        }

        // injeta via construtor (além dos parâmetros que o AuthenticationHandler já exige)

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
                return AuthenticateResult.NoResult();

            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(apiKey!)));
            var cliente = await _clienteRepository.BuscarPorApiKeyHashAsync(hash);

            if (cliente == null)
                return AuthenticateResult.Fail("API Key inválida.");

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, cliente.IDCliente.ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }

}
