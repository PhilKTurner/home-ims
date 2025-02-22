namespace BlazorWasmAuth.Identity.Models
{
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
}