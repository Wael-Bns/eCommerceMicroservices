using FluentValidation;
using ProductsService.Core.DTO;

namespace ProductsService.API.Validators
{
    public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
    {
        public ProductAddRequestValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Category is required.");
            RuleFor(x => x.UnitPrice)
                .InclusiveBetween(0, double.MaxValue).WithMessage($"Unit price must be a value between 0 and {double.MaxValue}.");
            RuleFor(x => x.QuantityStock)
                .InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity in stock must be a value between 0 and {int.MaxValue}.");
        }
    }
}
