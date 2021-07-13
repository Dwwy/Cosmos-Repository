using System;

namespace Test1.Model.MenuCatgs
{
    public class ModifierGroups
    {
        public String Code { get; set; }
        public String Title { get; set; }
        public int SeqNo { get; set; }
        public String Status { get; set; }
        public int MinSelect { get; set; }
        public int MaxSelect { get; set; }
        public Modifiers[] Modifiers { get; set; }

    }
}
