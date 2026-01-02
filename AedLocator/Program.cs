using AedLocator.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. ADD CORS POLICY (Critical for GitHub Pages)
// This tells Azure: "It is safe to accept requests from my GitHub website."
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGitHub",
        policy =>
        {
            policy.WithOrigins("https://chunyim.github.io") // REPLACE with your actual GitHub URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 2. CONFIGURE DATABASE
// This looks for "DefaultConnection" in your appsettings.json
builder.Services.AddDbContext<AedContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. ADD CONTROLLERS
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. ENABLE SWAGGER (Helpful for testing your cloud API)
app.UseSwagger();
app.UseSwaggerUI();

// 5. SECURITY & ROUTING
app.UseHttpsRedirection();

// IMPORTANT: UseCors must be placed AFTER UseRouting and BEFORE UseAuthorization
app.UseCors("AllowGitHub");

app.UseAuthorization();

app.MapControllers();

app.Run();