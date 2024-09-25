using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace FFmpeg.AutoGen.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FFmpegBinariesHelper.RegisterFFmpegBinaries();

            DecodeAllFramesToImages();
        }


        private static unsafe void DecodeAllFramesToImages()
        {
            // hard-coding for debug

            var url = "C:\\\\Users\\\\Oscar Romero\\\\AppData\\\\Local\\\\Temp\\\\Articulate\\\\Storyline\\\\5dASEOL8dBc\\\\5xbhN9K4QNQ.mp4"; // be advised this file holds 1440 frames


            using (var vsd = new VideoStreamDecoder(url))
            {
                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_NONE;
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using (var vfc = new VideoFrameConverter(sourceSize,
                           sourcePixelFormat,
                           destinationSize,
                           destinationPixelFormat))
                {
                    var stopwatch = Stopwatch.StartNew();


                    // todo: implement seeking

                    var frameNumber = 0;
                    while (vsd.TryDecodeNextFrame(out var frame) && frameNumber < 25)
                    {

                        var convertedFrame = vfc.Convert(frame);

                        var width = convertedFrame.width;
                        var height = convertedFrame.height;

                        var format = PixelFormat.Format24bppRgb;
                        var scan0 = (IntPtr)convertedFrame.data[0];
                        var stride = convertedFrame.linesize[0];

                        using (var bitmap = new Bitmap(width, height, stride, format, scan0))
                            bitmap.Save($"frame.{frameNumber:D8}.jpg", ImageFormat.Jpeg);

                        frameNumber++;

                        stopwatch.Stop();
                        Console.WriteLine($"Generated image in: {stopwatch.Elapsed.ToString()}");
                        stopwatch = Stopwatch.StartNew();
                    }
                }
            }
        }
    }
}
