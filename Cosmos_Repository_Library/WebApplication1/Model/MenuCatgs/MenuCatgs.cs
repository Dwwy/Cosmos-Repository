using System;

namespace Test1.Model.MenuCatgs
{
    public class MenuCatgs
    {
        public String Code { get; set; }
        public String Title { get; set; }
        public int SeqNo { get; set; }
        public String Status { get; set; }
        public MenuItems[] MenuItems { get; set; }

    }
}
