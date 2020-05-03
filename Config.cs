using IdentityServer4.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("resourceapi", "Resource API")
                {
                    Scopes = {new Scope("api.read")}
                }
            };
        }

        public static IEnumerable<Client> GetClients(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            return new[]
            {
                new Client {
                    RequireClientSecret= false,
                    RequireConsent = false,
                    ClientId = "angular_southindianvillage_admin",
                    ClientName = "SouthIndianVillageAdmin",
                    //ClientSecrets = { new Secret("secret") },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "openid", "profile", "email", "api.read"}, // To be checked
                    RedirectUris = {configuration["SouthIndianVillage:RedirectUris"] },
                    PostLogoutRedirectUris = {configuration["SouthIndianVillage:PostLogoutRedirectUris"] },
                    AllowedCorsOrigins = { configuration["SouthIndianVillage:AllowedOrigins"]},
                    AllowAccessTokensViaBrowser = true, // need to check..
                    AccessTokenLifetime = 1800, // 1 hour
                    //AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }

    }
}
