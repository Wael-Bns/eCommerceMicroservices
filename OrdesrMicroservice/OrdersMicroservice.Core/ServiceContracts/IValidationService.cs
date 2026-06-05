namespace OrdersMicroservice.Core.ServiceContracts
{
    public interface IValidationService
    {
        Task ValidateAsync<T>(T instance);
    }
}
