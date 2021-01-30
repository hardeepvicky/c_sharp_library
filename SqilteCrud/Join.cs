using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteCrud
{
    class Join
    {
        public Model model;
        public String foreignKey;
        public QueryBuilder.Select select;

        public Join(Model m, String f)
        {
            this.model = m;
            this.foreignKey = f;
        }

        public Join(Model m, String f, QueryBuilder.Select s) : this(m, f)
        {
            this.select = s;
        }
    }
}
