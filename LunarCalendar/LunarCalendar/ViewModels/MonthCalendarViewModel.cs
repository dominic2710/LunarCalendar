using LunarCalendar.Models;
using LunarCalendar.Utilities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

        const int BUFFER_SIZE = 3;

        public ObservableCollection<ObservableCollection<DayItem>> ListOfMonth { get; set; }        
        public ObservableCollection<DayItem> MonthItems { get; set; }
        public int CurrentPosition { get; set; }
        public int PreviousPosition { get; set; }
        public int DisplayMonth { get; set; }
        public int DisplayYear { get; set; }
        public DateTime DisplayYearMonth { get; set; }
        public ICommand NextMonthCommand { get; set; }
        public ICommand PrevMonthCommand { get; set; }
        public string StrDisplayYearMonth
        {
            get { return string.Format("Tháng {0} - {1}", DisplayYearMonth.Month.ToString(), DisplayYearMonth.Year.ToString()); }
        }

        public MonthCalendarViewModel() {
            DisplayYearMonth = DateTime.Now;
            Task.Run(()=> { LoadData(); });

            NextMonthCommand = new Command(NextMonth);
            PrevMonthCommand = new Command(PrevMonth);
        }

        void AddMonthData(int month, int year, bool isInsert = false)
        {
            MonthItems = new ObservableCollection<DayItem>();

            var beginMonthDayOfWeek = (int)new DateTime(year, month, 1).DayOfWeek;
            var endMonthDayOfWeek = (int)new DateTime(year, month, 1).AddMonths(1).AddDays(-1).DayOfWeek;

            var beginDate = new DateTime(year, month, 1).AddDays(-beginMonthDayOfWeek);
            var endDate = new DateTime(year, month, 1).AddMonths(1).AddDays(6 - endMonthDayOfWeek - 1);

            for (var date = beginDate; date <= endDate; date = date.AddDays(1))
            {
                MonthItems.Add(new DayItem
                {
                    CurrentMonth = month,
                    SolarDay = date,
                    LunarDay = LunarCalendarCalc.GetInstance().ConvertSolarToLunar(date, 7),
                    //LunarDay = GetLunarDate(date),
                    IsSelected = date.Date == DateTime.Now.Date
                });
            }

            OnPropertyChanged(nameof(MonthItems));

            //monthItem.Add(new DayItem
            //{
            //    CurrentMonth = month,
            //    SolarDay = beginDate,
            //    //LunarDay = LunarCalendarCalc.GetInstance().ConvertSolarToLunar(date, 7),
            //    IsSelected = beginDate.Date == DateTime.Now.Date
            //});

            //if (isInsert)
            //    ListOfMonth.Insert(0, monthItem);
            //else
            //    ListOfMonth.Add(monthItem);
        }

        public void LoadData()
        {
            //ListOfMonth = new ObservableCollection<ObservableCollection<DayItem>>();
            //for (int i = -(BUFFER_SIZE * 2); i <= (BUFFER_SIZE * 2); i++)
            //{
            //    var yearMonth = DisplayYearMonth.AddMonths(i);

            //    AddMonthData(yearMonth.Month, yearMonth.Year);
            //}
            //CurrentPosition = (BUFFER_SIZE * 2);
            //PreviousPosition = (BUFFER_SIZE * 2);

            AddMonthData(DisplayYearMonth.Month, DisplayYearMonth.Year);

            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged("DisplayMonth");
            OnPropertyChanged("DisplayYear");
            OnPropertyChanged("ListOfMonth");
            OnPropertyChanged("DisplayYearMonth");
            OnPropertyChanged("PreviousPosition");
            OnPropertyChanged("StrDisplayYearMonth");
        }

        public void NextMonth()
        {
            DisplayYearMonth = DisplayYearMonth.AddMonths(1);
            LoadData();
        }

        public void PrevMonth()
        {
            DisplayYearMonth = DisplayYearMonth.AddMonths(-1);
            LoadData();
        }
        public ICommand CurrentPositionChangedCommand => new Command<int>(async (p) =>
        {
            //On next month
            if (PreviousPosition < p)
            {
                NextMonth();
                PreviousPosition = p;
            }

            //On previous month
            if (PreviousPosition > p)
            {
                PrevMonth();
                PreviousPosition = p;
            }

            if (p > ListOfMonth.Count - BUFFER_SIZE)
            {
                await FetchMoreMonth(ListOfMonth.Last().First().SolarDay, false);
            }

            if (p < BUFFER_SIZE)
            {
                await FetchMoreMonth(ListOfMonth.First().First().SolarDay, true);
            }

            OnPropertyChanged("DisplayMonth");
            OnPropertyChanged("DisplayYear");
            OnPropertyChanged("ListOfMonth");
            OnPropertyChanged("StrDisplayYearMonth");
            OnPropertyChanged("PreviousPosition");
        });

        async Task FetchMoreMonth(DateTime beginYearMonth, bool isDirectionPrev)
        {
            await Task.Run(() =>
            {
                for (int i = 1; i <= BUFFER_SIZE; i++)
                {
                    var yearMonth = beginYearMonth.AddMonths(i);

                    AddMonthData(yearMonth.Month, yearMonth.Year, isDirectionPrev);
                }
            });
        }

        DateTime DuongSangAm(DateTime ngayDuong)
        {
            // Lấy lịch âm của Việt Nam
            var lichAmVN = new CultureInfo("vi-VN");

            // Tạo một Calendar sử dụng lịch âm của Việt Nam
            var calendar = lichAmVN.DateTimeFormat.Calendar;

            // Chuyển đổi ngày dương lịch sang ngày âm lịch
            return calendar.ToDateTime(ngayDuong.Year, ngayDuong.Month, ngayDuong.Day, 0, 0, 0, 0);
        }

        LunarDate GetLunarDate(DateTime ngayDuong)
        {
            var date = DuongSangAm(ngayDuong);

            return new LunarDate
            {
                LunarDay = date.Day,
                LunarMonth = date.Month,
                LunarYear = date.Year,
                IsLeapMonth = false
            };
        }
    }
}
