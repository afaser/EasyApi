using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace Afaser.EasyApi
{
    public class Responce
    {
        public Socket Socket { get; }
        public string ContentType { get; set; } = "text/json";
        public ResponceResult ResponceResult { get; set; }
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, Cookie> Cookies => _cookies.AsReadOnly();

        internal Request _request;

        private IDictionary<string, Cookie> _cookies;

        internal Responce(Request request)
        {
            _request = request;
            Socket = _request.Socket;
            ResponceResult = ResponceResult.Ok;
            _cookies = new Dictionary<string, Cookie>(_request.Cookies);
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

            foreach (var _cookie in Cookies.Values)
            {
                if(_request.Cookies.TryGetValue(_cookie.Key, out var _requestCookie))
                {
                    if (_requestCookie.Equals(_cookie)) continue;
                }

                var _str = _cookie.ToString();
                _sb.AppendLine($"Set-Cookie: {_cookie}");
                Console.WriteLine(_str);
            }

            _sb.Append("\n");

            var _bytes = Encoding.UTF8.GetBytes(_sb.ToString());

            await Socket.SendAsync(_bytes);

            await Socket.SendAsync(data);
        }
        public void SetCookie(Cookie cookie)
        {
            if (_cookies.ContainsKey(cookie.Key))
                _cookies[cookie.Key] = cookie;
            else _cookies.Add(cookie.Key, cookie);
        }
    }
}
