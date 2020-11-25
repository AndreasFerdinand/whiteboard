namespace wssserver
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    public static class StringExtension
    {
        public static string ReplaceSymbols(this string str)
        {
            string tempstr = str;

            var symbols = new Dictionary<string,string>()
            {
                {":-)","&#128512;"},
                {":-D","&#128513;"},
                {":UP:","&#128077;"},
                {":DOWN:","&#128078;"},
                {":INHERIT:","&#8701;"},
                {":COFFEE:","&#9749;"},
                {":ROCKET:","&#128640;"},
                {":1ST:","&#129351;"},
                {":2ND:","&#129352;"},
                {":3RD:","&#129353;"},
                {":RAISEHAND:","&#9995;&#127999;"}
            };

            foreach (KeyValuePair<string,string> kvp in symbols)
            {
                tempstr = tempstr.Replace(kvp.Key,kvp.Value);

                Console.WriteLine(tempstr + ": " + kvp.Key + " " + kvp.Value);
            }

            return tempstr;
        }
    }
}