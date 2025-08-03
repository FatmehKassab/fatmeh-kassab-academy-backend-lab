using DefaultNamespace.Services;
using DefaultNamespace.Middlewares;
using DefaultNamespace.Filters;
using Microsoft.EntityFrameworkCore;
using DefaultNamespace.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; 
using MediatR;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<IStudentService, StudentService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// builder.Services.AddControllers(options =>
// {
//     options.Filters.Add<CallerHeaderFilter>();
// });


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();
app.UseHttpsRedirection();
app.MapControllers();



app.Run();


