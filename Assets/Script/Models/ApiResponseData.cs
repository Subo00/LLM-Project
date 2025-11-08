using System;
using System.Collections.Generic;

namespace Script.Models
{
    [Serializable]
    public class ApiResponseData
    {
        public string response;
        public List<long> context;
    }
}