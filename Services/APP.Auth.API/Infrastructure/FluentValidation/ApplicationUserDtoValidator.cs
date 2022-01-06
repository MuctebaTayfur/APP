using APP.Auth.Model.Dto;
using APP.Infra.Base.Validations.FluentValidation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.FluentValidation
{
    public class ApplicationUserDtoValidator: NullReferenceAbstractValidator<ApplicationUserDto>
    {
        public ApplicationUserDtoValidator()
        {
            When(x => x != null, () =>
              {
                  RuleFor(p => p.UserName).NotNull().NotEmpty();
                  RuleFor(p => p.FirstName).NotNull().NotEmpty();
                  RuleFor(p => p.LastName).NotNull().NotEmpty();
                  RuleFor(p => p.Email).NotNull().NotEmpty();

              });
        }
    }
}
