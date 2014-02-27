using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using Microsoft.Win32;
using System.Data;

namespace DotNetSqliteBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection sqliteConnection;

        public MainWindow()
        {
            InitializeComponent();
        }

        public SQLiteConnection getSqlite
        {
            get { return sqliteConnection; }
            set { sqliteConnection = value; }
        }

        private void loadTables()
        {
            try
            {
                bool emptyFlag = true;
                getSqlite.Open();
                if (getSqlite.State == ConnectionState.Open)
                {
                    ListBoxItem lbi;
                    string query = "SELECT name from sqlite_master WHERE type='table';";
                    SQLiteCommand command = new SQLiteCommand(query, getSqlite);
                    SQLiteDataReader rd = command.ExecuteReader();
                    tables_lb.Items.Clear();
                    while (rd.Read())
                    {
                        lbi = new ListBoxItem();
                        lbi.Content = rd.GetValue(0).ToString();
                        lbi.MouseLeftButtonUp += tables_MouseLeftButtonUp;
                        tables_lb.Items.Add(lbi);
                        emptyFlag = false;
                    }
                    if (emptyFlag)
                    {
                        lbi = new ListBoxItem();
                        lbi.Content = "no any table";
                        tables_lb.Items.Add(lbi);
                    }
                }
                getSqlite.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tables_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lItem = sender as ListBoxItem;
            if (lItem != null)
            {
                getTableColumns(lItem.Content.ToString());
                getTableData(lItem.Content.ToString());
            }
        }

        private void getTableData(string _tableName)
        {
            try
            {
                getSqlite.Open();
                if (getSqlite.State == ConnectionState.Open)
                {
                    string query = "SELECT * from " + _tableName + ";";
                    SQLiteDataAdapter adapt = new SQLiteDataAdapter(query, getSqlite);
                    DataTable dt = new DataTable();
                    adapt.Fill(dt);
                    fulldatagrid_grd.ItemsSource = dt.DefaultView;
                }
                getSqlite.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
                    
        private void getTableColumns(string _tableName)
        {
            try
            {
                getSqlite.Open();
                if (getSqlite.State == ConnectionState.Open)
                {
                    ListBoxItem lbi;
                    string query = "PRAGMA table_info(" + _tableName + ");";
                    columns_lb.Items.Clear();
                    SQLiteCommand command = new SQLiteCommand(query, getSqlite);
                    SQLiteDataReader rd = command.ExecuteReader();
                    string strPrimaryKey;
                    while (rd.Read())
                    {
                        lbi = new ListBoxItem();
                        lbi.Name = rd.GetValue(1).ToString();
                        lbi.Tag = _tableName;
                        strPrimaryKey = (rd.GetInt16(5) > 0) ? " PrimaryKey" : "";
                        lbi.Content = rd.GetValue(1).ToString() + " (" + rd.GetValue(2).ToString() + ")" + strPrimaryKey;
                        lbi.MouseLeftButtonUp += columns_MouseLeftButtonUp;
                        columns_lb.Items.Add(lbi);
                    }
                }
                        
                getSqlite.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void columns_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListBoxItem lbi = sender as ListBoxItem;
                if (lbi != null)
                {
                    getSqlite.Open();
                    DataTable dt = getSqlite.GetSchema("Columns");
                    dt.DefaultView.RowFilter = "table_name='" + lbi.Tag  + "' and column_name='" + lbi.Name + "'";
                    fulldatagrid_grd.ItemsSource = dt.DefaultView;

                    getSqlite.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void openDB()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog().Value)
                if (openDialog.FileName != null)
                    getSqlite = new SQLiteConnection("Data Source=" + openDialog.FileName);
        }

        private void newCommand(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog().Value)
                if (saveDialog.FileName != null)
                {
                    SQLiteConnection.CreateFile(saveDialog.FileName);
                    getSqlite = new SQLiteConnection("Data Source=" + saveDialog.FileName + ";Version=3;");
                }
        }
        private void openCommand(object sender, RoutedEventArgs e)
        {
            openDB();
            loadTables();
        }

        private void queryexecute_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                getSqlite.Open();
                if (getSqlite.State == ConnectionState.Open)
                {
                    string query = queryfield_txt.Text;
                    SQLiteCommand command = new SQLiteCommand(query, getSqlite);
                    DataTable dt = new DataTable();
                    dt.Load(command.ExecuteReader());
                    fulldatagrid_grd.ItemsSource = dt.DefaultView;
                }

                getSqlite.Close();
                queryerrors_txt.Text = "No error";
            }
            catch (Exception ex)
            {
                getSqlite.Close();
                queryerrors_txt.Text = ex.Message;
            }
        }

        private void clearQueryField(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "query...")
                textBox.Text = string.Empty;
            else
                textBox.SelectAll();
        }

        private void removeTable_btn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void addTable_btn_Click(object sender, RoutedEventArgs e)
        {
            addTable at = new addTable();
            at.Show();
        }
    }
}
