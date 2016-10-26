﻿using NLog;
using System.Xml.Serialization;

namespace Rubberduck.Settings
{
    public interface IGeneralSettings
    {
        DisplayLanguageSetting Language { get; set; }
        bool ShowSplash { get; set; }
        bool AutoSaveEnabled { get; set; }
        int AutoSavePeriod { get; set; }
        char Delimiter { get; set; }
        int MinimumLogLevel { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GeneralSettings : IGeneralSettings
    {
        public DisplayLanguageSetting Language { get; set; }
        public bool ShowSplash { get; set; }
        public bool AutoSaveEnabled { get; set; }
        public int AutoSavePeriod { get; set; }
        public char Delimiter { get; set; }
        public int MinimumLogLevel { get; set; }

        public GeneralSettings()
        {
            Language = new DisplayLanguageSetting("en-US");
            ShowSplash = true;
            AutoSaveEnabled = false;
            AutoSavePeriod = 10;
            Delimiter = '.';
            MinimumLogLevel = LogLevel.Off.Ordinal;
        }
    }
}