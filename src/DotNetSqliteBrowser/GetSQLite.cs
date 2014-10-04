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
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DotNetSqliteBrowser
{
    public class GetSQLite
    {
        private SQLiteConnection sqliteConnection;

        public GetSQLite(string _connection)
        {
            sqliteConnection = new SQLiteConnection("Data Source=" + _connection);
            if (!isValidSqlite(sqliteConnection)) sqliteConnection = null;
        }

        public GetSQLite()
        {
            sqliteConnection = null;
        }

        public SQLiteConnection getDB()
        {
            if (Open())
                return sqliteConnection;
            return null;
        }
        private bool Open()
        {
            if (sqliteConnection != null)
            {
                if (sqliteConnection.State != System.Data.ConnectionState.Open)
                    sqliteConnection.Open();
                return true;
            }
            return false;
        }

        private void Close()
        {
            if (sqliteConnection.State == System.Data.ConnectionState.Open)
                sqliteConnection.Close();
        }

        private bool isValidSqlite(SQLiteConnection connection_)
        {
            try
            {
                connection_.Open();
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter("PRAGMA integrity_check;", connection_))
                {
                    DataTable result = new DataTable();
                    adapter.Fill(result);
                    connection_.Close();
                    
                    if (result.Rows.Count == 1 && (result.Rows[0])[0].ToString().ToLower() == "ok")
                        return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public DataTable getValueByQuery(string query)
        {
            if (query != string.Empty)
            {
                if (Open())
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, sqliteConnection))
                    {
                        DataTable result = new DataTable();
                        adapter.Fill(result);
                        return result;
                    }
            }
            return null;
        }

        public void ExecuteNonQuery_(string query)
        {
            if (query != string.Empty)
            {
                if (Open())
                using (SQLiteCommand command = new SQLiteCommand(query, sqliteConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(string tableName_, DataTable data)
        {
            if (tableName_ != string.Empty && data != null)
            {
                if (Open())
                {
                    string query = "UPDATE " + tableName_ + " SET ";
                    foreach (DataColumn column in data.Columns)
                    {
                        if (column.ColumnName != "ROWID")
                        {
                            query += column.ColumnName + " = @" + column.ColumnName + ", ";
                        }
                    }
                    query = query.Remove(query.LastIndexOf(", "), 2);
                    query += " where ROWID=";

                    SQLiteCommand command;
                    string temp;
                    foreach (DataRow row in data.Rows)
                    {
                        command = sqliteConnection.CreateCommand();
                        command.CommandText = query + row["ROWID"].ToString();

                        for (int i=0; i<row.ItemArray.Length; i++)
                        {
                            temp = data.Columns[i].ColumnName.ToString();
                            //MessageBox.Show(temp);
                            if (temp != "ROWID")
                                command.Parameters.AddWithValue("@" + temp, row.ItemArray[i].ToString());
                        }

                        command.ExecuteNonQuery();
                    }
                }
                    
            }
        }

        public string FillGrid(DataGrid grid_, string query_)
        {
            if (query_ != string.Empty && grid_ != null)
            {
                try
                {
                    if (Open())
                        using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query_, sqliteConnection))
                        {
                            DataTable resutlDT = new DataTable();
                            dataAdapter.Fill(resutlDT);
                            grid_.ItemsSource = resutlDT.DefaultView;
                            return "No error";
                        }
                    return "Problem in opening database.";
                }
                catch (SQLiteException ex)
                {
                    return ex.Message.ToString();
                }
            }
            return "Query field is empty";
        }

    }
}
