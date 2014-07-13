using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DotNetSqliteBrowser
{
    /// <summary>
    /// Interaction logic for editTable.xaml
    /// </summary>
    public partial class editTable : Window
    {
        private GetSQLite getSqlite;
        private string tableName;
        private int currentIndex;
        public editTable(GetSQLite getSqlite_, string tableName_)
        {
            InitializeComponent();
            getSqlite = getSqlite_;
            tableName = tableName_;
            currentIndex = -1;

            this.load();
        }

        private void load()
        {
            tablename_txt.Text = tableName;
            string query = "PRAGMA table_info('" + tableName + "');";
            
            DataTable tableInfo = getSqlite.getValueByQuery(query);
            columns_grd.ItemsSource = tableInfo.DefaultView;
        }

        private void getCell(object sender, SelectionChangedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                currentIndex = columns_grd.SelectedIndex;

                DataRowView drv = (DataRowView)columns_grd.SelectedItem;
                editablecolumnname_txt.Text = drv["name"].ToString();
                
                int index = -1;
                foreach (ComboBoxItem cmbItem in editabledatatype_cbx.Items)
                {
                    index++;
                    if (cmbItem.Content.ToString() == drv["type"].ToString())
                    { break; }
                } 
                editabledatatype_cbx.SelectedIndex = index;
            }
        }

        private void applychange_btn_Click(object sender, RoutedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                currentIndex = columns_grd.SelectedIndex;

                int changes = 0; // 0 for no change, 1 for change name/type and 2 for add new column
                DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
                checkTableColumns(tableOfColumns);

                if ((tableOfColumns.Rows[currentIndex])["name"].ToString() != editablecolumnname_txt.Text.Trim())
                {
                    (tableOfColumns.Rows[currentIndex])["name"] = editablecolumnname_txt.Text.Trim();
                    changes = 1;
                }
                if ((tableOfColumns.Rows[currentIndex])["type"].ToString() != editabledatatype_cbx.SelectionBoxItem.ToString())
                {
                    (tableOfColumns.Rows[currentIndex])["type"] = editabledatatype_cbx.SelectionBoxItem.ToString();
                    changes = 1;
                }
                if (changes == 1 && (tableOfColumns.Rows[currentIndex])["oldname"].ToString().Length == 0)
                {
                    (tableOfColumns.Rows[currentIndex])["oldname"] = (tableOfColumns.Rows[currentIndex])["name"].ToString();
                    (tableOfColumns.Rows[currentIndex])["oldtype"] = (tableOfColumns.Rows[currentIndex])["type"].ToString();
                }
                if ((tableOfColumns.Rows[currentIndex])["changes"].ToString() != "2")
                    (tableOfColumns.Rows[currentIndex])["changes"] = changes;

                columns_grd.ItemsSource = tableOfColumns.DefaultView;
            }
        }

        private void removecolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;

                tableOfColumns.Rows[columns_grd.SelectedIndex].Delete();
                columns_grd.ItemsSource = tableOfColumns.DefaultView;
            }
        }

        private void addcolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
            checkTableColumns(tableOfColumns);

            DataRow newColumn = tableOfColumns.NewRow();
            newColumn["name"] = editablecolumnname_txt.Text.Trim();
            newColumn["type"] = editabledatatype_cbx.SelectionBoxItem.ToString();
            newColumn["changes"] = "2";

            tableOfColumns.Rows.Add(newColumn);
            columns_grd.ItemsSource = tableOfColumns.DefaultView;
        }

        private void checkTableColumns(DataTable tableOfColumns_)
        {
            if (!tableOfColumns_.Columns.Contains("changes"))
                tableOfColumns_.Columns.Add("changes");
            if (!tableOfColumns_.Columns.Contains("oldname"))
                tableOfColumns_.Columns.Add("oldname");
            if (!tableOfColumns_.Columns.Contains("oldtype"))
                tableOfColumns_.Columns.Add("oldtype");
        }

        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
                checkTableColumns(tableOfColumns);

                string query = "ALTER TABLE " + tableName + " RENAME TO temp_" + tableName + ";";
                getSqlite.ExecuteNonQuery_(query);

                query = "CREATE TABLE " + tablename_txt.Text.Trim() + "(";
                foreach (DataRow anyColumn in tableOfColumns.Rows)
                {
                    query += anyColumn["name"].ToString() + " " + anyColumn["type"].ToString() + ", ";
                }
                query += ");";
                query = query.Remove(query.LastIndexOf(", "), 2);

                getSqlite.ExecuteNonQuery_(query);

                query = "INSERT INTO " + tablename_txt.Text.Trim() + "(";
                foreach (DataRow anyColumn in tableOfColumns.Rows)
                {
                    if (anyColumn["changes"].ToString() == "1")
                        query += anyColumn["name"].ToString() + ", ";
                }
                query = query.Remove(query.LastIndexOf(", "), 2);
                query += ") SELECT ";
                foreach (DataRow anyColumn in tableOfColumns.Rows)
                {
                    if (anyColumn["changes"].ToString() == "1")
                        query += anyColumn["oldname"].ToString() + ", ";
                }
                query = query.Remove(query.LastIndexOf(", "), 2);
                query += "FROM temp_" + tableName + " ;";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
