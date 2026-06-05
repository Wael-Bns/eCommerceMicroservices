using FluentValidation;

namespace ProductsService.API.Filters;

public class FluentValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Loop through all incoming arguments in the Minimal API endpoint
        foreach (var argument in context.Arguments)
        {
            if (argument is null)
                continue;

            // Check if there is a registered validator for this specific model type
            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator is not null)
            {
                // Create a validation context and validate the model
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    // Return a standard 400 Validation Problem response for Minimal APIs
                    return Results.ValidationProblem(
                        validationResult.ToDictionary(),
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "One or more validation errors occurred."
                    );
                }
            }
        }

        // Move to the next filter or execute the endpoint handler
        return await next(context);
    }
}