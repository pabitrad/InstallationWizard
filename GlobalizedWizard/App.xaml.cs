using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

/// <summary>
/// The base code has been reffered from
/// https://www.codeproject.com/Articles/31837/Creating-an-Internationalized-Wizard-in-WPF
///
/// ClickOnce deployment
/// https://bohops.com/2017/12/02/clickonce-twice-or-thrice-a-technique-for-social-engineering-and-untrusted-command-execution/
///
/// </summary>
namespace ZingitWizard
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // *******************************************************************
            // TODO - Uncomment one of the lines of code that create a CultureInfo
            // in order to see the application run with localized text in the UI.
            // *******************************************************************

            CultureInfo culture = null;

            // ITALIAN
            // *******************************************************************
            // Thanks to Corrado Cavalli for translating this application's display 
            // text to Italian, as spoken in Italy.
            // Corrado's blog: http://blogs.ugidotnet.org/corrado/Default.aspx
            //
            //culture = new CultureInfo("it-IT");


            // FRENCH
            // *******************************************************************
            // Thanks to Laurent Bugnion for translating this application's display 
            // text to French, as spoken in Switzerland.
            // Laurent's blog: http://www.galasoft.ch/
            //
            //culture = new CultureInfo("fr-CH");


            // GERMAN
            // *******************************************************************
            // Thanks to Marco Goertz , Microsoft Cider Team Senior Development Lead, 
            // for translating this application's display text to German, as spoken in Germany.
            //
            //culture = new CultureInfo("de-DE");

            if (culture != null)
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            FrameworkElement.LanguageProperty.OverrideMetadata(
              typeof(FrameworkElement),
              new FrameworkPropertyMetadata(
                  XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            base.OnStartup(e);
        }
    }
}