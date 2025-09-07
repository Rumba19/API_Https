using Npgsql;
using NpgsqlTypes;
using SecureAPI.Domain;

namespace SecureAPI.Infrastructure;

public sealed class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _ds;
    public UserRepository(NpgsqlDataSource ds) => _ds = ds;

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        const string sql = "SELECT id, email FROM public.app_users WHERE id = @id";
        await using var cmd = _ds.CreateCommand(sql);
        cmd.Parameters.Add(new NpgsqlParameter<int>("id", NpgsqlDbType.Integer) { Value = id });

        await using var r = await cmd.ExecuteReaderAsync(ct);
        return await r.ReadAsync(ct) ? new User(r.GetInt32(0), r.GetString(1)) : null;
    }

    public async Task<int> CreateAsync(CreateUserDto dto, CancellationToken ct)
    {
        await using var conn = await _ds.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO public.app_users(email, password_hash) VALUES(@e,@p) RETURNING id", conn, tx);

        cmd.Parameters.Add(new NpgsqlParameter<string>("e", NpgsqlDbType.Text) { Value = dto.Email });
        cmd.Parameters.Add(new NpgsqlParameter<string>("p", NpgsqlDbType.Text) { Value = dto.PasswordHash });

        var id = (int)(await cmd.ExecuteScalarAsync(ct))!;
        await tx.CommitAsync(ct);
        return id;
    }

    public async Task<IReadOnlyList<User>> SearchByEmailAsync(string query, CancellationToken ct)
    {
        // Escape LIKE wildcards; still parameterized.
        var safe = query.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
        const string sql = "SELECT id, email FROM public.app_users WHERE email ILIKE @q ESCAPE '\\'";
        await using var cmd = _ds.CreateCommand(sql);
        cmd.Parameters.Add(new NpgsqlParameter<string>("q", NpgsqlDbType.Text) { Value = $"%{safe}%" });

        var list = new List<User>();
        await using var r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct)) list.Add(new User(r.GetInt32(0), r.GetString(1)));
        return list;
    }
}
