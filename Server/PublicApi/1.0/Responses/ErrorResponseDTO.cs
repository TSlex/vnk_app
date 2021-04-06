using System.Collections.Generic;

namespace PublicApi.v1.Common
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