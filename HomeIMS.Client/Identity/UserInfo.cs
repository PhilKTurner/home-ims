namespace HomeIMS.Client.Identity;

// source: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAssemblyStandaloneWithIdentity

/// <summary>
/// User info from identity endpoint to establish claims.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// The user name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// The list of claims for the user.
    /// </summary>
    public Dictionary<string, string> Claims { get; set; } = [];
}