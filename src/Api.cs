using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Afaser.EasyApi.Services;
using Afaser.EasyApi.Transport;

namespace Afaser.EasyApi
{
    public class Api
    {
        private bool _isTreminating = false;

        private List<Service> _services = new List<Service>();
        private IRequestsHandler? _reqestsHandler;

        public async Task RunAsync()
        {
            using (var _server = new HttpServer("127.0.0.1", 80))
            {
                _server.SetReqestHandler((req, resp) =>
                {
                    return _reqestsHandler?.HandleRequest(req, resp);
                });
                await _server.Run();
            }
        }
        public void AddService(Service service, bool ignoreHandler = false)
        {
            if(_services.Where(x => x.GetType() == service.GetType()).Count() > 0)
                throw new InvalidOperationException("Невозможно зарегестрировать сервис т.к. сервис этого типа уже зарегестрирован");

            if (service is IRequestsHandler handler && !ignoreHandler)
            {
                if (_reqestsHandler != null)
                    throw new InvalidOperationException("Невозможно зарегестрировать сервис т.к. слушатель запросов уже зарегестрирован");

                _reqestsHandler = handler;
            }

            _services.Add(service);
        }
        public void GetService<T>() where T : Service => _services.OfType<T>().Single();
        public void RemoveService(Service service) => 
            _services.Remove(service);
        public void Run() => RunAsync().GetAwaiter().GetResult();
    }
}
