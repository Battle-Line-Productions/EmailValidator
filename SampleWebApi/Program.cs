using AdvancedEmailValidator;
using AdvancedEmailValidator.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEmailValidator();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/validateEmail/{email}", async (string email, IEmailValidator emailValidator) =>
    {
        var validationResult = await emailValidator.ValidateAsync(email);

        return validationResult;
    })
.WithName("ValidateEmail")
.WithOpenApi();

app.Run();
