using System;

namespace Test1.Model
{
    //The column name of the partition key in the database must match the name in the model object (case-sensitive).
    public class Menu 
    {
        
        public String id;
        public int storeid { get; set; }
        public String refid { get; set; }

        public OrderMenus[] OrderMenus { get; set; }
        public String CreationTime { get; set; } = DateTime.Now.ToString();

        
    }
}
