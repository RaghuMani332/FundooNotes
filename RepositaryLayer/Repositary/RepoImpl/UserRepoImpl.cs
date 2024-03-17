using BuisinessLayer.Entity;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<UserEntity> GetUserByEmail(string email)
        {
            String Query = "Select * from User_Entity where UserEmail = @Email";
            IDbConnection connection = context.CreateConnection();
           
               return await connection.QueryFirstAsync<UserEntity>(Query, new { Email = email });
            
            

        }

        public async Task<int> UpdatePassword(string mailid, string password)
        {
            String Query = "update User_Entity set UserPassword = @Password where UserEmail = @mail";
            IDbConnection connection= context.CreateConnection();
           return await connection.ExecuteAsync(Query,new {mail=mailid,Password=password});
        }
    }
}
