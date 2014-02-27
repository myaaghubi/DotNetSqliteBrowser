using System;
using System.Collections.Generic;
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
    /// Interaction logic for addTable.xaml
    /// </summary>
    public partial class addTable : Window
    {
        public List<string> dataTypeValues { get; set; }
        public addTable()
        {
            InitializeComponent();
            dataTypeValues = new List<string>() { "NUMERIC", "TEXT", "BLOB", "INTEGER PRIMARY KEY" };
            grid_datatype_column_clm.ItemsSource = dataTypeValues;
        }

        private void addcolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            DataView view = (DataView) columns_grd.ItemsSource;
            DataTable table;
            if (view != null)
            {
                table = ((DataView)columns_grd.ItemsSource).ToTable();
                table.Rows.Add("Column" + table.Rows.Count);
            }
            else
            {
                table = new DataTable();
                table.Columns.Add("Columns");
                table.Rows.Add("Column" + table.Rows.Count);
            }
            columns_grd.ItemsSource = table.DefaultView;
        }

        private void removecolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                DataTable table = ((DataView)columns_grd.ItemsSource).ToTable();
                table.Rows[columns_grd.SelectedIndex].Delete();
                columns_grd.ItemsSource = table.DefaultView;
            }
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
