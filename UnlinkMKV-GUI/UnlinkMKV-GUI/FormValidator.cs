using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnlinkMKV_GUI.Valdiators;

namespace UnlinkMKV_GUI
{
    public partial class FormValidator : Form
    {
        public FormValidator()
        {
            InitializeComponent();
            
            // We're not too concerned with threading manipulations here, so we won't worry about it here
            CheckForIllegalCrossThreadCalls = false;

        }

        private void CheckRequirements()
        {
            var validatorTasks = new List<IValidatorTask>();
            validatorTasks.Add(new IsAdministratorValidatorTask());
            validatorTasks.Add(new MkvToolNixValidatorTask());;
            validatorTasks.Add(new PerlExistsValidatorTask());

            foreach (var task in validatorTasks)
            {
                var statusControl = new ValidationStatusControl(task.GetStatusText());
                flowLayoutPanel1.Controls.Add(statusControl);

                bool requirementMet = task.IsRequirementMet();

                if (!requirementMet)
                {
                    bool wasFixed = task.AttemptFixRequirement();

                    if (!wasFixed)
                    {
                        Application.Exit();    
                    }
                }
                    
                // Otherwise, okay move onto the next
                statusControl.SetStatus(true);

                Application.DoEvents();
            }

            Close();

        }

        private void FormValidator_Load(object sender, EventArgs e)
        {
 
        }

        private void FormValidator_Shown(object sender, EventArgs e)
        {
            CheckRequirements();
        }

    }
}
