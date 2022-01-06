﻿using APP.Auth.Model.Dto;
using APP.Auth.Model.Entity;
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
                .ForMember(dest => dest.AuthorizedFolders, act => act.Ignore())
                .IgnoreAllNonExisting()              
                .ForAllOtherMembers(m => m.UseDestinationValue());
            
             
            
        }
    }
}
