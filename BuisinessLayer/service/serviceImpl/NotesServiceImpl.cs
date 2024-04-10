using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.Extensions.Logging;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;

namespace BuisinessLayer.service.serviceImpl
{
    public class NotesServiceImpl : INotesService
    {
        private readonly INotesRepo NotesRepo;
        private readonly IUserRepo UserRepo;
        private readonly ILogger<NotesServiceImpl> log;

        public NotesServiceImpl(INotesRepo notesRepo, IUserRepo userRepo,ILogger<NotesServiceImpl> log)
        {
            NotesRepo = notesRepo;
            UserRepo = userRepo;
            this.log = log;
        }

        private NotesEntity MapToEntity(NotesRequest request)
        {
            UserEntity user = null;
            try
            {
               user = UserRepo.GetUserByEmail(request.UserEmailId).Result;
            }
            catch (Exception ex)
            {
                log.LogError("user not found");
                throw new UserNotFoundException("UserNotFound");
            }
            if (user == null)
            {
                log.LogError("User not found");    
                throw new UserNotFoundException("User not found");
            }
            return new NotesEntity
            {
                Title = request.Title,
                Description = request.Description,
                BgColor = request.BgColor,
                ImagePath = request.ImagePath,
                Remainder = request.Remainder,
                IsArchive = request.IsArchive,
                IsPinned = request.IsPinned,
                IsTrash = request.IsTrash,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CollabId = request.CollabEmailId == null ? null : UserRepo.GetCollaboratorIdsByEmails(request.CollabEmailId).Result,
                UserId = user.UserId,
                           };
        }

