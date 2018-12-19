namespace BinaryMan.Azure
{
    using Core.Schema;
    using Newtonsoft.Json;
    using StorageMate.Core.Utils;
    using System;
    using System.Linq.Expressions;

    internal static class BinaryInfoExtensions
    {
        public static string GetIdKey<TBinaryInfo>(this TBinaryInfo binaryInfo, Expression<Func<TBinaryInfo, object>> selector) where TBinaryInfo : BinaryInfo, new()
        {
            var bodyExp = GetMemberExpBody(selector);
            var val = selector.Compile().Invoke(binaryInfo);
            var valStr = val is string s ? s : JsonConvert.SerializeObject(val);

            var partitionKey =
                $"{typeof(TBinaryInfo)}_{bodyExp.Member.Name}_{HashUtil.ComputeMd5Hash(valStr)}";

            return partitionKey;
        }


        private static MemberExpression GetMemberExpBody(LambdaExpression expression)
        {
            if (!(expression.Body is MemberExpression body))
            {
                throw new ArgumentException("'expression' should be a member expression");
            }

            return body;
        }
    }
}
