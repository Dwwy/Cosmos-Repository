using System;

namespace Test1.Model
{
    public class OrderMenus
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String TaxCode { get; set; }
        public String LanguageCode { get; set; }

        public Currency currency { get; set; }

        public MenuSet[] MenuSets { get; set; }
    }
}
