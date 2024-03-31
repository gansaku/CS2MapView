using log4net;
using System.Globalization;
namespace CS2MapView.Form
{
    internal static class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();

                var args = Environment.GetCommandLineArgs();
                foreach(var arg in args)
                {
                    if (arg.StartsWith("-locale="))
                    {
                        var locale = arg.Substring(arg.IndexOf("=") + 1);
                        var culture = CultureInfo.GetCultureInfo(locale);
                        CultureInfo.CurrentCulture = culture;
                        CultureInfo.CurrentUICulture = culture;
                        CultureInfo.DefaultThreadCurrentCulture = culture;
                        CultureInfo.DefaultThreadCurrentUICulture = culture;
                    }
                }

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {

                Logger.Error("error", ex);
                MessageBox.Show(ex.Message, "ERROR");
            }
        }
    }
}