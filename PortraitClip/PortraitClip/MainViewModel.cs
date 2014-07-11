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

        public SolidColorBrush BackgroundBrush
        {
            get { return GetValue<SolidColorBrush>(); }
            private set { SetValue(value); }
        }

        public SolidColorBrush BorderBrush
        {
            get { return GetValue<SolidColorBrush>(); }
            private set { SetValue(value); }
        }

        public bool ShowBorder
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public MainViewModel()
        {
            Portrait = new PortraitTracker();

            RefreshBorder();
            Portrait.AddPropertyChangedHandler("HasSkeleton", () => RefreshBorder());
            AddPropertyChangedHandler("ShowBorder", () => RefreshBorder());
        }

        void RefreshBorder()
        {
            var showBorder = ShowBorder || !Portrait.HasSkeleton;
            BackgroundBrush = showBorder ? OpaqueBackground : Transparent;
            BorderBrush = showBorder ? OpaqueBorder : Transparent;
        }
    }
}
