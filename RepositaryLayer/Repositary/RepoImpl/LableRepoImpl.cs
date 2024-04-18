/*using Dapper;
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
*/
using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class LableRepoImpl : ILableRepo
    {
        private readonly DapperContext context;

        public LableRepoImpl(DapperContext context)
        {
            this.context = context;
        }

        public LableEntity CreateLable(LableEntity entity, bool flag)
        {
            string query = "INSERT INTO Lable (LabelName, UserId) VALUES (@LabelName, @UserId); SELECT SCOPE_IDENTITY();";
            int labelId = context.CreateConnection().QuerySingle<int>(query, entity);
            entity.LabelId = labelId;

            if (flag && entity.NoteId != null && entity.NoteId.Any())
            {
                InsertLabelNotes(entity);
            }

            return entity;
        }

        private void InsertLabelNotes(LableEntity entity)
        {
            foreach (var noteId in entity.NoteId)
            {
                string query = "INSERT INTO LabelNote (LabelId, NoteId) VALUES (@LabelId, @NoteId);";
                context.CreateConnection().Execute(query, new { LabelId = entity.LabelId, NoteId = noteId });
            }
        }

        public LableEntity getLabelById(int labelId)
        {
            string query = @"
                SELECT L.LabelId, L.LabelName, LN.NoteId
                FROM Lable L
                LEFT JOIN LabelNote LN ON L.LabelId = LN.LabelId
                WHERE L.LabelId = @LabelId";

            var labelDictionary = new Dictionary<int, LableEntity>();
            var label = context.CreateConnection().Query<LableEntity, int, LableEntity>(
                query,
                (lable, noteId) =>
                {
                    if (!labelDictionary.TryGetValue(lable.LabelId, out LableEntity lableEntity))
                    {
                        lableEntity = lable;
                        lableEntity.NoteId = new List<int>();
                        labelDictionary.Add(lable.LabelId, lableEntity);
                    }
                    lableEntity.NoteId.Add(noteId);
                    return lableEntity;
                },
                new { LabelId = labelId },
                splitOn: "NoteId"
            ).Distinct().FirstOrDefault();

            return label;
        }

        public List<LableEntity> GetLableByEmail(int userId)
        {
            // Query to retrieve labels along with their associated note IDs
            string query = @"
        SELECT L.LabelId, L.LabelName, LN.NoteId
        FROM Lable L
        LEFT JOIN LabelNote LN ON L.LabelId = LN.LabelId
        WHERE L.UserId = @UserId";

            // Dictionary to store labels by their ID
            var labelsDictionary = new Dictionary<int, LableEntity>();

            // Execute the query and process the results
            var results = context.CreateConnection().Query<LableEntity, int?, LableEntity>(
                query,
                ProcessResults,
                new { UserId = userId }, // Parameter for the query
                splitOn: "NoteId" // Split the results based on NoteId column
            );

            // Set the UserId for each label entity
            foreach (var label in labelsDictionary.Values)
            {
                label.UserId = userId;
            }

            // Convert the dictionary values to a list and return
            return labelsDictionary.Values.ToList();

            // Method to process query results
            LableEntity ProcessResults(LableEntity label, int? noteId)
            {
                // Check if the label already exists in the dictionary
                if (!labelsDictionary.TryGetValue(label.LabelId, out LableEntity existingLabel))
                {
                    // If the label doesn't exist, add it to the dictionary
                    existingLabel = label;
                    existingLabel.NoteId = new List<int>();
                    labelsDictionary.Add(label.LabelId, existingLabel);
                }

                // Add the note ID to the label's NoteId list if it's not null
                if (noteId.HasValue)
                {
                    existingLabel.NoteId.Add(noteId.Value);
                }

                // Return the label
                return existingLabel;
            }
        }


        public LableEntity UpdateLable(LableEntity labelEntity, bool flag)
        {
            string query = "UPDATE Lable SET LabelName = @LabelName WHERE LabelId = @LabelId AND UserId = @UserId";
            context.CreateConnection().Execute(query, new { LabelName = labelEntity.LabelName, LabelId = labelEntity.LabelId, UserId = labelEntity.UserId });

            if (flag && labelEntity.NoteId != null && labelEntity.NoteId.Any())
            {
                // Delete existing label notes
                string deleteLabelNotesQuery = "DELETE FROM LabelNote WHERE LabelId = @LabelId";
                context.CreateConnection().Execute(deleteLabelNotesQuery, new { LabelId = labelEntity.LabelId });

                // Insert updated label notes
                InsertLabelNotes(labelEntity);
            }

            return getLabelById(labelEntity.LabelId);
        }

        public int DeleteLable(string labelName, int userId)
        {
            // Get label Id by name
            string labelIdQuery = "SELECT LabelId FROM Lable WHERE LabelName = @LabelName AND UserId = @UserId";
            int labelId = context.CreateConnection().QueryFirstOrDefault<int>(labelIdQuery, new { LabelName = labelName, UserId = userId });

            if (labelId != 0)
            {
                // Delete associated records from the LabelNote table
                string deleteLabelNotesQuery = "DELETE FROM LabelNote WHERE LabelId = @LabelId";
                context.CreateConnection().Execute(deleteLabelNotesQuery, new { LabelId = labelId });

                // Delete the label itself
                string deleteLabelQuery = "DELETE FROM Lable WHERE LabelId = @LabelId";
                return context.CreateConnection().Execute(deleteLabelQuery, new { LabelId = labelId });
            }

            return 0; // Label not found or no labels deleted
        }

       
    }
}
