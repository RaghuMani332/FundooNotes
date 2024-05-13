using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;


namespace RepositaryLayer.Repositary.RepoImpl
{
    public class NotesRepoImpl : INotesRepo
    {
        private readonly DapperContext Context;

        public NotesRepoImpl(DapperContext context)
        {
            this.Context = context;
        }

        public Dictionary<string, List<NotesEntity>> createNote(NotesEntity entity)
        {
            using (IDbConnection con = Context.CreateConnection())
            {
                try
                {
                    string insertQuery = "INSERT INTO Notes_Entity (Title, Description, BgColor, ImagePath, Remainder, IsArchive, IsPinned, IsTrash, CreatedAt, ModifiedAt, UserId) VALUES (@Title, @Description, @BgColor, @ImagePath, @Remainder, @IsArchive, @IsPinned, @IsTrash, @CreatedAt, @ModifiedAt, @UserId); SELECT SCOPE_IDENTITY();";
                    int noteId = con.QuerySingle<int>(insertQuery, entity);
                    if (entity.CollabId != null && entity.CollabId.Any())
                    {
                        foreach (var v in entity.CollabId)
                        {
                            string collabQuery = "INSERT INTO Colabrator (UserId, NotesId) VALUES (@UserId, @NoteId);";
                            con.Execute(collabQuery, new { UserId = v , NoteId = noteId });
                        }
                    }
                    return GetAllNotes(entity.UserId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the note: {ex.StackTrace}");
                    throw ex;
                }
            }
        }
        private int checkcollabtable(int uid,int noteid)
        {
            String query = "select UserId from colabrator where UserId = @userId and NotesId = @noteId ";
            IDbConnection con=Context.CreateConnection();
            return con.Query<int>(query, new { userId = uid, noteId = noteid }).FirstOrDefault(0);
        }
         public Dictionary<string, List<NotesEntity>> GetAllNotes(int userId)
         {
            string queryForOwnNotes = @"
                 SELECT notes.*
                 FROM Notes_Entity notes
                 WHERE notes.UserId = @UserId";
            String queryForOwnColabrators = @"  select collab.userid
                                     from (SELECT notes.*
                                    FROM Notes_Entity notes 
                                    WHERE notes.UserId = @UserId) notess join colabrator collab
                                    on collab.notesId = notess.NoteId";
            string queryForCollabedNotes = @"
                 SELECT notes.*
                 FROM Notes_Entity notes
                 INNER JOIN Colabrator colab ON notes.NoteId = colab.NotesId
                 WHERE colab.UserId = @UserId";

             Dictionary<string, List<NotesEntity>> dict = new Dictionary<string, List<NotesEntity>>();
             using (IDbConnection con = Context.CreateConnection())
             {
                 try
                 {
                     List<NotesEntity> ownNotes = con.Query<NotesEntity>(queryForOwnNotes, new { UserId = userId }).AsList();
                     List<NotesEntity> collabNotes = con.Query<NotesEntity>(queryForCollabedNotes, new { UserId = userId }).AsList();
                     List<int> colabedNotesDetail=con.Query<int>(queryForOwnColabrators, new { UserId = userId }).AsList();

                    foreach (NotesEntity e in ownNotes)
                    {
                        e.CollabId = colabedNotesDetail.Where(e1 => e1 == checkcollabtable(e1, e.NoteId)).ToList();
                    }
                    dict.Add("OwnNotes", ownNotes);
                     dict.Add("CollabNotes", collabNotes);
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine($"An error occurred while retrieving notes: {ex.Message}");
                    throw ex;
                 }
             }
             return dict;
         }
        public void UpdateNotes(NotesEntity notesEntity)
        {
            String Query = @"
            UPDATE Notes_Entity 
            SET Title = @Title, 
            Description = @Description, 
            BgColor = @BgColor, 
            ImagePath = @ImagePath, 
            Remainder = @Remainder, 
            IsArchive = @IsArchive, 
            IsPinned = @IsPinned, 
            IsTrash = @IsTrash, 
            ModifiedAt = @ModifiedAt
            WHERE NoteId = @NoteId and UserId= @UserId";

            using (IDbConnection con = Context.CreateConnection())
            {
                con.Execute(Query, new
                {
                    notesEntity.Title,
                    notesEntity.Description,
                    notesEntity.BgColor,
                    notesEntity.ImagePath,
                    notesEntity.Remainder,
                    notesEntity.IsArchive,
                    notesEntity.IsPinned,
                    notesEntity.IsTrash,
                    notesEntity.ModifiedAt,
                    notesEntity.NoteId,
                    notesEntity.UserId
                });
                string collabQuery = @"INSERT INTO colabrator (UserId, NotesId) VALUES (@UserId, @NoteId);";
               if (notesEntity.CollabId !=null && (notesEntity.CollabId.Any()))
                {
                    con.Execute("DELETE FROM colabrator WHERE NotesId = @NoteId;", new { notesEntity.NoteId });
                    foreach (int collabId in notesEntity.CollabId)
                    {
                        con.Execute(collabQuery, new { UserId = collabId, notesEntity.NoteId });
                    }
                }
               else
                {
                  con.Execute("delete from colabrator where Notesid = @notesid",new {notesid=notesEntity.NoteId});
                }
            }
        }
        public void DeleteNotes(int noteId)
        {
           // String deleteQuery = "delete from Colabrator where notesId = @id";
            IDbConnection connection = Context.CreateConnection();
           // connection.Execute(deleteQuery, new { id = noteId });
            string notesdelete = "update Notes_Entity set IsTrash=1 where NoteId= @id";
            connection.Execute(notesdelete, new { id=noteId});
        }
        public NotesEntity GetById(int noteId)
        {
            IDbConnection con = Context.CreateConnection();
            NotesEntity e= con.Query<NotesEntity>("Select * from Notes_Entity where NoteId = @id", new {id=noteId}).FirstOrDefault();
            e.CollabId = new List<int>();
            String queryforCollab = " SELECT colab.userId FROM Notes_Entity notes INNER JOIN Colabrator colab ON notes.NoteId = colab.NotesId WHERE colab.NotesId=notes.NoteId And notes.NoteId=@noteId";
            List<int> i=con.Query<int>(queryforCollab, new { noteId = noteId}).ToList();
            foreach(int v in i)
            {
                e.CollabId.Add(v);
            }
            return e;
        }

        public List<int> GetUserByNotesIdInCollab(int noteId)
        {
           return Context.CreateConnection().Query<int>("Select * from colabrator where " +
                                                        "NotesId = @NoteId", new {NoteId=noteId}).ToList();
        }

        public int updateColor(int noteId, int userId, string colour)
        {
            try
            {
                var selectQuery = "SELECT NoteId FROM Notes_Entity WHERE UserId = @UserId AND NoteId = @UserNotesId";

                using (var connection = Context.CreateConnection())
                {
                    var currentNoteId = connection.QueryFirstOrDefault<int>(selectQuery, new { UserId = userId, UserNotesId = noteId });
                    if (currentNoteId == 0)
                    {
                        throw new FileNotFoundException("Note not found");
                    }

                    var updateQuery = "UPDATE Notes_Entity SET BgColor = @Colour WHERE NoteId = @UserNotesId AND UserId = @UserId";
                    var parameters = new { UserId = userId, UserNotesId = noteId, Colour = colour };

                    return connection.Execute(updateQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int updateArchive(int noteId, int userId)
        {
            try
            {
                var selectQuery = "SELECT NoteId FROM Notes_Entity WHERE UserId = @UserId AND NoteId = @UserNotesId";

                using (var connection = Context.CreateConnection())
                {
                    var currentNoteId = connection.QueryFirstOrDefault<int>(selectQuery, new { UserId = userId, UserNotesId = noteId });
                    if (currentNoteId == 0)
                    {
                        throw new FileNotFoundException("Note not found");
                    }
                    var updateQuery = "UPDATE Notes_Entity SET IsArchive = CASE WHEN IsArchive  = 0 THEN 1 ELSE 0 END WHERE NoteId = @UserNotesId AND UserId = @UserId";
                    var parameters = new { UserId = userId, UserNotesId = noteId };

                    return connection.Execute(updateQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int UpdateTrash(int noteId, int userId)
        {
            try
            {
                var selectQuery = "SELECT NoteId FROM Notes_Entity WHERE UserId = @UserId AND NoteId = @UserNotesId";

                using (var connection = Context.CreateConnection())
                {
                    var currentNote = connection.QueryFirstOrDefault<int>(selectQuery, new { UserId = userId, UserNotesId = noteId });

                    if (currentNote == null)
                    {
                        throw new FileNotFoundException("Note not found");
                    }

                    var updateQuery = "UPDATE Notes_Entity SET IsTrash = CASE WHEN IsTrash = 0 THEN 1 ELSE 0 END WHERE NoteId = @UserNotesId AND UserId = @UserId";

                    var parameters = new { UserId = userId, UserNotesId = noteId };

                    return connection.Execute(updateQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int permanentDelete(int noteId)
        {
            return Context.CreateConnection().Execute("UPDATE Notes_Entity SET IsPermanentDelete = 1", new { NoteId = noteId });
        }
    }
}
