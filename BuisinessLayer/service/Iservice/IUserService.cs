using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.DTO.RequestDto;

namespace BuisinessLayer.service.Iservice
{
    public interface IUserService
    {
        public Task<int> createUser(UserRequest request);
    }
}
