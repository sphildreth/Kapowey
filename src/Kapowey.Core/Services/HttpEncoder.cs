using System.Text;
using System.Web;
using Kapowey.Core.Common.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace Kapowey.Core.Services
{
    public class HttpEncoder : IHttpEncoder
    {
        public string HtmlEncode(string s) => HttpUtility.HtmlEncode(s);

        public string UrlDecode(string s) => HttpUtility.UrlDecode(s);

        public string UrlEncode(string s) => HttpUtility.UrlEncode(s);

        public string UrlEncodeBase64(byte[] input) => WebEncoders.Base64UrlEncode(input);

        public string UrlEncodeBase64(string input) => WebEncoders.Base64UrlEncode(Encoding.ASCII.GetBytes(input));
    }
}