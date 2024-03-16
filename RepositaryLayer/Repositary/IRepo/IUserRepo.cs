using BuisinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface IUserRepo
    {
      public Task<int> createUser(UserEntity entity);
    }
}
