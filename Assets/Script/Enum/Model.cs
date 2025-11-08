using System;

namespace Script.Enum
{
    public enum Model
    {
        Lama32
    }

    public static class ModelParser
    {
        public static string GetString(this Model model)
        {
            return model switch
            {
                Model.Lama32 => "llama3.2",
                _ => throw new Exception("Invalid LLM model")
            };
        }
    }
}