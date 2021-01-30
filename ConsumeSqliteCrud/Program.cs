using System;

namespace ConsumeSqliteCrud
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = new SQLiteCrud.Model("db.sqlite", "cars");

            var result = model.insert(new System.Collections.Generic.SortedDictionary<Object, Object>()
            {
                { "name", "Platina" },
                { "price", "2000" },
            });

            Console.WriteLine(result);

            var log = model.getDatabase().queryLogCSV();
            Console.WriteLine(log);
        }
    }
}
