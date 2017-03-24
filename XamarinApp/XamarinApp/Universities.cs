using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApp
{
    class Universities
    {
        public int count { get; set; }
        public List<University> universitiesList { get; set; }
    }

    public class University
    {
        public int id { get; set; }
        public string title { get; set; }
    }    
}
