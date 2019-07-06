using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
    {
    public class CommonDialogFunctions
        {
        public static void AssignAccessibleNames ( Control.ControlCollection   inputControlsList)
            {
            // commented for release 1.0
            /*
                                    if (inputControlsList != null && inputControlsList.Count > 0)
                {
                Control[] controlsList = new Control[inputControlsList.Count];

                foreach (Control c in inputControlsList)
                    {
                    if (c != null)
                        controlsList[c.TabIndex] = c;
                    }

                for (int i = 1; i < controlsList.Length ; i++)
                    {
                    if (controlsList[i] != null && controlsList[i - 1] != null)
                        {
                        if (controlsList[i].TabIndex != controlsList[i - 1].TabIndex + 1)
                            MessageBox.Show ( "Tab index not in order" );

                        if ((controlsList[i] is TextBox || controlsList[i] is NumericUpDown || controlsList[i] is ComboBox || controlsList[i] is MaskedTextBox)
                            && controlsList[i - 1] is Label)
                            {
                                                        controlsList[i].AccessibleName = controlsList[i - 1].Text.Replace ( "&", "" );
                                                        }
                        }

                    }


                }
             */ 
            }

        }
    }
