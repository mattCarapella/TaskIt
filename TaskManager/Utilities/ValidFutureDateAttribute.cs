using System.ComponentModel.DataAnnotations;

namespace TaskManager.Utilities;

public class ValidFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        DateTime targetDate = Convert.ToDateTime(value);
        return targetDate > DateTime.Now;
    }
}
