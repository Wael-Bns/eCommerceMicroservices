using FluentValidation;
using FluentValidation.Results;
using OrdersMicroservice.Core.ServiceContracts;

namespace OrdersMicroservice.Core.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;
        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task ValidateAsync<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var validator = _serviceProvider.GetService(typeof(IValidator<T>)) as IValidator<T>;
            if(validator == null)
            {
                throw new Exception($"No validator found for type {typeof(T).Name}");
            }
            ValidationResult result = await validator.ValidateAsync(instance);

            if (!result.IsValid)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }
        }
    }
}
