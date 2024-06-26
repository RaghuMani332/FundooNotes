﻿using BuisinessLayer.CustomException;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;

namespace BuisinessLayer.service.serviceImpl
{
    public class LableServiceImpl : ILableService
    {
        private ILableRepo lableRepo;
        private INotesRepo notesRepo;
        private IUserRepo userRepo;
        private readonly IDistributedCache cache;

        public LableServiceImpl(ILableRepo lableRepo, INotesRepo notesRepo, IUserRepo userRepo, IDistributedCache cache)
        {
            this.lableRepo = lableRepo;
            this.notesRepo = notesRepo;
            this.userRepo = userRepo;
            this.cache = cache;
        }

        public LableResponce CreateLable(LableRequest request)
        {
            ClearCache();
            LableResponce responce = new LableResponce();
            int uId = userRepo.GetUserByEmail(request.UserEmail).Result.UserId;
            foreach (var item in request.NoteId)
            {
                if ( item> 0)
                {
                    
                        if (notesRepo.GetById(item) != null)
                        {
                            return MapToResponce(lableRepo.CreateLable(MapToEntity(request, uId), true));
                        }
                        else
                        {
                            throw new Exception("Lable Not Created Because The Given NoteId is invalid");
                        }
                    
                }
            }
            return MapToResponce(lableRepo.CreateLable(MapToEntity(request, uId), false));
        }

        public string? DeleteLable(string lableName, string userEmail)
        {
            ClearCache();
            if (lableRepo.DeleteLable(lableName, userRepo.GetUserByEmail(userEmail).Result.UserId) == 1)
                return "Deleted Successfully";
            else return "Lable Not Deleted";
        }

        public LableResponce GetByLableId(int lableId)
        {
            String CacheKey = "Lable_" + lableId;
            if (lableId <= 0)
                throw new LableNotFoundException("INVALID LABLE ID");
            if (GetCache<LableResponce>(CacheKey) == null)
            {
              //  Console.WriteLine("from db");
                SetCache(CacheKey, MapToResponce(lableRepo.getLabelById(lableId)));
            }
            //Console.WriteLine("from cache");
            return GetCache<LableResponce>(CacheKey);
            //  return JsonSerializer.Deserialize<LableResponce>(cache.GetString(CacheKey));
        }

        public List<LableResponce> GetLableByEmail(string userEmail)
        {
            String cacheKey = "GetLableByEmail";
            if (GetCache<List<LableResponce>>(cacheKey) == null)
            {
                List<LableResponce> res = new List<LableResponce>();
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
                // return res;
                SetCache(cacheKey, res);
            }
            return GetCache<List<LableResponce>>(cacheKey);
            /* List<LableResponce> res =new List<LableResponce> ();
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
             return res;*/
        }

        public LableResponce UpdateLable(LableRequest request)
        {
            if (request.LableId <= 0)
            {
                throw new LableNotFoundException("Invalid Lable Id");
            }
            ClearCache();
            return MapToResponce(lableRepo.UpdateLable(MapToEntity(request, userRepo.GetUserByEmail(request.UserEmail).Result.UserId), request.NoteId.Any(e=> e>0)  ? true : false));
        }

        private LableEntity MapToEntity(LableRequest request, int Uid)
        {
            return new LableEntity { LabelName = request.LabelName, NoteId = request.NoteId, UserId = Uid, LabelId = request.LableId };
        }

        private LableResponce MapToResponce(LableEntity entity)
        {
            try
            {
                return new LableResponce { LabelName = entity.LabelName, NoteId = entity.NoteId, UserEmail = userRepo.GetUserEmailsByIds(new List<int> { entity.UserId }).FirstOrDefault(), LabelId = entity.LabelId };

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private bool SetCache(String CacheKey, Object obj)
        {
            Console.WriteLine("set cache");
            String cacheObj = cache.GetString(CacheKey);
            Console.WriteLine("cache key ->" + CacheKey);
            if (cacheObj.IsNullOrEmpty())
            {
                cache.SetString(CacheKey,
                                JsonSerializer.Serialize(obj),
                                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
                return true;
            }
            else
            {
                return false;
            }
        }

        private T GetCache<T>(String CacheKey)
        {
            Console.WriteLine("get cache");
            String cacheObj = cache.GetString(CacheKey);
            if (cacheObj.IsNullOrEmpty())
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(cache.GetString(CacheKey));
        }

        private void ClearCache()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            IDatabase db=redis.GetDatabase();
            var keys=redis.GetServer("127.0.0.1:6379").Keys();
            foreach (var key in keys)
            {
                Console.WriteLine("key to delete "+key);
                db.KeyDelete(key);
            }
        }
    }
}