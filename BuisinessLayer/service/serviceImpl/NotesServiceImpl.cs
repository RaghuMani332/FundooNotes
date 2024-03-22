using System;
using System.Collections.Generic;
using System.Linq;
using BuisinessLayer.CustomException;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;

namespace BuisinessLayer.service.serviceImpl
{
    public class NotesServiceImpl : INotesService
    {
        private readonly INotesRepo NotesRepo;
        private readonly IUserRepo UserRepo;

        public NotesServiceImpl(INotesRepo notesRepo, IUserRepo userRepo)
        {
            NotesRepo = notesRepo;
            UserRepo = userRepo;
        }

        private NotesEntity MapToEntity(NotesRequest request)
        {
            var user = UserRepo.GetUserByEmail(request.UserEmailId).Result;
            if (user == null)
            {
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
                IsTrash = false,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CollabId = request.CollabEmailId == null ? null : UserRepo.GetCollaboratorIdsByEmails(request.CollabEmailId).Result,
                UserId = user.UserId
            };
        }

        private List<NotesResponce> MapToResponse(List<NotesEntity> entities)
        {
            var response = new List<NotesResponce>();
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
                  //  CreatedAt = entity.CreatedAt,
                    ModifiedAt = entity.ModifiedAt,
                    CollabEmailId = UserRepo.GetUserEmailsByIds(entity.CollabId),
                    UserEmailId = userEmail
                });
            }
            
            return response;
        }

        public Dictionary<string, List<NotesResponce>> createNotes(NotesRequest request)
        {
            NotesEntity entity = MapToEntity(request);
            var createdNotes = NotesRepo.createNote(entity);

            // Check if createdNotes is null or empty
            if (createdNotes == null || createdNotes.Count == 0)
            {
                // Log or handle the case where no notes are created
                Console.WriteLine("No notes created.");
                return new Dictionary<string, List<NotesResponce>>(); // Return an empty dictionary
            }

            Dictionary<String, List<NotesResponce>> res = new Dictionary<string, List<NotesResponce>>();
            foreach (var item in createdNotes)
            {
                res.Add(item.Key, MapToResponse(item.Value));
            }
            return res;
            
        }



        public Dictionary<string, List<NotesResponce>> GetAllNotes(string email)
        {
            var user = UserRepo.GetUserByEmail(email).Result;
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }
            var notes = NotesRepo.GetAllNotes(user.UserId);

            Dictionary<String, List<NotesResponce>> d = new Dictionary<string, List<NotesResponce>>();
            foreach (var item in notes)
            {
                d.Add(item.Key, MapToResponse(item.Value));
            }
             Console.WriteLine("print collabed id--------------");
            foreach (var item in d)
            {
                foreach (var item1 in item.Value)
                {
                    foreach (var item2 in item1.CollabEmailId)
                    {
                        Console.WriteLine(item2);
                    }
                }
            }
            Console.WriteLine("--------------------");
            return d;
        }

        public void UpdateNotes(NotesRequest update,int noteId)
        { 
            var entity = MapToEntity(update);
            entity.NoteId = noteId;
            if (!CheckChanges(update.CollabEmailId,UserRepo.GetUserEmailsByIds(NotesRepo.GetUserByNotesIdInCollab(noteId))))
            {
                if (NotesRepo.GetById(noteId).UserId == UserRepo.GetUserByEmail(update.UserEmailId).Result.UserId)
                {
                    NotesRepo.UpdateNotes(entity);
                }
                else
                {
                    throw new Exception("UserMissMatchException()only owner can change the colabrations");
                }
            }
            else{

                NotesRepo.UpdateNotes(entity);
            }
            
            
           
        }
        private bool CheckChanges(List<string> email1, List<string> email2)
        {
            if (email1 == null && email2 == null)
            {
                return true;
            }
            else if (!email1.Any() && !email1.Any())
            {
                return true;
            }
            else if (email1 == null || email2 == null || email1.Count != email2.Count || !email1.Any() || !email2.Any() )
            {
                return false;
            }

            email1.Sort();
            email2.Sort();
            
            return email1.SequenceEqual(email2);
        }

        public void DeleteNote(int noteId, String Email)
        {
            if (NotesRepo.GetById(noteId).UserId == UserRepo.GetUserByEmail(Email).Result.UserId)//if there is no note id present im getting null pointer want to solve that
            {
                try
                {
                    NotesRepo.DeleteNotes(noteId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("UserMisssMatchingWithNote()user id and note is=d is miss matching");
            }
        }
    }
}






