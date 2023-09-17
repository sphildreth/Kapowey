using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Kapowey.Core.Common.Helpers;

namespace Kapowey.Core.Common.Extensions
{
    public static class StringExt
    {
        private static readonly Dictionary<char, string> UnicodeAccents = new Dictionary<char, string>
        {
            {'À', "A"}, {'Á', "A"}, {'Â', "A"}, {'Ã', "A"}, {'Ä', "Ae"}, {'Å', "A"}, {'Æ', "Ae"},
            {'Ç', "C"},
            {'È', "E"}, {'É', "E"}, {'Ê', "E"}, {'Ë', "E"},
            {'Ì', "I"}, {'Í', "I"}, {'Î', "I"}, {'Ï', "I"},
            {'Ð', "Dh"}, {'Þ', "Th"},
            {'Ñ', "N"},
            {'Ò', "O"}, {'Ó', "O"}, {'Ô', "O"}, {'Õ', "O"}, {'Ö', "Oe"}, {'Ø', "Oe"},
            {'Ù', "U"}, {'Ú', "U"}, {'Û', "U"}, {'Ü', "Ue"},
            {'Ý', "Y"},
            {'ß', "ss"},
            {'à', "a"}, {'á', "a"}, {'â', "a"}, {'ã', "a"}, {'ä', "ae"}, {'å', "a"}, {'æ', "ae"},
            {'ç', "c"},
            {'è', "e"}, {'é', "e"}, {'ê', "e"}, {'ë', "e"},
            {'ì', "i"}, {'í', "i"}, {'î', "i"}, {'ï', "i"},
            {'ð', "dh"}, {'þ', "th"},
            {'ñ', "n"},
            {'ò', "o"}, {'ó', "o"}, {'ô', "o"}, {'õ', "o"}, {'ö', "oe"}, {'ø', "oe"},
            {'ù', "u"}, {'ú', "u"}, {'û', "u"}, {'ü', "ue"},
            {'ý', "y"}, {'ÿ', "y"}
        };

        static Dictionary<string, string> _replacements = new Dictionary<string, string>
        {
            { "o'reilly", "O'Reilly" }
        };

        static string[] _conjunctions = { "Y", "E", "I" };

        static string _romanRegex = @"\b((?:[Xx]{1,3}|[Xx][Ll]|[Ll][Xx]{0,3})?(?:[Ii]{1,3}|[Ii][VvXx]|[Vv][Ii]{0,3})?)\b";

        static Dictionary<string, string> _mcExceptions = new Dictionary<string, string>
        {
            {@"\bMacEdo"     ,"Macedo"},
            {@"\bMacEvicius" ,"Macevicius"},
            {@"\bMacHado"    ,"Machado"},
            {@"\bMacHar"     ,"Machar"},
            {@"\bMacHin"     ,"Machin"},
            {@"\bMacHlin"    ,"Machlin"},
            {@"\bMacIas"     ,"Macias"},
            {@"\bMacIulis"   ,"Maciulis"},
            {@"\bMacKie"     ,"Mackie"},
            {@"\bMacKle"     ,"Mackle"},
            {@"\bMacKlin"    ,"Macklin"},
            {@"\bMacKmin"    ,"Mackmin"},
            {@"\bMacQuarie"  ,"Macquarie"},
            {@"\bMacEy "     ,"Macey "}
        };

        /// <summary>
        /// For the given key add the id and return formatted key to user for Caching
        /// </summary>
        /// <param name="key">Key template</param>
        /// <param name="id">Id for operation</param>
        /// <returns>Formatted key</returns>
        public static string ToCacheKey(this string key, object id) => string.Format(key, id);

        public static string ToFileNameFriendly(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return Regex.Replace(PathSanitizer.SanitizeFilename(input, ' '), @"\s+", " ").Trim();
        }

        public static string ToFolderNameFriendly(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            input = input.Replace("$", "s");
            return Regex.Replace(PathSanitizer.SanitizeFilename(input, ' '), @"\s+", " ").Trim().TrimEnd('.');
        }

        public static string ToTitleCase(this string input, bool doPutTheAtEnd = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            input = input.Replace("’", "'");
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var r = textInfo.ToTitleCase(input.Trim().ToLower());
            r = Regex.Replace(r, @"\s+", " ");
            if (doPutTheAtEnd)
            {
                if (r.StartsWith("The "))
                {
                    r = $"{(r.Replace("The ", string.Empty))}, The";
                }
            }
            return r.NameCase();
        }

        /// <summary>
        /// Case a name field into its appropriate case format 
        /// e.g. Smith, de la Cruz, Mary-Jane,  O'Brien, McTaggart
        /// </summary>
        /// <param name="nameString"></param>
        public static string NameCase(this string nameString, bool doFixConjuntion = false)
        {
            nameString = Capitalize(nameString);
            nameString = UpdateRoman(nameString);
            nameString = UpdateIrish(nameString);
            if (doFixConjuntion)
            {
                nameString = FixConjunction(nameString);
            }
            nameString = Regex.Replace(nameString, @"('[A-Z])", m => m.ToString().ToLower(), RegexOptions.IgnoreCase);
            foreach (var replacement in _replacements.Keys)
            {
                nameString = nameString.Replace(replacement, _replacements[replacement], StringComparison.OrdinalIgnoreCase);
            }
            return nameString;
        }

