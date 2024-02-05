using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarCalendar.Models
{
    public class LunarDate
    {
        public int LunarYear {  get; set; }
        public int LunarMonth { get; set; }
        public int LunarDay { get; set; }
        public bool IsLeapMonth { get; set; }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2} Năm nhuận: {3}", LunarDay.ToString(), LunarMonth.ToString(), LunarYear.ToString(), IsLeapMonth.ToString());
        }
    }
}
