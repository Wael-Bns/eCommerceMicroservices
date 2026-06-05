using FluentValidation;
using OrdersMicroservice.Core.DTO;

namespace OrdersMicroservice.Core.Validators
{
    public class OrderUpdateRequestValidator : AbstractValidator<OrderUpdateRequest>
    {
        public OrderUpdateRequestValidator()
        {
            // OrderID
            RuleFor(x => x.OrderID)
                .NotEmpty().WithErrorCode("OrderID is required.");
            // UserID
            RuleFor(x => x.UserID)
                .NotEmpty().WithErrorCode("UserID is required.");
            // OrderDate
            RuleFor(x => x.OrderDate)
                .NotEmpty().WithErrorCode("OrderDate is required.");
            // OrderItems
            RuleFor(x => x.OrderItems)
                .NotEmpty().WithErrorCode("At least one order item is required.");
        }
    }
}
