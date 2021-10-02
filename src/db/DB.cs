using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace OthelloBot.src.db
{
    enum Notification {
        Hour,
        Youtube,
    }

    class DB
    {
        private static string connStr;

        public static void SetConnStr(string server, string port, string database, string uid, string pwd)
        {
            connStr = $"Server={server};Port={port};Database={database};Uid={uid};Pwd={pwd};CharSet=utf8;";
        }

        public static DataRow GetChannel(string notification)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"SELECT * FROM channel WHERE notification = '{notification}'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            adpt.Fill(ds, "channel");

            try
            {
                return ds.Tables[0].Rows[0];
            }
            catch
            {
                throw;
            }
        }

        public static int UpdateChannel(ulong channelID, string notification)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"REPLACE INTO channel (id, notification) VALUES('{channelID}', '{notification}')";
            using var cmd = new MySqlCommand(query, conn);

            return cmd.ExecuteNonQuery();
        }

        public static int DeleteChannel(string notification)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"DELETE FROM channel WHERE notification = '{notification}'";
            using var cmd = new MySqlCommand(query, conn);

            return cmd.ExecuteNonQuery();
        }

        public static DataRowCollection GetYoutubeChannels()
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"SELECT * FROM youtube_channel";
            MySqlDataAdapter adpt = new MySqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            adpt.Fill(ds, "channel");

            try
            {
                return ds.Tables[0].Rows;
            }
            catch
            {
                throw;
            }
        }

        public static int UpdateYoutubeChannel(string channelID, long last_upload)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"REPLACE INTO youtube_channel (id, last_upload) VALUES('{channelID}', {last_upload})";
            using var cmd = new MySqlCommand(query, conn);

            return cmd.ExecuteNonQuery();
        }

        public static int DeleteYoutubeChannel(string channelID)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"DELETE FROM youtube_channel WHERE id = '{channelID}'";
            using var cmd = new MySqlCommand(query, conn);

            return cmd.ExecuteNonQuery();
        }
    }
}
