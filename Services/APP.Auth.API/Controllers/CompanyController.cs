using APP.Auth.Model;
using APP.Auth.Model.Dto;
using APP.Auth.Model.Entity;
using APP.Auth.Model.Model;
using APP.Base.Model.Dto;
using APP.Base.Model.Entity;
using APP.Base.Model.Enum;
using APP.Data.Interface;
using APP.Infra.Base.BaseHandler;
using APP.Infra.Base.BaseResult;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public CompanyController(IUnitOfWork<AuthContext> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _apiHandler =new ApiHandler();
            _mapper = mapper;
        }
        [ActionName("CompanyAndAdminUser")]
        [HttpPost("CompanyAndAdminUser")]
        public async Task<ActionResult<Company>> Company([FromBody] CompanyDto model)
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
                    Address = model.Address              
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
                if (apiResult==null)
                {
                    return Ok(new ApiResult<Company>()
                    { Message = "Kullanıcı eklenemedi", Result = false, HttpStatusCode = (int?)HttpStatusCode.InternalServerError });
                }
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
        [ActionName("CompanyUser")]
        [HttpPost("CompanyUser")]
        public async Task<ActionResult<ApplicationUser>> AddCompanyUser([FromBody] CompanyUserDto model)
        {
            try
            {
                var repoApplicationUser = await _apiHandler.Get<ApplicationUser>($"ApplicationUser/{model.AdminUserName}");                               
                if ( repoApplicationUser==null)
                {
                    return NotFound(new ApiResult<ApplicationUser>()
                    {
                        Message = " Admin Kullanıcı Bulunamadı",
                        Result = false,
                        HttpStatusCode = (int?)HttpStatusCode.NotFound
                    });
                }
                var repoApplicationUserProducts = _unitOfWork.Repository<ApplicationUserProduct>().GetMany(p => p.CompanyId == repoApplicationUser.CompanyId && p.ProductId==model.ProductId);

                var repoApplicationUserProduct = repoApplicationUserProducts.FirstOrDefault();
               
                var packetUserAmount = _unitOfWork.Repository<Packet>().GetById(repoApplicationUserProduct.PacketId).UserAmount;
                if (repoApplicationUserProducts.Count() >= packetUserAmount)
                {
                    return Ok(new ApiResult<ApplicationUser>()
                    {
                        HttpStatusCode = (int?)HttpStatusCode.BadRequest,
                        Message = "daha fazla kullanıcı ekleyemezsiniz"
                    });
                }
                var applicationUser = new ApplicationUserDto()
                {
                    CompanyId = repoApplicationUser.CompanyId,
                    Email=model.Email,
                    FirstName=model.FirstName,
                    LastName=model.LastName,
                    UserName=model.UserName,
                    Password=model.Password,
                    Role=model.Role,
                    ProductId=model.ProductId               
                    
                };
                var apiResult = await _apiHandler.Post<ApplicationUser>($"AddApplicationUserWithRole", applicationUser);
                var applicationUserProduct = new ApplicationUserProduct() {
                    ProductId = model.ProductId,
                    ApplicationUserId = apiResult.Id, 
                    CompanyId= apiResult.CompanyId,
                    PacketId = repoApplicationUserProduct.PacketId,
                    ProductStartDate=repoApplicationUserProduct.ProductStartDate,
                    ProductEndDate=repoApplicationUserProduct.ProductEndDate
                };
                _unitOfWork.Repository<ApplicationUserProduct>().Insert(applicationUserProduct);
                _unitOfWork.SaveChanges();
                if (apiResult ==null)
                {
                    return Ok(new ApiResult<ApplicationUser>() {
                        Message = "Kullanıcı eklenemedi", Result = false, HttpStatusCode = (int?)HttpStatusCode.InternalServerError
                    });
                }
                return Ok(new ApiResult<ApplicationUser>(){
                   Result=true, Message="kullanıcı kaydı başarılı", HttpStatusCode=(int?)HttpStatusCode.OK,Data=apiResult
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResult<ApplicationUser>()
                { Message = $"Error: {ex.Message}", Result = false, HttpStatusCode = (int?)HttpStatusCode.BadRequest });
            }
         
        }

    }
}
