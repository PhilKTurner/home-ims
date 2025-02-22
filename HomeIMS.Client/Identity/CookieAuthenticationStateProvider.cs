using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorWasmAuth.Identity.Models;
using System.Text;

namespace BlazorWasmAuth.Identity
{
    /// <summary>
    /// Handles state for cookie-based auth.
    /// </summary>
    /// <remarks>
    /// Create a new instance of the auth provider.
    /// </remarks>
    /// <param name="httpClientFactory">Factory to retrieve auth client.</param>
    public class CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILogger<CookieAuthenticationStateProvider> logger)
        : AuthenticationStateProvider, IAccountManagement
    {
        /// <summary>
        /// Map the JavaScript-formatted properties to C#-formatted classes.
        /// </summary>
        private readonly JsonSerializerOptions jsonSerializerOptions =
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

        /// <summary>
        /// Special auth client.
        /// </summary>
        private readonly HttpClient httpClient = httpClientFactory.CreateClient("Auth");

        /// <summary>
        /// Authentication state.
        /// </summary>
        private bool authenticated = false;

        /// <summary>
        /// Default principal for anonymous (not authenticated) users.
        /// </summary>
        private readonly ClaimsPrincipal unauthenticated = new(new ClaimsIdentity());

        /// <summary>
        /// User login.
        /// </summary>
        /// <param name="userName">The user's name.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The result of the login request serialized to a <see cref="FormResult"/>.</returns>
        public async Task<FormResult> LoginAsync(string userName, string password)
        {
            try
            {
                // login with cookies
                var result = await httpClient.PostAsJsonAsync(
                    "login?useCookies=true", new
                    {
                        userName,
                        password
                    });

                // success?
                if (result.IsSuccessStatusCode)
                {
                    // need to refresh auth state
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                    // success!
                    return new FormResult { Succeeded = true };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "App error");
            }

            // unknown error
            return new FormResult
            {
                Succeeded = false,
                ErrorList = [ "Invalid email and/or password." ]
            };
        }

        /// <summary>
        /// Get authentication state.
        /// </summary>
        /// <remarks>
        /// Called by Blazor anytime and authentication-based decision needs to be made, then cached
        /// until the changed state notification is raised.
        /// </remarks>
        /// <returns>The authentication state asynchronous request.</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            authenticated = false;

            // default to not authenticated
            var user = unauthenticated;

            try
            {
                // the user info endpoint is secured, so if the user isn't logged in this will fail
                var userResponse = await httpClient.GetAsync("manage/info");

                // throw if user info wasn't retrieved
                userResponse.EnsureSuccessStatusCode();

                // user is authenticated,so let's build their authenticated identity
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, jsonSerializerOptions);

                if (userInfo != null)
                {
                    // in this example app, name and email are the same
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.UserName)
                    };

                    // add any additional claims
                    claims.AddRange(
                        userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                            .Select(c => new Claim(c.Key, c.Value)));

                    // request the roles endpoint for the user's roles
                    var rolesResponse = await httpClient.GetAsync("roles");

                    // throw if request fails
                    rolesResponse.EnsureSuccessStatusCode();

                    // read the response into a string
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();

                    // deserialize the roles string into an array
                    var roles = JsonSerializer.Deserialize<RoleClaim[]>(rolesJson, jsonSerializerOptions);

                    // add any roles to the claims collection
                    if (roles?.Length > 0)
                    {
                        foreach (var role in roles)
                        {
                            if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
                            {
                                claims.Add(new Claim(role.Type, role.Value, role.ValueType, role.Issuer, role.OriginalIssuer));
                            }
                        }
                    }

                    // set the principal
                    var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                    user = new ClaimsPrincipal(id);
                    authenticated = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "App error");
            }

            // return the state
            return new AuthenticationState(user);
        }

        public async Task LogoutAsync()
        {
            const string Empty = "{}";
            var emptyContent = new StringContent(Empty, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("logout", emptyContent);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<bool> CheckAuthenticatedAsync()
        {
            await GetAuthenticationStateAsync();
            return authenticated;
        }
    }
}