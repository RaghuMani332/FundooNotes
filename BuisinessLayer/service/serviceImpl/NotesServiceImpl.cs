using BuisinessLayer.CustomException;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
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

        public void createNotes(NotesRequest request)
        {
            NotesRepo.createNote(MapToEntity(request));
        }
    }
}
