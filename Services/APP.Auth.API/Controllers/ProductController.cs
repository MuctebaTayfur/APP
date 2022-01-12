using APP.Auth.Model;
using APP.Base.Model.Dto;
using APP.Base.Model.Entity;
using APP.Data.Interface;
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
        [ActionName("AddProductUser")]
        [HttpPost("AddProductUser")]
        public async Task<ActionResult<Product>> AddProductUser([FromBody] app model)
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
    }
}
