using FluentValidation;
using OrdersMicroservice.Core.DTO;

namespace OrdersMicroservice.Core.Validators
{
    public class OrderAddRequestValidator : AbstractValidator<OrderAddRequest>
    {
        public OrderAddRequestValidator()
        {
            RuleFor(x => x.UserID).NotEmpty().WithErrorCode("UserID is required.");
            RuleFor(x => x.OrderDate).NotEmpty().WithErrorCode("OrderDate is required.");
            RuleFor(x => x.OrderItems).NotEmpty().WithErrorCode("At least one order item is required.");
        }
    }
}
