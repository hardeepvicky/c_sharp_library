using System;
using System.Collections.Generic;

namespace QueryBuilder
{
    public class Select
    {
        private string table;
        private List<string> fields, groupby;
        private Dictionary<string, string> orderby;
        private Where wh;
        private int limit, offset;
        public Select(string table)
        {
            this.table = table;

            this.fields = new List<string>();

            this.groupby = new List<string>();

            this.orderby = new Dictionary<string, string>();

            limit = offset = 0;
        }

        public Select addField(string f)
        {
            if (!fields.Contains(f))
            {
                fields.Add(f);
            }

            return this;
        }

        public string getFields()
        {
            if (fields.Count == 0)
            {
                return "*";
            }

            return String.Join(",", fields);
        }

        public Select addGroupBy(string f)
        {
            if (!groupby.Contains(f))
            {
                groupby.Add(f);
            }

            return this;
        }
        public string getGroupBy()
        {
            if (groupby.Count == 0)
            {
                return "";
            }

            return String.Join(",", groupby);
        }

        public Select addOrderBy(string f, string o = "ASC")
        {
            if (!orderby.ContainsKey(f))
            {
                orderby.Add(f, o);
            }

            return this;
        }

        public string getOrderBy()
        {
            if (orderby.Count == 0)
            {
                return "";
            }

            var list = new List<string>();

            foreach (var d in orderby)
            {
                list.Add(d.Key + " " + d.Value);
            }

            return String.Join(",", list);
        }

        public Where getWhere(string op = "AND")
        {
            if (wh == null)
            {
                wh = new Where(op);
            }

            return wh;
        }

        public Select setWhere(Where wh)
        {
            this.wh = wh;
            return this;
        }

        public Select setLimit(int n)
        {
            this.limit = n;
            return this;
        }

        public Select setOffset(int n)
        {
            this.offset = n;
            return this;
        }

        public string get()
        {
            string query = "SELECT " + this.getFields() + " FROM " + table;

            string q = this.getWhere().get();

            if (q.Length > 0)
            {
                query += " WHERE " + q;
            }

            q = this.getGroupBy();

            if (q.Length > 0)
            {
                query += " GROUP BY " + q;
            }

            q = this.getOrderBy();

            if (q.Length > 0)
            {
                query += " ORDER BY " + q;
            }

            if (this.limit > 0)
            {
                query += " LIMIT " + this.limit;
            }

            if (this.offset > 0)
            {
                query += " OFFSET " + this.limit;
            }

            return query;
        }
    }

    public class Where
    {
        private string join_op;
        private Dictionary<string, string> conditions;

        public Where(string op = "AND")
        {
            this.join_op = op;
            conditions = new Dictionary<string, string>();
        }

        public Where add(string field, string value, string op = "=")
        {   
            conditions.Add(field + op, value);

            return this;
        }

        public Where addList(String field, List<String> list)
        {
            conditions.Add(field + " IN ", "(" + String.Join(",", list) + ")");
            return this;
        }

        public string get()
        {
            if (conditions.Count == 0)
            {
                return "";
            }

            var wh_list = new List<string>();

            foreach (var c in conditions)
            {
                wh_list.Add(c.Key + " '" + c.Value + "'");
            }

            return String.Join(" " + join_op + " ", wh_list);
        }
    }


}
