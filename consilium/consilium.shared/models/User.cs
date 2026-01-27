namespace Consilium.Shared.Models;

public class User {
    public int id { get; set; }
    public string email { get; set; } = "";
    public string displayName { get; set; } = "";
    public string? themePreference { get; set; } = "Green";
    public string? notes { get; set; }
}