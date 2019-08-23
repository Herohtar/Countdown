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

        public DataObject()
        {
            Rules.Add(new DelegateRule<DataObject>("CompletionText", "Completion Text can not be empty!", x => !String.IsNullOrEmpty(x.CompletionText)));

            this.TargetDate = DateTime.Now;
            this.TextColor = System.Windows.Media.Colors.White;
            this.ShadowColor = System.Windows.Media.Colors.Black;
            this.MinimumLevel = 0;
            this.MaximumLevel = 5;
            this.CompletionText = "It's time!";
            this.CountdownFontSize = 24;
            this.CountdownFontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            this.MonitorList = new ObservableCollection<string>();
            foreach (Monitor m in Monitor.AllMonitors)
            {
                this.MonitorList.Add(m.Name.TrimStart(new char[] { '\\', '.' }) + (m.IsPrimary ? " (Primary)" : ""));
            }
            this.SelectedMonitor = 0;

            WhenPropertyChanged.Subscribe(x => saveProperties());
        }

        private void saveProperties()
        {
            /* Properties.Settings.Default.TargetDate = this.TargetDate;
            Properties.Settings.Default.TextColor = this.TextColor;
            Properties.Settings.Default.ShadowColor = this.ShadowColor;
            Properties.Settings.Default.MinimumLevel = this.MinimumLevel;
            Properties.Settings.Default.MaximumLevel = this.MaximumLevel;
            Properties.Settings.Default.CompletionText = this.CompletionText;
            Properties.Settings.Default.CountdownFontSize = this.CountdownFontSize;
            Properties.Settings.Default.CountdownFontFamily = this.CountdownFontFamily;
            Properties.Settings.Default.SelectedMonitor = this.SelectedMonitor;
            Properties.Settings.Default.Save(); */
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
