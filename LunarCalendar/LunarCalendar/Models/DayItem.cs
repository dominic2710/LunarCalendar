using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarCalendar.Models
{
    public class DayItem
    {
        public int CurrentMonth { get; set; }
        public DateTime SolarDay { get; set; }
        public LunarDate LunarDay { get; set; }
        public bool IsSelected { get; set; }
        public Color DayItemTextColor
        {
            get
            {
                if (SolarDay.Month != CurrentMonth) return Colors.LightGray;
                if (SolarDay.DayOfWeek == DayOfWeek.Sunday) return Colors.Red;
                if (SolarDay.DayOfWeek == DayOfWeek.Saturday) return Colors.Blue;
                return Colors.Black;
            }
        }

        public Color DayItemBorderColor
        {
            get
            {
                if (!IsSelected) return Colors.Transparent;

                return Colors.Gray;
            }
        }
    }
}
