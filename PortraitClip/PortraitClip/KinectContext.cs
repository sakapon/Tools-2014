using KLibrary.ComponentModel;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
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
            private set { SetValue(value); }
        }

        public KinectSensor Sensor
        {
            get { return GetValue<KinectSensor>(); }
            private set { SetValue(value); }
        }

        public BackgroundRemovedColorStream BackgroundRemovedColorStream
        {
            get { return GetValue<BackgroundRemovedColorStream>(); }
            private set { SetValue(value); }
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
                if (BackgroundRemovedColorStream != null)
                {
                    BackgroundRemovedColorStream.Dispose();
                    BackgroundRemovedColorStream = null;
                }
                if (SensorChooser.Status == ChooserStatus.SensorStarted)
                {
                    SensorChooser.Stop();
                }
            }
            catch (Exception)
            {
            }
        }

        event EventHandler<AllFramesReadyEventArgs> allFramesReady = (o, e) => { };
        public event EventHandler<AllFramesReadyEventArgs> AllFramesReady
        {
            add
            {
                allFramesReady += value;
                if (Sensor != null)
                {
                    Sensor.AllFramesReady += value;
                }
            }
            remove
            {
                allFramesReady -= value;
                if (Sensor != null)
                {
                    Sensor.AllFramesReady -= value;
                }
            }
        }

        event EventHandler<BackgroundRemovedColorFrameReadyEventArgs> backgroundRemovedFrameReady = (o, e) => { };
        public event EventHandler<BackgroundRemovedColorFrameReadyEventArgs> BackgroundRemovedFrameReady
        {
            add
            {
                backgroundRemovedFrameReady += value;
                if (BackgroundRemovedColorStream != null)
                {
                    BackgroundRemovedColorStream.BackgroundRemovedFrameReady += value;
                }
            }
            remove
            {
                backgroundRemovedFrameReady -= value;
                if (BackgroundRemovedColorStream != null)
                {
                    BackgroundRemovedColorStream.BackgroundRemovedFrameReady -= value;
                }
            }
        }

        void SensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (e.OldSensor != null)
            {
                try
                {
                    if (BackgroundRemovedColorStream != null)
                    {
                        BackgroundRemovedColorStream.BackgroundRemovedFrameReady -= backgroundRemovedFrameReady;
                        BackgroundRemovedColorStream.Dispose();
                        BackgroundRemovedColorStream = null;
                    }

                    e.OldSensor.AllFramesReady -= allFramesReady;
                    e.OldSensor.ColorStream.Disable();
                    e.OldSensor.DepthStream.Disable();
                    e.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (e.NewSensor != null)
            {
                try
                {
                    e.NewSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    e.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                    e.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        e.NewSensor.DepthStream.Range = DepthRange.Near;
                        e.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        e.NewSensor.DepthStream.Range = DepthRange.Default;
                        e.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                    e.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;


                    BackgroundRemovedColorStream = new BackgroundRemovedColorStream(e.NewSensor);
                    BackgroundRemovedColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution320x240Fps30);

                    e.NewSensor.AllFramesReady += allFramesReady;
                    BackgroundRemovedColorStream.BackgroundRemovedFrameReady += backgroundRemovedFrameReady;
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            Sensor = e.NewSensor;
        }
    }
}
