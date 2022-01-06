using APP.Base.Model.Dto;
using APP.Infra.Base.Validations.FluentValidation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.FluentValidation
{
    public class CreateTokenDtoValidator: NullReferenceAbstractValidator<CreateTokenDto>
    {
        public CreateTokenDtoValidator()
        {
            When(x => x != null, () =>
             {
                 RuleFor(p => p.Password).NotNull().NotEmpty();

                 When(p => p.Email == null, () =>
                  {
                      RuleFor(p => p.Username).NotNull().NotEmpty();
                  });

                 When(p => p.Email != null, () =>
                 {
                     RuleFor(p => p.Email).NotEmpty().EmailAddress();
                 });
             });
        }
    }
}
