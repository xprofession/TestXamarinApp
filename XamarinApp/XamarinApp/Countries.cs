using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApp
{
    public class Countries
    {
        public int count { get; set; }
        public List<Country> countriesList { get; set; }
    }

    public class Country
    {
        public int id { get; set; }
        public string title { get; set; }
    }
}
