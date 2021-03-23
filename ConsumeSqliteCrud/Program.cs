using System;
using System.Collections.Generic;

namespace ConsumeSqliteCrud
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = new SQLiteCrud.Model("db.sqlite", "cars");

            var group_records = model.find(new QueryBuilder.Select(model.table));

            var records = group_records[model.table];

            foreach (var record in records)
            {
                SortedDictionary<Object, Object> r = new SortedDictionary<object, object>();
                r["price"] = 100;
                model.update(r, long.Parse(record["id"].ToString()) );
            }
            
            var log = model.getDatabase().queryLogCSV();
            Console.WriteLine(log);
        }
    }
}
