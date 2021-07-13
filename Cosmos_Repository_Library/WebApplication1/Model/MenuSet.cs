using System;

namespace Test1.Model
{
    public class MenuSet
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public int SeqNo { get; set; }
        public String[] MenuTypes { get; set; }
        public ServHours.ServHours ServHours { get; set; }
        public MenuCatgs.MenuCatgs[] MenuCatgs { get; set; }
    }
}
