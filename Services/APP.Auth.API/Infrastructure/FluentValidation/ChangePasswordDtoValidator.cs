using APP.Base.Model.Dto;
using APP.Infra.Base.Validations.FluentValidation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.FluentValidation
{
    public class ChangePasswordDtoValidator: NullReferenceAbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            When(x => x != null, () =>
            {
                RuleFor(p => p.Username).NotNull().NotEmpty();
                RuleFor(p => p.CurrentPassword).NotNull().NotEmpty();
                RuleFor(p => p.NewPassword).NotNull().NotEmpty();
            });
        }
    }
}
