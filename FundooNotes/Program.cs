using BuisinessLayer.service.Iservice;
using BuisinessLayer.service.serviceImpl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



//--------USED FOR DEPENDENCY INJECTION--------
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUserRepo, UserRepoImpl>();
builder.Services.AddScoped<IUserService,UserServiceImpl>();

builder.Services.AddScoped<INotesRepo, NotesRepoImpl>();
builder.Services.AddScoped<INotesService,NotesServiceImpl>();


//-----------LOGGER--------
builder.Services.AddScoped<ILogger<UserServiceImpl>, Logger<UserServiceImpl>>();
builder.Services.AddScoped<ILogger<NotesServiceImpl>, Logger<NotesServiceImpl>>();

//--------------------------------

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
