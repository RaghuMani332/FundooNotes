using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepositaryLayer.DTO.RequestDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {
       
        private readonly IUserService service;
        private readonly IConfiguration _configuration;
        private readonly IProducer<string, string> _kafkaProducer;
        private readonly IConsumer<string, string> _kafkaConsumer;
        public UserController(IUserService userService, IConfiguration configuration, IProducer<string, string> prod, IConsumer<string, string> kafkaConsumer)
        {
            service = userService;
            _configuration = configuration;
            _kafkaProducer = prod;
            _kafkaConsumer = kafkaConsumer;
            // Start Kafka consumer background task
            Task.Run(() => ConsumeKafkaMessages(new CancellationTokenSource().Token));
        }

        [HttpPost("/RegisterUser")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> createUser(UserRequest request)
        {
            var result = await service.createUser(request);
                // Produce Kafka message
                var message = $"{result} UserCreatedSuccessfully: ";
                var kafkaMessage = new Message<string, string>
                {
                    Key = "user_created",
                    Value = message
                };
                await _kafkaProducer.ProduceAsync(_configuration["Kafka:Topic"], kafkaMessage);
                return Ok(message);   
        }

        private async Task ConsumeKafkaMessages(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _kafkaConsumer.Consume(cancellationToken);
                    var message = consumeResult.Message.Value;
                    Console.WriteLine($"Received Kafka message (key):{consumeResult.Message.Key} : Received Kafka message (value) {message}"+"----------------------------------------");
                }
            }
            catch (OperationCanceledException ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
            }
            finally
            {
                _kafkaConsumer.Close();
            }
        }
       
        [HttpGet("Login/{Email}/{password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> Login(String Email, String password)
        {
            UserResponce response = await service.Login(Email, password);
            if (response != null)
            {
                var token = GenerateToken(Email);

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    
                });

                return Ok(token);
            }
            return Unauthorized();
        }




        private string GenerateToken(string email)
        {
      
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Minutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }











        [HttpPut("ForgotPassword/{Email}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePasswordRequest(String Email)
        {
            return Ok( await service.ChangePasswordRequest(Email));
        }



        [HttpPut("ResetPassword/{otp}/{password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePassword(String otp,String password)
        {
            return Ok(await service.ChangePassword(otp,password));
        }



    }
}
