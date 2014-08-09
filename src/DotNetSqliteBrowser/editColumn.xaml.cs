/*
 * DotNetSqliteBrowser 
 * Copyright (C) 2014 "Mohammad Yaghobi Beyrami" m.yaghobi.abc@gmail.com

 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */
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
using System.Windows.Shapes;

namespace DotNetSqliteBrowser
{
    /// <summary>
    /// Interaction logic for editColumn.xaml
    /// </summary>
    public partial class editColumn : Window
    {
        private GetSQLite getSqlite;
        private string tableName;
        private string columnName;
        public editColumn(GetSQLite getSqlite_, string columnName_, string tableName_)
        {
            InitializeComponent();
            getSqlite = getSqlite_;
            tableName = tableName_;
            columnName = columnName_;
            newcolumnname_txt.Text = columnName;
        }

        private void applyedit_btn_Click(object sender, RoutedEventArgs e)
        {
            string query = "PRAGMA table_info('" + tableName + "');";

            DataTable tableInfo = getSqlite.getValueByQuery(query);
            query = "ALTER TABLE " + tableName + " RENAME TO temp_" + tableName + ";";
            getSqlite.ExecuteNonQuery_(query);

            query = "CREATE TABLE " + tableName + "(";
            foreach (DataRow anyColumn in tableInfo.Rows)
            {
                if (anyColumn["name"].ToString() != columnName)
                    query += anyColumn["name"].ToString() + " " + anyColumn["type"].ToString() + ", ";
                else
                    query += newcolumnname_txt.Text.Trim() + " " + datatype_cbx.SelectionBoxItem.ToString() +", ";
            }
            query += ");";
            query = query.Remove(query.LastIndexOf(", "), 2);

            getSqlite.ExecuteNonQuery_(query);

            query = "INSERT INTO " + tableName + "(";
            foreach (DataRow anyColumn in tableInfo.Rows)
            {
                if (anyColumn["name"].ToString() != columnName)
                    query += anyColumn["name"].ToString() + ", ";
                else
                    query += newcolumnname_txt.Text.Trim() + ", ";
            }

            query = query.Remove(query.LastIndexOf(", "), 2);
            query += ") SELECT ";

            foreach (DataRow anyColumn in tableInfo.Rows)
            {
                query += anyColumn["name"].ToString() + ", ";   
            }
            query = query.Remove(query.LastIndexOf(", "), 2);
            query += " FROM temp_" + tableName + " ;";

            getSqlite.ExecuteNonQuery_(query);

            query = "DROP TABLE temp_" + tableName + ";";
            getSqlite.ExecuteNonQuery_(query);

            var mainwindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
            mainwindow.getTableColumns(tableName);
            this.Close();
        }
    }
}
