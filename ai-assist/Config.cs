namespace ai_assist
{
    public class Config
    {
        public string OpenAIApiKey { get; set; } = "";
        public string OpenAIModel { get; set; } = "gpt-5-chat-latest";
        public int OpenAIMaxContextTokens { get; set; } = 200000;
    }
}
