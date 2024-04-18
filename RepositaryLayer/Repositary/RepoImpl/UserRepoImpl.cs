/*using BuisinessLayer.Entity;
using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System.Data;


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
        public List<string> GetUserEmailsByIds(List<int> userIds)
        {
            string query = "SELECT UserEmail FROM User_Entity WHERE UserId IN @UserIds";
            using (IDbConnection connection = context.CreateConnection())
            {
                var userEmails =  connection.Query<string>(query, new { UserIds = userIds });
                return userEmails.ToList();
            }
        }

    }
}
*/

using BuisinessLayer.Entity;
using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserRepoImpl : IUserRepo
    {
        private readonly DapperContext context;
        public UserRepoImpl(DapperContext context)
        {
            this.context = context;
        }

        /*public async Task<int> createUser(UserEntity entity)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserFirstName", entity.UserFirstName);
            parameters.Add("@UserLastName", entity.UserLastName);
            parameters.Add("@UserEmail", entity.UserEmail);
            parameters.Add("@UserPassword", entity.UserPassword);

            using (var connection = context.CreateConnection())
            {
                return await connection.ExecuteAsync("InsertUser", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<UserEntity> GetUserByEmail(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);

            using (var connection = context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<UserEntity>("GetUserByEmail", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> UpdatePassword(string email, string password)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            parameters.Add("@Password", password);

            using (var connection = context.CreateConnection()) 
            {
                return await connection.ExecuteAsync("UpdateUserPassword", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public UserEntity GetById(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using (var connection = context.CreateConnection())
            {
                return connection.QueryFirstOrDefault<UserEntity>("GetUserById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<int>> GetCollaboratorIdsByEmails(List<string> emailIds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EmailIds", emailIds);

            using (var connection = context.CreateConnection())
            {
                var userIds = await connection.QueryAsync<int>("GetCollaboratorIdsByEmails", parameters, commandType: CommandType.StoredProcedure);
                return userIds.ToList();
            }
        }

        public List<string> GetUserEmailsByIds(List<int> userIds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserIds", userIds);

            using (var connection = context.CreateConnection())
            {
                var userEmails = connection.Query<string>("GetUserEmailsByIds", parameters, commandType: CommandType.StoredProcedure);
                return userEmails.ToList();
            }
        }*/
        public async Task<int> createUser(UserEntity entity)
        {
            String query = "insert into User_Entity values (@UserFirstName,@UserLastName,@UserEmail,@UserPassword)";
            var connection = context.CreateConnection();
            return await connection.ExecuteAsync(query, entity);
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
            IDbConnection connection = context.CreateConnection();
            return await connection.ExecuteAsync(Query, new { mail = mailid, Password = password });
        }
        public UserEntity GetById(int id)
        {
            String query = "select * from User_Entity where UserId = @Id";
            IDbConnection connection = context.CreateConnection();
            return connection.Query<UserEntity>(query, new { Id = id }).FirstOrDefault();

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
        public List<string> GetUserEmailsByIds(List<int> userIds)
        {
            string query = "SELECT UserEmail FROM User_Entity WHERE UserId IN @UserIds";
            using (IDbConnection connection = context.CreateConnection())
            {
                var userEmails = connection.Query<string>(query, new { UserIds = userIds });
                return userEmails.ToList();
            }
        }
    }
}
