using BuisinessLayer.Filter.ActionFilter;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.DTO.RequestDto;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service ;
        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpPost]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> createUser(UserRequest request)
        {
               return Ok(await service.createUser(request));
           
           
        }


    }
}
