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

        private string tableName;
        public addColumn(GetSQLite getSqlite_, string tableName_)
        {
            InitializeComponent();
            this.getSQLite = getSqlite_;
            this.tableName = tableName_;
        }

        private void addcolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            if (tableName != string.Empty && newcolumn_txt.Text != string.Empty)
            {
                getSQLite.ExecuteNonQuery_("ALTER TABLE " + tableName + " ADD COLUMN "
                    + newcolumn_txt.Text.Trim() + " " + datatype_cbx.SelectionBoxItem.ToString());
            }
        }
    }
}
