using Afaser.EasyApi.Objects;

namespace Afaser.EasyApi.Services
{
    public interface IRequestsHandler
    {
        public Task<object> HandleRequest(Request reqest, Responce responce);
    }
}
