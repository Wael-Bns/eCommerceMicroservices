using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eCommerce.API.Filters
{
    public class FluentValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public FluentValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (object? argument in context.ActionArguments.Values)
            {
                if (argument == null)
                {
                    continue;
                }

                Type validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                object? validator = _serviceProvider.GetService(validatorType);

                if (validator is not IValidator nonGenericValidator)
                {
                    continue;
                }

                ValidationContext<object> validationContext = new(argument);
                ValidationResult result = await nonGenericValidator.ValidateAsync(
                    validationContext,
                    context.HttpContext.RequestAborted);

                if (!result.IsValid)
                {
                    Dictionary<string, string[]> errors = result.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(error => error.ErrorMessage).ToArray());

                    context.Result = new BadRequestObjectResult(new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    });

                    return;
                }
            }

            await next();
        }
    }
}