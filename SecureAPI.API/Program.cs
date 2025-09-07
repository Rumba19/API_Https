using Microsoft.AspNetCore.HttpLogging;
using SecureAPI.Domain;
using SecureAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// HTTPS + HSTS
builder.Services.AddHsts(o => { o.MaxAge = TimeSpan.FromDays(180); o.IncludeSubDomains = true; o.Preload = true; });
builder.Services.AddHttpsRedirection(o => o.HttpsPort = 443);
builder.Services.AddControllers();
// Log only metadata (no bodies/headers)
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestMethod |
                      HttpLoggingFields.RequestPath |
                      HttpLoggingFields.ResponseStatusCode;
});

var raw = builder.Configuration.GetConnectionString("App")
          ?? throw new InvalidOperationException("ConnectionStrings:App missing");
builder.Services.AddSingleton(Database.BuildDataSource(raw));
// Repos
builder.Services.AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseHsts();
app.UseHttpsRedirection();
app.UseHttpLogging();

if (!app.Environment.IsDevelopment()) app.UseHsts();
app.UseHttpsRedirection();
app.UseHttpLogging();

 app.MapControllers();



app.Run();
