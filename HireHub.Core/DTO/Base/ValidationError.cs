namespace HireHub.Core.DTO.Base;

public class ValidationError
{
    public string PropertyName { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
}
