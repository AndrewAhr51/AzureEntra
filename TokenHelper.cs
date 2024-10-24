using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
namespace EntraIdBL.Helper
{
    public static class TokenHelper
    {

        public static async Task<string> GetAccessToken(string _clientId, string _tenantId, string _clientSecret)
        {
            var clientId = _clientId;
            var clientSecret = _clientSecret;
            var authority = $"https://login.microsoftonline.com/{_tenantId}";

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            var authResult = await app.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" }).ExecuteAsync();

            return authResult.AccessToken;
        }

        public static void DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken != null)
            {
                var claims = jsonToken.Claims;
                var scopes = claims.FirstOrDefault(c => c.Type == "scp")?.Value;
                var roles = claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList();

                Console.WriteLine($"Scopes: {scopes}");
                Console.WriteLine("Roles:");
                foreach (var role in roles)
                {
                    Console.WriteLine(role);
                }
            }
        }
    }
}
