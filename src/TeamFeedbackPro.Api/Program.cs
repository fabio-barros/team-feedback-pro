using TeamFeedbackPro.Api.Endpoints;
using TeamFeedbackPro.Application;
using TeamFeedBackPro.Infrastructure;
using TeamFeedbackPro.Api.Extensions;
using TeamFeedBackPro.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamFeedbackPro API v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "TeamFeedbackPro API";
        c.DisplayRequestDuration();
    });
    await app.MigrateAndSeedAsync();

}

//app.MapOpenApi();
//app.UseSwagger();
//app.UseSwaggerUI();
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapAuthEndpoints();
app.MapTeamEndpoints();
app.MapUserEndpoints();
app.MapFeedbackEndpoints();

app.Run();