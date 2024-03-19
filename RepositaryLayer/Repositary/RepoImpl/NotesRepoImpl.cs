using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    
    public class NotesRepoImpl : INotesRepo
    {
        private readonly DapperContext Context;
       
        public NotesRepoImpl(DapperContext context) { 
            this.Context = context;
            
        }
        public int createNote(NotesEntity entity)
        {
            String Query = "insert into Notes_Entity values (@Title,@Description,@BgColor,@ImagePath,@Remainder,@IsArchive,@IsPinned,@IsTrash,@CreatedAt,@ModifiedAt,@UserId,@CollabId)";
            IDbConnection con = Context.CreateConnection();
            return con.Execute(Query,entity);
        }
    }
}
