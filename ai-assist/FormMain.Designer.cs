namespace ai_assist
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxUserPrompt = new System.Windows.Forms.TextBox();
            this.buttonUserPromptSubmit = new System.Windows.Forms.Button();
            this.panelChat = new System.Windows.Forms.Panel();
            this.textBoxChat = new System.Windows.Forms.TextBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelChat.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxUserPrompt
            // 
            this.textBoxUserPrompt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxUserPrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxUserPrompt.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxUserPrompt.Location = new System.Drawing.Point(0, 0);
            this.textBoxUserPrompt.Multiline = true;
            this.textBoxUserPrompt.Name = "textBoxUserPrompt";
            this.textBoxUserPrompt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxUserPrompt.Size = new System.Drawing.Size(709, 62);
            this.textBoxUserPrompt.TabIndex = 1;
            this.textBoxUserPrompt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxUserPrompt_KeyDown);
            // 
            // buttonUserPromptSubmit
            // 
            this.buttonUserPromptSubmit.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonUserPromptSubmit.Location = new System.Drawing.Point(709, 0);
            this.buttonUserPromptSubmit.Name = "buttonUserPromptSubmit";
            this.buttonUserPromptSubmit.Size = new System.Drawing.Size(75, 62);
            this.buttonUserPromptSubmit.TabIndex = 2;
            this.buttonUserPromptSubmit.Text = "Submit";
            this.buttonUserPromptSubmit.UseVisualStyleBackColor = true;
            this.buttonUserPromptSubmit.Click += new System.EventHandler(this.buttonUserPromptSubmit_Click);
            // 
            // panelChat
            // 
            this.panelChat.Controls.Add(this.textBoxChat);
            this.panelChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChat.Location = new System.Drawing.Point(0, 0);
            this.panelChat.Name = "panelChat";
            this.panelChat.Size = new System.Drawing.Size(784, 349);
            this.panelChat.TabIndex = 3;
            // 
            // textBoxChat
            // 
            this.textBoxChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxChat.Font = new System.Drawing.Font("Cascadia Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxChat.Location = new System.Drawing.Point(0, 0);
            this.textBoxChat.Multiline = true;
            this.textBoxChat.Name = "textBoxChat";
            this.textBoxChat.ReadOnly = true;
            this.textBoxChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxChat.Size = new System.Drawing.Size(784, 349);
            this.textBoxChat.TabIndex = 1;
            this.textBoxChat.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.textBoxUserPrompt);
            this.panelBottom.Controls.Add(this.buttonUserPromptSubmit);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 349);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(784, 62);
            this.panelBottom.TabIndex = 4;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.panelChat);
            this.Controls.Add(this.panelBottom);
            this.Name = "FormMain";
            this.Text = "AI Assistant";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panelChat.ResumeLayout(false);
            this.panelChat.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private TextBox textBoxUserPrompt;
        private Button buttonUserPromptSubmit;
        private Panel panelChat;
        private TextBox textBoxChat;
        private Panel panelBottom;
    }
}