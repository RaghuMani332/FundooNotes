using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.MailSender;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.DTO.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepo UserRepo;
        private static string otp;
        private static string mailid;

        public UserServiceImpl(IUserRepo UserRepo)
        {
            this.UserRepo = UserRepo;
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
          return UserRepo.createUser(MapToEntity(request));
        }

        public Task<UserResponce> Login(string Email, string password)
        {
            UserEntity entity = UserRepo.GetUserByEmail(Email).Result;
            if(password.Equals(Decrypt(entity.UserPassword)))
            {
                return Task.FromResult(MapToResponce(entity));
            }
            else
            {
                throw new PasswordMissmatchException("Incorrect Password");
            }

        }

        public string ChangePasswordRequest(string Email)
        {
            if(UserRepo.GetUserByEmail(Email) != null)
            {
                string generatedotp = "";
                Random r = new Random();
                
                for(int i = 0;i<6;i++)
                {
                    generatedotp += r.Next(0, 10);
                }
                otp = generatedotp;
                mailid = Email;
                MailSenderClass.sendMail(Email,generatedotp);
                Console.WriteLine(otp);
                return "MailSent ✔️";
            }
            else
            {
                throw new UserNotFoundException("UserNot Present Please Register");
            }
        }

        public string ChangePassword(string otp,string password)
        {
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*]{8,16}$"))
            {
                if (UserServiceImpl.otp.Equals(otp))
                {
                   if( UserRepo.UpdatePassword(mailid,password).Result==1)
                    {
                        return "password changed successfully";
                    }
                }
                else
                {
                    return "otp miss matching";
                }
            }
            else
            {
                return "regex is mismatching";
            }
            return "password not changed";
            
        }
    }
}
