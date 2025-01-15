using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public static class CommandUtility
    {
        public static string[] ParseArgs(string command, int argNumber)
        {
            var split = command.Split(' ');
            var cmd = split[0];
            var args = command.Substring(cmd.Length).Trim();
            var argArray = new string[argNumber];
            for (var i = 0; i < argNumber; i++)
                argArray[i] = "";

            var inQuotes = false;
            var ignoreSpecialChar = false;
            var parsedArgs = 0;
            var currentArg = "";
            for(var i = 0; i < args.Length; i++)
            {
                var ch = args[i];
                if (ignoreSpecialChar)
                {
                    currentArg += ch;
                    ignoreSpecialChar = false;
                }
                else if (ch == ' ')
                {
                    if (parsedArgs == argNumber - 1)
                    {
                        currentArg += ch;
                    }
                    else
                    {
                        if (!inQuotes)
                        {
                            if (!string.IsNullOrWhiteSpace(currentArg))
                            {
                                argArray[parsedArgs] = currentArg.Trim();
                                currentArg = "";
                                parsedArgs++;
                            }
                        }
                        else
                            currentArg += ch;
                    }
                }
                else if (ch == '"')
                {
                    if (inQuotes)
                    {
                        if (!string.IsNullOrWhiteSpace(currentArg))
                        {
                            argArray[parsedArgs] = currentArg;
                            currentArg = "";
                            parsedArgs++;
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        inQuotes = true;
                    }
                }
                else if (ch == '\\')
                {
                    ignoreSpecialChar = true;
                }
                else
                {
                    currentArg += ch;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentArg) && parsedArgs < argNumber)
                argArray[parsedArgs] = currentArg.Trim();

            return argArray;
        }
    }
}
