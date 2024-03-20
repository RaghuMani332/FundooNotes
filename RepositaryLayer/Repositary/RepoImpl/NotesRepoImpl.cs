/*using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    
    public class NotesRepoImpl : INotesRepo
    {
        private readonly DapperContext Context;
        private readonly IUserRepo userRepo;
        public NotesRepoImpl(DapperContext context,IUserRepo userRepo) { 
            this.Context = context;
            this.userRepo = userRepo;
            
        }
        *//* public Dictionary<String, List<NotesEntity>> createNote(NotesEntity entity)
         {
             Console.WriteLine(entity.CollabId);
             String Query = "insert into Notes_Entity values (@Title,@Description,@BgColor,@ImagePath,@Remainder,@IsArchive,@IsPinned,@IsTrash,@CreatedAt,@ModifiedAt,@UserId)  SELECT SCOPE_IDENTITY()";
             IDbConnection con = Context.CreateConnection();
            int notesid = con.QuerySingle<int>(Query, entity);
             if (entity.CollabId!=0)
             {
                 String collabQuery = "Insert into colabrator values (@userId,@NotesId)";
                 con.Execute(collabQuery,new { userId = entity.UserId ,NotesId=notesid});
             }

             return GetAllNotes(entity.UserId);
         }*//*
        public Dictionary<string, List<NotesEntity>> createNote(NotesEntity entity)
        {
            using (IDbConnection con = Context.CreateConnection())
            {
                try
                {
                    // Insert the new note and retrieve the generated note ID
                    string insertQuery = @"
                INSERT INTO Notes_Entity (Title, Description, BgColor, ImagePath, Remainder, IsArchive, IsPinned, IsTrash, CreatedAt, ModifiedAt, UserId)
                VALUES (@Title, @Description, @BgColor, @ImagePath, @Remainder, @IsArchive, @IsPinned, @IsTrash, @CreatedAt, @ModifiedAt, @UserId);
                SELECT SCOPE_IDENTITY();
            ";
                    int noteId = con.QuerySingle<int>(insertQuery, entity);

                    // If collaboration is requested, add the collaboration entry
                    if (entity.CollabId != 0)
                    {
                        string collabQuery = "INSERT INTO Colabrator (UserId, NotesId) VALUES (@UserId, @NoteId)";
                        con.Execute(collabQuery, new { UserId = entity.CollabId, NoteId = noteId });
                    }

                    // Return all notes for the user
                    return GetAllNotes(entity.UserId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the note: {ex.Message}");
                    return null;
                }
            }
        }



        public Dictionary<String,List<NotesEntity>> GetAllNotes(int userId)
        {
            *//* string queryForOwnNotes = "SELECT * FROM Notes_Entity notes inner join Colabrator colab on notes.NoteId=colab.NotesId WHERE notes.UserId = @UserId";
             string queryForCollabedNotes = "SELECT * FROM Notes_Entity WHERE CollabId = @UserId";
             Dictionary<String, List<NotesEntity>> dict = new Dictionary<string, List<NotesEntity>>();
             IDbConnection con = Context.CreateConnection();

             try
             {
                 dict.Add("OwnNotes", con.Query<NotesEntity>(queryForOwnNotes, new { UserId = userId }).AsList());
                 dict.Add("CollabNotes", con.Query<NotesEntity>(queryForCollabedNotes, new { UserId = userId }).AsList());
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"An error occurred while retrieving notes: {ex.Message}");
             }

             return dict;*//*
            string queryForOwnNotes = @"
                                        SELECT notes.*
                                        FROM Notes_Entity notes
                                        WHERE notes.UserId = @UserId
                                    ";

            string queryForCollabedNotes = @"
                                        SELECT notes.*
                                        FROM Notes_Entity notes
                                        INNER JOIN Colabrator colab ON notes.NoteId = colab.NotesId
                                        WHERE colab.UserId = @UserId
                                    ";

            Dictionary<string, List<NotesEntity>> dict = new Dictionary<string, List<NotesEntity>>();

            using (IDbConnection con = Context.CreateConnection())
            {
                try
                {
                    List<NotesEntity> ownNotes = con.Query<NotesEntity>(queryForOwnNotes, new { UserId = userId }).AsList();
                    List<NotesEntity> collabNotes = con.Query<NotesEntity>(queryForCollabedNotes, new { UserId = userId }).AsList();

                    dict.Add("OwnNotes", ownNotes);
                    dict.Add("CollabNotes", collabNotes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while retrieving notes: {ex.Message}");
                }
            }

            return dict;

        }

        public void UpdateNotes(NotesEntity notesEntity)
        {
            String Query = "update Notes_Entity set (@Title = title,@Description = description,@BgColor = color,@ImagePath = img,@Remainder = rem,@IsArchive = isarchive,@IsPinned = pinned,@IsTrash = trash,@ModifiedAt = modified,@CollabId = collab)";
            // still want to do mapping for all
        }
    }
}
*/













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
        private readonly IUserRepo userRepo;

        public NotesRepoImpl(DapperContext context, IUserRepo userRepo)
        {
            this.Context = context;
            this.userRepo = userRepo;
        }

        public Dictionary<string, List<NotesEntity>> createNote(NotesEntity entity)
        {
            using (IDbConnection con = Context.CreateConnection())
            {
                try
                {
                    // Insert the new note and retrieve the generated note ID
                    string insertQuery = "INSERT INTO Notes_Entity (Title, Description, BgColor, ImagePath, Remainder, IsArchive, IsPinned, IsTrash, CreatedAt, ModifiedAt, UserId) VALUES (@Title, @Description, @BgColor, @ImagePath, @Remainder, @IsArchive, @IsPinned, @IsTrash, @CreatedAt, @ModifiedAt, @UserId); SELECT SCOPE_IDENTITY();";
                    int noteId = con.QuerySingle<int>(insertQuery, entity);
                    Console.WriteLine("Inserted in title");
                    // If collaboration is requested, add the collaboration entry
                    if (entity.CollabId != null && entity.CollabId.Any())
                    {
                        Console.WriteLine("entered if");
                        foreach (var v in entity.CollabId)
                        {
                            Console.WriteLine("entered foreach...........");
                            // string collabQuery = "INSERT INTO Colabrator (UserId, NotesId) VALUES (@UserId, @NoteId)";
                            string collabQuery = "INSERT INTO Colabrator (UserId, NotesId) VALUES (@UserId, @NoteId);";
                            con.Execute(collabQuery, new { UserId = v , NoteId = noteId });
                         
                        }
                    }

                    // Return all notes for the user
                    return GetAllNotes(entity.UserId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the note: {ex.StackTrace}");
                    return null;
                }
            }
        }

        public Dictionary<string, List<NotesEntity>> GetAllNotes(int userId)
        {
            string queryForOwnNotes = @"
                SELECT notes.*
                FROM Notes_Entity notes
                WHERE notes.UserId = @UserId
            ";

            string queryForCollabedNotes = @"
                SELECT notes.*
                FROM Notes_Entity notes
                INNER JOIN Colabrator colab ON notes.NoteId = colab.NotesId
                WHERE colab.UserId = @UserId
            ";

            Dictionary<string, List<NotesEntity>> dict = new Dictionary<string, List<NotesEntity>>();

            using (IDbConnection con = Context.CreateConnection())
            {
                try
                {
                    List<NotesEntity> ownNotes = con.Query<NotesEntity>(queryForOwnNotes, new { UserId = userId }).AsList();
                    List<NotesEntity> collabNotes = con.Query<NotesEntity>(queryForCollabedNotes, new { UserId = userId }).AsList();

                    dict.Add("OwnNotes", ownNotes);
                    dict.Add("CollabNotes", collabNotes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while retrieving notes: {ex.Message}");
                }
            }

            return dict;
        }

        public void UpdateNotes(NotesEntity notesEntity)
        {
            // You need to complete the update query and mapping for all properties
            String Query = @"
                UPDATE Notes_Entity 
                SET Title = @Title, Description = @Description, BgColor = @BgColor, ImagePath = @ImagePath, 
                    Remainder = @Remainder, IsArchive = @IsArchive, IsPinned = @IsPinned, IsTrash = @IsTrash, 
                    ModifiedAt = @ModifiedAt, CollabId = @CollabId
                WHERE NoteId = @NoteId
            ";

            using (IDbConnection con = Context.CreateConnection())
            {
                con.Execute(Query, notesEntity);
            }
        }


        public void AddCollaborator(int noteId, int collaboratorId)
        {
            string query = "INSERT INTO Colabrator (NotesId, UserId) VALUES (@NoteId, @CollaboratorId)";
            using (IDbConnection connection = Context.CreateConnection())
            {
                connection.Execute(query, new { NoteId = noteId, CollaboratorId = collaboratorId });
            }
        }
    }
}
