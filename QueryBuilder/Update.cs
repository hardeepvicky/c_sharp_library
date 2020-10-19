using System;
using System.Collections.Generic;
using System.Text;

namespace QueryBuilder
{
    public class Update
    {
        private string table;
        private SortedDictionary<string, string> record;
        private Where wh;

        public Update(String table)
        {
            this.table = table;

            this.record = new SortedDictionary<string, string>();
        }

        public Update setRecord(SortedDictionary<string, string> record)
        {
            this.record = record;

            return this;
        }

        public Update add(string field, string value)
        {   
            this.record.Add(field, value);

            return this;
        }

        public Update setWhere(Where wh)
        {
            this.wh = wh;
            return this;
        }

        public Where getWhere()
        {
            if (wh == null)
            {
                wh = new Where("AND");
            }

            return wh;
        }


        public string get()
        {
            string query = "UPDATE " + this.table;

            if (record.Count == 0)
            {
                throw new Exception("Update : No any item is added in record");
            }

            if (wh == null)
            {
                throw new Exception("Update : where is not set");
            }

            var v_list = new List<string>();
            foreach (var d in record)
            {
                v_list.Add(d.Key + "='" + d.Value + "'");
            }

            query += " SET " + String.Join(", ", v_list);


            query += " WHERE " + wh.get();

            return query;
        }
    }
}
