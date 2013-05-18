using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworksLab3Server
{
    public partial class Form1 : Form
    {
        private Classes.ServerRun sr;

        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // TODO: create server and begin listening
            sr = new Classes.ServerRun(richTextBox);
            string nicTest = sr.Start();

            richTextBox.Text = nicTest;

            #region exampleStringResponseBuilder
            //string stuff = "REQ|" + "msTime"
            //    + "|RequestNo:" + "i" + "|WurdingerO|19-3410|" + "0|" + "ip.ToString()"
            //    + "|" + "portNum" + "|" + "sock.Handle.ToString()" + "|" + "serverIP" + "|"
            //    + "serverPort" + "|StudentData:" + "i" + "|1|";

            //string[] answer = stuff.Split('|');

            // This is some testing code to test message build
            //Classes.ResponseBuilder rb = new Classes.ResponseBuilder(stuff);
            //testLabel.Text = stuff;
            //string test = System.Text.Encoding.ASCII.GetString(rb.Response(1));
            //testLabel2.Text = test;
            #endregion
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            testLabel1.Text = "Finish Clicked 1";
            testLabel2.Text = "Finish Clicked 2";

            richTextBox.Text = sr.CloseServer();

            // TODO: kill processes then close window
            //Form1.ActiveForm.Close();
        }
    }
}
