using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureAPI.Domain;

namespace SecureAPI.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
          private readonly IUserRepository _repo;
    public UsersController(IUserRepository repo) => _repo = repo;

    // GET api/users/5
    [HttpGet("{id:int}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetById(int id, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(id, ct);
        return u is null ? NotFound() : Ok(u);
    }

    // POST api/users
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var id = await _repo.CreateAsync(dto, ct);
        return CreatedAtRoute("GetUserById", new { id }, new { Id = id });
    }

    // GET api/users/search?q=foo
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<User>>> Search([FromQuery] string q, CancellationToken ct)
        => Ok(await _repo.SearchByEmailAsync(q, ct));
    }
}
