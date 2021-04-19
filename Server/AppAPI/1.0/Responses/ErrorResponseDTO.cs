namespace AppAPI._1._0.Responses
{
    public class ErrorResponseDTO
    {
        public string? Error { get; set; }

        public ErrorResponseDTO()
        {
            
        }
        
        public ErrorResponseDTO(string? error)
        {
            Error = error;
        }
    }
}