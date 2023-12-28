using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Afaser.EasyApi.Transport
{
    public sealed class HttpServer : IDisposable
    {
        private EndPoint _ep;
        private bool _active;
        private Socket _listener;
        private volatile CancellationTokenSource _cts;

        private Func<Request, Responce, Task<object>> _reqestHandler;
        public HttpServer(string ip, int port)
        {
            _ep = new IPEndPoint(IPAddress.Parse(ip), port);
            _cts = new CancellationTokenSource();
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public async Task Run()
        {
            if (!_active)
            {
                _listener.Bind(_ep);
                _listener.Listen(16);
                _active = true;

                while (_active || !_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        Socket _listenerAccept = await _listener.AcceptAsync();

                        if (_listenerAccept != null)
                            Task.Factory.StartNew(async () => await ProcessClient(_listenerAccept), _cts.Token);
                    }
                    catch { }
                }
            }
            else
            {
                Console.WriteLine("Server was started");
            }
        }

        public void SetReqestHandler(Func<Request, Responce, Task<object>> reqestHandler) =>
            _reqestHandler = reqestHandler;
        public void SetReqestHandler(Func<Request, Responce, object> reqestHandler) =>
            _reqestHandler = (req, resp) => Task.FromResult(reqestHandler(req, resp));
        public void SetReqestHandler(Action<Request, Responce> reqestHandler) =>
            _reqestHandler = (req, resp) =>
            {
                reqestHandler(req, resp);
                return Task.FromResult<object>(null);
            };
        public void SetReqestHandler(Func<Request, Responce, Task> reqestHandler) =>
            _reqestHandler = async (req, resp) =>
            {
                await reqestHandler(req, resp);
                return Task.FromResult<object>(null);
            };

        public void Dispose()
        {
            _active = false;
            _cts.Cancel();
        }

        private async Task ProcessClient(Socket client)
        {
            try
            {
                var _request = await GetRequest(client);

                if (_request == null) return;

                var _responce = new Responce(_request)
                {
                    ResponceResult = ResponceResult.Ok
                };

                object _data = null;

                if(_reqestHandler != null)
                    _data = await _reqestHandler(_request, _responce);

                if (_data is string _stringData)
                    await _responce.Send(_stringData);
                else if (_data is byte[] _byteData)
                    await _responce.Send(_byteData);
                else
                {
                    if(_reqestHandler == null)
                        _responce.ResponceResult = ResponceResult.NotFound;

                    await _responce.Send(new byte[0]);
                }

                client.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex); client.Dispose(); }
        }
        private async Task<Request> GetRequest(Socket client)
        {
            var _bytes = new byte[client.ReceiveBufferSize];
            var _length = await client.ReceiveAsync(_bytes);
            var _request = Encoding.UTF8.GetString(_bytes, 0, _length);
            if (string.IsNullOrWhiteSpace(_request)) return null;
            var _data = _request.Split('\n');

            var _meta = _data[0].Split(' ');
            var _method = _meta[0];
            var _url = _meta[1].Split('?');
            var _ep = _url[0];
            if (_ep.Last() != '/') _ep += '/';
            var _queue = new Dictionary<string, string>(_url.Length == 2 ? _url[1].Split('&').Select(x =>
            {
                var _parts = x.Split('=');
                return KeyValuePair.Create(_parts[0], _parts[1]);
            }) : new KeyValuePair<string, string>[0]);
            var _headers = new Dictionary<string, string>(_data.Skip(1).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x =>
            {
                var _key = new string(x.TakeWhile(s => s != ':').ToArray());
                var _value = new string(x.Skip(_key.Length + 2).ToArray());
                return KeyValuePair.Create(_key, _value);
            }));
            Dictionary<string, Cookie> _cookies;
            if(_headers.Remove("Cookie", out var _cookiesString))
            {
                Console.WriteLine(_cookiesString);

                var _cookiesStrings = _cookiesString.Split(';').Select(x => x.Trim());

                var _cookiesList = _cookiesStrings.Select(x =>
                {
                    var _parts = x.Split('=');
                    var _cookie = new Cookie()
                    {
                        Key = _parts[0],
                        Value = _parts[1]
                    };
                    return KeyValuePair.Create(_cookie.Key, _cookie);
                });

                _cookies = new Dictionary<string, Cookie>(_cookiesList);
            }
            else _cookies = new Dictionary<string, Cookie>();

            return new Request()
            {
                Socket = client,
                Method = _method,
                Endpoint = _ep,
                Queue = _queue,
                Headers = _headers,
                Cookies = _cookies
            };

        }
    }
}