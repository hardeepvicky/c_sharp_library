using System;
using System.Collections.Generic;
using System.Text;

namespace QueryBuilder
{   
    public class Insert
    {
        private string table;
        private List<SortedDictionary<string, string>> records;

        public Insert(String table)
        {
            this.table = table;

            this.records = new List<SortedDictionary<string, string>>();
        }

        public Insert addRecord(SortedDictionary<string, string> record)
        {
            this.records.Add(record);
            return this;
        }

        public Insert add(string field, string value, int row = 0)
        {
            var index = this.records.Count - 1;
            if (index < 0)
            {
                this.records.Add(new SortedDictionary<string, string>());
                index = 0;
            }
            else if (row > index)
            {
                this.records.Add(new SortedDictionary<string, string>());
                index++;
            }
            else
            {
                index = row;
            }

            this.records[index].Add(field, value);

            return this;
        }


        public string get()
        {
            string query = "INSERT INTO " + this.table;

            var field_list = new List<string>();
            var value_list = new List<List<string>>();

            if (records.Count == 0)
            {
                throw new Exception("INSERT : No any item is added in record");
            }

            foreach (var record in records)
            {
                var v_list = new List<string>();
                foreach (var d in record)
                {
                    if (value_list.Count == 0)
                    { 
                        field_list.Add(d.Key);
                    }

                    v_list.Add("'" + d.Value + "'");
                }

                if (v_list.Count == 0)
                {
                    var row_index = value_list.Count - 1;
                    throw new Exception("INSERT : No any item is added in record[" + row_index + "]");
                }

                if (value_list.Count > 0)
                {
                    var first_v_list = value_list[0];

                    if (first_v_list.Count != v_list.Count)
                    {
                        var row_index = value_list.Count - 1;
                        throw new Exception("INSERT : Records has un equal no of value in rows, first row : " + first_v_list.Count + ", row " + row_index + " : " + v_list.Count);
                    }
                }

                value_list.Add(v_list);
            }

            query += "(" + String.Join(",", field_list) + ")";

            query += " VALUES ";

            for (int i = 0; i < value_list.Count; i++)
            {
                var v_list = value_list[i];
                query += "(" + String.Join(",", v_list) + ")";

                if (i < value_list.Count - 1)
                {
                    query += ", ";
                }
            }

            return query;
        }
    }
}
