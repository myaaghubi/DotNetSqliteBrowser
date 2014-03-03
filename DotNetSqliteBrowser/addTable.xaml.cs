using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
        private SQLiteConnection getSqlite;

        public addTable(SQLiteConnection getSqlite)
        {
            InitializeComponent();
            this.getSqlite = getSqlite;
        }

        private void addcolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            if (newcolumn_txt.Text.Length > 1)
            {
                DataView view = (DataView)columns_grd.ItemsSource;
                DataTable table;
                if (view != null)
                {
                    table = ((DataView)columns_grd.ItemsSource).ToTable();
                    table.Rows.Add(newcolumn_txt.Text, datatype_cbx.Text);
                }
                else
                {
                    table = new DataTable();
                    table.Columns.Add("Name");
                    table.Columns.Add("DataType");
                    table.Rows.Add(newcolumn_txt.Text, datatype_cbx.Text);
                }
                columns_grd.ItemsSource = table.DefaultView;
            }
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

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            setQuery(getQuery());
        }

        public string getQuery()
        {
            DataView view = (DataView)columns_grd.ItemsSource;
            DataTable table;
            if (tablename_txt.Text != string.Empty && view != null)
            {
                table = ((DataView)columns_grd.ItemsSource).ToTable();

                string query = "CREATE TABLE IF NOT EXISTS '" + tablename_txt.Text + "'(";

                foreach (DataRow row in table.Rows)
                {
                    query += "" + row[0].ToString() + " " + row[1].ToString() + ",";
                }

                int position = query.LastIndexOf(',');
                if (position > -1) query = query.Substring(0, position);

                query += ")";

                return query;
            }
            return null;
        }
        public void setQuery(string query_)
        {
            getSqlite.Open();
            if (query_ != null && getSqlite.State == ConnectionState.Open)
            {
                SQLiteCommand command = new SQLiteCommand(query_, getSqlite);
                command.ExecuteNonQuery();
                getSqlite.Close();
                this.Close();
            }
        }
    }
}