        /// <summary>
        /// Fix Spanish conjunctions.
        /// </summary>
        /// <param name=""></param>
        private static string FixConjunction(string nameString)
        {
            foreach (var conjunction in _conjunctions)
            {
                nameString = Regex.Replace(nameString, @"\b" + conjunction + @"\b", x => x.ToString().ToLower());
            }
            return nameString;
        }

        /// <summary>
        /// Capitalize first letters.
        /// </summary>
        /// <param name="nameString"></param>
        private static string Capitalize(string nameString)
        {
            nameString = nameString.ToLower();
            nameString = Regex.Replace(nameString, @"\b\w", x => x.ToString().ToUpper());
            nameString = Regex.Replace(nameString, @"'\w\b", x => x.ToString().ToLower()); // Lowercase 's
            return nameString;
        }

        /// <summary>
        /// Update for Irish names.
        /// </summary>
        /// <param name="nameString"></param>
        /// <returns></returns>
        private static string UpdateIrish(string nameString)
        {
            if (Regex.IsMatch(nameString, @".*?\bMac[A-Za-z^aciozj]{2,}\b") || Regex.IsMatch(nameString, @".*?\bMc"))
            {
                nameString = UpdateMac(nameString);
            }
            return nameString;
        }

        /// <summary>
        /// Updates irish Mac & Mc.
        /// </summary>
        /// <param name="nameString"></param>
        /// <returns></returns>
        private static string UpdateMac(string nameString)
        {
            MatchCollection matches = Regex.Matches(nameString, @"\b(Ma?c)([A-Za-z]+)");
            if (matches.Count == 1 && matches[0].Groups.Count == 3)
            {
                string replacement = matches[0].Groups[1].Value;
                replacement += matches[0].Groups[2].Value.Substring(0, 1).ToUpper();
                replacement += matches[0].Groups[2].Value.Substring(1);
                nameString = nameString.Replace(matches[0].Groups[0].Value, replacement);

                // Now fix "Mac" exceptions
                foreach (var exception in _mcExceptions.Keys)
                {
                    nameString = Regex.Replace(nameString, exception, _mcExceptions[exception]);
                }
            }
            return nameString;
        }

        /// <summary>
        /// Fix roman numeral names.
        /// </summary>
        /// <param name="nameString"></param>
        /// <returns></returns>
        private static string UpdateRoman(string nameString)
        {
            MatchCollection matches = Regex.Matches(nameString, _romanRegex);
            if (matches.Count > 1)
            {
                foreach (Match match in matches)
                {
                    if (!string.IsNullOrEmpty(match.Value))
                    {
                        nameString = Regex.Replace(nameString, match.Value, x => x.ToString().ToUpper());
                    }
                }
            }
            return nameString;
        }

        public static string ToAlphanumericName(this string input, bool stripSpaces = true, bool stripCommas = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            input = input.ToLower()
                         .Replace("$", "s")
                         .Replace("%", "per");
            input = WebUtility.HtmlDecode(input);
            input = input.ScrubHtml().ToLower()
                                     .Replace("&", "and");
            var arr = input.ToCharArray();
            arr = Array.FindAll(arr, c => (c == ',' && !stripCommas) || (char.IsWhiteSpace(c) && !stripSpaces) || char.IsLetterOrDigit(c));
            input = new string(arr).RemoveDiacritics().RemoveUnicodeAccents().Translit();
            input = Regex.Replace(input, $"[^A-Za-z0-9{ (!stripSpaces ? @"\s" : string.Empty) }{ (!stripCommas ? "," : string.Empty)}]+", string.Empty);
            return input;
        }

        public static string Left(this String input, int length)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return (input.Length < length) ? input : input.Substring(0, length);
        }

        /// <summary>
        /// Remove extra spaces, remove any HTML, and TitleCase. 
        /// </summary>
        public static string Clean(this string input, bool? doPutTheAtEnd = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            var result = input.ScrubHtml().ToTitleCase(doPutTheAtEnd ?? true).Trim();
            if(string.IsNullOrWhiteSpace(result))
            {
                return null;
            }
            return result;
        }

        public static string ScrubHtml(this string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", string.Empty).Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

        public static string RemoveUnicodeAccents(this string text)
        {
            return text.Aggregate(
                new StringBuilder(),
                (sb, c) =>
                {
                    string r;
                    if (UnicodeAccents.TryGetValue(c, out r))
                    {
                        return sb.Append(r);
                    }

                    return sb.Append(c);
                }).ToString();
        }

        public static string Translit(this string str)
        {
            string[] lat_up =
            {
                "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T",
                "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya"
            };
            string[] lat_low =
            {
                "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t",
                "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya"
            };
            string[] rus_up =
            {
                "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У",
                "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я"
            };
            string[] rus_low =
            {
                "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у",
                "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я"
            };
            for (var i = 0; i <= 32; i++)
            {
                str = str.Replace(rus_up[i], lat_up[i]);
                str = str.Replace(rus_low[i], lat_low[i]);
            }

            return str;
        }

        public static string RemoveDiacritics(this string s)
        {
            var normalizedString = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < normalizedString.Length; i++)
            {
                var c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }
            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static string ToBase64(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string FromBase64(this string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            }
            catch
            {
            }
            return null;
        }

        public static string ToSHA512(this string input, string secret)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            using (var myhmacsha1 = new HMACSHA512(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(input);
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
                }
            }
        }

        public static string ToMD5(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            using (var md5 = MD5.Create())
            {
                return String.Concat(md5.ComputeHash(Encoding.ASCII.GetBytes(input)).Select(x => x.ToString("x2")));
            }
        }
    }
}