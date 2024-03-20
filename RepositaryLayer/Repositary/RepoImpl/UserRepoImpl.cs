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
        public UserEntity GetById(int id)
        {
            String query = "select * from User_Entity where UserId = @Id";
            IDbConnection connection=context.CreateConnection();
           return connection.Query<UserEntity>(query,new {Id=id}).FirstOrDefault();

        }

        public async Task<List<int>> GetCollaboratorIdsByEmails(List<string> emailIds)
        {
            string query = "SELECT UserId FROM User_Entity WHERE UserEmail IN @EmailIds";
            using (IDbConnection connection = context.CreateConnection())
            {
                var userIds = await connection.QueryAsync<int>(query, new { EmailIds = emailIds });
                return userIds.ToList();
            }
        }

        public async Task<List<string>> GetUserEmailsByIds(List<int> userIds)
        {
            string query = "SELECT UserEmail FROM User_Entity WHERE UserId IN @UserIds";
            using (IDbConnection connection = context.CreateConnection())
            {
                var userEmails = await connection.QueryAsync<string>(query, new { UserIds = userIds });
                return userEmails.ToList();
            }
        }
    }
}
