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
                getSqlite.Open();
                if (getSqlite.State == ConnectionState.Open)
                {
                    string query = "SELECT name from sqlite_master WHERE type='table';";
                    SQLiteCommand command = new SQLiteCommand(query, getSqlite);
                    SQLiteDataReader rd = command.ExecuteReader();
                    while (rd.Read())
                    {
                        tables_lb.Items.Add(rd.GetValue(0).ToString());
                    }
                }
            }
            catch (Exception ex)
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

        private void openCommand(object sender, RoutedEventArgs e)
        {
            openDB();
            loadTables();
        }
    }
}
