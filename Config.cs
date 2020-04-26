using IdentityServer4.Models;
using System.Collections.Generic;

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

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client {
                    RequireClientSecret= false,
                    RequireConsent = false,
                    ClientId = "angular_southindianvillage_admin",
                    ClientName = "SouthIndianVillageAdmin",
                    //ClientSecrets = { new Secret("secret") },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "openid", "profile", "email", "api.read"}, // To be checked
                    RedirectUris = {"http://localhost:4001/customers"}, 
                    PostLogoutRedirectUris = {"http://localhost:4001/"},
                    AllowedCorsOrigins = {"http://localhost:4001"},
                    AllowAccessTokensViaBrowser = true, // need to check..
                    AccessTokenLifetime = 3600, // 1 hour
                    //AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }

    }
}
