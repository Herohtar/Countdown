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
        private int maximumLevel;
        private Units maximumUnits;
        private string completionText;
        private int countdownFontSize;
        private FontFamily countdownFontFamily;
        private int selectedMonitor;
        private ObservableCollection<string> monitorList;

        public DataObject()
        {
            this.TargetDate = Properties.Settings.Default.TargetDate;
            this.TextColor = Properties.Settings.Default.TextColor;
            this.ShadowColor = Properties.Settings.Default.ShadowColor;
            this.MinimumLevel = Properties.Settings.Default.MinimumLevel;
            this.MaximumLevel = Properties.Settings.Default.MaximumLevel;
            this.CompletionText = Properties.Settings.Default.CompletionText;
            this.CountdownFontSize = Properties.Settings.Default.CountdownFontSize;
            this.CountdownFontFamily = Properties.Settings.Default.CountdownFontFamily;
            this.MonitorList = new ObservableCollection<string>();
            foreach (Monitor m in Monitor.AllMonitors)
            {
                this.MonitorList.Add(m.Name.TrimStart(new char[] { '\\', '.' }) + (m.IsPrimary ? " (Primary)" : ""));
            }
            this.SelectedMonitor = Properties.Settings.Default.SelectedMonitor;
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

        public int MaximumLevel
        {
            get { return this.maximumLevel; }
            set
            {
                if (this.maximumLevel != value)
                {
                    this.maximumLevel = value;
                    this.MaximumUnits = (Units)value;
                    Properties.Settings.Default.MaximumLevel = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("MaximumLevel");
                }
            }
        }

        public Units MaximumUnits
        {
            get { return this.maximumUnits; }
            set
            {
                if (this.maximumUnits != value)
                {
                    this.maximumUnits = value;
                    RaisePropertyChangedEvent("MaximumUnits");
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
            difference.MaximumUnits(this.MaximumUnits);
            difference.MinumumUnits(this.MinimumUnits);

            this.TimeLeft = difference.FormattedDifference(this.CompletionText);
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
