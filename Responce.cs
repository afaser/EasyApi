using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace Afaser.EasyApi
{
    public class Responce
    {
        public Socket Socket { get; internal set; }
        public string ContentType { get; set; }
        public ResponceResult ResponceResult { get; set; }
        public IDictionary<string, string> Headers { get; internal set; } = new Dictionary<string, string>();
        internal IDictionary<string, string> Cookies { get; } = new Dictionary<string, string>();
        internal Request Request { get; set; }


        public void SetCookie(
            string key, string value,
            DateTime? expires = null,
            int? maxAge = null,
            string? domain = null,
            string? path = "/",
            bool secure = false,
            bool httpOnly = false)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            var _cookie = new List<string>()
            { 
                $"{key}={value}"
            };

            if (expires != null) 
            {
                if (expires.Value < DateTime.UtcNow)
                    throw new ArgumentOutOfRangeException(nameof(expires));

                var _format = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";
                var _httpTime = expires.Value.ToString(_format, CultureInfo.InvariantCulture);
                _cookie.Add(_httpTime);
            }

            if(maxAge != null)
            {
                if(maxAge <= 0)
                    throw new ArgumentOutOfRangeException(nameof(maxAge));

                _cookie.Add($"Max-Age={maxAge}");
            }

            if(!string.IsNullOrWhiteSpace(domain))
                _cookie.Add($"Domain={domain}");

            if(!string.IsNullOrWhiteSpace(path))
                _cookie.Add($"Path={path}");

            if (secure) _cookie.Add("Secure");

            if (httpOnly) _cookie.Add("HttpOnly");

            Cookies.Add(key, string.Join(": ", _cookie));
        }

        internal async Task Send(string data) =>
            await Send(Encoding.UTF8.GetBytes(data));
        internal async Task Send(byte[] data)
        {
            var _sb = new StringBuilder();

            _sb.AppendLine($"HTTP/1.1 {ResponceResult.Code} {ResponceResult.Message.ToUpper()}");

            _sb.AppendLine($"Content-type: text/json");
            _sb.AppendLine($"Content-Length: {data.Length}");

            foreach (var _header in Headers)
                _sb.AppendLine($"{_header.Key}: {_header.Value}");

            foreach (var _cookie in Cookies)
                _sb.AppendLine($"Set-Cookie: {_cookie}");

            _sb.Append("\n\n");

            var _bytes = Encoding.UTF8.GetBytes(_sb.ToString());

            await Socket.SendAsync(_bytes);

            await Socket.SendAsync(data);
        }
    }
}
