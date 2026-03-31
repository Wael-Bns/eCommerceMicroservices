namespace eCommerce.Core.DTO
{
    public record RegisterRequest(
            string? Email,
            string? Password,
            string? PersonName,
            GenderOptions Gender
    )
    {
        // Parameterless constructor for AutoMapper to work properly.
        public RegisterRequest() : this(default, default, default, default) { }
    }
}
