//using GoogleGson.Annotations;
//using Java.Net;
//using Java.Util.Functions;
using LunarCalendar.Models;


namespace LunarCalendar.Utilities
{
    public class LunarCalendarCalc
    {
        private static LunarCalendarCalc _lunarCalendarCalc;
        private LunarCalendarCalc() { }

        public static LunarCalendarCalc GetInstance()
        {
            if (_lunarCalendarCalc == null)
            {
                _lunarCalendarCalc = new LunarCalendarCalc();
            }

            return _lunarCalendarCalc;
        }
        public int DateToJulianDayNumber(int year, int month, int day)
        {
            var a = (14 - month) / 12;
            var y = year + 4800 - a;
            var m = month + 12 * a - 3;

            var julianDay = day + ((153 * m + 2) / 5) + 365 * y + (y / 4) - (y / 100) + (y / 400) - 32045; 
            if (julianDay < 2299161)
            {
                julianDay = day + ((153 * m + 2) / 5) + 365 * y + (y / 4) - 32083;
            }

            return julianDay;
        }

        public DateTime JulianDayNumberToDate(int julianDay)
        {
            var b = 0;
            var c = julianDay + 32082;
            var d = (4 * c + 3) / 1461;
            var e = c - (1461 * d) / 4;
            var m = (5 * e + 2) / 153;
            var day = e - (153 * m + 2) / 5 + 1;
            var month = m + 3 - 12 * (m / 10);
            var year = b * 100 + d - 4800 + m / 10;

            return new DateTime(year, month, day);
        }

        public int GetNewMoonDay(int k, double timeZone)
        {
            var T = k / 1236.85; // Time in Julian centuries from 1900 January 0.5
            var T2 = T * T;
            var T3 = T2 * T;
            var dr = Math.PI / 180;
            var Jd1 = 2415020.75933 + 29.53058868 * k + 0.0001178 * T2 - 0.000000155 * T3;
            Jd1 = Jd1 + 0.00033 * Math.Sin((166.56 + 132.87 * T - 0.009173 * T2) * dr); // Mean new moon
            var M = 359.2242 + 29.10535608 * k - 0.0000333 * T2 - 0.00000347 * T3; // Sun's mean anomaly
            var Mpr = 306.0253 + 385.81691806 * k + 0.0107306 * T2 + 0.00001236 * T3; // Moon's mean anomaly
            var F = 21.2964 + 390.67050646 * k - 0.0016528 * T2 - 0.00000239 * T3; // Moon's argument of latitude
            var C1 = (0.1734 - 0.000393 * T) * Math.Sin(M * dr) + 0.0021 * Math.Sin(2 * dr * M);
            C1 = C1 - 0.4068 * Math.Sin(Mpr * dr) + 0.0161 * Math.Sin(dr * 2 * Mpr);
            C1 = C1 - 0.0004 * Math.Sin(dr * 3 * Mpr);
            C1 = C1 + 0.0104 * Math.Sin(dr * 2 * F) - 0.0051 * Math.Sin(dr * (M + Mpr));
            C1 = C1 - 0.0074 * Math.Sin(dr * (M - Mpr)) + 0.0004 * Math.Sin(dr * (2 * F + M));
            C1 = C1 - 0.0004 * Math.Sin(dr * (2 * F - M)) - 0.0006 * Math.Sin(dr * (2 * F + Mpr));
            C1 = C1 + 0.0010 * Math.Sin(dr * (2 * F - Mpr)) + 0.0005 * Math.Sin(dr * (2 * Mpr + M));
            double deltat;
            if (T < -11)
            {
                deltat = 0.001 + 0.000839 * T + 0.0002261 * T2 - 0.00000845 * T3 - 0.000000081 * T * T3;
            }
            else
            {
                deltat = -0.000278 + 0.000265 * T + 0.000262 * T2;
            };
            var JdNew = Jd1 + C1 - deltat;
            return (int)Math.Round(JdNew + 0.5 + timeZone / 24, MidpointRounding.ToZero);
        }

