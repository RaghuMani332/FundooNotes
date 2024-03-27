/*using BuisinessLayer.service.Iservice;
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


builder.Services.AddScoped<ILableRepo, LableRepoImpl>();
builder.Services.AddScoped<ILableService, LableServiceImpl>();


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

*/



 using BuisinessLayer.service.Iservice;
using BuisinessLayer.service.serviceImpl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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
builder.Services.AddScoped<IUserService, UserServiceImpl>();


builder.Services.AddScoped<INotesRepo, NotesRepoImpl>();
builder.Services.AddScoped<INotesService, NotesServiceImpl>();


builder.Services.AddScoped<ILableRepo, LableRepoImpl>();
builder.Services.AddScoped<ILableService, LableServiceImpl>();


//-----------LOGGER--------
builder.Services.AddScoped<ILogger<UserServiceImpl>, Logger<UserServiceImpl>>();
builder.Services.AddScoped<ILogger<NotesServiceImpl>, Logger<NotesServiceImpl>>();


//--------session-----------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
//----------------------------------

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FundooNotes", Version = "v1" });
    //For Authorization
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });
});
builder.Services.AddDistributedMemoryCache();

//jwt
// Add JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
builder.Services.AddAuthentication(au =>
{
    au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    au.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.RequireHttpsMetadata = true;
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        //Validate the expiration and not before values in the token
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy =>
    {
        policy.WithOrigins("http://localhost:7254", "https://localhost:7254")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithHeaders(HeaderNames.ContentType);
    });
}


app.UseHttpsRedirection();
app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

