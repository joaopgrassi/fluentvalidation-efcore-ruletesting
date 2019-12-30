using FluentValidation;
using Shop.Data.Entities;

namespace Shop.API.Controllers
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Surname)
                .NotEmpty()
                .MaximumLength(255)
                .WithMessage("Please specify a last name");

            RuleFor(x => x.Forename)
                .NotEmpty()
                .MaximumLength(255)
                .WithMessage("Please specify a first name");

            RuleFor(x => x.Address).Length(20, 250);
        }
    }
}
