using Npgsql;

namespace WatchStore.Api.Settings;

/// <summary>
/// Database Connection Setting
/// </summary>
public sealed class DatabaseSetting
{
    /// <summary>
    /// Host name
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Port number
    /// </summary>
    public int Port { get; set; } = 5432;

    /// <summary>
    /// Username
    /// </summary>
    public required string User { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Database name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// SSL mode
    /// </summary>
    public SslMode SslMode { get; set; } = SslMode.Disable;

    public string ConnectionString
    {
        get
        {
            var builder = new NpgsqlConnectionStringBuilder()
            {
                Host = this.Host,
                Port = this.Port,
                Database = this.Name,
                Username = this.User,
                Password = this.Password,
                SslMode = this.SslMode,
            };

            return builder.ConnectionString;
        }
    }
}
