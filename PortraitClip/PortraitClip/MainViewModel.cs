using KLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace PortraitClip
{
    public class MainViewModel : NotifyBase
    {
        static readonly SolidColorBrush Transparent = new SolidColorBrush(Colors.Transparent);
        static readonly SolidColorBrush OpaqueBackground = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255));
        static readonly SolidColorBrush OpaqueBorder = new SolidColorBrush(Color.FromArgb(128, 16, 48, 224));

        public PortraitTracker Portrait
        {
            get { return GetValue<PortraitTracker>(); }
            private set { SetValue(value); }
        }

        public bool IsSettingMode
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        [DependentOn("IsSettingMode")]
        public bool ShowBorder
        {
            get { return IsSettingMode || !Portrait.HasSkeleton; }
        }

        [DependentOn("ShowBorder")]
        public SolidColorBrush BackgroundBrush
        {
            get { return ShowBorder ? OpaqueBackground : Transparent; }
        }

        [DependentOn("ShowBorder")]
        public SolidColorBrush BorderBrush
        {
            get { return ShowBorder ? OpaqueBorder : Transparent; }
        }

        public MainViewModel()
        {
            Portrait = new PortraitTracker();
            Portrait.AddPropertyChangedHandler("HasSkeleton", () => NotifyPropertyChanged("ShowBorder"));
        }
    }
}
