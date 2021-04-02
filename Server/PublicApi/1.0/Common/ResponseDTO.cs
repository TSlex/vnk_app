namespace PublicApi.v1.Common
{
    public class ResponseDTO<TKey>
    {
        public string? Message { get; set; }
        public TKey? Data { get; set; }
    }
}