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
