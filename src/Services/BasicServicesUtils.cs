namespace Afaser.EasyApi.Services
{
    public static class BasicServicesUtils
    {
        public static MapperService AddMapperService(this Api api) 
        { 
            var _mapperService = new MapperService();
            api.AddService(_mapperService);
            return _mapperService;
        }
    }
}
