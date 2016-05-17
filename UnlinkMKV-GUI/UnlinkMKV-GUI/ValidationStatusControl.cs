using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace UnlinkMKV_GUI
{
    public partial class ValidationStatusControl : UserControl
    {
        public ValidationStatusControl()
        {
            InitializeComponent();
        }

        public ValidationStatusControl(string message)
        {
            InitializeComponent();
            labelStatus.Text = message;        
        }

        public void SetStatus(bool status)
        {
            if (status)
                labelStatus.ForeColor = Color.LimeGreen;
            else
                labelStatus.ForeColor = Color.Red;
        }

    }
}
