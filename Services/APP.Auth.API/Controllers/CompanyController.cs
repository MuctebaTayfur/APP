using APP.Auth.Model;
using APP.Auth.Model.Dto;
using APP.Auth.Model.Entity;
using APP.Auth.Model.Model;
using APP.Base.Model.Entity;
using APP.Base.Model.Enum;
using APP.Data.Interface;
using APP.Infra.Base.BaseHandler;
using APP.Infra.Base.BaseResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace APP.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        public ApiHandler _apiHandler;
        private readonly IUnitOfWork<AuthContext> _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        public CompanyController(IUnitOfWork<AuthContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _apiHandler =new ApiHandler();
        }
        [ActionName("AddCompanyAndAdminUser")]
        [HttpPost("AddCompanyAndAdminUser")]
        public async Task<ActionResult<Company>> AddCompanyAndAdminUser([FromBody] UserCompanyDto model)
        { 
            try
            {
                var company = new Company()
                {
                    Name = model.CompanyName,
                    PhoneNumber = model.PhoneNumber,
                    Status = Status.Active,
                    Email = model.Email,
                    CreatedOn = DateTime.Now,
                    CreatedBy = 111111110,
                    Avatar = model.UserAvatar,
                    Address = model.Address,
                    DeletedBy = model.DeletedBy,
                };
                
                _unitOfWork.Repository<Company>().Insert(company);
                if (_unitOfWork.SaveChanges() <= 0)
                {
                    return Ok(new ApiResult<Company>()
                    {
                        Message = "Beklenmedik bir hata oluştu",
                        Result = false,
                        HttpStatusCode = (int?)HttpStatusCode.InternalServerError
                    });
                }              
                var applicationUserDto = new ApplicationUserDto()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,               
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,                                   
                    Avatar = model.UserAvatar,
                    Role = "Admin",
                    Theme = model.Theme,
                    CompanyId = company.Id,
                };
                var apiResult =await _apiHandler.Post<ApplicationUser>($"AddApplicationUserWithRole", applicationUserDto);
                List<ApplicationUser> userList = new List<ApplicationUser>();
                userList.Add(apiResult);
                company.Users = userList;
             
                //provider.Flush();
                return Ok(new ApiResult<Company>()
                { Message = "Başarılı", Result = true, Data = company, HttpStatusCode = (int?)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResult<Company>()
                { Message = $"Error: {ex.Message}", Result = false, HttpStatusCode = (int?)HttpStatusCode.BadRequest });
            }
        }

    }
}
