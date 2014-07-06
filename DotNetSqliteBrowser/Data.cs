using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DotNetSqliteBrowser
{
    class Data
    {
        public static DataTable GridToTable(DataGrid grid_)
        {
            DataView dView = grid_.ItemsSource as DataView;
            return dView.Table;
        }
    }
}
