using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ValidationBase;

namespace Countdown
{
    public class DataObject : ValidatableObject<DataObject>
    {
        private string timeLeft;
        private DateTime targetDate;
        private Color textColor;
        private Color shadowColor;
        private int minimumLevel;
        private int maximumLevel;
        private string completionText;
        private int countdownFontSize;
        private FontFamily countdownFontFamily;
        private int selectedMonitor;
        private ObservableCollection<string> monitorList;

        private Settings settings;

        public DataObject()
        {
            Rules.Add(new DelegateRule<DataObject>("CompletionText", "Completion Text can not be empty!", x => !String.IsNullOrEmpty(x.CompletionText)));

            settings = new Settings();

            this.TargetDate = settings.TargetDate;
            this.TextColor = settings.TextColor;
            this.ShadowColor = settings.ShadowColor;
            this.MinimumLevel = settings.MinimumLevel;
            this.MaximumLevel = settings.MaximumLevel;
            this.CompletionText = settings.CompletionText;
            this.CountdownFontSize = settings.CountdownFontSize;
            this.CountdownFontFamily = settings.CountdownFontFamily;
            this.MonitorList = new ObservableCollection<string>();
            foreach (Monitor m in Monitor.AllMonitors)
            {
                this.MonitorList.Add(m.Name.TrimStart(new char[] { '\\', '.' }) + (m.IsPrimary ? " (Primary)" : ""));
            }
            this.SelectedMonitor = settings.SelectedMonitor;

            WhenPropertyChanged.Subscribe(x => saveProperties());
        }

        private void saveProperties()
        {
            settings.TargetDate = this.TargetDate;
            settings.TextColor = this.TextColor;
            settings.ShadowColor = this.ShadowColor;
            settings.MinimumLevel = this.MinimumLevel;
            settings.MaximumLevel = this.MaximumLevel;
            settings.CompletionText = this.CompletionText;
            settings.CountdownFontSize = this.CountdownFontSize;
            settings.CountdownFontFamily = this.CountdownFontFamily;
            settings.SelectedMonitor = this.SelectedMonitor;
            settings.Save();
        }

        public ObservableCollection<string> MonitorList
        {
            get { return this.monitorList; }
            set
            {
                SetProperty(ref this.monitorList, value);
            }
        }

        public int SelectedMonitor
        {
            get { return this.selectedMonitor; }
            set
            {
                SetProperty(ref this.selectedMonitor, value);
            }
        }

        public FontFamily CountdownFontFamily
        {
            get { return this.countdownFontFamily; }
            set
            {
                SetProperty(ref this.countdownFontFamily, value);
            }
        }

        public int CountdownFontSize
        {
            get { return this.countdownFontSize; }
            set
            {
                SetProperty(ref this.countdownFontSize, value);
            }
        }

        public string CompletionText
        {
            get { return this.completionText; }
            set
            {
                SetProperty(ref this.completionText, value);
            }
        }

        public int MinimumLevel
        {
            get { return this.minimumLevel; }
            set
            {
                SetProperty(ref this.minimumLevel, value, "MinimumLevel", "MinimumUnits");
            }
        }

        public Units MinimumUnits
        {
            get { return (Units)this.minimumLevel; }
            private set { }
        }

        public int MaximumLevel
        {
            get { return this.maximumLevel; }
            set
            {
                SetProperty(ref this.maximumLevel, value, "MaximumLevel", "MaximumUnits");
            }
        }

        public Units MaximumUnits
        {
            get { return (Units)this.maximumLevel; }
            private set { }
        }

        public DateTime TargetDate
        {
            get { return this.targetDate; }
            set
            {
                SetProperty(ref this.targetDate, value);
            }
        }

        public Brush TextColorBrush
        {
            get { return new SolidColorBrush(this.textColor); }
            private set { }
        }

        public Color TextColor
        {
            get { return this.textColor; }
            set
            {
                SetProperty(ref this.textColor, value, "TextColor", "TextColorBrush");
            }
        }

        public Color ShadowColor
        {
            get { return this.shadowColor; }
            set
            {
                SetProperty(ref this.shadowColor, value);
            }
        }

        public string TimeLeft
        {
            get { return this.timeLeft; }
            set
            {
                SetProperty(ref this.timeLeft, value);
            }
        }

        public void UpdateCountdown()
        {
            TimeSpan difference = this.targetDate - DateTime.Now;
            difference.MaximumUnits(this.MaximumUnits);
            difference.MinimumUnits(this.MinimumUnits);

            this.TimeLeft = difference.FormattedDifference(this.CompletionText);
        }
    }
}
