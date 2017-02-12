using ExpressionSerialization;
using SnurfReportService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ValidationService.Contracts;

namespace ValidationService
{
    public static class Serialization
    {
        /// <summary>
        /// Hey friends! If anyone is looking at this, they are probably wondering wat da eff is going on. This is a custom serializer for
        /// expressions. I wish I was smart enough to build this, but about the extent of my skill set has me copying the code and debugging
        /// through this bad bitch to try and figure out everything it needs. Since a lot of the xml deserialization relies on runtime reflection
        /// of known types, you need to make damn sure every type which is going to be serialized/deserialized is loaded up into this mofo. 
        /// Otherwise, you get to be like me and step through the source code trying to reverse engineer something built by people smarter than you.
        /// </summary>
        /// <returns></returns>
		public static ExpressionSerializer CreateSerializer()
		{
			var assemblies = new Assembly[] { typeof(ValidationRules).Assembly, typeof(ExpressionType).Assembly };
			var resolver = new TypeResolver(assemblies, new Type[] 
			{ 
				typeof(ValidationRules), typeof(ComparableType), typeof(ValidationMessage), typeof(ReportBase), typeof(SurfReport),
                typeof(SnowReport), typeof(Ratings), typeof(Ratings) //yeah, so adding a nullable operator to a custom enum has the compiler inline a new type for you, which isn't accessible conventionally from the compile time known types and assemblies. fun fact!
			});
			//var creator = new QueryCreator();
			//CustomExpressionXmlConverter queryconverter = new QueryExpressionXmlConverter(creator: null, resolver: resolver);
			CustomExpressionXmlConverter knowntypeconverter = new KnownTypeExpressionXmlConverter(resolver);
			//ExpressionSerializer serializer = new ExpressionSerializer(resolver, new CustomExpressionXmlConverter[] { queryconverter, knowntypeconverter });
			ExpressionSerializer serializer = new ExpressionSerializer(resolver, new CustomExpressionXmlConverter[] { knowntypeconverter });
			return serializer;
			//ExpressionSerializer serializer = new ExpressionSerializer()
		}
    }
}
