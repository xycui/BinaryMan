using System.Text;

namespace BinaryMan.Azure
{
    using System.Collections.Generic;

    internal class NamingUtil
    {
        private static readonly ISet<char> AcceptCharSet = new HashSet<char>();

        static NamingUtil()
        {
            for (var i = 'a'; i <= 'z'; i++)
            {
                AcceptCharSet.Add(i);
            }

            for (var i = 'A'; i <= 'Z'; i++)
            {
                AcceptCharSet.Add(i);
            }

            for (var i = '0'; i < '9'; i++)
            {
                AcceptCharSet.Add(i);
            }

            AcceptCharSet.Add('-');
        }

        public static string CamelCase2Dash(string camelInput)
        {
            var strBuilder = new StringBuilder();
            var lowerStr = camelInput.ToLower();
            for (var i = 0; i < camelInput.Length; i++)
            {
                var tobeAppend = camelInput[i] == lowerStr[i] && AcceptCharSet.Contains(camelInput[i]) || i == 0
                    ? lowerStr[i] + ""
                    : "-" + lowerStr[i];
                strBuilder.Append(tobeAppend);
            }

            return strBuilder.ToString();
        }

        public static string Dash2CamelCase(string dashStyleInput)
        {
            var strBuilder = new StringBuilder();
            dashStyleInput = dashStyleInput.Trim('-').ToLower();
            var upper = dashStyleInput.ToUpper();
            for (var i = 0; i < dashStyleInput.Length; i++)
            {
                var cur = i;
                while (dashStyleInput[i] == '-' && i + 1 < dashStyleInput.Length && dashStyleInput[i + 1] == '-')
                {
                    i++;
                }

                var tobeAppend = cur == i ? "" + dashStyleInput[i] : upper[i] + "";
                strBuilder.Append(tobeAppend);
            }

            return strBuilder.ToString();
        }
    }
}
