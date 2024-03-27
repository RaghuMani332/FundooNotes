using BuisinessLayer.CustomException;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class LableServiceImpl : ILableService
    {
        private ILableRepo lableRepo;
        private INotesRepo notesRepo;
        private IUserRepo userRepo;

        public LableServiceImpl(ILableRepo lableRepo, INotesRepo notesRepo, IUserRepo userRepo)
        {
            this.lableRepo = lableRepo;
            this.notesRepo = notesRepo;
            this.userRepo = userRepo;
        }

        public LableResponce CreateLable(LableRequest request)
        {
            int uId=userRepo.GetUserByEmail(request.UserEmail).Result.UserId;

            if(request.NoteId>0)
            {
                if(notesRepo.GetById(request.NoteId)!=null)
                {

                    return MapToResponce(lableRepo.CreateLable(MapToEntity(request, uId),true));
                }
                else
                {
                    throw new Exception("Lable Not Created Because The Given NoteId is invalid");
                }
            }
            return MapToResponce(lableRepo.CreateLable(MapToEntity(request, uId),false));

        }

        public string? DeleteLable(string lableName, string userEmail)
        {
            if (lableRepo.DeleteLable(lableName, userRepo.GetUserByEmail(userEmail).Result.UserId) == 1)
                return "Deleted Successfully";
            else return "Lable NotDeketed";
        }

        public LableResponce GetByLableId(int lableId)
        {
            try
            {
                return MapToResponce(lableRepo.getLabelById(lableId));

            }
            catch (NullReferenceException e) 
            {
                throw new LableNotFoundException("Lable Not Found For Given Id");
            }
        }

        public List<LableResponce> GetLableByEmail(string userEmail)
        {
            List<LableResponce> res =new List<LableResponce> ();
            try
            {
                foreach (LableEntity e in lableRepo.GetLableByEmail(userRepo.GetUserByEmail(userEmail).Result.UserId))
                {
                    res.Add(MapToResponce(e));
                }
            }
            catch (NullReferenceException e)
            {
                throw new LableNotFoundException("Lable Not Found For Given EmailId");
            }
            return res;
        }

        public LableResponce UpdateLable(LableRequest request)
        {
            
           return MapToResponce(lableRepo.UpdateLable(MapToEntity(request,userRepo.GetUserByEmail(request.UserEmail).Result.UserId),request.NoteId>0?true:false));
        }

        private LableEntity MapToEntity(LableRequest request,int Uid)
        {
            return new LableEntity { LabelName = request.LabelName, NoteId = request.NoteId, UserId=Uid, LabelId=request.LableId };
        }

        private LableResponce MapToResponce(LableEntity entity)
        {
            try
            {
                return new LableResponce { LabelName = entity.LabelName, NoteId = entity.NoteId, UserEmail = userRepo.GetUserEmailsByIds(new List<int> { entity.UserId }).FirstOrDefault(), LabelId = entity.LabelId };

            }
            catch(Exception e)
            {
                throw e;
            }
        }

    }
}