        public int GetSunLongitude(int julianDay, double timeZone)
        {
            var T = (julianDay - 2451545.5 - timeZone / 24) / 36525; // Time in Julian centuries from 2000-01-01 12:00:00 GMT
            var T2 = T * T;
            var dr = Math.PI / 180; // degree to radian
            var M = 357.52910 + 35999.05030 * T - 0.0001559 * T2 - 0.00000048 * T * T2; // mean anomaly, degree
            var L0 = 280.46645 + 36000.76983 * T + 0.0003032 * T2; // mean longitude, degree
            var DL = (1.914600 - 0.004817 * T - 0.000014 * T2) * Math.Sin(dr * M);
            DL = DL + (0.019993 - 0.000101 * T) * Math.Sin(dr * 2 * M) + 0.000290 * Math.Sin(dr * 3 * M);
            var L = L0 + DL; // true longitude, degree
            L = L * dr;
            L = L - Math.PI * 2 * (Math.Round(L / (Math.PI * 2), MidpointRounding.ToZero)); // Normalize to (0, 2*PI)
            return (int)Math.Round(L / Math.PI * 6, MidpointRounding.ToZero);
            //return (int)Math.Round(L / (Math.PI * 12), MidpointRounding.ToZero);
        }

        public int GetLunarMonth11(int year, double timeZone)
        {
            var off = DateToJulianDayNumber(year, 12, 31) - 2415021;
            var k = (int)Math.Round(off / 29.530588853, MidpointRounding.ToZero);
            var nm = GetNewMoonDay(k, timeZone);
            var sunLong = GetSunLongitude(nm, timeZone); // sun longitude at local midnight
            if (sunLong >= 9)
            {
                nm = GetNewMoonDay(k - 1, timeZone);
            }
            return nm;
        }

        public int GetLeapMonthOffset(int a11, double timeZone)
        {
            var k = (int)Math.Round((a11 - 2415021.076998695) / 29.530588853 + 0.5, MidpointRounding.ToZero);
            var i = 1; // We start with the month following lunar month 11
            var arc = GetSunLongitude(GetNewMoonDay(k + i, timeZone), timeZone);
            int last;
            do
            {
                last = arc;
                i++;
                arc = GetSunLongitude(GetNewMoonDay(k + i, timeZone), timeZone);
            } while (arc != last && i < 14);
            return i - 1;
        }

        public LunarDate ConvertSolarToLunar(int day, int month, int year, double timeZone)
        {
            var lunarDate = new LunarDate
            {
                IsLeapMonth = false
            };
            var dayNumber = DateToJulianDayNumber(year, month, day);
            var k = (int)Math.Round((dayNumber - 2415021.076998695) / 29.530588853, MidpointRounding.ToZero);
            var monthStart = GetNewMoonDay(k + 1, timeZone);
            if (monthStart > dayNumber) {
                monthStart = GetNewMoonDay(k, timeZone);
            }
            var a11 = GetLunarMonth11(year, timeZone);
            var b11 = a11;
            if (a11 >= monthStart) {
                lunarDate.LunarYear = year;
                a11 = GetLunarMonth11(year - 1, timeZone);
            } else
            {
                lunarDate.LunarYear = year + 1;
                b11 = GetLunarMonth11(year + 1, timeZone);
            }
            lunarDate.LunarDay = dayNumber - monthStart + 1;
            var diff = (monthStart - a11) / 29;
            lunarDate.LunarMonth = diff + 11;
            if (b11 - a11 > 365)
            {
                var leapMonthDiff = GetLeapMonthOffset(a11, timeZone);
                if (diff >= leapMonthDiff)
                {
                    lunarDate.LunarMonth = diff + 10;
                    if (diff == leapMonthDiff)
                    {
                         lunarDate.IsLeapMonth = true;
                    }
                }
            }
            if (lunarDate.LunarMonth > 12)
            {
                lunarDate.LunarMonth = lunarDate.LunarMonth - 12;
            }
            if (lunarDate.LunarMonth >= 11 && diff < 4)
            {
                lunarDate.LunarYear -= 1;
            }

            return lunarDate;
        }

        public LunarDate ConvertSolarToLunar(DateTime solarDate, double timeZone)
        {
            return ConvertSolarToLunar(solarDate.Day, solarDate.Month, solarDate.Year, timeZone);
        }
    }
}
