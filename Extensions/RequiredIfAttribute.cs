using System.ComponentModel.DataAnnotations;
using TimeManager.Model;

namespace TimeManager.Extensions
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        public bool AllowEmptyStrings;
        public string mustBeTrueProperty;
        public string mustNotBeEmptyProperty;
        public new string ErrorMessage;

        public RequiredIfAttribute() {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(mustBeTrueProperty == null && mustNotBeEmptyProperty == null){
                throw new System.Exception("Eigther mustBeTrueProperty or mustNotBeEmptyProperty must contain the name of a dependent property as string.");
            }
            if(mustBeTrueProperty != null && (bool)validationContext.ObjectInstance.GetType().GetProperty(this.mustBeTrueProperty).GetValue(validationContext.ObjectInstance, null)){
                return new ValidationResult(this.ErrorMessage);
            }
            if(mustNotBeEmptyProperty != null && string.IsNullOrEmpty((string)validationContext.ObjectInstance.GetType().GetProperty(this.mustNotBeEmptyProperty).GetValue(validationContext.ObjectInstance, null))){
                return new ValidationResult(this.ErrorMessage);
            }

            if(value == null)
            {
                return new ValidationResult(this.ErrorMessage);
            }
            var stringValue = value as string;
            if (stringValue != null && !AllowEmptyStrings) {
                //return stringValue.Trim().Length != 0;
            }
            return new ValidationResult(this.ErrorMessage);
        }
    }
}