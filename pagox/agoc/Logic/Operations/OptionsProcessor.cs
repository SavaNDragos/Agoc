using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Agoc.Logic.Operations
{
    public static class OptionsProcessor
    {
        public static Dictionary<string, string> GetEnvironmentFromParameters(string inComamnd)
        {
            var forReturn = new Dictionary<string,string>();

            if (!string.IsNullOrWhiteSpace(inComamnd))
            {
                var resultedValues = inComamnd.Split(new string[] {";"}, StringSplitOptions.None).ToList();
                foreach (var resultedValue in resultedValues)
                {
                    var tempKeyAndValue = resultedValue.Split(new string[] {":"}, StringSplitOptions.None).ToList();
                    if (tempKeyAndValue.Count != 0 && tempKeyAndValue.Count != 2)
                    {
                        throw new ArgumentException("The environmentParameters are incorrectly configurated;");
                    }
                    forReturn.Add(tempKeyAndValue[0], tempKeyAndValue[1]);
                }
            }

            return forReturn;
        }
    }
}
