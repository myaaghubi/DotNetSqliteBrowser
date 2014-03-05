using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DotNetSqliteBrowser
{
    public class GetSQLite
    {
        private SQLiteConnection sqliteConnection;

        public GetSQLite(string _connection)
        {
            sqliteConnection = new SQLiteConnection("Data Source=" + _connection);
        }

        public SQLiteConnection getDB()
        {
            Open();
            return sqliteConnection;
        }
        private void Open()
        {
            if (sqliteConnection != null && sqliteConnection.State != System.Data.ConnectionState.Open)
                sqliteConnection.Open();
        }

        private void Close()
        {
            if (sqliteConnection.State == System.Data.ConnectionState.Open)
                sqliteConnection.Close();
        }

        public void ExecuteNonQuery_(string query)
        {
            if (query != string.Empty)
            {
                Open();
                using (SQLiteCommand command = new SQLiteCommand(query, sqliteConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public string FillGrid(DataGrid grid_, string query_)
        {
            if (query_ != string.Empty && grid_ != null)
            {
                try
                {
                    Open();
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query_, sqliteConnection))
                    {
                        DataTable resutlDT = new DataTable();
                        dataAdapter.Fill(resutlDT);
                        grid_.ItemsSource = resutlDT.DefaultView;
                    }
                    return "No error";
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
