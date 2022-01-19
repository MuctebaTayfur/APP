using APP.Auth.Model;
using APP.Auth.Model.Dto;
using APP.Auth.Model.Entity;
using APP.Base.Model.Base;
using APP.Base.Model.Dto;
using APP.Base.Model.Enum;
using APP.Infra.Base.BaseResult;
using APP.Infra.Base.Validations.FluentValidation;
using AutoMapper;
using FLP.Auth.Model.Dto;
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
        
            //if (!getApiResult.Result)
            //    return BadRequest(new ApiResult<UserRegions>()
            //    {
            //        Result = true,
            //        Data = null,
            //        HttpStatusCode = (int)HttpStatusCode.BadRequest
            //    });

          

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.GivenName, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, ToStringRoleEnum(role.Name).ToString()),
                new(JwtRegisteredClaimNames.UniqueName, ClaimValueTypes.Integer64),
                new(JwtRegisteredClaimNames.Email, user.Email),                
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
        //[Authorize]
        [ActionName("ApplicationUser")]
        [HttpGet("ApplicationUser/{username}")]
        public ActionResult<ApplicationUserDto> ApplicationUser(string username)
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
          

            return Ok(new ApiResult<ApplicationUserDto>()
            {
                Result = true,
                Data = userDto,
                HttpStatusCode = (int)HttpStatusCode.OK
            });
        }
       
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
                    CompanyId=applicationUserDto.CompanyId,
                    PhoneNumber = applicationUserDto.PhoneNumber,
              
                  
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
        [Authorize]
        [ActionName("ChangePassword")]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApplicationUserDto>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByNameAsync(changePasswordDto.Username);
            var userDto = _mapper.Map<ApplicationUserDto>(user);
            userDto.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()?.ToString();
          

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                return Ok(new ApiResult<ApplicationUserDto>()
                {
                    Result = true,
                    Data = userDto,
                    HttpStatusCode = (int)HttpStatusCode.OK
                });
            }
            else
            {
                return Ok(new ApiResult<ApplicationUserDto>()
                {
                    Result = false,
                    Data = userDto,
                    HttpStatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = changePasswordResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j)
                });
            }
        }
        [Authorize]
        [ActionName("AddPassword")]
        [HttpPost("AddPassword")]
        public async Task<ActionResult<ApplicationUser>> AddPassword([FromBody] PasswordDto passwordDto)
        {
            var repoApplicationUser = await _userManager.FindByNameAsync(passwordDto.Username);
            var addPasswordResult = await _userManager.AddPasswordAsync(repoApplicationUser, passwordDto.Password);
            if (addPasswordResult.Succeeded)
            {
                return Ok(new ApiResult<ApplicationUser>()
                {
                    Result = true,
                    Data = repoApplicationUser,
                    HttpStatusCode = (int)HttpStatusCode.OK
                });
            }
            else
            {
                return Ok(new ApiResult<ApplicationUser>()
                {
                    Result = false,
                    Data = repoApplicationUser,
                    HttpStatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = addPasswordResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j)
                });
            }
        }
        [Authorize]
        [ActionName("AddToRole")]
        [HttpPost("AddToRole")]
        public async Task<ActionResult<ApplicationUser>> AddToRole([FromBody] RoleDto roleDto)
        {
            var repoApplicationUser = await _userManager.FindByNameAsync(roleDto.Username);
            var addToRoleResult = await _userManager.AddToRoleAsync(repoApplicationUser, roleDto.Role);
            if (addToRoleResult.Succeeded)
            {
                return Ok(new ApiResult<ApplicationUser>()
                {
                    Result = true,
                    Data = repoApplicationUser,
                    HttpStatusCode = (int)HttpStatusCode.OK
                });
            }
            else
            {
                return Ok(new ApiResult<ApplicationUser>()
                {
                    Result = false,
                    Data = repoApplicationUser,
                    HttpStatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = addToRoleResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j)
                });
            }
        }
        [Authorize]
        [ActionName("UpdateApplicationUserWithRole")]
        [HttpPost("UpdateApplicationUserWithRole")]
        public async Task<ActionResult<ApplicationUserDto>> UpdateApplicationUserWithRole([FromBody] ApplicationUserDto applicationUserDto)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var repoApplicationUser = await _userManager.FindByNameAsync(applicationUserDto.UserName);

                if (!string.IsNullOrEmpty(applicationUserDto.FirstName)) repoApplicationUser.FirstName = applicationUserDto.FirstName;
                if (!string.IsNullOrEmpty(applicationUserDto.LastName)) repoApplicationUser.LastName = applicationUserDto.LastName;
                repoApplicationUser.Status = Status.Modified;
                repoApplicationUser.SecurityStamp = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(applicationUserDto.Email)) repoApplicationUser.Email = applicationUserDto.Email;
                if (!string.IsNullOrEmpty(applicationUserDto.PhoneNumber)) repoApplicationUser.PhoneNumber = applicationUserDto.PhoneNumber;
             


                var updateResult = await _userManager.UpdateAsync(repoApplicationUser);
                if (!updateResult.Succeeded) throw new Exception(updateResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j));

                if (!string.IsNullOrEmpty(applicationUserDto.Role))
                {
                    var roles = await _userManager.GetRolesAsync(repoApplicationUser);
                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(repoApplicationUser, roles);
                    if (!removeRolesResult.Succeeded) throw new Exception(removeRolesResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j));

                    var addToRoleResult = await _userManager.AddToRoleAsync(repoApplicationUser, applicationUserDto.Role);
                    if (!addToRoleResult.Succeeded) throw new Exception(addToRoleResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j));
                }
                var userDto = _mapper.Map<ApplicationUserDto>(repoApplicationUser);
                userDto.Role = _userManager.GetRolesAsync(repoApplicationUser).GetAwaiter().GetResult().FirstOrDefault()?.ToString();
               

                scope.Complete();

                return Ok(new ApiResult<ApplicationUserDto>()
                {
                    Result = true,
                    Data = userDto,
                    HttpStatusCode = (int)HttpStatusCode.OK
                });

            }
            catch (System.Exception ex)
            {
                scope.Dispose();
                return Ok(new ApiResult<ApplicationUserDto>()
                { HttpStatusCode = (int)HttpStatusCode.Unauthorized, Message = $"{ex.Message}" });
            }
        }
      
        [Authorize]
        [ActionName("ApplicationUser")]
        [HttpDelete("ApplicationUser")]
        public async Task<ActionResult<ApplicationUser>> DeleteApplicationUser(string username)
        {
            if (string.IsNullOrEmpty(username)) ModelState.AddModelError("[username]", "username parametresi boş olamaz.");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(username);
                var roles = await _userManager.GetRolesAsync(user);
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!removeRolesResult.Succeeded) throw new Exception(removeRolesResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j));

                var deleteResult = await _userManager.DeleteAsync(user);
                if (deleteResult.Succeeded)
                {
                    return Ok(new ApiResult<ApplicationUser>()
                    {
                        Result = true,
                        Data = user,
                        HttpStatusCode = (int)HttpStatusCode.OK
                    });
                }
                else
                {
                    return Ok(new ApiResult<ApplicationUser>()
                    {
                        Result = false,
                        Data = user,
                        HttpStatusCode = (int)HttpStatusCode.InternalServerError,
                        Message = deleteResult.Errors.Select(x => x.Description).Aggregate((i, j) => i + "//" + j)
                    });
                }
            }
            return Ok(new ApiResult<ApplicationUser>()
            {
                Errors = ModelState.GetErrors(),
                HttpStatusCode = (int?)HttpStatusCode.BadRequest,
                Result = false,
            });


        }

        [Authorize]
        [ActionName("GetApplicationUsers")]
        [HttpGet("GetApplicationUsers")]
        public ActionResult<List<ApplicationUserDto>> GetApplicationUsers(int page, int limit)
        {
            if (page < 0) ModelState.AddModelError(nameof(page), "Page 0'dan küçük olmamalı.");
            if (limit < 1) ModelState.AddModelError(nameof(limit), "Limit 1'den küçük olmamalı.");

            if (ModelState.IsValid)
            {
                var repoUsers = _userManager.Users.OrderBy(x => x.Id);

                return Ok(new ApiResult<List<ApplicationUserDto>>()
                {
                    Data = repoUsers.Skip((page) * limit).Take(limit).ToList().Select(x => new ApplicationUserDto
                    {
                        UserName = x.UserName,
                        Email = x.Email,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        PhoneNumber = x.PhoneNumber,
                        Role = _userManager.GetRolesAsync(x).GetAwaiter().GetResult().FirstOrDefault()?.ToString(),
                      
                    }).ToList(),
                    Message = "",
                    Result = true,
                    HttpStatusCode = (int)HttpStatusCode.OK,
                    PaginationMetaData = new PaginationMetaData() { Page = page, Limit = limit, TotalCount = repoUsers.Count() }
                });
            }
            return Ok(new ApiResult<List<ApplicationUserDto>>()
            {
                Data = null,
                HttpStatusCode = (int?)HttpStatusCode.BadRequest,
                Errors = ModelState.GetErrors(),
            });
        }
        [Authorize]
        [ActionName("GetApplicationUsersWithRole")]
        [HttpGet("GetApplicationUsersWithRole/{role}")]
        public ActionResult<List<ApplicationUserDto>> GetApplicationUsersWithRole(string role, int page, int limit)
        {
            //query by region
            if (page < 0) ModelState.AddModelError(nameof(page), "Page 0'dan küçük olmamalı.");
            if (limit < 1) ModelState.AddModelError(nameof(limit), "Limit 1'den küçük olmamalı.");

            if (!ModelState.IsValid)
                return BadRequest(new ApiResult<List<ApplicationUserDto>>()
                {
                    Data = null,
                    HttpStatusCode = (int?)HttpStatusCode.BadRequest,
                    Errors = ModelState.GetErrors(),
                });

            var repoUsers = _userManager.Users.OrderBy(x => x.Id).ToList();
            var users = repoUsers.Select(x => new ApplicationUserDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber,
                Role = _userManager.GetRolesAsync(x).Result.FirstOrDefault()?.ToString(),
               
            }).ToList();

            users = users.Where(x => x.Role != null && x.Role.ToLower().Trim().Equals(role.ToLower().Trim())).Skip((page) * limit).Take(limit).ToList();

            return Ok(new ApiResult<List<ApplicationUserDto>>()
            {
                Data = users,
                Message = "",
                Result = true,
                HttpStatusCode = (int)HttpStatusCode.OK,
                PaginationMetaData = new PaginationMetaData { Page = page, Limit = limit, TotalCount = repoUsers.Count() }
            });
        }

        [Authorize]
        [ActionName("GetApplicationRoles")]
        [HttpGet("GetApplicationRoles")]
        public ActionResult<List<ApplicationRole>> GetApplicationRoles()
        {
            try
            {
                var repoRoles = _roleManager.Roles.ToList();
                return Ok(new ApiResult<List<ApplicationRole>>()
                {
                    Result = true,
                    Data = repoRoles,
                    HttpStatusCode = (int)HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResult<List<ApplicationRole>>()
                {
                    HttpStatusCode = (int?)HttpStatusCode.Unauthorized,
                    Message = $"{ex.Message}"
                });
            }
        }
      
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(AppRoleDto model)
        {
            IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole { Name = model.Name});
            if (result.Succeeded)
            {
                return Ok(new ApiResult() { Message = "rol oluşturuldu" });
            }
            return BadRequest(new ApiResult() { Message = "rol oluşturma başarısız" });
        }

        //[HttpPost("CreateRole")]
        //public async Task<IActionResult> CreateRole(AppRoleDto model)
        //{
        //    IdentityResult result = await _roleManager.CreateAsync(new AppRole { Name = model.Name, OlusturulmaTarihi = DateTime.Now });
        //    if (result.Succeeded)
        //    {
        //        return Ok(new ApiResult() { Message = "rol oluşturuldu" });
        //    }
        //    return BadRequest(new ApiResult() { Message = "rol oluşturma başarısız" });
        //}
        [ApiExplorerSettings(IgnoreApi = true)]
        private Role ToStringRoleEnum(string roles)
        {
            Role cValue = (Role)Enum.Parse(typeof(Role), roles);
            return cValue;
        }
    }
}
