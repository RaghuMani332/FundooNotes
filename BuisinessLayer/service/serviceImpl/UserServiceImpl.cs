using BuisinessLayer.Entity;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.DTO.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepo UserRepo;
        public UserServiceImpl(IUserRepo UserRepo)
        {
            this.UserRepo = UserRepo;
        }
        private UserEntity MapToEntity(UserRequest request)
        {
            return new UserEntity {UserFirstName=request.FirstName,
                                   UserLastName=request.LastName,
                                   UserEmail=request.Email,
                                   UserPassword=request.Password
            
            };
        }
        private UserResponce MapToResponce(UserEntity responce)
        {
            return new UserResponce
            {
               FirstName= responce.UserFirstName,
               LastName= responce.UserLastName ,
               Email = responce.UserEmail,
                Password= responce.UserPassword

            };
        }

        public Task<int> createUser(UserRequest request)
        {
          return UserRepo.createUser(MapToEntity(request));
        }

    }
}
