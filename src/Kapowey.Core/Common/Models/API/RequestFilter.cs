using System.Text.RegularExpressions;
using Kapowey.Core.Common.Extensions;

namespace Kapowey.Core.Common.Models.API
{
    public enum RequestFilterAndOr
    {
        NotSet,
        And,
        Or
    }

    [Serializable]
    public sealed class RequestFilter
    {
        public RequestFilterAndOr AndOr { get; set; } = RequestFilterAndOr.And;

        public string Operation { get; set; } = "EqualTo";

        public string Prop { get; set; }

        public object Value { get; set; }

        public object Value2 { get; set; }

        public bool IsBetweenFilter => Value != null && Value2 != null;

        public bool IsStartsWithFilter => String.Equals(Operation, "StartsWith", StringComparison.OrdinalIgnoreCase) ||
                                    String.Equals(Operation, "Starts", StringComparison.OrdinalIgnoreCase);

        public bool IsEndsWithFilter => String.Equals(Operation, "EndsWith", StringComparison.OrdinalIgnoreCase) ||
                                    String.Equals(Operation, "Ends", StringComparison.OrdinalIgnoreCase);

        public bool IsLikeFilter => String.Equals(Operation, "like", StringComparison.OrdinalIgnoreCase) ||
                                    String.Equals(Operation, "Contains", StringComparison.OrdinalIgnoreCase);

        public RequestFilter()
        {
        }

        public string FilterSql()
        {
            if(IsBetweenFilter)
            {
                return $"{ CleanValue(Prop, false) } >= { CleanValue(Value)} AND { Prop } <= { CleanValue(Value2) }";
            }
            if(IsStartsWithFilter)
            {
                return $"{ CleanValue(Prop, false) }.ToUpper().StartsWith(\"{ CleanValue(Value, false)?.ToUpper()}\")";
            }
            if (IsEndsWithFilter)
            {
                return $"{ CleanValue(Prop, false) }.ToUpper().EndsWith(\"{ CleanValue(Value, false)?.ToUpper()}\")";
            }
            if (IsLikeFilter)
            {
                return $"{ CleanValue(Prop, false) }.ToUpper().Contains(\"{ CleanValue(Value, false)?.ToUpper()}\")";
            }
            return $"{ CleanValue(Prop, false) } { OperationToSql(Operation) } { CleanValue(Value) }";
        }

        public static string CleanValue(object value, bool doQuote = true)
        {
            if (value == null)
            {
                return "";
            }
            if(Regex.IsMatch(value.ToString(), @"\;.*"))
            {
                throw new RequestException("Filter parameters are invalid.");
            }
            var result = value.ToString();
            if (value.IsNumber())
            {
                return result;
            }
            if(!doQuote)
            {
                return result;
            }
            return $"\"{ result}\"";
        }

        public static string OperationToSql(string operation)
        {
            if (string.IsNullOrWhiteSpace(operation))
            {
                return "";
            }
            switch (operation.ToLower())
            {
                case "equals":
                case "equalto":
                    return "==";

                case "not":
                case "notequals":
                case "notequalto":
                    return "<>";

                case "less":
                case "lessthan":
                    return "<";

                case "greater":
                case "greaterthan":
                    return ">";
            }
            throw new NotImplementedException();
        }

        public override string ToString() => $"AndOr [{ AndOr }] Operation [{ Operation }] Prop [{ Prop }] Value [{ Value }]";
    }
}