using System;

namespace Script.Models
{
    [Serializable]
    public class AgentResponse
    {
        public string answer;
        public string action;
    }

    [Serializable]
    public class AgentResponseJSON
    {
        public string answer;
        public string[] flags;
    }
}