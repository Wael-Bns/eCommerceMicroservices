using FluentValidation;
using OrdersMicroservice.Core.DTO;

namespace OrdersMicroservice.Core.Validators
{
    public class OrderItemUpdateRequestValidator : AbstractValidator<OrderItemUpdateRequest>
    {
        public OrderItemUpdateRequestValidator() 
        { 
            RuleFor(x => x.ProductID)
                .NotEmpty().WithErrorCode("ProductID is required.");
            RuleFor(x => x.Quantity)
                .NotEmpty().WithErrorCode("Quantity is required.")
                .GreaterThan(0).WithErrorCode("Quantity must be greater than 0.");
            RuleFor(x => x.UnitPrice)
                .NotEmpty().WithErrorCode("UnitPrice is required.")
                .GreaterThan(0).WithErrorCode("UnitPrice must be greater than 0.");
        }
    }
}
