namespace Afaser.EasyApi.Objects
{
    public struct ResponceResult
    {
        public static ResponceResult Ok = new ResponceResult() { Code = 200, Message = "OK" };
        public static ResponceResult NotFound = new ResponceResult() { Code = 404, Message = "NOT-FOUND" };
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
