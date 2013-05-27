namespace NetworksLab3Server
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.testLabel1 = new System.Windows.Forms.Label();
            this.testLabel2 = new System.Windows.Forms.Label();
            this.finishButton = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.asyncStartButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(13, 13);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // testLabel1
            // 
            this.testLabel1.AutoSize = true;
            this.testLabel1.Location = new System.Drawing.Point(10, 39);
            this.testLabel1.Name = "testLabel1";
            this.testLabel1.Size = new System.Drawing.Size(54, 13);
            this.testLabel1.TabIndex = 1;
            this.testLabel1.Text = "TestLabel";
            // 
            // testLabel2
            // 
            this.testLabel2.AutoSize = true;
            this.testLabel2.Location = new System.Drawing.Point(13, 133);
            this.testLabel2.Name = "testLabel2";
            this.testLabel2.Size = new System.Drawing.Size(60, 13);
            this.testLabel2.TabIndex = 2;
            this.testLabel2.Text = "TestLabel2";
            // 
            // finishButton
            // 
            this.finishButton.Location = new System.Drawing.Point(197, 13);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 3;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(13, 162);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(259, 96);
            this.richTextBox.TabIndex = 4;
            this.richTextBox.Text = "";
            // 
            // asyncStartButton
            // 
            this.asyncStartButton.Location = new System.Drawing.Point(16, 81);
            this.asyncStartButton.Name = "asyncStartButton";
            this.asyncStartButton.Size = new System.Drawing.Size(75, 23);
            this.asyncStartButton.TabIndex = 0;
            this.asyncStartButton.Text = "AsyncStart";
            this.asyncStartButton.UseVisualStyleBackColor = true;
            this.asyncStartButton.Click += new System.EventHandler(this.asyncStartButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.asyncStartButton);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.testLabel2);
            this.Controls.Add(this.testLabel1);
            this.Controls.Add(this.startButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label testLabel1;
        private System.Windows.Forms.Label testLabel2;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Button asyncStartButton;
    }
}

