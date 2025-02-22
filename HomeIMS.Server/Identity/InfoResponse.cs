namespace HomeIMS.Server.Identity;

/// <summary>
/// The response type for the "/manage/info" endpoints added by <see cref="CustomIdentityApiEndpointRouteBuilderExtensions.MapCustomIdentityApi"/>.
/// Adapted from <see cref="Microsoft.AspNetCore.Identity.Data.InfoResponse"/>.
/// </summary>
public sealed class InfoResponse
{
    /// <summary>
    /// The user name associated with the authenticated user.
    /// </summary>
    public required string UserName { get; init; }
}
