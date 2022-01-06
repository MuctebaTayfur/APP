using APP.Infra.Base.Validations.FluentValidation;
using FLP.Auth.Model.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.FluentValidation
{
    public class PasswordDtoValidator: NullReferenceAbstractValidator<PasswordDto>
    {
        public PasswordDtoValidator()
        {
            When(x => x != null, () =>
            {
                RuleFor(p => p.Username).NotNull().NotEmpty();
                RuleFor(p => p.Password).NotNull().NotEmpty();
            });
        }
    }
}
