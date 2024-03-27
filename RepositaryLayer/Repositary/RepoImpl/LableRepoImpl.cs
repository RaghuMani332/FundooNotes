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
    public class LableRepoImpl : ILableRepo
    {
        private readonly DapperContext context;
       
        public LableRepoImpl(DapperContext context)
        {
            this.context = context;
        }
        public LableEntity CreateLable(LableEntity entity,bool flag)
        {
            if (flag)
            {
                String Query = "insert into Lable values (@LabelName,@NoteId,@UserId) SELECT SCOPE_IDENTITY();";
                return getLabelById(context.CreateConnection().QuerySingle<int>(Query, entity));
            }
            else
            {
               String Query= "insert into Lable(LabelName, UserId) values(@LabelName, @UserId); SELECT SCOPE_IDENTITY()";
                var v = getLabelById(context.CreateConnection().QuerySingle<int>(Query, entity));
                Console.WriteLine(v+"else");
                return v;
            }
        }

        

        public LableEntity getLabelById(int Id)
        {
            String Query = "Select * from Lable where LabelId=@id";
            var v= context.CreateConnection().Query<LableEntity>(Query, new {id=Id}).FirstOrDefault();
            return v;
        }

        public List<LableEntity> GetLableByEmail(int userId)
        {
            String Query = "Select * from Lable where UserId=@id";
            return context.CreateConnection().Query<LableEntity>(Query, new { id = userId }).ToList();
        }

        public LableEntity UpdateLable(LableEntity lableEntity,bool flag)
        {
            if(flag)
            {
                String Query = "Update Lable Set LabelName = @LabelName,NoteId = @NoteId Where LabelId = @LabelId and UserId = @UserId";
                if (context.CreateConnection().Execute(Query, new { LabelName = lableEntity.LabelName, NoteId = lableEntity.NoteId, LabelId = lableEntity.LabelId, UserId = lableEntity.UserId }) == 1)
                    return getLabelById(lableEntity.LabelId);
                else
                    return null;
            }
            else
            {
                String Query = "Update Lable Set LabelName = @LabelName Where LabelId = @LabelId and UserId = @UserId";
                if (context.CreateConnection().Execute(Query, new { LabelName = lableEntity.LabelName, LabelId = lableEntity.LabelId, UserId = lableEntity.UserId }) == 1)
                    return getLabelById(lableEntity.LabelId);
                else
                    return null;
            }
            
        }
        public int DeleteLable(string lableName, int userId)
        {
            String Query = "DELETE FROM Lable WHERE LabelName = @lableName and UserId = @UserId ;";
            return context.CreateConnection().Execute(Query, new { lableName= lableName , UserId= userId });
        }
    }
}
