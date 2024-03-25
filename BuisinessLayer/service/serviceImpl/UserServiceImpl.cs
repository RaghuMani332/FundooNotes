using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.MailSender;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositaryLayer.DTO.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepo UserRepo;
        private readonly ILogger<UserServiceImpl> log;
        private static string otp;
        private static string mailid;
        private static UserEntity entity;
        public UserServiceImpl(IUserRepo UserRepo, ILogger<UserServiceImpl> log)
        {
            this.UserRepo = UserRepo;
            this.log = log;
        }
        private UserEntity MapToEntity(UserRequest request)
        {
            return new UserEntity {UserFirstName=request.FirstName,
                                   UserLastName=request.LastName,
                                   UserEmail=request.Email,
                                   UserPassword=Encrypt(request.Password)
            
            };
        }
        private String Encrypt(String password)
        {

            
           
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passByte);

        }
        private String Decrypt(String encryptedPass)
        {
           
            byte[] passbyte=Convert.FromBase64String(encryptedPass);
            String res= Encoding.UTF8.GetString(passbyte);
           
            return res;

        }
        private UserResponce MapToResponce(UserEntity responce)
        {
            return new UserResponce
            {
               FirstName= responce.UserFirstName,
               LastName= responce.UserLastName ,
               Email = responce.UserEmail,
               

            };
        }

        public Task<int> createUser(UserRequest request)
        {
            var v = UserRepo.createUser(MapToEntity(request));
            log.LogInformation("User Created");
            return v;
        }

        public Task<UserResponce> Login(string Email, string password)
        {
            UserEntity entity ;
            try
            {
                 entity = UserRepo.GetUserByEmail(Email).Result;
            }
            catch(AggregateException e)
            {
                log.LogError("UserNotFoundByEmailId");
                throw new UserNotFoundException("UserNotFoundByEmailId");
            }
            if(password.Equals(Decrypt(entity.UserPassword)))
            {
                var v = Task.FromResult(MapToResponce(entity));
                log.LogInformation("User lOGED in");
                return v;
            }
            else
            {
                log.LogError("Incorrect Password");
                throw new PasswordMissmatchException("Incorrect Password");
            }

        }

        public Task<String> ChangePasswordRequest(string Email)
        {
            try
            {
                 entity = UserRepo.GetUserByEmail(Email).Result;
            }
            catch (Exception e)
            {
                log.LogError("UserNotFoundByEmailId");
                throw new UserNotFoundException("UserNotFoundByEmailId" + e.Message);
            }

            string generatedotp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                generatedotp += r.Next(0, 10);
            }
            otp = generatedotp;
            mailid = Email;
            MailSenderClass.sendMail(Email, generatedotp);
            //            Console.WriteLine(otp);
            log.LogInformation(otp +" OTP SENT");
           return Task.FromResult("MailSent ✔️");
            
        }
            
            
        

        public Task<string> ChangePassword(string otp,string password)
        {
            if (otp.Equals(null))
            {
                log.LogWarning("Generate OTP first");
                return Task.FromResult("Generate Otp First");
            }
            if (Decrypt(entity.UserPassword).Equals(password))
            {
                log.LogError("Dont give the existing password");
                throw new PasswordMissmatchException("Dont give the existing password");
            }
           
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]{8,16}$"))
            {
                if (UserServiceImpl.otp.Equals(otp))
                {
                   if( UserRepo.UpdatePassword(mailid,Encrypt(password)).Result==1)
                    {
                        entity = null;otp = null;mailid = null;
                        log.LogInformation("password changed successfully");
                        return Task.FromResult("password changed successfully");
                    }
                }
                else
                {
                    log.LogWarning("OTP MissMatching");
                    return Task.FromResult("otp miss matching");
                }
            }
            else
            {
                log.LogWarning("Regex MissMatching");
                return Task.FromResult("regex is mismatching");
            }
            log.LogWarning("Password Not Changed");
            return Task.FromResult("password not changed");
            
        }
    }
}
