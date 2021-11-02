//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;
using System.Windows.Forms;

namespace ColorfulSoft.DeOldify
{

    /// <summary>
    /// Main class.
    /// </summary>
    public static class Program
    {

        /// <summary>
        /// Entry point.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            DeOldify.Initialize();
            Application.EnableVisualStyles();
            try
            {
                Application.Run(new MainForm());
            }
            catch
            {
            }
        }

    }

}
