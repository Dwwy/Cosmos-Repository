using System;

namespace Test1.Model.MenuCatgs
{
    public class Modifiers
    {
        public String ItemCode { get; set; }
        public String Title { get; set; }
        public int SeqNo { get; set; }
        public String Status { get; set; }
        public int Price { get; set; }
        public int MinSelect { get; set; }
        public int MaxSelect { get; set; }
        public SubModifierGroups[] SubmodifierGroups { get; set; }

    }
}
