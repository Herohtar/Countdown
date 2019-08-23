using System;
using System.Configuration;
using System.Windows.Media;

namespace Countdown
{
    public class Settings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue("04/08/2017 16:32:00")]
        public DateTime TargetDate
        {
            get { return (DateTime)(this["TargetDate"]); }
            set { this["TargetDate"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("#FFFFFFFF")]
        public Color TextColor
        {
            get { return (Color)(this["TextColor"]); }
            set { this["TextColor"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("#FF000000")]
        public Color ShadowColor
        {
            get { return (Color)(this["ShadowColor"]); }
            set { this["ShadowColor"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int MinimumLevel
        {
            get { return (int)(this["MinimumLevel"]); }
            set { this["MinimumLevel"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("It's time!")]
        public string CompletionText
        {
            get { return (string)(this["CompletionText"]); }
            set { this["CompletionText"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("24")]
        public int CountdownFontSize
        {
            get { return (int)(this["CountdownFontSize"]); }
            set { this["CountdownFontSize"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("Segoe UI")]
        public FontFamily CountdownFontFamily
        {
            get { return (FontFamily)(this["CountdownFontFamily"]); }
            set { this["CountdownFontFamily"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int SelectedMonitor
        {
            get { return (int)(this["SelectedMonitor"]); }
            set { this["SelectedMonitor"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("5")]
        public int MaximumLevel
        {
            get { return (int)(this["MaximumLevel"]); }
            set { this["MaximumLevel"] = value; }
        }
    }
}
