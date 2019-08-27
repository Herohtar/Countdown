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
            get => (DateTime)this["TargetDate"];
            set => this["TargetDate"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("#FFFFFFFF")]
        public Color TextColor
        {
            get => (Color)this["TextColor"];
            set => this["TextColor"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("#FF000000")]
        public Color ShadowColor
        {
            get => (Color)this["ShadowColor"];
            set => this["ShadowColor"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int MinimumLevel
        {
            get => (int)this["MinimumLevel"];
            set => this["MinimumLevel"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("It's time!")]
        public string CompletionText
        {
            get => (string)this["CompletionText"];
            set => this["CompletionText"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("24")]
        public int CountdownFontSize
        {
            get => (int)this["CountdownFontSize"];
            set => this["CountdownFontSize"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("Segoe UI")]
        public FontFamily CountdownFontFamily
        {
            get => (FontFamily)this["CountdownFontFamily"];
            set => this["CountdownFontFamily"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int SelectedMonitor
        {
            get => (int)this["SelectedMonitor"];
            set => this["SelectedMonitor"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("5")]
        public int MaximumLevel
        {
            get => (int)this["MaximumLevel"];
            set => this["MaximumLevel"] = value;
        }
    }
}
