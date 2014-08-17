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
using System.Collections.ObjectModel;
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
    /// Interaction logic for editTable.xaml
    /// </summary>
    public partial class editTable : Window
    {
        private GetSQLite getSqlite;
        private DataTable tableColumns;
        private string tableName;
        private int currentIndex;
        public editTable(GetSQLite getSqlite_, string tableName_)
        {
            InitializeComponent();
            getSqlite = getSqlite_;
            tableName = tableName_;
            currentIndex = -1;

            this.load();
        }

        private void load()
        {
            tablename_txt.Text = tableName;
            string query = "PRAGMA table_info('" + tableName + "');";

            tableColumns = getSqlite.getValueByQuery(query);
            columns_grd.ItemsSource = tableColumns.DefaultView;
        }

        private void getCell(object sender, SelectionChangedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                currentIndex = columns_grd.SelectedIndex;

                DataRowView drv = (DataRowView)columns_grd.SelectedItem;
                editablecolumnname_txt.Text = drv["name"].ToString();
                
                int index = -1;
                foreach (ComboBoxItem cmbItem in editabledatatype_cbx.Items)
                {
                    index++;
                    if (cmbItem.Content.ToString() == drv["type"].ToString())
                    { break; }
                } 
                editabledatatype_cbx.SelectedIndex = index;
            }
        }

        private void applychange_btn_Click(object sender, RoutedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                currentIndex = columns_grd.SelectedIndex;

                int changes = 0; // 0 for no change, 1 for change name/type and 2 for add new column
                DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
                checkTableColumns(tableOfColumns);

                if ((tableOfColumns.Rows[currentIndex])["name"].ToString() != editablecolumnname_txt.Text.Trim() ||
                    (tableOfColumns.Rows[currentIndex])["type"].ToString() != editabledatatype_cbx.SelectionBoxItem.ToString())
                {
                    if ((tableOfColumns.Rows[currentIndex])["oldname"].ToString().Length == 0)
                    {
                        (tableOfColumns.Rows[currentIndex])["oldname"] = (tableOfColumns.Rows[currentIndex])["name"].ToString();
                        (tableOfColumns.Rows[currentIndex])["oldtype"] = (tableOfColumns.Rows[currentIndex])["type"].ToString();
                    }
                    (tableOfColumns.Rows[currentIndex])["name"] = editablecolumnname_txt.Text.Trim();
                    (tableOfColumns.Rows[currentIndex])["type"] = editabledatatype_cbx.SelectionBoxItem.ToString();
                    
                    changes = 1;

                    if ((tableOfColumns.Rows[currentIndex])["changes"].ToString() != "2")
                        (tableOfColumns.Rows[currentIndex])["changes"] = changes;
                }

                columns_grd.ItemsSource = tableOfColumns.DefaultView;
            }
        }

        private void removecolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            if (columns_grd.SelectedIndex != -1)
            {
                DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;

                tableOfColumns.Rows[columns_grd.SelectedIndex].Delete();
                columns_grd.ItemsSource = tableOfColumns.DefaultView;
            }
        }

        private void addcolumn_btn_Click(object sender, RoutedEventArgs e)
        {
            DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
            checkTableColumns(tableOfColumns);

            DataRow newColumn = tableOfColumns.NewRow();
            newColumn["name"] = editablecolumnname_txt.Text.Trim();
            newColumn["type"] = editabledatatype_cbx.SelectionBoxItem.ToString();
            newColumn["changes"] = "2";

            tableOfColumns.Rows.Add(newColumn);
            columns_grd.ItemsSource = tableOfColumns.DefaultView;
        }

        private void checkTableColumns(DataTable tableOfColumns_)
        {
            if (!tableOfColumns_.Columns.Contains("changes"))
                tableOfColumns_.Columns.Add("changes");
            if (!tableOfColumns_.Columns.Contains("oldname"))
                tableOfColumns_.Columns.Add("oldname");
            if (!tableOfColumns_.Columns.Contains("oldtype"))
                tableOfColumns_.Columns.Add("oldtype");
        }

        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string newTableName = tablename_txt.Text.Trim();
                DataTable tableOfColumns = (columns_grd.ItemsSource as DataView).Table;
                checkTableColumns(tableOfColumns);

                string query = "ALTER TABLE " + tableName + " RENAME TO temp_" + tableName + ";";
                getSqlite.ExecuteNonQuery_(query);

                query = "CREATE TABLE " + newTableName + "(";
                foreach (DataRow anyColumn in tableOfColumns.Rows)
                {
                    if (anyColumn.RowState != DataRowState.Deleted)
                        query += anyColumn["name"].ToString() + " " + anyColumn["type"].ToString() + ", ";
                }
                query += ");";
                query = query.Remove(query.LastIndexOf(", "), 2);

                getSqlite.ExecuteNonQuery_(query);


                if (true)
                {
                    query = "INSERT INTO " + newTableName + "(";
                    foreach (DataRow anyColumn in tableOfColumns.Rows)
                    {
                        if (anyColumn.RowState != DataRowState.Deleted && anyColumn["changes"].ToString() != "2")
                            query += anyColumn["name"].ToString() + ", ";
                    }

                    query = query.Remove(query.LastIndexOf(", "), 2);
                    query += ") SELECT ";

                    foreach (DataRow anyColumn in tableOfColumns.Rows)
                    {
                        if (anyColumn.RowState != DataRowState.Deleted && anyColumn["changes"].ToString() != "2")
                            if (anyColumn["changes"].ToString() == "1")
                                query += anyColumn["oldname"].ToString() + ", ";
                            else
                                query += anyColumn["name"].ToString() + ", ";
                    }
                    query = query.Remove(query.LastIndexOf(", "), 2);
                    query += " FROM temp_" + tableName + " ;";

                    getSqlite.ExecuteNonQuery_(query);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                string query = "DROP TABLE temp_" + tableName + ";";
                getSqlite.ExecuteNonQuery_(query);
            }

            var mainwindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
            mainwindow.loadTables();
            this.Close();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
