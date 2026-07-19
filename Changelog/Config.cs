using Microsoft.Extensions.Configuration;

namespace Changelog;

/// <summary>
/// Class containing configuration. This is taken from the env variables or a .env file in the working directory
/// </summary>
public sealed class Config
{
    public static Config Instance = new();

    [ConfigurationKeyName("REPO")]
    public string? Repo { get; set; }
    [ConfigurationKeyName("BRANCH")]
    public string? Branch { get; set; }
    [ConfigurationKeyName("CHANGELOG_REPO_PATH")]
    public string? ChangelogRepoPath { get; set; }
    [ConfigurationKeyName("EXTRA_CATEGORIES")]
    public string? ExtraCategories { get; set; }
    [ConfigurationKeyName("GITHUB_TOKEN")]
    public string? GithubToken { get; set; }
    [ConfigurationKeyName("DISCORD_WEBHOOK")]
    public string? DiscordWebHook { get; set; }
    [ConfigurationKeyName("MAX_GRAPQHL_PAGES")]
    public int MaxPages { get; set; } = 50;

    public Config()
    {
        new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddEnvFile(".env", optional: true)
            .Build()
            .Bind(this);
    }
}
