using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Countdown
{
    public class DataObject : INotifyPropertyChanged
    {
        private string timeLeft;
        private DateTime targetDate;
        private Brush textColorBrush;
        private Color textColor;
        private Color shadowColor;
        private int minimumLevel;
        private Units minimumUnits;
        private string completionText;
        private int countdownFontSize;
        private FontFamily countdownFontFamily;
        private int selectedMonitor;
        private ObservableCollection<string> monitorList;
        private bool useWeeks;

        public enum Units
        {
            Milliseconds,
            Seconds,
            Minutes,
            Hours,
            Days
        }

        public DataObject()
        {
            this.TargetDate = Properties.Settings.Default.TargetDate;
            this.TextColor = Properties.Settings.Default.TextColor;
            this.ShadowColor = Properties.Settings.Default.ShadowColor;
            this.MinimumLevel = Properties.Settings.Default.MinimumLevel;
            this.CompletionText = Properties.Settings.Default.CompletionText;
            this.CountdownFontSize = Properties.Settings.Default.CountdownFontSize;
            this.CountdownFontFamily = Properties.Settings.Default.CountdownFontFamily;
            this.MonitorList = new ObservableCollection<string>();
            foreach (Monitor m in Monitor.AllMonitors)
            {
                this.MonitorList.Add(m.Name.TrimStart(new char[] { '\\', '.' }) + (m.IsPrimary ? " (Primary)" : ""));
            }
            this.SelectedMonitor = Properties.Settings.Default.SelectedMonitor;
            this.UseWeeks = Properties.Settings.Default.UseWeeks;
        }

        public bool UseWeeks
        {
            get { return this.useWeeks; }
            set
            {
                if (this.useWeeks != value)
                {
                    this.useWeeks = value;
                    Properties.Settings.Default.UseWeeks = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("UseWeeks");
                }
            }
        }

        public ObservableCollection<string> MonitorList
        {
            get { return this.monitorList; }
            set
            {
                if (this.monitorList != value)
                {
                    this.monitorList = value;
                    RaisePropertyChangedEvent("MonitorList");
                }
            }
        }

        public int SelectedMonitor
        {
            get { return this.selectedMonitor; }
            set
            {
                if (this.selectedMonitor != value)
                {
                    this.selectedMonitor = value;
                    Properties.Settings.Default.SelectedMonitor = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("SelectedMonitor");
                }
            }
        }

        public FontFamily CountdownFontFamily
        {
            get { return this.countdownFontFamily; }
            set
            {
                if (this.countdownFontFamily != value)
                {
                    this.countdownFontFamily = value;
                    Properties.Settings.Default.CountdownFontFamily = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("CountdownFontFamily");
                }
            }
        }

        public int CountdownFontSize
        {
            get { return this.countdownFontSize; }
            set
            {
                if (this.countdownFontSize != value)
                {
                    this.countdownFontSize = value;
                    Properties.Settings.Default.CountdownFontSize = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("CountdownFontSize");
                }
            }
        }

        public string CompletionText
        {
            get { return this.completionText; }
            set
            {
                if (this.completionText != value)
                {
                    this.completionText = value;
                    Properties.Settings.Default.CompletionText = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("CompletionText");
                }
            }
        }

        public int MinimumLevel
        {
            get { return this.minimumLevel; }
            set
            {
                if (this.minimumLevel != value)
                {
                    this.minimumLevel = value;
                    this.MinimumUnits = (Units)value;
                    Properties.Settings.Default.MinimumLevel = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("MinimumLevel");
                }
            }
        }

        public Units MinimumUnits
        {
            get { return this.minimumUnits; }
            set
            {
                if (this.minimumUnits != value)
                {
                    this.minimumUnits = value;
                    RaisePropertyChangedEvent("MinimumUnits");
                }
            }
        }

        public DateTime TargetDate
        {
            get { return this.targetDate; }
            set
            {
                if (this.targetDate != value)
                {
                    this.targetDate = value;
                    Properties.Settings.Default.TargetDate = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("TargetDate");
                }
            }
        }

        public Brush TextColorBrush
        {
            get { return this.textColorBrush; }
            set
            {
                if (this.textColorBrush != value)
                {
                    this.textColorBrush = value;
                    RaisePropertyChangedEvent("TextColorBrush");
                }
            }
        }

        public Color TextColor
        {
            get { return this.textColor; }
            set
            {
                if (this.textColor != value)
                {
                    this.textColor = value;
                    this.TextColorBrush = new SolidColorBrush(value);
                    Properties.Settings.Default.TextColor = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("TextColor");
                }
            }
        }

        public Color ShadowColor
        {
            get { return this.shadowColor; }
            set
            {
                if (this.shadowColor != value)
                {
                    this.shadowColor = value;
                    Properties.Settings.Default.ShadowColor = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("ShadowColor");
                }
            }
        }

        public string TimeLeft
        {
            get { return this.timeLeft; }
            set
            {
                if (this.timeLeft != value)
                {
                    this.timeLeft = value;
                    RaisePropertyChangedEvent("TimeLeft");
                }
            }
        }

        public void UpdateCountdown()
        {
            TimeSpan difference = this.targetDate - DateTime.Now;
            difference.UseWeeks(this.UseWeeks);
            List<string> timeStrings = new List<string>();

            if (difference.Weeks() > 0)
            {
                timeStrings.Add(formatUnits(difference.Weeks(), "week"));
            }
            if ((difference.Days() > 0) || (timeStrings.Count > 0))
            {
                timeStrings.Add(formatUnits(difference.Days(), "day"));
            }
            if (((this.MinimumUnits <= Units.Hours) || (timeStrings.Count == 0)) && ((difference.Hours > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Hours, "hour"));
            }
            if (((this.MinimumUnits <= Units.Minutes) || (timeStrings.Count == 0)) && ((difference.Minutes > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Minutes, "minute"));
            }
            if (((this.MinimumUnits <= Units.Seconds) || (timeStrings.Count == 0)) && ((difference.Seconds > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Seconds, "second"));
            }
            if (((this.MinimumUnits <= Units.Milliseconds) || (timeStrings.Count == 0)) && ((difference.Milliseconds > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Milliseconds, "millisecond"));
            }

            this.TimeLeft = (timeStrings.Count > 0) ? String.Join(", ", timeStrings) : this.CompletionText;
        }

        private string formatUnits(int count, string units)
        {
            if (units == "millisecond")
            {
                return String.Format("{0:D3} {1}{2}", count, units, (count == -1) ? "" : "s");
            }

            return String.Format("{0} {1}{2}", count, units, (count == 1) ? "" : "s");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
