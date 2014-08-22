using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DotNetSqliteBrowser
{
    /// <summary>
    /// Interaction logic for tableObject.xaml
    /// </summary>
    public partial class tableObject : UserControl
    {
        private string tblName;
        private GetSQLite getSQLite;

        public tableObject(GetSQLite getSQLite_,string tblName_)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.getSQLite = getSQLite_;
            this.tblName = tblName_;
            this.load();
        }
        private void load()
        {
            string query = "PRAGMA table_info('" + tblName + "');";

            DataTable tableInfo = getSQLite.getValueByQuery(query);
            tableInfo.NewRow();
            (tableInfo.Rows[tableInfo.Rows.Count - 1])["name"] = "*";
            (tableInfo.Rows[tableInfo.Rows.Count - 1])["type"] = "";

            columns_dgd.ItemsSource = tableInfo.DefaultView;
        }
        private void change(object sender, MouseButtonEventArgs e)
        {
            DataGrid dgtc = sender as DataGrid;
            DataTable dt = Data.GridToTable(dgtc);
            MessageBox.Show((dt.Rows[0])["checked"].ToString());
        }
        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
