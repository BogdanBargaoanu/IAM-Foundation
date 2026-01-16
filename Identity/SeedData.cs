using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace Identity;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            // Migrate and seed ApplicationDbContext
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            // Migrate and seed ConfigurationDbContext
            var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            configContext.Database.Migrate();

            // Migrate PersistedGrantDbContext
            var persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            persistedGrantContext.Database.Migrate();

            // Seed Clients
            var clientCount = 0;
            var clientUpdateCount = 0;
            foreach (var client in Config.Clients)
            {
                var existing = configContext.Clients
                    .Include(c => c.AllowedScopes)
                    .FirstOrDefault(c => c.ClientId == client.ClientId);

                if (existing is null)
                {
                    configContext.Clients.Add(client.ToEntity());
                    clientCount++;
                }
                else
                {
                    // Update existing client scopes
                    var configuredScopes = client.AllowedScopes.ToHashSet();
                    var existingScopes = existing.AllowedScopes.Select(s => s.Scope).ToHashSet();

                    if (!configuredScopes.SetEquals(existingScopes))
                    {
                        // Remove scopes that are no longer configured
                        var scopesToRemove = existing.AllowedScopes
                            .Where(s => !configuredScopes.Contains(s.Scope))
                            .ToList();

                        foreach (var clientScope in scopesToRemove)
                        {
                            existing.AllowedScopes.Remove(clientScope);
                        }

                        // Add new scopes
                        foreach (var clientScope in configuredScopes.Except(existingScopes))
                        {
                            existing.AllowedScopes.Add(new Duende.IdentityServer.EntityFramework.Entities.ClientScope
                            {
                                Scope = clientScope
                            });
                        }

                        clientUpdateCount++;
                        Log.Debug("Client {ClientId} scopes updated", client.ClientId);
                    }
                }
            }

            if (clientCount > 0)
            {
                Log.Debug("Clients being populated");
                configContext.SaveChanges();
            }
            else if (clientUpdateCount > 0)
            {
                Log.Debug("{ClientUpdateCount} clients updated", clientUpdateCount);
                configContext.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            // Seed Identity Resources
            if (!configContext.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in Config.IdentityResources)
                {
                    configContext.IdentityResources.Add(resource.ToEntity());
                }
                configContext.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            // Seed API Scopes
            var apiScopeCount = 0;
            foreach (var apiScope in Config.ApiScopes)
            {
                var existing = configContext.ApiScopes.FirstOrDefault(s => s.Name == apiScope.Name);
                if (existing is null)
                {
                    configContext.ApiScopes.Add(apiScope.ToEntity());
                    apiScopeCount++;
                }
            }

            if (apiScopeCount > 0)
            {
                Log.Debug("ApiScopes being populated");
                configContext.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            // Seed Users
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@example.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.example.com"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@example.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.example.com"),
                            new Claim("location", "somewhere")
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }

            var mike = userMgr.FindByNameAsync("mike").Result;
            if (mike == null)
            {
                mike = new ApplicationUser
                {
                    UserName = "mike",
                    Email = "mikedavid@kiosk.com",
                    EmailConfirmed = true,
                    EmployeeId = "123456",
                    PinHash = "1234".Sha256()
                };
                var result = userMgr.CreateAsync(mike, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(mike, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Mike David"),
                            new Claim(JwtClaimTypes.GivenName, "Mike"),
                            new Claim(JwtClaimTypes.FamilyName, "David"),
                            new Claim(JwtClaimTypes.WebSite, "http://mike.example.com"),
                            new Claim("location", "somewhere")
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("mike created");
            }
            else
            {
                Log.Debug("mike already exists");
            }
        }
    }
}
