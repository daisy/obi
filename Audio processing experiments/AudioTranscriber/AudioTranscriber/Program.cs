using FFMpegCore;

namespace AudioTranscriber
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            string ffmpegFolder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ffmpeg");

            GlobalFFOptions.Configure(options =>
            {
                options.BinaryFolder =
                    ffmpegFolder;
            });

            ApplicationConfiguration.Initialize();

            Application.Run(
                new MainForm());
        }
    }
}





//namespace AudioTranscriber
//{
//    internal static class Program
//    {
//        [STAThread]
//        static void Main()
//        {
//            ApplicationConfiguration.Initialize();

//            Application.Run(new MainForm());
//        }
//    }
//}