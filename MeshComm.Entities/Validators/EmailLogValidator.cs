using FluentValidation;
using MeshComm.Entities.Entities;

namespace MeshComm.Entities.Validators
{
    public class EmailLogValidator:AbstractValidator<EmailLog>
    {
        public EmailLogValidator()
        {
            RuleFor(x => x.RequestingApplication).NotEmpty().WithMessage("Requesting application is required.");
            RuleFor(x => x.EmailBody).NotEmpty().WithMessage("Content is required.");
        }
    }
}
