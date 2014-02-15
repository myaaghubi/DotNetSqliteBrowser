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
        }
    }
}
