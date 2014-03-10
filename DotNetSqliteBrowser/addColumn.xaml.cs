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
using System.Windows.Shapes;

namespace DotNetSqliteBrowser
{
    /// <summary>
    /// Interaction logic for addColumn.xaml
    /// </summary>
    public partial class addColumn : Window
    {
        private GetSQLite getSQLite;

        public addColumn(GetSQLite getSQLite_)
        {
            InitializeComponent();
            this.getSQLite = getSQLite_;
        }

        private void addcolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            if (getSQLite.getCurrentTableName != string.Empty && newcolumn_txt.Text != string.Empty)
            {
                getSQLite.ExecuteNonQuery_("ALTER TABLE " + getSQLite.getCurrentTableName + " ADD COLUMN " 
                    + newcolumn_txt.Text + " " + datatype_cbx.Text);
                this.Close();
            }
        }
    }
}
