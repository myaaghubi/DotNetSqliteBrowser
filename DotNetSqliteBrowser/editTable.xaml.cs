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
            int changes = 0; // 0 for no change, 1 for change column name, 2 for change column type, 3 for change name/type and 4 for add new column
            DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
            if (!tableOfColumns.Columns.Contains("changes"))
                tableOfColumns.Columns.Add("changes");

            if ((tableOfColumns.Rows[currentIndex])["name"].ToString() != editablecolumnname_txt.Text.Trim())
            {
                (tableOfColumns.Rows[currentIndex])["name"] = editablecolumnname_txt.Text.Trim();
                changes += 1;
            }
            if ((tableOfColumns.Rows[currentIndex])["type"].ToString() != editabledatatype_cbx.SelectionBoxItem.ToString())
            {
                (tableOfColumns.Rows[currentIndex])["type"] = editabledatatype_cbx.SelectionBoxItem.ToString();
                changes += 2;

            }
            if ((tableOfColumns.Rows[currentIndex])["changes"].ToString() != "4")
                (tableOfColumns.Rows[currentIndex])["changes"] = changes;
            
            columns_grd.ItemsSource = tableOfColumns.DefaultView;
        }

        private void removecolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
                if (!tableOfColumns.Columns.Contains("changes"))
                    tableOfColumns.Columns.Add("changes");

                tableOfColumns.Rows[columns_grd.SelectedIndex].Delete();
                columns_grd.ItemsSource = tableOfColumns.DefaultView;
            }
        }

        private void addcolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
            if (!tableOfColumns.Columns.Contains("changes"))
                tableOfColumns.Columns.Add("changes");

            DataRow newColumn = tableOfColumns.NewRow();
            newColumn["name"] = editablecolumnname_txt.Text.Trim();
            newColumn["type"] = editabledatatype_cbx.SelectionBoxItem.ToString();
            newColumn["changes"] = "4";

            tableOfColumns.Rows.Add(newColumn);
            columns_grd.ItemsSource = tableOfColumns.DefaultView;
        }
    }
}
