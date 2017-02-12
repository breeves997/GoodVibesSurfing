using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ValidationService.Contracts
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IGoodVibesValidatorFor<T> : IGoodVibesValidator
    {
        ValidationResult Validate(T item);
    }

    public interface IGoodVibesValidator
    {

    }

    //public delegate ValidationMessage ValidationRule<T>(T entity);

    public class ValidationMessage
    {
        public string ErrorMessage { get; set; }
        public string Name { get; set; }
        public bool Success { get; set; }
    }

    public class ValidationResult 
    {
        public ValidationResult(List<ValidationMessage> messages)
        {
            this.ValidationMessages = messages;
        }
        public ValidationResult()
        {
            this.ValidationMessages = new List<ValidationMessage>();
        }
        public List<ValidationMessage> ValidationMessages { get; set; }
        public int ErrorCount { get { return this.ValidationMessages.Where(x => x.Success == false).Count(); } }
        public bool Success { get { return this.ErrorCount == 0; } }
    }
}
