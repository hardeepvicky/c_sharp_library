using System;
using System.Collections.Generic;
using System.Text;

namespace QueryBuilder
{
    public class Delete
    {
        private string table;
        private Where wh;

        public Delete(String table)
        {
            this.table = table;
        }
        public Delete setWhere(Where wh)
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
            string query = "DELTE FROM " + this.table;

            if (wh == null)
            {
                throw new Exception("Update : where is not set");
            }

            query += " WHERE " + wh.get();

            return query;
        }
    }

    
}
