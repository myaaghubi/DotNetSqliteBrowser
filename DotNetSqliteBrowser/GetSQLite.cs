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
    }
}
