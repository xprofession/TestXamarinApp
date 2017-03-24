using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApp
{
    public class Cities
    {
        public int count { get; set; }
        public List<City> citiesList { get; set; }
    }

    public class City
    {
        public int id { get; set; }
        public string title { get; set; }
        public string region { get; set; }
        public string area { get; set; }
    }
}
