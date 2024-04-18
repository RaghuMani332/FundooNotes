using Azure.Core;
using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [LableExceptionFilter]

    public class LableController : ControllerBase
    {
        private readonly ILableService service;

        public LableController(ILableService service)
        {
            this.service = service;
        }


        [HttpPost]
        public ResponceStructure<LableResponce> CreateLable(LableRequest request)
        {
            request.UserEmail = User.FindFirstValue(ClaimTypes.Email);
            //request.UserEmail = "raghum11154@gmail.com";
          //  return Ok(service.CreateLable(request));
            return new ResponceStructure<LableResponce>(service.CreateLable(request), "created successfully");
        }
        [HttpGet("getbyemail")]
        public ResponceStructure<List<LableResponce>> GetLableByEmail()
        {
                 return new ResponceStructure<List<LableResponce>>(service.GetLableByEmail(User.FindFirstValue(ClaimTypes.Email)),"SUCCESS");
                // return new ResponceStructure<List<LableResponce>>(service.GetLableByEmail("raghum11154@gmail.com"),"SUCCESS");
        }
        [HttpGet("{LableId}")]
        public ResponceStructure<LableResponce> GetByLableId(int LableId)
        {
            return new ResponceStructure<LableResponce>( service.GetByLableId(LableId),"Succuss");
        }
        [HttpPut]
        public ResponceStructure<LableResponce> UpdateLable(LableRequest request,int lableid)
        {
            request.UserEmail = User.FindFirstValue(ClaimTypes.Email);
          request.LableId= lableid;
          //  request.UserEmail = "raghum11154@gmail.com";
            return  new ResponceStructure<LableResponce>(service.UpdateLable(request),"updated successfully"); 
        }
        [HttpDelete]
        public ResponceStructure<String> DeleteLable(String LableName)
        {
            return new ResponceStructure<String>(service.DeleteLable(LableName, User.FindFirstValue(ClaimTypes.Email)),"Successfully deleted");
           // return new ResponceStructure<String>(service.DeleteLable(LableName, "raghum11154@gmail.com"),"Successfully deleted");
        }
        
    }
}
