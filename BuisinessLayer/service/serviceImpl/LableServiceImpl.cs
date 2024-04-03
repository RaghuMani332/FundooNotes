using BuisinessLayer.CustomException;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Linq;


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
            this.cache=cache;
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
            String CacheKey = "Lable_" + lableId;
            if (lableId <= 0)
                throw new LableNotFoundException("INVALID LABLE ID");
            
           if(GetCache<LableResponce>(CacheKey)==null)
            {
                Console.WriteLine("from db");
                SetCache(CacheKey, MapToResponce(lableRepo.getLabelById(lableId)));
            }

                Console.WriteLine("from cache");
                 return GetCache<LableResponce>(CacheKey);
          //  return JsonSerializer.Deserialize<LableResponce>(cache.GetString(CacheKey));


        }

        public List<LableResponce> GetLableByEmail(string userEmail)
        {
            String cacheKey = "GetLableByEmail";
            if(GetCache<List<LableResponce>>(cacheKey)==null)
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
            if(request.LableId<=0)
                throw new LableNotFoundException("Invalid Lable Id");

            ClearCache();
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
        private bool SetCache(String CacheKey,Object obj)
        {
            Console.WriteLine("set cache");
            String cacheObj = cache.GetString(CacheKey);
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
           if(cacheObj.IsNullOrEmpty())
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(cache.GetString(CacheKey));
        }

        public void ClearCache()//it is not deleting in cache memory want to fix the bug
        {
            // Get all keys from the cache
            var allKeys = GetAllKeys();
           
            // Remove each key from the cache
            foreach (var key in allKeys)
            {
                Console.WriteLine("key->"+key);
               bool b= SetCache(key,null);
                Console.WriteLine(b);
                Console.WriteLine(GetCache<Object>(key));
            }
        }

        /*private IEnumerable<string> GetAllKeys()
        {
            // Get all keys from the cache using the available method based on your cache provider
            // For example, if you're using StackExchange.Redis:
            var endPoint = cache.GetType().GetProperty("EndPoint").GetValue(cache);
            var server = cache.GetType().GetMethod("GetServer").Invoke(cache, new[] { endPoint });
            var keys = server.GetType().GetMethod("Keys").Invoke(server, new object[] { "*" });
            return ((IEnumerable<object>)keys).Select(k => (string)k);
            
        }*/
        private IEnumerable<string> GetAllKeys()
        {
            var multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379"); // Connect to Redis
            var server = multiplexer.GetServer("127.0.0.1", 6379); // Get server
            var keys = server.Keys(); // Retrieve keys
            return keys.Select(k => (string)k);
        }





    }
}
