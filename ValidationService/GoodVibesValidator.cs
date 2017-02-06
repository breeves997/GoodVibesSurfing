using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SnurfReportService.Interfaces;
using System.Linq.Expressions;

namespace ValidationService
{
    //I would like to leverage this lib but I don't know how to register their rules remotely without delving into source.
    //So, let's simplify
    //public abstract class GoodVibesValidator<T> : AbstractValidator<T>
    //{
    //    public void AddRule<TProperty>(Expression<Func<T, TProperty>> expression)
    //    {
    //        RuleFor<TProperty>(expression);
    //    }
    //}

    public abstract class GoodVibesValidator<T> : IGoodVibesValidator
    {
        private List<Func<T, string>> Validators { get; set; }
        public void AddRule(Func<T, string> rule)
        {
            this.Validators.Add(rule);
        }
        public ValidationResult Validate(T item)
        {
            List<string> errors = new List<string>();
            int errorCount = 0;
            foreach (var validator in Validators)
            {
                string message = validator(item);
                if (!String.IsNullOrWhiteSpace(message))
                {
                    errorCount++;
                    errors.Add(message);
                }
            }

            return new ValidationResult()
            {
                ErrorCount = errorCount,
                Errors = errors
            };

        }
    }

    public interface IGoodVibesValidator { }

    public class ValidationResult
    {
        public List<string> Errors { get; set; }
        public int ErrorCount { get; set; }
    }
}
