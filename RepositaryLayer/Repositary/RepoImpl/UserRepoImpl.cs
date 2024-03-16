using BuisinessLayer.Entity;
using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserRepoImpl : IUserRepo
    {
        private readonly DapperContext context;
        public UserRepoImpl(DapperContext contex)
        {
            this.context = contex;
        }

        public async Task<int> createUser(UserEntity entity)
        {
            String query = "insert into User_Entity values (@UserFirstName,@UserLastName,@UserEmail,@UserPassword)";
            var connection = context.CreateConnection();
            return await connection.ExecuteAsync(query,entity);
        }
    }
}
