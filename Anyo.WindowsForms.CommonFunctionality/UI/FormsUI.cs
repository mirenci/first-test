namespace Anyo.WindowsForms.CommonFunctionality.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Drawing;
    using Anyo.WindowsForms.Controls.Buttons;
    using Anyo.WindowsForms.Controls.Panels;

    /// <summary>
    /// Specifies nesting level of the child form
    /// </summary>
    public enum ChildNastedLevel
    {
        Nasted,
        NastedNasted,
        NastedNastedNasted,
    }

    /// <summary>
    /// This class provides the UI utilities
    /// </summary>
    public class FormsUI
    {
        /// <summary>
        /// Disable sorting of all columns in specific DataGridView object
        /// </summary>
        /// <param name="dataGridViewName">DataGridView object name</param>
        public static void DisableSorting(DataGridView dataGridViewName)
        {
            foreach (DataGridViewColumn column in dataGridViewName.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// Set Background color of MDI parent form
        /// </summary>
        /// <param name="color">Color name</param>
        /// <param name="controls">MDI parent object</param>
        public static void SetBackGroundColorOfMDIForm(Color color, Control.ControlCollection controls)
        {
            foreach (Control ctl in controls)
            {
                if ((ctl) is MdiClient)

                // If the control is the correct type, 
                // change the color.
                {
                    ctl.BackColor = color;
                }
            }

        }

        /// <summary>
        /// Load Child form in MDI parent
        /// </summary>
        /// <param name="parentForm">Name of parent form</param>
        /// <param name="childlForm">Name of child form</param>
        /// <param name="nestedLevel">Nesting level of the child form</param>
        /// <param name="buttonName">Button that activate child form.</param>
        public static void LoadChildForm(Form parentForm, Form childlForm, ChildNastedLevel nestedLevel, GradientButton buttonName)
        {
            switch (nestedLevel)
            {
                case ChildNastedLevel.Nasted:
                    {
                        //Deactivate buttonName
                        DeactivateButton(parentForm, buttonName);

                        childlForm.MdiParent = parentForm;
                        childlForm.Show();

                        CloseAllOpenChildtrenExceptLoadingChild(parentForm, childlForm, ChildNastedLevel.Nasted);
                        break;
                    }
                case ChildNastedLevel.NastedNasted:
                    {
                        //Deactivate buttonName
                        DeactivateButton(parentForm, buttonName);

                        childlForm.MdiParent = parentForm.MdiParent;
                        childlForm.TopLevel = false;
                        childlForm.Show();

                        CloseAllOpenChildtrenExceptLoadingChild(parentForm, childlForm, ChildNastedLevel.NastedNasted);
                        break;
                    }
                case ChildNastedLevel.NastedNastedNasted:
                    {
                        DeactivateButton(parentForm, buttonName);

                        childlForm.TopLevel = false;
                        //parentForm.MdiParent.Controls.Add(childlForm);

                        foreach (Control control in parentForm.MdiParent.Controls)
                        {
                            if (control is Form)
                            {
                                if (!parentForm.MdiParent.Controls.Contains(childlForm))
                                {
                                    parentForm.MdiParent.Controls.Add(childlForm);
                                }
                            }

                        }
                        childlForm.Parent = parentForm.MdiParent;
                        childlForm.TopMost = true;
                        childlForm.Show();


                        CloseAllOpenChildtrenExceptLoadingChild(parentForm, childlForm, ChildNastedLevel.NastedNastedNasted);
                        break;
                    }
            }
        }

        private static void CloseAllOpenChildtrenExceptLoadingChild(Form parentForm, Form childlForm, ChildNastedLevel nestedLevel)
        {
            switch (nestedLevel)
            {
                case ChildNastedLevel.Nasted:
                    {
                        foreach (Form form in parentForm.MdiChildren)
                        {
                            if (form.GetType() != childlForm.GetType() && form != null)
                            {
                                //form.Hide();
                                form.Close();
                            }
                        }
                        foreach (Control form in parentForm.Controls)
                        {
                            if (form is Form)
                            {
                                (form as Form).Close();
                            }
                        }
                        break;
                    }
                case ChildNastedLevel.NastedNasted:
                case ChildNastedLevel.NastedNastedNasted:
                    {
                        foreach (Control form in parentForm.MdiParent.Controls)
                        {
                            if (form is Form)
                            {
                                if (form.GetType() != childlForm.GetType() && form != null)
                                {
                                    //form.Hide();
                                    (form as Form).Close();
                                }
                            }
                        }
                        break;
                    }
            }
        }

        private static void DeactivateButton(Form parentForm, GradientButton button)
        {
            foreach (Control control in parentForm.Controls)
            {
                if (control is GroupBox || control is RoundedPanel || control is Panel)
                {
                    foreach (Control btn in control.Controls)
                    {
                        if (btn is GradientButton)
                        {
                            if (btn.Name == button.Name)
                            {
                                (btn as GradientButton).Active = false;
                            }
                            else
                            {
                                (btn as GradientButton).Active = true;
                            }
                        }
                    }
                }
            }

        }
    }
}
