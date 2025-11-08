using System;
using System.Collections.Generic;

namespace Script.Models
{
    [Serializable]
    public class RequestData
    {
        public string model;
        public string system;
        public string prompt;
        public Options options;
        public bool stream;
        public List<long> context;

        [Serializable]
        public class Options
        {
            public int num_ctx;
        }
    }
}