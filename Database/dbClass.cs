using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.IO;

namespace Database
{
    public class dbClass
    {
        public struct User
        {
            public int ID;
            public string username;
            public string password;
            public int situation;
            public List<string> logs;
            public string last_login;
            public string email;
        }

        private SQLiteConnection connection;
        private SQLiteCommand command = new SQLiteCommand();
        private SQLiteDataReader reader;
        private string dbName;

        public dbClass(string _dbName)
        {
            this.dbName = _dbName;
            if (!File.Exists(_dbName))
                SQLiteConnection.CreateFile(_dbName);
            try
            {
                connection = new SQLiteConnection($"Data Source = {this.dbName}", true);
                connection.Open();
                command.CommandText = "CREATE TABLE IF NOT EXISTS cal_User (ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, username VARCHAR," +
                    "password VARCHAR, situation INTEGER DEFAULT 1, log VARCHAR DEFAULT NULL, last_login VARCHAR DEFAULT NULL," +
                    "email VARCHAR DEFAULT NULL)";
                command.Connection = connection;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }
        }
        public void registerUser(User _user)
        {
            string sqlCommand = $"INSERT INTO cal_User(username, password, last_login) VALUES (@name, @password, @last_login)";
            connection = new SQLiteConnection($"Data Source = {this.dbName}");
            connection.Open();
            command.CommandText = sqlCommand;
            command.Parameters.AddWithValue("@name",_user.username);
            command.Parameters.AddWithValue("@password",_user.password);
            command.Parameters.AddWithValue("@last_login",_user.last_login);
            command.Connection = connection;
            command.ExecuteNonQuery();
            reader.Close();
            connection.Close();
        }
        public User getUser(object _user)
        {
            User temp = new User();
            string sqlCommand = _user.GetType() == typeof(string) ? "SELECT * FROM cal_User WHERE username=@param" : "SELECT * FROM cal_User WHERE ID=@param";
            connection = new SQLiteConnection($"Data Source = {this.dbName}");
            connection.Open();
            command.CommandText = sqlCommand;
            command.Parameters.AddWithValue("@param", _user);
            command.Connection = connection;
            reader = command.ExecuteReader();
            reader.Read();
            temp.ID = reader.GetInt32(0);
            temp.username = reader.GetString(1);
            temp.password = reader.GetString(2);
            temp.situation = reader.GetInt32(3);
            temp.logs = reader.GetValue(4).GetType() == typeof(DBNull) ? null : reader.GetString(4).Split('^').ToList<string>();
            temp.last_login = reader.GetString(5);
            temp.email = reader.GetValue(6).GetType() == typeof(DBNull) ? null : reader.GetString(6);
            reader.Close();
            connection.Close();
            return temp;
        }
        public List<User> getAllUser()
        {
            List<User> allUser = new List<User>();
            connection = new SQLiteConnection($"Data Source = {this.dbName}");
            connection.Open();
            command.CommandText = "SELECT * FROM cal_User";
            command.Connection = connection;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                User temp = new User();
                temp.ID = reader.GetInt32(0);
                temp.username = reader.GetString(1);
                temp.password = reader.GetString(2);
                temp.situation = reader.GetInt32(3);
                temp.logs = reader.GetValue(4).GetType() == typeof(DBNull) ? null : reader.GetString(4).Split('^').ToList<string>();
                temp.last_login = reader.GetString(5);
                temp.email = reader.GetValue(6).GetType() == typeof(DBNull) ? null : reader.GetString(6);
                allUser.Add(temp);
            }
            reader.Close();
            connection.Close();
            return allUser;
        }

        public void updateUser(User _user)
        {
            string sqlCommand = "UPDATE cal_User SET username = @uname, password = @paswd," +
                "log = @log, situation = @sit, last_login = @last, email = @mail WHERE ID = @id";
            connection = new SQLiteConnection($"Data Source = {this.dbName}");
            connection.Open();
            command.CommandText = sqlCommand;
            command.Parameters.AddWithValue("@id", _user.ID);
            command.Parameters.AddWithValue("@uname",_user.username);
            command.Parameters.AddWithValue("@paswd",_user.password);
            command.Parameters.AddWithValue("@log", _user.logs == null ? null : string.Join("^", _user.logs));
            command.Parameters.AddWithValue("@sit",_user.situation);
            command.Parameters.AddWithValue("@last",_user.last_login);
            command.Parameters.AddWithValue("@mail",_user.email == null ? null : _user.email);
            command.Connection = connection;
            command.ExecuteNonQuery();
            connection.Close();
        }

        public bool isAdmin(string _username)
        {
            bool flag = false;
            User _user = getUser(_username);
            if (_user.ID == 1)
                flag = true;
            return flag;
        }
    }
}
