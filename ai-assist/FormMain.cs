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

        private void buttonUserPromptSubmit_Click(object sender, EventArgs e)
        {
            SubmitUserPromptAsync();
        }

        private void SubmitUserPromptAsync()
        {
            var userPrompt = textBoxUserPrompt.Text.Trim();
            textBoxUserPrompt.Clear();

            if (_openAIClient != null && userPrompt != "")
            {
                // Append user's message on UI thread
                textBoxChat.AppendText("[User]\r\n" + userPrompt + " \r\n\r\n[Assistant]\r\n");

                // Run streaming in a separate thread
                Thread thread = new Thread(async () =>
                {
                    try
                    {
                        await foreach (var chunk in _openAIClient.ChatCompletionStreamAsync(userPrompt))
                        {
                            var formatted = chunk.Replace("\r", "").Replace("\n", "\r\n");

                            // Update UI from the thread
                            this.Invoke((Action)(() =>
                            {
                                textBoxChat.AppendText(formatted);
                                textBoxChat.SelectionStart = textBoxChat.Text.Length;
                                textBoxChat.ScrollToCaret();
                            }));
                        }

                        // Append separator line
                        this.Invoke((Action)(() =>
                        {
                            textBoxChat.AppendText("\r\n\r\n================================\r\n\r\n");
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((Action)(() =>
                        {
                            textBoxChat.AppendText($"\r\n[Error] {ex.Message}\r\n");
                        }));
                    }
                });

                thread.IsBackground = true;
                thread.Start();
            }
        }

        private async void textBoxUserPrompt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (e.Shift)
                {
                    // Shift+Enter = insert newline
                    // Let the TextBox handle it normally (do nothing)
                }
                else
                {
                    // Enter alone = submit
                    e.SuppressKeyPress = true; // prevent newline

                    // Call submit method
                    SubmitUserPromptAsync();
                }
            }
        }
    }
}