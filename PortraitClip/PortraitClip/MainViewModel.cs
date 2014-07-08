using KLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortraitClip
{
    public class MainViewModel : NotifyBase
    {
        public PortraitTracker Portrait
        {
            get { return GetValue<PortraitTracker>(); }
            private set { SetValue(value); }
        }

        public MainViewModel()
        {
            Portrait = new PortraitTracker();
        }
    }
}
