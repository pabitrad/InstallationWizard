using System;
using System.Windows.Input;
using System.Windows.Forms;
using System.Net;
using System.IO;

using ZingitWizard.Resources;
using ZingitWizard.Command;

namespace ZingitWizard.ViewModel
{

    class ZingitCopyFilesViewModel : ZingitWizardPageViewModelBase
    {
        bool _skipCopyFilesInstallation = false;
        protected string _installDir = "C:\\Program Files\\Zingit";

        private const string FTP_FOLDER_NAME = "ftp://ftp.zingitsolutions.com/Chirotouch/";
        private const string FTP_USER_NAME = "uifiles@zingitsolutions.com";
        private const string FTP_PASSWORD = "cxTuJFK649U+";

        RelayCommand _folderBrowseCommand;

        public ZingitCopyFilesViewModel()
        {
            App.Current.Properties["InstallDir"] = _installDir;
        }

        public string InstallDir
        {
            get { return _installDir; }

            set
            {
                if (value != _installDir)
                {
                    _installDir = value;
                    App.Current.Properties["InstallDir"] = value;
                    this.OnPropertyChanged("InstallDir");
                }
            }
        }

        public ICommand FolderBrowseCommand
        {
            get
            {
                if (_folderBrowseCommand == null)
                    _folderBrowseCommand = new RelayCommand(() => this.DisplayFolderDialog());

                return _folderBrowseCommand;
            }
        }
 
        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return Strings.PageDisplayName_CopyFiles; }
        }

        public bool IsSelectedSkipCopyFiles
        {
            get { return _skipCopyFilesInstallation; }

            set
            {
                if (value != _skipCopyFilesInstallation)
                {
                    _skipCopyFilesInstallation = value;
                }
            }
        }

        public override bool CanMoveToNextPage()
        {
            if (_skipCopyFilesInstallation == false)
            {
                return PerformCopyOperation();
            }

            return true;
        }

        private void DisplayFolderDialog()
        {
            var dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                InstallDir = dlg.SelectedPath;
            }
        }

        private bool PerformCopyOperation()
        {
            bool bCopyOperation = true;

            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;                
                Directory.CreateDirectory(InstallDir);

                CopyFile(FTP_FOLDER_NAME + "AppConfig.dat", "AppConfig.dat");
                CopyFile(FTP_FOLDER_NAME + "Zingit_Auto_Uploader.exe", "Zingit_Auto_Uploader.exe");
            }
            catch(WebException ex)
            {
                string errMessage = "An error occured while downloaging file from FTP server.\n" + ex.Message; 
                MessageBox.Show(errMessage);
                bCopyOperation = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                bCopyOperation = false;
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }

            return bCopyOperation;
        }

        private void CopyFile(string googlefileName, string actualFileName)
        {
            string localFileName = InstallDir + "\\" + actualFileName;
            if (File.Exists(localFileName))
            {
                string confirmMessage = "Do you want to overwrite file " + localFileName + "?";
                DialogResult dialogResult = MessageBox.Show(confirmMessage, "Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    downloadFile(googlefileName, localFileName);
                }
            }
            else
            {
                downloadFile(googlefileName, localFileName);
            }
        }

        private void downloadFile(string googlefileName, string actualFileName)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(FTP_USER_NAME, FTP_PASSWORD);
            webClient.DownloadFile(googlefileName, actualFileName);
        }
    }
}
