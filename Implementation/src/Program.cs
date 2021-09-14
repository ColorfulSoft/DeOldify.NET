//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;
using System.Windows.Forms;

namespace ColorfulSoft.DeOldify
{

    public static class Program
    {

        [STAThread]
        public static void Main()
        {
            DeOldify.Initialize();
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }

    }

}