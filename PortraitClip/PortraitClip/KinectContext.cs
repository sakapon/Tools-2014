using KLibrary.ComponentModel;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortraitClip
{
    public class KinectContext : NotifyBase, IDisposable
    {
        public static KinectContext Current { get; private set; }

        static KinectContext()
        {
            Current = new KinectContext();
            AppDomain.CurrentDomain.ProcessExit += (o, e) =>
            {
                ((IDisposable)Current).Dispose();
            };
        }

        public KinectSensorChooser SensorChooser
        {
            get { return GetValue<KinectSensorChooser>(); }
            set { SetValue(value); }
        }

        public KinectSensor Sensor
        {
            get { return GetValue<KinectSensor>(); }
            set { SetValue(value); }
        }

        public KinectContext()
        {
            SensorChooser = new KinectSensorChooser();
            SensorChooser.KinectChanged += SensorChooser_KinectChanged;
            SensorChooser.Start();
        }

        ~KinectContext()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (SensorChooser.Status == ChooserStatus.SensorStarted)
                {
                    SensorChooser.Stop();
                }
            }
            catch (Exception)
            {
            }
        }

        void SensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
        }
    }
}
