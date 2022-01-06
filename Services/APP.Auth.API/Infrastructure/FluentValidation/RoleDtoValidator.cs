using APP.Auth.Model.Dto;
using APP.Infra.Base.Validations.FluentValidation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.FluentValidation
{
    public class RoleDtoValidator: NullReferenceAbstractValidator<RoleDto>
    {
        public RoleDtoValidator()
        {
            When(x => x != null, () =>
            {
                RuleFor(p => p.Username).NotNull().NotEmpty();
                RuleFor(p => p.Role).NotNull().NotEmpty();
            });
        }
    }
}
