using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SQLiteCrud
{
    public class Database
    {
        private SQLiteConnection conn;
        private List<QueryLog> log;

        public Database(String db_file)
        {
            if ( !File.Exists(db_file) )
            {
                SQLiteConnection.CreateFile(db_file);
            }

            SQLiteConnectionStringBuilder conn_builder = new SQLiteConnectionStringBuilder
            {
                DateTimeFormat = SQLiteDateFormats.ISO8601,                
                Version = 3,
                DataSource = db_file
            };


            Console.WriteLine(conn_builder.ConnectionString);
            this.conn = new SQLiteConnection(conn_builder.ConnectionString);
            this.conn.Open();
            Console.WriteLine(db_file + ": open");

            this.log = new List<QueryLog>();
        }

        ~Database()
        {
            Console.WriteLine("database : closed");
        }

        public Dictionary<String, List<Dictionary<String, Object>>> select(String q)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var records = new Dictionary<String, List<Dictionary<String, Object>>>();

            using (SQLiteCommand st = this.conn.CreateCommand())
            {   
                st.CommandType = CommandType.Text;
                st.CommandText = q;
                SQLiteDataReader r = st.ExecuteReader();

                while (r.Read())
                {
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        var table = r.GetTableName(i);
                        if (table.Equals(""))
                        {
                            table = "0";
                        }
                        var field = r.GetName(i);
                        var value = r.GetValue(i);
                        
                        if (!records.ContainsKey(table))
                        {
                            records[table] = new List<Dictionary<String, Object>>();
                        }

                        if (i == 0)
                        {
                            records[table].Add(new Dictionary<String, Object>());                            
                        }

                        var row = records[table].Count - 1;

                        records[table][row].Add(field, value);
                    }
                }
            }

            watch.Stop();

            log.Add(new QueryLog(q, records.Count, watch.ElapsedMilliseconds));

            return records;
        }

        public int insert(String q, ref long last_insert_id)
        {
            int n = 0;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (SQLiteCommand st = this.conn.CreateCommand())
            {
                st.CommandType = CommandType.Text;
                st.CommandText = q;

                n = st.ExecuteNonQuery();

                last_insert_id = this.conn.LastInsertRowId;
            }

            watch.Stop();

            log.Add(new QueryLog(q, n, watch.ElapsedMilliseconds));

            return n;
        }

        public int nonQuery(String q)
        {
            int n = 0;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (SQLiteCommand st = this.conn.CreateCommand())
            {
                st.CommandType = CommandType.Text;
                st.CommandText = q;

                n = st.ExecuteNonQuery();
            }

            watch.Stop();

            log.Add(new QueryLog(q, n, watch.ElapsedMilliseconds));

            return n;
        }

        public List<QueryLog> queryLog()
        {
            return this.log;
        }

        public string queryLogCSV()
        {
            var list = new List<String>();

            foreach(var item in log)
            {
                list.Add(item.ToString());
            }

            return String.Join("\n", list);
        }
    }


    public class QueryLog : Object
    {
        public string query;
        public int affect_rows;
        public float time_in_miliseconds;

        public QueryLog(string query, int affect_rows, float time_in_miliseconds)
        {
            this.query = query;
            this.affect_rows = affect_rows;
            this.time_in_miliseconds = time_in_miliseconds;
        }

        override
        public String ToString()
        {
            return this.query + "," + this.affect_rows + "," + this.time_in_miliseconds + " ms";
        } 
    }
}
