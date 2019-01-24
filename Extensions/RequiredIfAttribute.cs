using System;
using System.ComponentModel.DataAnnotations;

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
    public Result propertyRelation;
    public string matchingString;
    public new string ErrorMessage;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      // Gets the value of the related property
      var prop = validationContext.ObjectInstance.GetType().GetProperty(this.relatedProperty).GetValue(validationContext.ObjectInstance, null);

      // Check if related property was defined
      if (relatedProperty == null)
        throw new System.Exception("Please set relatedProperty to the name of the dependent property as string.");

      switch (propertyRelation)
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