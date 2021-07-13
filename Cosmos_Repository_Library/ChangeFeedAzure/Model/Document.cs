using System;

namespace Test1.Model
{
    public class Document
    {
        public String _rid { get; set; }
        public int _count { get; set; }
        public Menu[] Documents { get; set; }
    }
}
