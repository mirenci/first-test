namespace Anyo.WindowsForms.CommonFunctionality.UI
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// This class provides the functionality to clear controls
    /// </summary>
    public class Clear
    {
        /// <summary>
        /// Clear all TeztBoxes and ComboBoxes in specific container like Panels,GroupeBox,etc.
        /// </summary>
        /// <param name="containerControl">Name of the container control</param>
        public static void AllTextBoxesAndComboBoxes(Control containerControl)
        {
            foreach (Control c in containerControl.Controls)
            {
                if (c is ComboBox)
                {
                    c.Text = null;
                }
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }


            }
        }

        /// <summary>
        /// Clear all TextBoxes in specific container like Panels,GroupeBox,etc.
        /// </summary>
        /// <param name="controlContainerName">Name of the container control</param>
        public static void TextBoxesInSpecificContainer(Control controlContainerName)
        {
            foreach (Control c in controlContainerName.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }
            }
        }

        /// <summary>
        /// Clear all ComboBoxes in specific container like Panels,GroupeBox,etc.
        /// </summary>
        /// <param name="controlContainerName">Name of the container control</param>
        public static void ComboBoxesInSpecificContainer(Control controlContainerName)
        {
            foreach (Control c in controlContainerName.Controls)
            {
                if (c is ComboBox)
                {
                    c.Text = null;
                }
            }
        }

        // Makes no sense methods
        /*public static void TextBoxesInSpecificForm(Form formName)
        {
            foreach (Control c in formName.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }
            }
        }*/

        /*public static void ComboBoxesInSpecificForm(Form formName)
        {
            foreach (Control c in formName.Controls)
            {
                if (c is ComboBox)
                {
                    c.Text = null;
                }
            }
        }*/

        /*public static void TextBox(Control textboxName)
        {
            ((TextBox)textboxName).Clear();
        }*/

        /*public static void ComboBox(Control comboboxName)
        {
            ((ComboBox)comboboxName).Text=null;
        }*/
    }
}
