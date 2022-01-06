using APP.Auth.Model.Model;
using APP.Infra.Base.Validations.FluentValidation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.FluentValidation
{
    public class ResetPasswordModelValidator: NullReferenceAbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordModelValidator()
        {
            RuleFor(p => p.Token).NotEmpty();
            RuleFor(p => p.Email).NotEmpty();
            When(p => p.Email != null, () =>
            {
                RuleFor(p => p.Email).EmailAddress().WithMessage("Geçerli bir eposta adresi değil.");
            });
            RuleFor(p => p.NewPassword).NotEmpty()
               .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.,])[A-Za-z\d@$!%*?&.,]{8,}$").WithMessage("Şifre en az bir küçük harf, bir büyük harf, bir rakam, bir özel karakter(@$!%*?&.,) ve minimum 8 karakterden oluşmalıdır.");
            RuleFor(p => p.ConfirmPassword).NotEmpty();
            RuleFor(p => p.ConfirmPassword).Equal(p => p.NewPassword).WithMessage("ConfirmPassword NewPassword ile eşleşmelidir.");

        }
    }
}
