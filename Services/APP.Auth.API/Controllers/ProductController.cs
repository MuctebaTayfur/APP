using APP.Auth.Model;
using APP.Auth.Model.Dto;
using APP.Auth.Model.Entity;
using APP.Base.Model.Dto;
using APP.Base.Model.Entity;
using APP.Common.Data.Concrete;
using APP.Data.Interface;
using APP.Infra.Base.BaseResult;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork<AuthContext> _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper mapper;
        public ProductController(IUnitOfWork<AuthContext> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [ActionName("AddProduct")]
        [HttpPost("AddProduct")]
        public async Task<ActionResult<Product>> AddProduct([FromBody] ProductDto model)
        {
            try
            {
                var product = mapper.Map<Product>(model);
                product.CreatedOn = DateTime.Now;

                _unitOfWork.Repository<Product>().Insert(product);
                if (_unitOfWork.SaveChanges() <= 0)
                {
                    return Ok(new ApiResult<Product>()
                    {
                        Message = "Beklenmedik bir hata oluştu",
                        Result = false,
                        HttpStatusCode = (int?)HttpStatusCode.InternalServerError
                    });

                }
                //provider.Flush();
                return Ok(new ApiResult<Product>()
                { Message = "Başarılı", Result = true, Data = mapper.Map(model, product), HttpStatusCode = (int?)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResult<Product>()
                { Message = $"Error: {ex.Message}", Result = false, HttpStatusCode = (int?)HttpStatusCode.BadRequest });
            }
        }
        //  [Authorize(Roles ="Admin")]
        [ActionName("AddProductUser")]
        [HttpPost("AddProductUser")]
        public async Task<ActionResult<Product>> AddProductUser([FromBody] ProductUserDto model)
        {
            try
            {
                var productPacket = _unitOfWork.Repository<Packet>().GetById(model.PacketId);
                var applicationUserProduct = new ApplicationUserProduct()
                {
                    ApplicationUserId = model.UserId,
                    ProductId = model.ProductId,
                    ProductStartDate = DateTime.Now,
                    ProductEndDate = DateTime.Now.AddMonths(productPacket.PacketTime)
                };                        
                _unitOfWork.Repository<ApplicationUserProduct>().Insert(applicationUserProduct);               
                if (_unitOfWork.SaveChanges() <= 0)
                {
                    return Ok(new ApiResult<Product>()
                    {
                        Message = "Beklenmedik bir hata oluştu",
                        Result = false,
                        HttpStatusCode = (int?)HttpStatusCode.InternalServerError
                    });

                }
                //provider.Flush();
                return Ok(new ApiResult<Product>()
                { Message = "Başarıyla Eklendi", Result = true, HttpStatusCode = (int?)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResult<Product>()
                { Message = $"Error: {ex.Message}", Result = false, HttpStatusCode = (int?)HttpStatusCode.BadRequest });
            }
        }
    }
}
