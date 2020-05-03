using System;
using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.SourceCodeService.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts.Syntax;

namespace Speiser.Masterthesis.Analyzer.Extensions
{
    internal static class SyntaxExtensions
    {
        public static bool ContainsVersionString(this CodeAttribute attribute)
            // Note: this obviously only finds v1 version strings
            => attribute != null && attribute.Param.Any(p => p.Contains("v1"));

        public static CodeAttribute GetAttribute(this IEnumerable<CodeAttribute> attributes, string attribute)
            // Note: A controller could have multiple Route Attributes.
            //       https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.1#route-name
            //       This will throw, in such a case.
            => attributes.SingleOrDefault(a => a.Identifier == attribute);

        public static IEnumerable<CodeMethod> GetEndpoints(this CodeClass codeClass)
        {
            var endpointAttributes = new[] { "HttpDelete", "HttpGet", "HttpHead", "HttpOptions", "HttpPatch", "HttpPost", "HttpPut" };
            return codeClass.Methods.Where(method => method.Attributes.Any(attr => endpointAttributes.Contains(attr.Identifier)));
        }

        public static IEnumerable<CodeClass> GetControllers(this SourceProjectRepresentation source) => source.Types.Where(t => t.Base.Contains("ControllerBase") || t.Base.Contains("Controller"));

        public static bool IsAspNetProject(this SourceProjectRepresentation source) => source.GetClass("Startup") != null;

        public static CodeClass GetClass(this SourceProjectRepresentation source, string className)
            => source.Types.SingleOrDefault(t => t.Identifier == className);

        public static CodeMethod GetMethod(this CodeClass codeClass, string methodName)
            => codeClass.Methods.SingleOrDefault(m => m.Identifier == methodName);
    }
}
