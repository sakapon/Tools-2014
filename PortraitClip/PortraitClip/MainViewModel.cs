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
        static readonly SolidColorBrush OpaqueBorder = new SolidColorBrush(Color.FromArgb(51, 0, 0, 0));

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

        public MainViewModel()
        {
            Portrait = new PortraitTracker();

            BackgroundBrush = OpaqueBackground;
            BorderBrush = OpaqueBorder;

            Portrait.AddPropertyChangedHandler("HasSkeleton", () =>
            {
                BackgroundBrush = Portrait.HasSkeleton ? Transparent : OpaqueBackground;
                BorderBrush = Portrait.HasSkeleton ? Transparent : OpaqueBorder;
            });
        }
    }
}
