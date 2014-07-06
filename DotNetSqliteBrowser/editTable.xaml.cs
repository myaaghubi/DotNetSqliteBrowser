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
            DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
            (tableOfColumns.Rows[currentIndex])["name"] = editablecolumnname_txt.Text;
            (tableOfColumns.Rows[currentIndex])["type"] = editabledatatype_cbx.SelectionBoxItem.ToString();

            columns_grd.ItemsSource = tableOfColumns.DefaultView;
        }
    }
}
