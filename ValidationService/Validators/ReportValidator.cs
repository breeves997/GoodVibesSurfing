using SnurfReportService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ValidationService.Contracts;

namespace ValidationService.Validators
{

    class ReportValidator : GoodVibesValidator<ReportBase>
    {
        public ReportValidator() : base()
        {
            Expression<Func<ReportBase, ValidationMessage>> ratingNotNull = x => new ValidationMessage()
            {
                Success = x.Rating != Ratings.None,
                Name = "Rating populated",
                ErrorMessage = (x.Rating != Ratings.None) ? "" : "A rating must be entered"
                
            } ;
            AddRule(ratingNotNull);
        }

        //private Expression<Func<ReportBase, ValidationMessage>> FieldsNotNull(ReportBase report)
        //{
        //    Expression<Func<ReportBase, ValidationMessage>> fieldsNotNull = x => new ValidationMessage() { Success = x.Rating != null } ;
        //    var parameterExp = Expression.Parameter(typeof(ReportBase));
        //    var propertyExp = Expression.Property(parameterExp, propertyName);
        //    MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        //    var someValue = Expression.Constant(propertyValue, typeof(string));
        //    var containsMethodExp = Expression.Call(propertyExp, method, someValue);

        //    return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);

        //    var rtn = new ValidationMessage();
        //    rtn.Name = "Required fields filled out";
        //    var sb = new StringBuilder();
        //    sb.Append("The following required fields were null: ");
        //    bool success = true;
        //    if (String.IsNullOrWhiteSpace(report.Location))
        //    {
        //        success = false;
        //        sb.Append("Location, ");
        //    }
        //    if (report.Rating == null)
        //    {
        //        success = false;
        //        sb.Append("Rating, ");
        //    }
        //    if (!success)
        //    {
        //        rtn.ErrorMessage = sb.ToString();
        //    }
        //    rtn.Success = success;
        //    return rtn;
        //}
        //class Foo
        //{
        //    public string Bar { get; set; }
        //}
        //static void Main()
        //{
        //    var lambda = GetExpression<Foo>("Bar", "abc");
        //    Foo foo = new Foo { Bar = "aabca" };
        //    bool test = lambda.Compile()(foo);
        //}
        //static Expression<Func<T, bool>> GetExpression<T>(string propertyName, string propertyValue)
        //{
        //    var parameterExp = Expression.Parameter(typeof(T), "type");
        //    var propertyExp = Expression.Property(parameterExp, propertyName);
        //    MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        //    var someValue = Expression.Constant(propertyValue, typeof(string));
        //    var containsMethodExp = Expression.Call(propertyExp, method, someValue);

        //    return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
        //}

    }

}
