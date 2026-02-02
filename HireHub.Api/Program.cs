using HireHub.Api.Utils.Extensions;
using HireHub.Shared.Authentication;
using HireHub.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterSwaggerGen();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddCors( options => 
    options.AddPolicy( "AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() ) 
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHireHubRequestLogging();
app.UseHireHubGlobalException();
app.UseHireHubAuth();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
