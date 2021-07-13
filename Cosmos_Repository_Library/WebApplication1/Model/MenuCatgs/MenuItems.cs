using System;

namespace Test1.Model.MenuCatgs
{
    public class MenuItems
    {
        public String ItemCode { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String HtmlContent { get; set; }
        public int SeqNo { get; set; }
        public String Status { get; set; }
        public int OrigPrice { get; set; }
        public int DispPrice { get; set; }
        public int NextPrice { get; set; }
        public String NextPriceDate { get; set; }
        public String Thumbnail { get; set; }
        public String[] Photos { get; set; }
        public ModifierGroups[] ModifierGroups { get; set; }



    }
}