        private List<NotesResponce> MapToResponse(List<NotesEntity> entities)
        {
            var response = new List<NotesResponce>();
            try
            {
                foreach (var entity in entities)
                {
                    var userEmail = UserRepo.GetById(entity.UserId).UserEmail;
                    response.Add(new NotesResponce
                    {
                        NoteId = entity.NoteId,
                        Title = entity.Title,
                        Description = entity.Description,
                        BgColor = entity.BgColor,
                        ImagePath = entity.ImagePath,
                        Remainder = entity.Remainder,
                        IsArchive = entity.IsArchive,
                        IsPinned = entity.IsPinned,
                        IsTrash = entity.IsTrash,
                        CreatedAt = entity.CreatedAt,
                        ModifiedAt = entity.ModifiedAt,
                        CollabEmailId = UserRepo.GetUserEmailsByIds(entity.CollabId),
                        UserEmailId = userEmail,


                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return response;
        }

        public Dictionary<string, List<NotesResponce>> createNotes(NotesRequest request)
        {
            foreach (var v in request.CollabEmailId)
            {
                try
                {
                    String s = UserRepo.GetUserByEmail(v).Result.UserEmail;
                    if (s == null || s.Equals(""))
                    {
                        log.LogError("user not in database please add the user " + v);
                        throw new UserNotFoundException("user not in database please add " + v);
                    }
                }
                catch (AggregateException ex)
                {
                    log.LogError("user not in database please add the user "+v);
                    throw new UserNotFoundException("user not in database please add " + v);

                }
            }
            NotesEntity entity = MapToEntity(request);
            entity.IsTrash = false;
            Dictionary<string, List<NotesEntity>> createdNotes;
            try
            {
                createdNotes = NotesRepo.createNote(entity);
            }
            catch(Exception ex) 
            {
                log.LogError("Error occured when creating notes");
                throw new UnableToCreateNoteException("Error occured when creating notes");
            }

            if (createdNotes == null || createdNotes.Count == 0)
            {
//                Console.WriteLine("No notes created.");
                log.LogInformation("No Notes Created");
                return new Dictionary<string, List<NotesResponce>> { { "own", new List<NotesResponce>() }, { "Collab", new List<NotesResponce>() } };
            }

            Dictionary<String, List<NotesResponce>> res = new Dictionary<string, List<NotesResponce>>();
            foreach (var item in createdNotes)
            {
                res.Add(item.Key, MapToResponse(item.Value));
            }
            log.LogInformation("Notes Created Successfully");
            return res;
            
        }



        public Dictionary<string, List<NotesResponce>> GetAllNotes(string email)
        {
            var user = UserRepo.GetUserByEmail(email).Result;
            if (user == null)
            {
                log.LogError("User Not Found "+email);
                throw new UserNotFoundException("User not found");
            }
            Dictionary<string, List<NotesEntity>> notes = null;
            try
            {
                 notes = NotesRepo.GetAllNotes(user.UserId);
            }
            catch(Exception ex) 
            {
                
                log.LogError("Unable to get all notes "+ ex.Message);
                throw new UnableToFetchAllNotesException("Unable to get all notes");
            }

            Dictionary<String, List<NotesResponce>> d = new Dictionary<string, List<NotesResponce>>();
            foreach (var item in notes)
            {
                d.Add(item.Key, MapToResponse(item.Value));
            }
            log.LogInformation("notes Sent Successfully");
            return d;
        }

        public Dictionary<string, List<NotesResponce>> UpdateNotes(NotesRequest update,int noteId)
        { 
            var entity = MapToEntity(update);
            entity.NoteId = noteId;
            if (!CheckChanges(update.CollabEmailId,UserRepo.GetUserEmailsByIds(NotesRepo.GetUserByNotesIdInCollab(noteId))))
            {
                if (NotesRepo.GetById(noteId).UserId == UserRepo.GetUserByEmail(update.UserEmailId).Result.UserId)
                {
                    foreach(var v in update.CollabEmailId)
                    {
                        try
                        {
                            String s = UserRepo.GetUserByEmail(v).Result.UserEmail;
                            if (s == null || s.Equals(""))
                            {
                                log.LogError("user not in database please add " + v);
                                throw new UserNotFoundException("user not in database please add " + v);
                            }
                        }
                        catch(AggregateException ex)
                        {

                            log.LogError("collab not in database please add " + v);
                            Console.WriteLine("this is for collab not found");
                            throw new UserNotFoundException("user not in database please add " + v);

                        }
                    }
                    NotesRepo.UpdateNotes(entity);
                }
                else
                {

                    log.LogError("only owner can change the colabrations");
                    throw new UserMissMatchException("only owner can change the colabrations");
                }
            }
            else{

                throw new Exception("CollabId MissMathing");
            }

            return GetAllNotes(update.UserEmailId);
           
        }
        private bool CheckChanges(List<string> email1, List<string> email2)
        {
            if (email1 == null && email2 == null)
            {
                Console.WriteLine("if 1");
                return true;
            }
            else if (!email1.Any() && !email1.Any())
            {
                Console.WriteLine("if 2");
                return true;
            }
            else if (email1 == null || email2 == null || email1.Count != email2.Count || !email1.Any() || !email2.Any() )
            {
                Console.WriteLine("if 3");
                return false;
            }

            email1.Sort();
            email2.Sort();
            
            return email1.SequenceEqual(email2);
        }

        public void DeleteNote(int noteId, String Email)
        {
            try
            {
                if (NotesRepo.GetById(noteId).UserId == UserRepo.GetUserByEmail(Email).Result.UserId)//if there is no note id present im getting null pointer want to solve that
                {
                    try
                    {
                        NotesRepo.DeleteNotes(noteId);
                        log.LogInformation($"Note {noteId} deleted");
                    }
                    catch (Exception ex)
                    {
                        log.LogError("This is note is unable to delete");
                        throw new UnableToDeletNoteException("This is note is unable to delete");
                    }
                }
                else
                {
                    log.LogError("user id and note id is miss matching");
                    throw new UserMissMatchException("user id and note id is miss matching");
                }
            }
            catch(NullReferenceException ex) 
            {
                log.LogError("Note Is Not Present By Id");
                throw new NoteNotPresentByIdException("Note Is Not Present By Id");
            }
        }

        public NotesResponce GetByNoteId(int noteId)
        {
            try
            {
                return MapToResponse(new List<NotesEntity>() { NotesRepo.GetById(noteId) }).FirstOrDefault();
            }
            catch(NullReferenceException ex)
            {
                throw new NoteNotPresentByIdException("Note is not present by the given id");
            }
           
        }
    }
}






