using LunarCalendar.Models;
using LunarCalendar.Utilities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LunarCalendar.ViewModels
{
    public class MonthCalendarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<ObservableCollection<DayItem>> ListOfMonth { get; set; }
        public int CurrentPosition { get; set; }
        private int PreviousPosition { get; set; }
        public int DisplayMonth { get; set; }
        public int DisplayYear { get; set; }
        public DateTime DisplayYearMonth { get; set; }
        public string StrDisplayYearMonth
        {
            get { return string.Format("Tháng {0} - {1}", DisplayYearMonth.Month.ToString(), DisplayYearMonth.Year.ToString()); }
        }

        public MonthCalendarViewModel() {
            //DisplayMonth = DateTime.Now.Month;
            //DisplayYear = DateTime.Now.Year;
            DisplayYearMonth = DateTime.Now;
            Task.Run(()=> { LoadData(); });

        }

        void AddMonthData(int month, int year, bool isInsert = false)
        {
            var monthItem = new ObservableCollection<DayItem>();

            var beginMonthDayOfWeek = (int)new DateTime(year, month, 1).DayOfWeek;
            var endMonthDayOfWeek = (int)new DateTime(year, month, 1).AddMonths(1).AddDays(-1).DayOfWeek;

            var beginDate = new DateTime(year, month, 1).AddDays(-beginMonthDayOfWeek);
            var endDate = new DateTime(year, month, 1).AddMonths(1).AddDays(6 - endMonthDayOfWeek - 1);

            for (var date = beginDate; date <= endDate; date = date.AddDays(1))
            {
                monthItem.Add(new DayItem
                {
                    CurrentMonth = month,
                    SolarDay = date,
                    LunarDay = LunarCalendarCalc.GetInstance().ConvertSolarToLunar(date, 7),
                    IsSelected = date.Date == DateTime.Now.Date
                });
            }
            if (isInsert)
                ListOfMonth.Insert(0, monthItem);
            else
                ListOfMonth.Add(monthItem);
        }

        public void LoadData()
        {
            ListOfMonth = new ObservableCollection<ObservableCollection<DayItem>>();
            for (int i = -12; i <= 12; i++)
            {
                //var month = DisplayMonth + i;
                //var year = DisplayYear;
                //if (month > 12) { year++; month = 1; }
                //if (month < 1) { year--; month = 12; }

                var yearMonth = DisplayYearMonth.AddMonths(i);

                AddMonthData(yearMonth.Month, yearMonth.Year);
            }
            CurrentPosition = 12;
            PreviousPosition = 12;

            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged("DisplayMonth");
            OnPropertyChanged("DisplayYear");
            OnPropertyChanged("ListOfMonth");
            OnPropertyChanged("DisplayYearMonth");
        }

        public void NextMonth()
        {
            //DisplayMonth++;
            //if (DisplayMonth > 12)
            //{
            //    DisplayMonth = 1;
            //    DisplayYear++;
            //}

            DisplayYearMonth = DisplayYearMonth.AddMonths(1);
        }

        public void PrevMonth()
        {
            //DisplayMonth--;
            //if (DisplayMonth < 1)
            //{
            //    DisplayMonth = 12;
            //    DisplayMonth--;
            //}

            DisplayYearMonth = DisplayYearMonth.AddMonths(-1);
        }
        public ICommand CurrentPositionChangedCommand => new Command<int>((p) =>
        {
            //On next month
            if (PreviousPosition < p)
            {
                NextMonth();
                //if (CurrentPosition == ListOfMonth.Count - 1)
                //{
                //    var month = DisplayMonth + 1;
                //    var year = DisplayYear;
                //    if (month > 12) { year++; month = 1; }

                //    AddMonthData(month, year);
                //}
                PreviousPosition = p;
            }

            //On previous month
            if (PreviousPosition > p)
            {
                PrevMonth();
                //if (CurrentPosition == 0)
                //{
                //    var month = DisplayMonth - 1;
                //    var year = DisplayYear;
                //    if (month < 1) { year--; month = 12; }

                //    AddMonthData(month, year, true);
                ////}
                //if (CurrentPosition == 0) CurrentPosition = 1;
                PreviousPosition = p;
            }

            OnPropertyChanged("DisplayMonth");
            OnPropertyChanged("DisplayYear");
            OnPropertyChanged("ListOfMonth");
            OnPropertyChanged("StrDisplayYearMonth");
        });
    }
}
