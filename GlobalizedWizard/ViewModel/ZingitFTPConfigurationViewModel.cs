using System;
using System.Windows.Input;
using System.Net;
using System.Windows.Forms;

using ZingitWizard.Command;

namespace ZingitWizard.ViewModel
{
    class ZingitFTPConfigurationViewModel : ZingitWizardPageViewModelBase
    {
        RelayCommand _testConnectionCommand;

        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return "FTP Configuration"; }
        }

        public ICommand TestConnectionCommand
        {
            get
            {
                if (_testConnectionCommand == null)
                    _testConnectionCommand = new RelayCommand(() => this.TestFTPServerConnection());

                return _testConnectionCommand;
            }
        }

        public override bool CanMoveToNextPage()
        {
            AppConfig.EncryptAndSaveData();

            return true;
        }
   
        private void TestFTPServerConnection()
        {
            try
            {
                string hostName = AppConfig.FTPHost.Trim();
                if(string.IsNullOrEmpty(hostName) == true)
                {
                    MessageBox.Show("Please enter FTP host name.");
                    return;
                }
                
                if (string.IsNullOrEmpty(AppConfig.FTPUserName.Trim()) == true)
                {
                    MessageBox.Show("Please enter user name.");
                    return;
                }
 
                if (hostName.StartsWith("ftp://") == false)
                {
                    hostName = "ftp://" + hostName;
                }
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(hostName));
                requestDir.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                requestDir.Credentials = new NetworkCredential(AppConfig.FTPUserName.Trim(), AppConfig.FTPPassword.Trim());

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                requestDir.Timeout = 10000;
                requestDir.KeepAlive = false;

                using (var response = requestDir.GetResponse())
                {
                    MessageBox.Show("Connection successful.");
                }
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }
    }
}
