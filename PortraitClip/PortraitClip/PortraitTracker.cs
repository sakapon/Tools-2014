using KLibrary.ComponentModel;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PortraitClip
{
    public class PortraitTracker : NotifyBase, IDisposable
    {
        const ColorImageFormat TheColorImageFormat = ColorImageFormat.RgbResolution640x480Fps30;
        const DepthImageFormat TheDepthImageFormat = DepthImageFormat.Resolution320x240Fps30;
        const int InvalidSkeletonId = -1;

        KinectSensorChooser sensorChooser;
        BackgroundRemovedColorStream backgroundRemovedColorStream;
        Skeleton[] skeletons;
        ValueShortCache<int> skeletonId = new ValueShortCache<int>(InvalidSkeletonId);

        public WriteableBitmap ClipBitmap
        {
            get { return GetValue<WriteableBitmap>(); }
            private set { SetValue(value); }
        }

        public bool HasSkeleton
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }

        public PortraitTracker()
        {
            sensorChooser = new KinectSensorChooser();
            sensorChooser.KinectChanged += KinectChanged;
            sensorChooser.Start();
        }

        ~PortraitTracker()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (backgroundRemovedColorStream != null)
                {
                    backgroundRemovedColorStream.BackgroundRemovedFrameReady -= BackgroundRemovedFrameReady;
                    backgroundRemovedColorStream.Dispose();
                    backgroundRemovedColorStream = null;
                }

                if (sensorChooser.Status == ChooserStatus.SensorStarted)
                {
                    sensorChooser.Stop();
                }
            }
            catch (Exception)
            {
            }
        }

        void KinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (e.OldSensor != null)
            {
                try
                {
                    if (backgroundRemovedColorStream != null)
                    {
                        backgroundRemovedColorStream.BackgroundRemovedFrameReady -= BackgroundRemovedFrameReady;
                        backgroundRemovedColorStream.Dispose();
                        backgroundRemovedColorStream = null;
                    }

                    e.OldSensor.AllFramesReady -= AllFramesReady;
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
                    e.NewSensor.ColorStream.Enable(TheColorImageFormat);
                    e.NewSensor.DepthStream.Enable(TheDepthImageFormat);
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

                    backgroundRemovedColorStream = new BackgroundRemovedColorStream(e.NewSensor);
                    backgroundRemovedColorStream.Enable(TheColorImageFormat, TheDepthImageFormat);

                    e.NewSensor.AllFramesReady += AllFramesReady;
                    backgroundRemovedColorStream.BackgroundRemovedFrameReady += BackgroundRemovedFrameReady;
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }

        void AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (backgroundRemovedColorStream == null) return;

            try
            {
                using (var cf = e.OpenColorImageFrame())
                {
                    if (cf != null)
                    {
                        backgroundRemovedColorStream.ProcessColor(cf.GetRawPixelData(), cf.Timestamp);
                    }
                }

                using (var df = e.OpenDepthImageFrame())
                {
                    if (df != null)
                    {
                        backgroundRemovedColorStream.ProcessDepth(df.GetRawPixelData(), df.Timestamp);
                    }
                }

                using (var sf = e.OpenSkeletonFrame())
                {
                    if (sf != null)
                    {
                        if (skeletons == null)
                        {
                            skeletons = new Skeleton[sf.SkeletonArrayLength];
                        }
                        sf.CopySkeletonDataTo(skeletons);
                        backgroundRemovedColorStream.ProcessSkeleton(skeletons, sf.Timestamp);

                        var skeleton = skeletons
                            .Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                            .OrderBy(s => s.Position.Z)
                            .FirstOrDefault();

                        HasSkeleton = skeleton != null;
                        skeletonId.UpdateValue(skeleton != null ? skeleton.TrackingId : InvalidSkeletonId);
                        if (skeletonId.Current != InvalidSkeletonId && skeletonId.Previous != skeletonId.Current)
                        {
                            backgroundRemovedColorStream.SetTrackedPlayer(skeleton.TrackingId);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        void BackgroundRemovedFrameReady(object sender, BackgroundRemovedColorFrameReadyEventArgs e)
        {
            try
            {
                using (var bf = e.OpenBackgroundRemovedColorFrame())
                {
                    if (bf != null)
                    {
                        if (ClipBitmap == null)
                        {
                            ClipBitmap = new WriteableBitmap(bf.Width, bf.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
                        }
                        ClipBitmap.WritePixels(new Int32Rect(0, 0, ClipBitmap.PixelWidth, ClipBitmap.PixelHeight), bf.GetRawPixelData(), 4 * ClipBitmap.PixelWidth, 0);
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
