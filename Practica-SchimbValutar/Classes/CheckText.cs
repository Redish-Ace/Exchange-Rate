using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_SchimbValutar.Classes
{
    internal class CheckText
    {
       public static bool CheckInt(string text)
        {
            if (text == null)
            {
                return true;
            }
            foreach (char character in text)
            {
                if ((character >= '!' && character <= '-') || (character == '/') || (character >= ':' && character <= '~'))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckString(string text)
        {
            if (text == null)
            {
                return true;
            }
            foreach (char character in text)
            {
                if ((character >= '!' && character <= '-') || (character == '/') || (character >= ':' && character <= '?') || (character >= '[' && character <= '`') || (character == '~'))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
