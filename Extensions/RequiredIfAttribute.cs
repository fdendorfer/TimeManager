using System.ComponentModel.DataAnnotations;
using TimeManager.Model;

namespace TimeManager.Extensions
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        public enum result {
            mustBeTrue,
            mustMatchString,
        }        
        public string relatedProperty;
        public result relatedPropertyValue;
        public string matchingString;
        public new string ErrorMessage;

        public RequiredIfAttribute() {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var prop = validationContext.ObjectInstance.GetType().GetProperty(this.relatedProperty).GetValue(validationContext.ObjectInstance, null);

            if(relatedProperty == null)
                throw new System.Exception("Please set relatedProperty to the name of the dependent property as string.");
            
            switch(relatedPropertyValue) {
                case result.mustBeTrue: 
                    if((bool)prop == true || !string.IsNullOrEmpty((string)value))
                        return ValidationResult.Success;
                    else
                        return new ValidationResult(this.ErrorMessage);

                case result.mustMatchString: 
                    if(string.IsNullOrEmpty(matchingString))
                        throw new System.Exception("matching String is required for this operation");
                    if((string)prop == matchingString && string.IsNullOrEmpty((string)value))
                        return new ValidationResult(this.ErrorMessage);
                    else
                        return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }
}