using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UnlinkMKV_GUI.ui
{
    public class TextBoxControlWriter : TextWriter
    {
        private readonly Control _textbox;
        private readonly Form _owner;

        public TextBoxControlWriter(Form owner, Control textbox)
        {
            this._textbox = textbox;
            this._owner = owner;
        }
        public override void Write(char value)
        {
            ExecuteSecure(() => _textbox.Text += value);
        }

        public override void Write(string value)
        {
            ExecuteSecure(() => _textbox.Text += CleanEscape(value));
        }

        public override Encoding Encoding => Encoding.ASCII;

        private string CleanEscape(string inputString) {
            return Regex.Replace(inputString, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
        }

        private void ExecuteSecure(Action a) {
            if (_owner.InvokeRequired)
                _owner.BeginInvoke(a);
            else
                a();
        }

    }
}