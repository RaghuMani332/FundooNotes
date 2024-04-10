using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using Microsoft.AspNetCore.Mvc;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LableController : ControllerBase
    {
        private readonly ILableService service;

        public LableController(ILableService service)
        {
            this.service = service;
        }


        [HttpPost("CreateLable")]
        public IActionResult CreateLable(LableRequest request)
        {
            return Ok(service.CreateLable(request));
        }
        [HttpGet("GetByEmail/{UserEmail}")]
        public IActionResult GetLableByEmail(String UserEmail)
        {
            return Ok(service.GetLableByEmail(UserEmail));
        }
        [HttpGet("GetByLableId")]
        [LableExceptionFilter]
        public IActionResult GetByLableId(int LableId)
        {
            return Ok(service.GetByLableId(LableId));
        }
        [HttpPut("UpdateLable")]
        public IActionResult UpdateLable(LableRequest request)
        {
            return Ok(service.UpdateLable(request)); 
        }
        [HttpDelete("DeleteLableByName")]
        public IActionResult DeleteLable(String UserEmail,String LableName)
        {
            return Ok(service.DeleteLable(LableName, UserEmail));
        }
        
    }
}
