namespace SecureAPI.Domain;

public sealed record User(int Id, string Email);
public sealed record CreateUserDto(string Email, string PasswordHash);

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<int> CreateAsync(CreateUserDto dto, CancellationToken ct);
    Task<IReadOnlyList<User>> SearchByEmailAsync(string query, CancellationToken ct);
}