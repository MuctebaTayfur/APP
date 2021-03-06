using APP.Auth.Model.Dto;
using APP.Auth.Model.Entity;
using APP.Base.Model.Dto;
using APP.Base.Model.Entity;
using APP.Base.Model.Model;
using APP.Infra.Base.Extensions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.AutoMapper
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<ApplicationUserDto, ApplicationUser>()
               .IgnoreAllNonExisting();

            CreateMap<ApplicationUser, ApplicationUserDto>()
                .IgnoreAllNonExisting()
                .ForAllOtherMembers(m => m.UseDestinationValue());

            CreateMap<ProductDto, Product>()
                .IgnoreAllNonExisting();
            CreateMap<PacketDto, Packet>()
                .IgnoreAllNonExisting();
            CreateMap<CompanyUserDto, ApplicationUserDto>()
                .IgnoreAllNonExisting();

        }
    }
}
