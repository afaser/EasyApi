
using Afaser.EasyApi.Objects;

namespace Afaser.EasyApi.Services
{
    public sealed class MapperService : Service, IRequestsHandler
    {
        private Dictionary<string, Func<Request, Responce, Task<object>>> _handlers = 
            new Dictionary<string, Func<Request, Responce, Task<object>>>();

        public MapperService Map(string method, string endpoint, Func<Request, Responce, Task<object>> handler)
        {
            var _key = $"{method.ToUpper()} {endpoint.ToLower()}";

            _handlers.Add(_key, handler);

            return this;
        }
        public MapperService Map(string method, string endpoint, Func<Request, Responce, object> handler)
        {
            Map(method, endpoint, (req, resp) =>
            {
                return Task.FromResult(handler(req, resp));
            });

            return this;
        }
        public async Task<object> HandleRequest(Request reqest, Responce responce)
        {
            var _key = $"{reqest.Method.ToUpper()} {reqest.Endpoint.ToLower()}";

            if (_handlers.TryGetValue(_key, out var _handler))
                return await _handler(reqest, responce);

            else
            {
                responce.ResponceResult = ResponceResult.NotFound;
                return Task.FromResult<object>(new byte[0]);
            }
        }
    }
}
