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
    public class PacketController : ControllerBase
    {
        private readonly IUnitOfWork<AuthContext> _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper mapper;
        public PacketController(IUnitOfWork<AuthContext> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [ActionName("AddPacket")]
        [HttpPost("AddPacket")]
        public async Task<ActionResult<Packet>> AddPacket([FromBody] PacketDto model)
        {
            try
            {
                var packet = mapper.Map<Packet>(model);
                packet.CreatedOn = DateTime.Now;

                _unitOfWork.Repository<Packet>().Insert(packet);
                if (_unitOfWork.SaveChanges() <= 0)
                {
                    return Ok(new ApiResult<Packet>()
                    {
                        Message = "Beklenmedik bir hata oluştu",
                        Result = false,
                        HttpStatusCode = (int?)HttpStatusCode.InternalServerError
                    });

                }
                //provider.Flush();
                return Ok(new ApiResult<Packet>()
                { Message = "Başarılı", Result = true, Data = mapper.Map(model, packet), HttpStatusCode = (int?)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResult<Packet>()
                { Message = $"Error: {ex.Message}", Result = false, HttpStatusCode = (int?)HttpStatusCode.BadRequest });
            }
        }
    }
}
