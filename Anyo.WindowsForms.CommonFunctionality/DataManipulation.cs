namespace Anyo.WindowsForms.CommonFunctionality
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using System.Data;
    using System.Drawing;
    using Anyo.WindowsForms.Controls.Navigations;
    using System.Reflection;
    using CommonFunctionality.UI;
    using Anyo.WindowsForms.MessageBoxes;

    /// <summary>
    /// This class provides functionality for processing,verification and receiving data
    /// </summary>
    public class DataManipulation
    {
        #region Used
        /// <summary>
        /// Creates and adds variable numbers of a System.Data.DataColumn object that has the specified name
        //  to the System.Data.DataColumnCollection.
        /// </summary>
        /// <param name="currentDataTable">Current DataTable object that store this DataColumnCollection</param>
        /// <param name="columnNames">Names of columns in this DataColumnCollection</param>
        public static void AddColumnsTheirNamesInDataTable(DataTable currentDataTable, params string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                currentDataTable.Columns.Add(columnNames[i]);
            }
        }

        /// <summary>
        /// Fill only TextBoxes and ComboBoxes in specific System.Windows.Forms.Panel 
        /// with data from selected row of specific System.Windows.Forms.DataGridView object.
        /// Data fill according tabIndex of controls in a Panel container 
        /// and index of columns in the DataGridView.Columns collection.
        /// </summary>
        /// <param name="panelName">Current Panel container that contains TextBoxes and ComboBoxes</param>
        /// <param name="dataGridViewName">Current DataGridView instance which is a source of data</param>
        public static void FillTextBoxesAndComboBoxesWithGridViewRowData(Panel panelName, DataGridView dataGridViewName)
        {
            try
            {
                var controls = from Control control in panelName.Controls
                               where control.GetType().Name.Equals("TextBox") || control.GetType().Name.Equals("ComboBox")
                               orderby control.TabIndex
                               select control;

                int nextCell = 0;
                foreach (Control control in controls)
                {
                    if (dataGridViewName.SelectedRows[0].Cells[nextCell].Value == null)
                    {
                        nextCell++;
                    }
                    if (dataGridViewName.SelectedRows[0].Cells[nextCell].Value != null)
                    {
                        control.Text = dataGridViewName.SelectedRows[0].Cells[nextCell].Value.ToString().Trim();
                        nextCell++;

                    }
                }
            }
            catch (Exception ex)
            {

                SampleMessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Makes sense methods
        /// <summary>
        /// Check whether the value exists in specific table in database
        /// </summary>
        /// <param name="connectionStr">Connection string to the database that contains the table</param>
        /// <param name="tableName">Table name</param>
        /// <param name="columName">Column name that contains the value</param>
        /// <param name="value">Value to check</param>
        /// <returns></returns>
        public static bool IsValueExistsInDB(string connectionStr, string tableName, string columName, string value)
        {
            SqlConnection dbCon = new SqlConnection(connectionStr);
            SqlCommand cmd = dbCon.CreateCommand();
            SqlParameterCollection parameters = cmd.Parameters;
            bool result = false;

            cmd.CommandText = "SELECT COUNT(" + columName + ") FROM " + tableName + " WHERE " + columName + "= @value";
            parameters.AddWithValue("@value", value);

            try
            {
                dbCon.Open();
                int counter = (int)cmd.ExecuteScalar();
                dbCon.Close();
                if (counter == 1 || counter > 1)
                {
                    result = true;
                    //SampleMessageBox.Show("Exist");
                }
                else
                {
                    result = false;
                    //SampleMessageBox.Show("Not exist");
                }
            }
            catch (Exception ex)
            {
                result = false;
                SampleMessageBox.Show(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Convert List<T> do DataTable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">List of items</param>
        /// <returns>Datatable object</returns>
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        #endregion

        #region Makes more or less no sense methods

        private static void DisplayRecords(string connectionStr, string cmdStr, DataGridView dataGridName)
        {
            SqlConnection dbCon = new SqlConnection(connectionStr);
            SqlCommand cmd = new SqlCommand(cmdStr, dbCon);

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                //dt.Columns.Add();
                da.Fill(dt);
                dataGridName.DataSource = dt;

                //da.Update(dt);
            }
            catch (Exception ex)
            {

                SampleMessageBox.Show(ex.Message);
            }
        }
        private static void DisplayRecords(DataGridNavigation navigationName, string connectionStr, string cmdStr, Display firstOrLastPage)
        {
            navigationName.ConnectionString = connectionStr;
            navigationName.CommandString = cmdStr;
            DataGridView dataGridName = navigationName.DataGridName;
            //navigationName.PageSize = 15;
            if (firstOrLastPage == Display.FirstPage)
            {
                navigationName.FillDataGridWithRecords(firstOrLastPage);
            }
            if (firstOrLastPage == Display.LastPage)
            {
                navigationName.FillDataGridWithRecords(firstOrLastPage);
            }
            FormsUI.DisableSorting(dataGridName);

        }

        private static DataTable FillDataTable(string connectionStr, string cmdStr)
        {
            SqlConnection dbCon = new SqlConnection(connectionStr);
            SqlCommand cmd = new SqlCommand(cmdStr, dbCon);
            DataTable dt = new DataTable(); ;

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                SampleMessageBox.Show(ex.Message);
            }

            return dt;
        }

        private static void FillTextBoxWithDBValues(TextBox textBoxName, string connectionStr, string tableName, string columnName)
        {

            AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();
            string cmdStr = @"SELECT " + columnName + " FROM " + tableName + " ";

            SqlConnection dbCon = new SqlConnection(connectionStr);
            SqlCommand cmd = new SqlCommand(cmdStr, dbCon);
            try
            {
                dbCon.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    namesCollection.Add(dr[columnName].ToString());
                }
                dbCon.Close();

                textBoxName.AutoCompleteMode = AutoCompleteMode.Suggest;
                textBoxName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                textBoxName.AutoCompleteCustomSource = namesCollection;

                //foreach (DataRow dr in dt.Rows)
                //{
                //    comboBoxName.Items.Add(dr[columnName].ToString().Replace(" ", ""));
                //    //comboBoxName.AutoCompleteCustomSource.Add(dr[columnName].ToString().Replace(" ", ""));
                //}

                //comboBoxName.SelectedIndex = 0;
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    comboBoxName.Items.Add(dt.Rows[i][columnName].ToString().Replace(" ", ""));
                //}
            }
            catch (SqlException ex)
            {

                SampleMessageBox.Show(ex.Message);
            }

        }

        private static void AddColumNamesToDataGridView(DataGridView dataGridViewName, string[] wcfColumnNames, string[] displayColumnNames)
        {
            for (int i = 0; i < wcfColumnNames.Length; i++)
            {
                dataGridViewName.Columns[wcfColumnNames[i]].HeaderText = displayColumnNames[i];
                dataGridViewName.Columns[wcfColumnNames[i]].DisplayIndex = i;
            }
        }

        private static void FillComboBoxWithDBValues(ComboBox comboBoxName, string connectionStr, string tableName, string columnName)
        {

            comboBoxName.Items.Clear();
            //string cmdStr = @"SELECT " + columnName + " FROM " + tableName + " ORDER BY " + columnName + " ASC";

            string cmdStr = @"SELECT " + columnName + " FROM " + tableName + " ";

            SqlConnection dbCon = new SqlConnection(connectionStr);
            SqlCommand cmd = new SqlCommand(cmdStr, dbCon);

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                //comboBoxName.DataSource = dt;
                //comboBoxName.DisplayMember = columnName;

                foreach (DataRow dr in dt.Rows)
                {
                    comboBoxName.Items.Add(dr[columnName].ToString().Replace(" ", ""));
                    //comboBoxName.AutoCompleteCustomSource.Add(dr[columnName].ToString().Replace(" ", ""));
                }


                //comboBoxName.SelectedIndex = 0;
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    comboBoxName.Items.Add(dt.Rows[i][columnName].ToString().Replace(" ", ""));
                //}
            }
            catch (SqlException ex)
            {

                SampleMessageBox.Show(ex.Message);
            }

        }

        private static void FillTextBoxesWithGridViewRowData(Panel panelName, DataGridView dataGridViewName, string[] wcfColumnNames)
        {
            try
            {
                var controls = from Control control in panelName.Controls
                               where control.GetType().Name.Equals("TextBox") || control.GetType().Name.Equals("ComboBox")
                               orderby control.TabIndex
                               select control;
                List<Control> orderedContolsList = controls.ToList();
                int nextCell = 0;
                int colmnNamesIndex = 0;

                while (colmnNamesIndex < wcfColumnNames.Length)
                {
                    if (dataGridViewName.SelectedRows[0].Cells[nextCell].Value == null)
                    {
                        nextCell++;
                    }
                    if (dataGridViewName.SelectedRows[0].Cells[nextCell].Value != null)
                    {
                        orderedContolsList[colmnNamesIndex].Text = dataGridViewName.SelectedRows[0].Cells[wcfColumnNames[colmnNamesIndex]].Value.ToString().Trim();
                        nextCell++;
                        colmnNamesIndex++;
                    }
                }
            }
            catch (Exception ex)
            {

                SampleMessageBox.Show(ex.Message);
            }
        }

        private static void FillListWithTextBoxValues(List<string> listName, Panel panelName)
        {
            try
            {
                var controls = from Control control in panelName.Controls
                               orderby control.TabIndex
                               select control;

                //foreach (Control control in panelName.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                foreach (Control control in controls)
                {
                    if (control is TextBox)
                    {
                        listName.Add(control.Text);
                    }

                }

            }
            catch (Exception ex)
            {

                SampleMessageBox.Show(ex.Message);
            }
        }

        //FillTextBoxWithGridViewRowData() Without LINQ
        //public static void FillTextBoxWithGridViewRowData(Panel panelName, DataGridView dataGridViewName, int minTextBoxTabIndex)
        //{
        //    try
        //    {
        //        foreach (Control control in panelName.Controls)
        //        {
        //            if (control is TextBox)
        //            {
        //                control.Text = dataGridViewName.SelectedRows[0].Cells[control.TabIndex - minTextBoxTabIndex].Value.ToString();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        SampleMessageBox.Show(ex.Message);
        //    }
        //}
        #endregion
    }
}
