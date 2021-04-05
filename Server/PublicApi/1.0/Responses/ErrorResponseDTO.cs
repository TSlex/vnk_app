﻿using System.Collections.Generic;

namespace PublicApi.v1.Common
{
    public class ErrorResponseDTO
    {
        public string? ErrorMessage { get; set; }
        public IEnumerable<Dictionary<string, string>>? ErrorsKeys { get; set; }
    }
}