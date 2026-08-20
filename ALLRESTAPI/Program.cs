using ALLRESTAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleLibraryEF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ALLRESTAPI.CRUDItem.ICRUDItemRepository, ALLRESTAPI.CRUDItem.CRUDItemRepository>();
builder.Services.AddScoped< ALLRESTAPI.CRUDItem.IAuthorService, ALLRESTAPI.CRUDItem.AuthorService>();
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<LibraryContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var allowedOrigins = builder.Configuration
	.GetSection("Cors:AllowedOrigins")
	.Get<string[]>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactDev", policy =>
	{
		policy.WithOrigins(allowedOrigins)
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});



var app = builder.Build();

app.UseCors("AllowReactDev"); // before UseAuthorization() / MapControllers()

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (!app.Environment.IsEnvironment("Production"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();



app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));


app.Run();
