using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem.Util
{
    public static class GeneralUtil
    {
        public static string EnumerableToString<T>(this IEnumerable<T> arr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            bool empty = true;
            foreach (T obj in arr)
            {
                empty = false;
                sb.Append(obj);
                sb.Append(", ");
            }
            if (!empty) sb.Length -= 2;
            sb.Append(']');
            return sb.ToString();
        }

        public static double SubtractAngles(double a, double b)
        {
            double diff = a - b;
            diff += (diff > Math.PI) ? -2 * Math.PI : (diff < -Math.PI) ? 2 * Math.PI : 0;
            return Math.Abs(diff);
        }

        public static double EuclideanMod(double x, double y)
        {
            double r = Math.Abs(x) % Math.Abs(y);
            r *= Math.Sign(x);
            r = (r + Math.Abs(y)) % Math.Abs(y);
            return r;
        }

        public static double FloorMod(double a, double n)
        {
            return (a % n + n) % n;
        }

        public static T RequireAttribute<T>(Type type) where T : Attribute
        {
            object[] attributesFound = type.GetCustomAttributes(typeof(T), false);
            if (attributesFound.Count() != 1)
            {
                throw new AttributeException(
                    $"Required attribute of class '{typeof(T)}' " +
                    ((attributesFound.Count() == 0) ? "not found" : "found more than once") +
                    $" in derived class '{type}'");
            }
            else return attributesFound.First() as T;
        }

        public static string ToSnakeCase(this string str)
        {
            StringBuilder sb = new StringBuilder();
            bool prevUpper = true;
            foreach(char c in str)
            {
                if (!prevUpper && c.ToString().ToUpper() == c.ToString()) sb.Append("_");
                //prevUpper = false;
                sb.Append(c.ToString().ToLower());
                prevUpper = c.ToString().ToUpper() == c.ToString();
            }
            return sb.ToString();
        }

        public static void SkipWhitespace(string text, ref int index)
        {
            for (; index < text.Length; index++)
            {
                if (char.IsWhiteSpace(text[index]) || text[index] == '\n' || text[index] == '\r') continue;
                else break;
            }
            if (index == text.Length) throw new FormatException("Unexpected end of input");
        }

        public static Tuple<string, int> ParseQuotedString(string text, int index)
        {
            SkipWhitespace(text, ref index);
            char delimiter = text[index];
            if (delimiter != '"' && delimiter != '\'') throw new FormatException("Expected string at index " + index);
            index++;
            StringBuilder sb = new StringBuilder();
            bool escaped = false;
            bool terminated = false;
            for (; index < text.Length; index++)
            {
                char c = text[index];
                if (!escaped)
                {
                    if (c == '\\')
                    {
                        escaped = true;
                        continue;
                    }
                    else if (c == delimiter)
                    {
                        index++;
                        terminated = true;
                        break;
                    }
                }
                else
                {
                    escaped = false;
                    bool unicodeSequence = false;
                    switch (c)
                    {
                        case '0': c = '\0'; break;
                        case 'a': c = '\a'; break;
                        case 'b': c = '\b'; break;
                        case 'f': c = '\f'; break;
                        case 'n': c = '\n'; break;
                        case 'r': c = '\r'; break;
                        case 't': c = '\t'; break;
                        case 'v': c = '\v'; break;
                        case '\\': break;
                        case '\'': break;
                        case '\"': break;
                        case 'u': unicodeSequence = true; break;
                        default: throw new FormatException("Illegal escape sequence");
                    }
                    if (unicodeSequence)
                    {
                        int sequenceLength = 4;
                        if (index + sequenceLength + 1 > text.Length) throw new FormatException("Unexpected end of unicode escape sequence");
                        string sequence = text.Substring(index + 1, sequenceLength);
                        int code;
                        if (!int.TryParse(sequence, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code)) throw new FormatException("Illegal unicode escape sequence");
                        string unicode = char.ConvertFromUtf32(code);
                        sb.Append(unicode);
                        index += sequenceLength;
                        continue;
                    }
                }
                sb.Append(c);
            }
            if (!terminated) throw new FormatException("Unexpected end of input");
            return new Tuple<string, int>(sb.ToString(), index);
        }
    }
    
    [Serializable]
    public sealed class AttributeException : Exception
    {
        public AttributeException() { }
        public AttributeException(string message) : base(message) { }
        public AttributeException(string message, Exception inner) : base(message, inner) { }
    }
}
