using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SnurfReportService.Interfaces;
using System.Linq.Expressions;
using ValidationService.Contracts;

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

    public class GoodVibesValidator<T> : IGoodVibesValidatorFor<T>
    {
        public static GoodVibesValidator<T> Create(List<Expression<Func<T, ValidationMessage>>> validationRules)
        {
            var instance = new GoodVibesValidator<T>(validationRules);
            return instance;

        }
        protected GoodVibesValidator()
        {
            ValidatorExpressions = new List<Expression<Func<T, ValidationMessage>>>();
        }
        protected GoodVibesValidator(List<Expression<Func<T, ValidationMessage>>> rules)
        {
            ValidatorExpressions = rules;
            Validators = rules.Select(x => x.Compile()).ToList();
        }
        protected List<Expression<Func<T, ValidationMessage>>> ValidatorExpressions { get; set; }
        protected List<Func<T, ValidationMessage>> Validators { get; set; }
        public void AddRule(Expression<Func<T, ValidationMessage>> rule)
        {
            this.ValidatorExpressions.Add(rule);
            this.Validators.Add(rule.Compile());
        }
        public ValidationResult Validate(T item)
        {
            List<ValidationMessage> results = new List<ValidationMessage>();
            Validators.ForEach(x => results.Add(x(item)));
            return new ValidationResult(results);
        }
    }

}
