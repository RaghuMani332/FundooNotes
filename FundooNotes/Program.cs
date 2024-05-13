using BuisinessLayer.service.Iservice;
using BuisinessLayer.service.serviceImpl;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;
using System.Reflection.Metadata;
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

//--------DI for NLOG--------
var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
NLog.GlobalDiagnosticsContext.Set("LogDirectory", logpath);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();
builder.Services.AddSingleton<NLog.ILogger>(NLog.LogManager.GetCurrentClassLogger());


//-----------LOGGER--------
//builder.Services.AddScoped<ILogger<UserServiceImpl>, Logger<UserServiceImpl>>();
    builder.Services.AddScoped<ILogger<NotesServiceImpl>, Logger<NotesServiceImpl>>();


    //---------KAFKA--------------
    // Register the ApacheKafkaPRODUCERService as a singleton hosted service
    builder.Services.AddSingleton<IProducer<string, string>>(sp =>
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
        };
        return new ProducerBuilder<string, string>(producerConfig).Build();
    });
    // Register the ApacheKafkaCONSUMERService as a singleton hosted service
    builder.Services.AddSingleton<IConsumer<string, string>>(sp =>

    {
        var consumer = new ConsumerBuilder<String, String>(new ConsumerConfig
        {
            BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
            GroupId = builder.Configuration["Kafka:ConsumerGroupId"]

        }).Build();
        consumer.Subscribe(builder.Configuration["Kafka:Topic"]);

        return consumer;
    });


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

    //------------REDIS-----------
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "127.0.0.1:6379"; // Redis server address
        options.InstanceName = "FundooNotesCache"; // Instance name for cache keys
    });


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


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://localhost:7004")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
            .AllowAnyOrigin();
        });
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");
app.UseCors();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
       /* app.UseCors(policy =>
        {
            policy.WithOrigins("http://localhost:7254", "https://localhost:7254")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithHeaders(HeaderNames.ContentType);
        });*/
    }


    app.UseHttpsRedirection();
    app.UseSession();
    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

