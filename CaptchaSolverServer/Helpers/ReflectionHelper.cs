using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CaptchaSolverServer.Helpers
{
    public static class ReflectionHelper
    {
        public static IEnumerable<MethodInfo> FindMethods<T>(Type returnType, IEnumerable<Type> args) where T : class
        {
            Type typeOfT = typeof(T);
            var methodInfoCollection = typeOfT.GetMethods();
            return methodInfoCollection.Where(method =>
                method.ReturnType.Name == returnType?.Name &&
                method.GetParameters().Select(p => p.ParameterType.Name)
                    .SequenceEqual(args?.Select(x => x.Name)));
        }

        public static string GetGuidApplication() => 
            ((GuidAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute), false)).Value;

        public static string GetVersionApplication() => Assembly.GetEntryAssembly().GetName().Version.ToString();

    }
}
