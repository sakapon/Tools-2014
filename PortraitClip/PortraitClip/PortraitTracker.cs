using KLibrary.ComponentModel;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PortraitClip
{
    public class PortraitTracker : NotifyBase
    {
        public WriteableBitmap ClipBitmap
        {
            get { return GetValue<WriteableBitmap>(); }
            private set { SetValue(value); }
        }

        Skeleton[] skeletons;

        public PortraitTracker()
        {
            KinectContext.Current.AllFramesReady += Context_AllFramesReady;
            KinectContext.Current.BackgroundRemovedFrameReady += BackgroundRemovedFrameReady;
        }

        void Context_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            var backgroundRemovedColorStream = KinectContext.Current.BackgroundRemovedColorStream;
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
                        if (skeleton != null)
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
    }
}
