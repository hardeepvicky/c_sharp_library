using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteCrud
{    
    public class Model
    {
        public long ID;

        protected SortedDictionary<String, String> data;
        protected string primaryField = "";

        private String table;
        private static Database db;


        public Model(String db_file, String table)
        {
            this.table = table;

            initDatabase(db_file);
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
                this.data.Add(d.Key.ToString(), d.Value.ToString());
            }
        }

        public bool beforeInsert()
        {
            return true;
        }

        public void afterInsert()
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

            var qb = new QueryBuilder.Update(this.table)
                        .setRecord(this.data)
                        .getWhere()
                        .add(this.primaryField, this.ID.ToString());

            var affected_rows = db.nonQuery(qb.get());

            if (affected_rows > 0)
            {
                this.afterUpdate();

                return true;
            }

            return false;
        }

        public bool beforeUpdate()
        {
            return true;
        }

        public void afterUpdate()
        {

        }

        public bool delete(long id)
        {
            this.ID = id;

            if (!this.beforeDelete())
            {
                return false;
            }

            var qb = new QueryBuilder.Delete(this.table)                        
                        .getWhere()
                        .add(this.primaryField, this.ID.ToString());

            var affected_rows = db.nonQuery(qb.get());

            if (affected_rows > 0)
            {
                this.afterDelete();

                return true;
            }

            return false;
        }

        public bool beforeDelete()
        {
            return true;
        }

        public void afterDelete()
        {

        }
    }
}
