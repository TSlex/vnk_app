namespace AppAPI._1._0.Responses
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