using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteCrud
{    
    public class Model
    {
        public long ID;

        protected SortedDictionary<String, String> data;
        protected string primaryField = "id";

        public String table { get; }
        private static Database db;

        protected List<String> uniqueFields;

        public Model(String db_file, String table)
        {
            this.table = table;            
            initDatabase(db_file);

            uniqueFields = new List<string>();
        }

        public static Database initDatabase(String db_file)
        {
            if (db == null)
            {
                db = new Database(db_file);
            }

            return db;
        }

        public Database getDatabase()
        {
            return db;
        }

        public void setUniqueFields(List<String> fields)
        {
            this.uniqueFields = fields;
        }


        public bool insert(SortedDictionary<Object, Object> record)
        {
            this.setData(record);

            if ( !this.beforeInsert() )
            {
                return false;
            }

            var qb = new QueryBuilder.Insert(this.table).addRecord(this.data);
            
            var affected_rows = db.insert(qb.get(), ref this.ID);

            if (affected_rows > 0)
            {
                this.afterInsert();

                return true;
            }

            return false;
        }

        private void setData(SortedDictionary<Object, Object> record)
        {
            this.data = new SortedDictionary<string, string>();

            foreach (var d in record)
            {
                if (d.Key != null && d.Value != null)
                {
                    this.data.Add(d.Key.ToString(), d.Value.ToString());
                }
            }
        }

        protected bool beforeInsert()
        {
            return true;
        }

        protected void afterInsert()
        { 

        }

        public bool update(SortedDictionary<Object, Object> record, long id)
        {
            this.ID = id;

            this.setData(record);

            if (!this.beforeUpdate())
            {
                return false;
            }

            var qb = new QueryBuilder.Update(this.table);
            qb.setRecord(this.data);
            qb.getWhere().add(this.primaryField, this.ID.ToString());

            var affected_rows = db.nonQuery(qb.get());

            if (affected_rows > 0)
            {
                this.afterUpdate();

                return true;
            }

            return false;
        }

        protected bool beforeUpdate()
        {
            return true;
        }

        protected void afterUpdate()
        {

        }

        public bool insertOrUpdate(SortedDictionary<Object, Object> record)
        {
            this.setData(record);

            QueryBuilder.Where wh = new QueryBuilder.Where();

            if (this.uniqueFields.Count == 0)
            {
                throw new Exception("Unique Fields are empty");
            }

            foreach (String field in this.uniqueFields)
            {
                if (!this.data.ContainsKey(field))
                {
                    throw new Exception(field + " not found in record");
                }

                wh.add(field, this.data[field]);
            }

            QueryBuilder.Select s = new QueryBuilder.Select(this.table);
            s.addField(this.primaryField);
            s.setWhere(wh);

            long id = 0;
            var group_records = this.find(s);
            if (group_records.Count > 0)
            {
                var records = group_records[this.table];

                if (records.Count > 0)
                {
                    var r = records[0];
                    id = long.Parse(r[this.primaryField].ToString());
                }
            }

            if (id > 0)
            {
                return this.update(record, id);
            }
            else
            {
                return this.insert(record);
            }

        }
        public bool delete(long id)
        {
            this.ID = id;

            if (!this.beforeDelete())
            {
                return false;
            }

            var qb = new QueryBuilder.Delete(this.table);                        
            qb.getWhere().add(this.primaryField, this.ID.ToString());

            var affected_rows = db.nonQuery(qb.get());

            if (affected_rows > 0)
            {
                this.afterDelete();

                return true;
            }

            return false;
        }

        protected bool beforeDelete()
        {
            return true;
        }

        protected void afterDelete()
        {

        }

        public Dictionary<String, List<Dictionary<String, Object>>> find(QueryBuilder.Select select)
        {
            this.beforeFind(select);
            var q = select.get();

            var records = db.select(q);

            return records;
        }

        public int findCount(QueryBuilder.Where wh = null)
        {
            QueryBuilder.Select select = new QueryBuilder.Select(this.table);
            select.addField("COUNT(1) AS C");
            if (wh != null)
            {
                select.setWhere(wh);
            }
            

            var group_records = db.select(select.get());

            int c = 0;

            if (group_records.Count > 0)
            {
                var records = group_records["0"];
                if (records.Count > 0)
                {
                    var record = records[0];
                    c = int.Parse(record["C"].ToString());
                }
            }

            return c;
        }

        protected void beforeFind(QueryBuilder.Select select)
        { 

        }
        
    }
}
