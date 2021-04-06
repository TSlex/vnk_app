namespace PublicApi.v1.Common
{
    public class ResponseDTO<TKey>
    {
        public TKey? Data { get; set; }

        public ResponseDTO()
        {
            
        }

        public ResponseDTO(TKey? data)
        {
            Data = data;
        }
    }
}