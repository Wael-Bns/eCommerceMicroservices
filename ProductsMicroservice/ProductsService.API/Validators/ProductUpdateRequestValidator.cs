using FluentValidation;
using ProductsService.Core.DTO;

namespace ProductsService.API.Validators
{
    public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
    {
        public ProductUpdateRequestValidator()
        {
            RuleFor(x => x.ProductID)
                .NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Category is required.");
            RuleFor(x => x.UnitPrice)
                .InclusiveBetween(0, double.MaxValue).WithMessage($"Unit price must be a value between 0 and {double.MaxValue}.");
            RuleFor(x => x.QuantityInStock)
                .InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity in stock must be a value between 0 and {int.MaxValue}.");
        }
    }
}
