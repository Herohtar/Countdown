using ReactiveComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;

namespace Countdown
{
    public class DataObject : NotifyDataErrorInfo<DataObject>
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
        private readonly string[] settingsProperties = new[] { "TargetDate", "TextColor", "ShadowColor", "MinumumLevel", "MaximumLevel", "CompletionText", "CountdownFontSize", "CountdownFontFamily", "SelectedMonitor" };

        private readonly Settings settings;

        public DataObject()
        {
            Rules.Add(new DelegateRule<DataObject>("CompletionText", "Completion Text can not be empty!", x => !string.IsNullOrEmpty(x.CompletionText)));
            
            settings = new Settings();

            TargetDate = settings.TargetDate;
            TextColor = settings.TextColor;
            ShadowColor = settings.ShadowColor;
            MinimumLevel = settings.MinimumLevel;
            MaximumLevel = settings.MaximumLevel;
            CompletionText = settings.CompletionText;
            CountdownFontSize = settings.CountdownFontSize;
            CountdownFontFamily = settings.CountdownFontFamily;
            MonitorList = new ObservableCollection<string>();
            foreach (var m in Monitor.AllMonitors)
            {
                MonitorList.Add(m.Name.TrimStart(new char[] { '\\', '.' }) + (m.IsPrimary ? " (Primary)" : ""));
            }
            SelectedMonitor = settings.SelectedMonitor;

            WhenPropertyChanged.Where(x => settingsProperties.Contains(x)).Subscribe(x => saveProperties());
        }

        private void saveProperties()
        {
            settings.TargetDate = TargetDate;
            settings.TextColor = TextColor;
            settings.ShadowColor = ShadowColor;
            settings.MinimumLevel = MinimumLevel;
            settings.MaximumLevel = MaximumLevel;
            settings.CompletionText = CompletionText;
            settings.CountdownFontSize = CountdownFontSize;
            settings.CountdownFontFamily = CountdownFontFamily;
            settings.SelectedMonitor = SelectedMonitor;
            settings.Save();
        }

        public ObservableCollection<string> MonitorList
        {
            get => monitorList;
            set => SetProperty(ref monitorList, value);
        }

        public int SelectedMonitor
        {
            get => selectedMonitor;
            set => SetProperty(ref selectedMonitor, value);
        }

        public FontFamily CountdownFontFamily
        {
            get => countdownFontFamily;
            set => SetProperty(ref countdownFontFamily, value);
        }

        public int CountdownFontSize
        {
            get => countdownFontSize;
            set => SetProperty(ref countdownFontSize, value);
        }

        public string CompletionText
        {
            get => completionText;
            set => SetProperty(ref completionText, value);
        }

        public int MinimumLevel
        {
            get => minimumLevel;
            set => SetProperty(ref minimumLevel, value, "MinimumLevel", "MinimumUnits");
        }

        public Units MinimumUnits => (Units)minimumLevel;

        public int MaximumLevel
        {
            get => maximumLevel;
            set => SetProperty(ref maximumLevel, value, "MaximumLevel", "MaximumUnits");
        }

        public Units MaximumUnits => (Units)maximumLevel;

        public DateTime TargetDate
        {
            get => targetDate;
            set => SetProperty(ref targetDate, value);
        }

        public Brush TextColorBrush => new SolidColorBrush(textColor);

        public Color TextColor
        {
            get => textColor;
            set => SetProperty(ref textColor, value, "TextColor", "TextColorBrush");
        }

        public Color ShadowColor
        {
            get => shadowColor;
            set => SetProperty(ref shadowColor, value);
        }

        public string TimeLeft
        {
            get => timeLeft;
            set => SetProperty(ref timeLeft, value);
        }

        public void UpdateCountdown()
        {
            var difference = targetDate - DateTime.Now;
            difference.MaximumUnits(MaximumUnits);
            difference.MinimumUnits(MinimumUnits);

            TimeLeft = difference.FormattedDifference(CompletionText);
        }
    }
}
