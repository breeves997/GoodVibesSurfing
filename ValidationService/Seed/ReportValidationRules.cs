using SnurfReportService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ValidationService.Contracts;

namespace ValidationService.Seed
{
    public static class ReportValidationRules
    {
        //public static Expression<Func<ReportBase, ValidationMessage>> RatingRule = (x) => new ValidationMessage()
        //{
        //    Success = x.Rating != Ratings.None,
        //    Name = "Rating populated",
        //    ErrorMessage = (x.Rating != Ratings.None) ? "" : "A rating must be entered"
        //};
        //public static Expression<Func<ReportBase, ValidationMessage>> LocationRule = (x) => new ValidationMessage()
        //{
        //    Success = !String.IsNullOrWhiteSpace(x.Location),
        //    Name = "Location Required",
        //    ErrorMessage = (!String.IsNullOrWhiteSpace(x.Location)) ? "" : "A location must be entered"
        //};
        //public static Expression<Func<ReportBase, ValidationMessage>> PosterRule = (x) => new ValidationMessage()
        //{
        //    Success = !String.IsNullOrWhiteSpace(x.Poster),
        //    Name = "Poster Required",
        //    ErrorMessage = (!String.IsNullOrWhiteSpace(x.Location)) ? "" : "A poster must be entered"
        //};
        public static List<Expression<Func<ReportBase, ValidationMessage>>> ReportRules =
            new List<Expression<Func<ReportBase, ValidationMessage>>>()
            {
                x => new ValidationMessage()
                {
                    Success = x.Rating != Ratings.None,
                    Name = "Rating populated",
                    ErrorMessage = (x.Rating != Ratings.None) ? "" : "A rating must be entered"
                } ,
                (x) => new ValidationMessage()
                {
                    Success = !String.IsNullOrWhiteSpace(x.Location),
                    Name = "Location Required",
                    ErrorMessage = (!String.IsNullOrWhiteSpace(x.Location)) ? "" : "A location must be entered"
                } ,
                (x) => new ValidationMessage()
                {
                    Success = !String.IsNullOrWhiteSpace(x.Poster),
                    Name = "Poster Required",
                    ErrorMessage = (!String.IsNullOrWhiteSpace(x.Location)) ? "" : "A poster must be entered"
                }

            };
    }
}
