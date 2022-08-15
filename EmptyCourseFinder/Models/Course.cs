using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyCourseFinder
{
    public class Course
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<string> Types { get; set; }
        public Coordinates Coordinates { get; set; }
        public float Rating { get; set; }
        public int Rating_N { get; set; }
        public string International_Phone_Number { get; set; }
        public IEnumerable<int> Time_Spent { get; set; }
        public int Current_Popularity { get; set; }
        public IEnumerable<PopularTime> PopularTimes { get; set; }
        public IEnumerable<TimeWait> Time_Wait { get; set; }
    }
}
