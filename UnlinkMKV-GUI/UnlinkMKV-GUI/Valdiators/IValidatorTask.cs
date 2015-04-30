using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnlinkMKV_GUI.Valdiators
{
    public interface IValidatorTask
    {

        /// <summary>
        /// Returns a status text string representing this task
        /// </summary>
        /// <returns></returns>
        string GetStatusText();

        /// <summary>
        /// Performs a check to see if the requirement is possible
        /// </summary>
        /// <returns></returns>
        bool IsRequirementMet();

        /// <summary>
        /// Attempts to fix a broken requirement test when possible.
        /// </summary>
        /// <returns>This will return true when it succeeds and false otherwise.</returns>
        bool AttemptFixRequirement();
    }


    // For simplicity sake, we'll just put all the validators in their own file

}
