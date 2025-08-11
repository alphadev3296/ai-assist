namespace ai_assist
{
    public partial class FormMain : Form
    {
        private Config _config = new Config();
        private OpenAIClient _openAIClient;
        public FormMain()
        {
            InitializeComponent();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            _config = await JsonConfig<Config>.LoadAsync("settings.json");
            _openAIClient = new OpenAIClient(_config.OpenAIApiKey, _config.OpenAIModel, _config.OpenAIMaxContextTokens);
        }

        private async void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            await JsonConfig<Config>.SaveAsync("settings.json", _config);
        }

        private async void buttonUserPromptSubmit_Click(object sender, EventArgs e)
        {
            var userPrompt = textBoxUserPrompt.Text.Trim();
            textBoxUserPrompt.ResetText();

            if (_openAIClient != null && userPrompt != "")
            {
                textBoxChat.AppendText("[User]\r\n" + userPrompt + " \r\n\r\n[Assistant]\r\n");
                await foreach (var chunk in _openAIClient.ChatCompletionStreamAsync(userPrompt))
                {
                    textBoxChat.AppendText(chunk);
                    textBoxChat.SelectionStart = textBoxChat.Text.Length;
                    textBoxChat.ScrollToCaret();

                }
                textBoxChat.AppendText("\r\n\r\n");
            }
        }
    }
}