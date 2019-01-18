using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;

namespace TimeManager.Extensions
{
  public class RequiredIfAttribute : ValidationAttribute
  {
    public enum Result
    {
      mustBeTrue,
      mustMatchString,
      dateDiffPositive,
    }
    public string relatedProperty;
    public Result relatedPropertyValue;
    public string matchingString;
    public new string ErrorMessage;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      var prop = validationContext.ObjectInstance.GetType().GetProperty(this.relatedProperty).GetValue(validationContext.ObjectInstance, null);

      if (relatedProperty == null)
        throw new System.Exception("Please set relatedProperty to the name of the dependent property as string.");

      switch (relatedPropertyValue)
      {
        case Result.mustBeTrue:
          if ((bool)prop == true || !string.IsNullOrEmpty((string)value))
            return ValidationResult.Success;
          else
            return new ValidationResult(this.ErrorMessage);

        case Result.mustMatchString:
          if (string.IsNullOrEmpty(matchingString))
            throw new System.Exception("matching String is required for this operation");
          if ((string)prop == matchingString && string.IsNullOrEmpty((string)value))
            return new ValidationResult(this.ErrorMessage);
          else
            return ValidationResult.Success;

        case Result.dateDiffPositive:
          if (value == null)
            return new ValidationResult("'Abwesend bis' darf nicht leer sein");
          if (prop == null)
            return new ValidationResult("'Abwesend von' darf nicht leer sein");
          var d1 = DateTime.Parse((string)prop);
          var d2 = DateTime.Parse((string)value);
          if ((d2 - d1).TotalDays < 0)  // ().TotalDays returns the difference, so 0 is same date
            return new ValidationResult(ErrorMessage);
          else
            return ValidationResult.Success;
          
      }
      return ValidationResult.Success;
    }
  }
}