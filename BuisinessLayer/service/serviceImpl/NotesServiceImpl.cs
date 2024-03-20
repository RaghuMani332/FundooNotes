/*using Azure.Core;
using BuisinessLayer.CustomException;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class NotesServiceImpl:INotesService
    {
        private readonly INotesRepo NotesRepo;
        private readonly IUserRepo UserRepo;
        public NotesServiceImpl(INotesRepo NotesRepo,IUserRepo userRepo)
        {
            this.NotesRepo = NotesRepo;
            this.UserRepo = userRepo;
        }
        private NotesEntity MapToEntity(NotesRequest request)
        {
            try
            {
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
                    CollabId = request.CollabEmailId == null ? 0 : UserRepo.GetUserByEmail(request.CollabEmailId).Result.UserId,
                    UserId = UserRepo.GetUserByEmail(request.UserEmailId).Result.UserId
                };
            }
            catch (AggregateException ex)
            {
                throw new UserNotFoundException("UserNotFoundById");
            }
        }

        public Dictionary<String, List<NotesResponce>> createNotes(NotesRequest request)
        {
            Dictionary<String, List<NotesResponce>> res = new Dictionary<string, List<NotesResponce>>();
            foreach (var item in NotesRepo.createNote(MapToEntity(request)))
            {
                res.Add(item.Key, MapToResponce(item.Value));
            }
            return res ;
        }

        private List<NotesResponce> MapToResponce(List<NotesEntity> entity)
        {
            List <NotesResponce> responce=new List<NotesResponce>();
            foreach (NotesEntity item in entity)
            {
                responce.Add(new NotesResponce { 
                                                  NoteId=item.NoteId,
                                                  Title=item.Title,
                                                  Description=item.Description,
                                                  BgColor=item.BgColor,
                                                  ImagePath=item.ImagePath,
                                                  Remainder=item.Remainder,
                                                  IsArchive=item.IsArchive,
                                                  IsPinned=item.IsPinned,
                                                  IsTrash=item.IsTrash,
                                                  CreatedAt=item.CreatedAt,
                                                  ModifiedAt=item.ModifiedAt,
                                                 // CollabEmailId=,
                                                  UserEmailId=UserRepo.GetById(item.UserId).UserEmail
                                                });
            }
            return responce ;
        }

        public Dictionary<String, List<NotesResponce>> GetAllNotes(string email)
        {
            var user = UserRepo.GetUserByEmail(email).Result;
            Dictionary<String, List<NotesResponce>> res = new Dictionary<string, List<NotesResponce>>();
            foreach (var item in NotesRepo.GetAllNotes(user.UserId))
            {
                res.Add(item.Key, MapToResponce(item.Value));
            }

            return res;
        }

        public void UpdateNotes(NotesRequest update)
        {
            NotesRepo.UpdateNotes(MapToEntity(update));
        }
    }
}
*/








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
            // Get user ID by email
            var user = UserRepo.GetUserByEmail(request.UserEmailId).Result;
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            // Map request to entity
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
                    CreatedAt = entity.CreatedAt,
                    ModifiedAt = entity.ModifiedAt,
                    CollabEmailId = UserRepo.GetUserEmailsByIds(entity.CollabId).Result,
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

            /*  // Iterate through each note and add collaborators if provided
              foreach (var noteList in createdNotes.Values)
              {
                  foreach (var note in noteList)
                  {
                      // Check if collaborators are provided in the request
                      if (request.CollabEmailId != null && request.CollabEmailId.Any())
                      {
                          // Iterate through each collaborator email
                          foreach (var email in request.CollabEmailId)
                          {
                              // Fetch collaborator information by email
                              var collaborator = UserRepo.GetUserByEmail(email).Result;

                              // If collaborator information is retrieved successfully
                              if (collaborator != null)
                              {
                                  // Add collaborator to the note
                                  NotesRepo.AddCollaborator(note.NoteId, collaborator.UserId);
                              }
                              else
                              {
                                  // Handle case where collaborator information is not found
                                  Console.WriteLine($"Collaborator with email '{email}' not found.");
                                  // Optionally, you can throw an exception or log the error
                              }
                          }
                      }
                  }
              }*/

            // Convert createdNotes to dictionary only if it's not null
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
            return notes.ToDictionary(x => x.Key, x => MapToResponse(x.Value));
        }

        public void UpdateNotes(NotesRequest update)
        {
            var entity = MapToEntity(update);
            NotesRepo.UpdateNotes(entity);
        }
    }
}






