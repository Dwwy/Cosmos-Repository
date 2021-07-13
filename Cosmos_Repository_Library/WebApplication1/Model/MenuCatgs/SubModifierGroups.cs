using System;

namespace Test1.Model.MenuCatgs
{
    public class SubModifierGroups
    {
        public String Code { get; set; }
        public String Title { get; set; }
        public int SeqNo { get; set; }
        public String Status { get; set; }
        public int MinSelect { get; set; }
        public int MaxSelect { get; set; }
        public SubModifiers[] Submodifiers { get; set; }
    }
}
