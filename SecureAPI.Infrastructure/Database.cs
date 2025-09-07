using Npgsql;

namespace SecureAPI.Infrastructure;

public static class Database
{
    public static NpgsqlDataSource BuildDataSource(string raw)
    {
        var csb = new NpgsqlConnectionStringBuilder(raw)
        {
            Timeout = 5,
            CommandTimeout = 5,
            MaxPoolSize = 50
            // Leave SSL settings to the connection string.
        };
        var dsb = new NpgsqlDataSourceBuilder(csb.ConnectionString);
        return dsb.Build();
    }
}