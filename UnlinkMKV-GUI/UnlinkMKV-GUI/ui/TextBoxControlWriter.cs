using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UnlinkMKV_GUI.ui
{
    public class TextBoxControlWriter : TextWriter
    {
        private readonly Control _textbox;
        public TextBoxControlWriter(Control textbox)
        {
            this._textbox = textbox;
        }
        public override void Write(char value) => _textbox.Text += value;
        public override void Write(string value) => _textbox.Text += value;
        public override Encoding Encoding => Encoding.ASCII;
    }
}