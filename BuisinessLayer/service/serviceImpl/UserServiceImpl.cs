using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.MailSender;
using BuisinessLayer.service.Iservice;
using NLog;
using RepositaryLayer.DTO.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System.Text;
using System.Text.RegularExpressions;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepo UserRepo;
        private readonly ILogger log;
        private static string otp;
        private static string mailid;
        private static UserEntity entity;
        public UserServiceImpl(IUserRepo UserRepo, ILogger log)
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
                
                UserId=responce.UserId,                     
               FirstName= responce.UserFirstName,
               LastName= responce.UserLastName ,
               Email = responce.UserEmail,
            };
        }

        public Task<int> createUser(UserRequest request)
        {
            var v = UserRepo.createUser(MapToEntity(request));
            log.Info("User Created");
            return v;
        }

        public Task<UserResponce> Login(string Email, string password)
        {
            UserEntity entity ;
            try
            {
                 entity = UserRepo.GetUserByEmail(Email).Result;
                if (entity==null)
                {
                    throw new UserNotFoundException("UserNotFoundByEmailId");
                }
            }
            catch(AggregateException e)
            {
                log.Error("UserNotFoundByEmailId");
                throw new UserNotFoundException("UserNotFoundByEmailId");
            }
            if(password.Equals(Decrypt(entity.UserPassword)))
            {
                var v = Task.FromResult(MapToResponce(entity));
                log.Info("User lOGED in");
                return v;
            }
            else
            {
                log.Error("Incorrect Password");
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
                log.Error("UserNotFoundByEmailId");
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
            log.Info(otp +" OTP SENT");
           return Task.FromResult("MailSent ✔️");
            
        }
            
            
        

        public Task<string> ChangePassword(string otp,string password)
        {
            if (UserServiceImpl.otp==null)
            {
                log.Warn("Generate OTP first");
                return Task.FromResult("Generate Otp First");
            }
            if (Decrypt(entity.UserPassword).Equals(password))
            {
                log.Error("Dont give the existing password");
                throw new PasswordMissmatchException("Dont give the existing password");
            }
           
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]{8,16}$"))
            {
                if (UserServiceImpl.otp.Equals(otp))
                {
                   if( UserRepo.UpdatePassword(mailid,Encrypt(password)).Result==1)
                    {
                        entity = null;otp = null;mailid = null;
                        log.Info("password changed successfully");
                        return Task.FromResult("password changed successfully");
                    }
                }
                else
                {
                    log.Warn("OTP MissMatching");
                    return Task.FromResult("otp miss matching");
                }
            }
            else
            {
                log.Warn("Regex MissMatching");
                return Task.FromResult("regex is mismatching");
            }
            log.Warn("Password Not Changed");
            return Task.FromResult("password not changed");
            
        }
    }
}
