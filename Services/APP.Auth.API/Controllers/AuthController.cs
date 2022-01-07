using APP.Auth.Model;
using APP.Auth.Model.Dto;
using APP.Auth.Model.Entity;
using APP.Base.Model.Base;
using APP.Base.Model.Dto;
using APP.Base.Model.Enum;
using APP.Infra.Base.BaseResult;
using APP.Infra.Base.Validations.FluentValidation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace APP.Auth.API.Controllers
{  
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
      //  private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
         //   _emailSender = emailSender;
        }
        [HttpPost("~/connect/token")]
        [AllowAnonymous]
        public async Task<ActionResult<Token>> CreateToken([FromBody] CreateTokenDto model)
        {
            try
            {
                var tokenExpiresTime = 60;
                var user = new ApplicationUser();
                if (model.Username is not null)
                    user = _userManager.FindByNameAsync(model.Username).GetAwaiter().GetResult();
                else if (model.Email is not null)
                    user = _userManager.FindByEmailAsync(model.Email).GetAwaiter().GetResult();

                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    throw new Exception("Kullanıcı bulunamadı.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles == null || !userRoles.Any())
                {
                    throw new Exception("Kullanıcı rol bulunamadı.");
                }

                var userRole = new ApplicationRole();
                if (string.IsNullOrEmpty(model.Role))
                {
                    var existRole = _roleManager.Roles.FirstOrDefault(x => x.Name == userRoles.FirstOrDefault());
                    if (existRole != null)
                    {
                        userRole = existRole;
                    }
                }
                else
                {
                    var repoRole = _roleManager.Roles.FirstOrDefault(i => i.RoleUniqueId == model.Role);
                    if (repoRole == null)
                    {
                        throw new Exception("Rol bulunamadı.");
                    }

                    if (userRoles.All(x => x != repoRole.Name))
                    {
                        throw new Exception("Kullanıcı rol bulunamadı.");
                    }

                    userRole = repoRole;
                }

                return CreateJwtSecurityToken(user, userRole, tokenExpiresTime);
            }
            catch (Exception ex)
            {
                return Ok(new ApiResult<Token>()
                { HttpStatusCode = (int)HttpStatusCode.Unauthorized, Message = $"{ex.Message}" });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]

        private ActionResult<Token> CreateJwtSecurityToken(ApplicationUser user, ApplicationRole role, int tokenExpiresTime = 30)
        {
            //var getApiResult = _apiHandler.GetApiResult<UserRegions>($"user-service/regionforuser/{user.UserName}").GetAwaiter().GetResult();
            var getApiResult = new ApiResult<UserRegions>()
            {
                Result = true,
                Data = new UserRegions()
            };
            //if (!getApiResult.Result)
            //    return BadRequest(new ApiResult<UserRegions>()
            //    {
            //        Result = true,
            //        Data = null,
            //        HttpStatusCode = (int)HttpStatusCode.BadRequest
            //    });

            var regionId = 0L;
            if (getApiResult.Result && getApiResult.Data != null)
                regionId = getApiResult.Data.RegionId;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.GivenName, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, ToStringRoleEnum(role.Name).ToString()),
                new(JwtRegisteredClaimNames.UniqueName, ClaimValueTypes.Integer64),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.GroupSid, regionId.ToString())
            };

            var jwtToken = CreateJWTToken(claims, tokenExpiresTime);
            return Ok(new ApiResult<Token>()
            {
                Result = true,
                Data = new Token
                {
                    token_type = "Bearer",
                    access_token = jwtToken,
                    expires_in = tokenExpiresTime
                },
                HttpStatusCode = (int)HttpStatusCode.OK
            });
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private string CreateJWTToken(IEnumerable<Claim> claims, int tokenExpiredTime)
        {
            string secretKey = "e5aa290cc4c264d4ba8aa4118321a36d8353150e0ca4b3eff39c2a6e8e9cda97";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: "https://authapi",
                audience: "Audience",
                notBefore: DateTime.Now,
                claims: claims,
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(tokenExpiredTime)),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        [Authorize]
        [ActionName("GetApplicationUser")]
        [HttpGet("GetApplicationUser/{username}")]
        public ActionResult<ApplicationUserDto> GetApplicationUser(string username)
        {
            if (string.IsNullOrEmpty(username)) ModelState.AddModelError("[username]", "username parametresi boş olamaz.");

            if (!ModelState.IsValid)
                return Ok(new ApiResult<ApplicationUserDto>()
                {
                    Errors = ModelState.GetErrors(),
                    HttpStatusCode = (int?)HttpStatusCode.BadRequest,
                    Result = false,
                });

            var user = _userManager.FindByNameAsync(username).GetAwaiter().GetResult();
            if (user == null) return Ok(new ApiResult<ApplicationUserDto>()
            {
                Result = false,
                Data = null,
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = $"'{username}' adlı kullanıcı bulunamadı."
            });

            var userDto = _mapper.Map<ApplicationUserDto>(user);
            userDto.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            if (user.AuthorizedFolders is { Length: > 0 })
                userDto.AuthorizedFolders = JsonConvert.DeserializeObject<List<AuthorizedFolderDto>>(user.AuthorizedFolders);

            return Ok(new ApiResult<ApplicationUserDto>()
            {
                Result = true,
                Data = userDto,
                HttpStatusCode = (int)HttpStatusCode.OK
            });
        }
        [Authorize]
        [ActionName("AddApplicationUserWithRole")]
        [HttpPost("AddApplicationUserWithRole")]
        public async Task<ActionResult<ApplicationUser>> AddApplicationUserWithRole([FromBody] ApplicationUserDto applicationUserDto)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var repoApplicationUserByEmail = await _userManager.FindByEmailAsync(applicationUserDto.Email);
                if (repoApplicationUserByEmail != null) return Ok(new ApiResult()
                { HttpStatusCode = (int)HttpStatusCode.BadRequest, Result = false, Message = $"Bu e-mail[{applicationUserDto.Email}] zaten alınmış. Başka bir tane deneyin." });

                var applicationUser = new ApplicationUser
                {
                    AccessFailedCount = 0,
                    CreatedBy = 111111110,
                    CreatedOn = DateTime.Now,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    FirstName = applicationUserDto.FirstName,
                    LastName = applicationUserDto.LastName,
                    LockoutEnabled = false,
                    Status = Status.Active,
                    TwoFactorEnabled = false,
                    UserName = applicationUserDto.UserName,
                    Email = applicationUserDto.Email,
                    PhoneNumber = applicationUserDto.PhoneNumber,
                    AuthorizedFolders = applicationUserDto.AuthorizedFolders == null ? null : JsonConvert.SerializeObject(applicationUserDto.AuthorizedFolders)
                };

                var addResult = await _userManager.CreateAsync(applicationUser);
                if (!addResult.Succeeded) throw new Exception(addResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j));

                var repoApplicationUser = await _userManager.FindByNameAsync(applicationUserDto.UserName);
                if (repoApplicationUser == null) throw new Exception("Kullanıcı bulunamadı.");

                var addPasswordResult = await _userManager.AddPasswordAsync(repoApplicationUser, applicationUserDto.Password);
                if (!addPasswordResult.Succeeded) throw new Exception(addPasswordResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j));

                var addToRoleResult = await _userManager.AddToRoleAsync(repoApplicationUser, applicationUserDto.Role);
                if (!addToRoleResult.Succeeded) throw new Exception(addToRoleResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j));

                scope.Complete();

                return Ok(new ApiResult<ApplicationUser>()
                {
                    Result = true,
                    Data = repoApplicationUser,
                    HttpStatusCode = (int)HttpStatusCode.OK
                });
            }
            catch (System.Exception ex)
            {
                scope.Dispose();
                return Ok(new ApiResult<ApplicationUser>()
                { HttpStatusCode = (int)HttpStatusCode.Unauthorized, Message = $"{ex.Message}" });
            }

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private Role ToStringRoleEnum(string roles)
        {
            Role cValue = (Role)Enum.Parse(typeof(Role), roles);
            return cValue;
        }
    }
}
