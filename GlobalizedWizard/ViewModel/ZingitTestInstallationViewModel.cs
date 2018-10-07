using System;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Windows.Data;

using Microsoft.Win32.TaskScheduler;
using ZingitWizard.Command;
using Zingit.TaskManagerUtil;
using System.Collections.Generic;

namespace ZingitWizard.ViewModel
{
    public class SendReminderEntry
    {
        public string Name { get; set; }
        public int Tag { get; set; }

        public SendReminderEntry(string name, int tag)
        {
            Name = name;
            Tag = tag;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class ZingitTestInstallationViewModel : ZingitWizardPageViewModelBase
    {
        private bool _executionModeTest;
        private bool _executionModeProduction;

        private bool _testTypeTaskScheduler = true;
        private bool _testTypeDirectExeRun;

        private RelayCommand _saveDataCommand;
        private RelayCommand _runTestCommand;
        private RelayCommand _viewLogFileFolder;
        private RelayCommand _viewDataFileFolder;

        private const string _ZingitAutoUploaderEXE = "Zingit_Auto_Uploader.exe";
        private const string TASK_NAME = "ZingitTask";

        private CollectionView _sendReminderList;
        private SendReminderEntry _serverNameEntry;

        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public CollectionView SendReminderList
        {
            get
            {
                if (_sendReminderList == null)
                {
                    PopulateSendReminderList();
                }

                return _sendReminderList;
            }
        }
   
        public override string DisplayName
        {
            get { return "Test Installation"; }
        }

        public SendReminderEntry SelectedReminderEntry
        {
            get { return _serverNameEntry; }

            set
            {
                _serverNameEntry = value;
                AppConfig.SendReminderFor = value.Tag;
                OnPropertyChanged("SelectedReminderEntry");
            }
        }

        public bool ExecutionModeTestChecked
        {
            get { return _executionModeTest; }

            set
            {
                if (value != _executionModeTest)
                {
                    _executionModeTest = value;
                    OnPropertyChanged("ExecutionModeTestChecked");
                }
            }
        }

        public bool ExecutionModeProductionChecked
        {
            get { return _executionModeProduction; }

            set
            {
                if (value != _executionModeProduction)
                {
                    _executionModeProduction = value;
                    OnPropertyChanged("ExecutionModeProductionChecked");
                }
            }
        }

        public bool TestTypeTaskSchedulerChecked
        {
            get { return _testTypeTaskScheduler; }

            set
            {
                if (value != _testTypeTaskScheduler)
                {
                    _testTypeTaskScheduler = value;
                    OnPropertyChanged("TestTypeTaskSchedulerChecked");
                }
            }
        }

        public bool TestTypeDirectExeRunChecked
        {
            get { return _testTypeDirectExeRun; }

            set
            {
                if (value != _testTypeDirectExeRun)
                {
                    _testTypeDirectExeRun = value;
                    OnPropertyChanged("TestTypeDirectExeRunChecked");
                }
            }
        }

        public override bool CanMoveToNextPage()
        {
            SaveRadioButtonState();

            AppConfig.EncryptAndSaveData();

            return true;
        }

        /// <summary>
        /// Set radio buttons and other data
        /// </summary>
        public void LoadTaskParams()
        {
            switch(AppConfig.ExecutionMode)
            {
                case "TEST":
                    ExecutionModeTestChecked = true;
                    break;

                case "PRODUCTION":
                    ExecutionModeProductionChecked = true;
                    break;

                default:
                    ExecutionModeTestChecked = true;
                    break;
            }

            AppConfig.SetTestInstallationData();
        }

        public ICommand SaveDataCommand
        {
            get
            {
                if (_saveDataCommand == null)
                    _saveDataCommand = new RelayCommand(() => this.SaveAppConfigData());

                return _saveDataCommand;
            }
        }

        public ICommand RunTestCommand
        {
            get
            {
                if (_runTestCommand == null)
                    _runTestCommand = new RelayCommand(() => this.RunInstallationTest());

                return _runTestCommand;
            }
        }

        public ICommand ViewLogFileCommand
        {
            get
            {
                if (_viewLogFileFolder == null)
                    _viewLogFileFolder = new RelayCommand(() => this.OpenLogFileFolder());

                return _viewLogFileFolder;
            }
        }

        public ICommand ViewDataFileCommand
        {
            get
            {
                if (_viewDataFileFolder == null)
                    _viewDataFileFolder = new RelayCommand(() => this.OpenDataFileFolder());

                return _viewDataFileFolder;
            }
        }

        private void SaveRadioButtonState()
        {
            if (ExecutionModeTestChecked)
            {
                AppConfig.ExecutionMode = "TEST";
            }
            else
            {
                AppConfig.ExecutionMode = "PRODUCTION";
            }
        }
   
        private void SaveAppConfigData()
        {
            SaveRadioButtonState();

            AppConfig.EncryptAndSaveData();
        }

        private void RunInstallationTest()
        {
            SaveAppConfigData();

            if (TestTypeDirectExeRunChecked)
            {
                RunAutoUploadedExe();
            }
            else if (TestTypeTaskSchedulerChecked)
            {
                RunZingitTaskScheduler();
            }
        }

        private void RunAutoUploadedExe()
        {
            string strInstallDir = App.Current.Properties["InstallDir"] as string;
            string fileName = strInstallDir + "\\" + _ZingitAutoUploaderEXE;

            try
            {
                Process.Start(fileName);
            }
            catch (FileNotFoundException fnfEx)
            {
                MessageBox.Show(fileName + " could not be found. " + fnfEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Run Task registered in task scheduler
        /// </summary>
        private void RunZingitTaskScheduler()
        {
            using (TaskService ts = new TaskService())
            {
                try
                {
                    Task zingitTask = TaskManager.getTask(TASK_NAME, ts);
                    if (zingitTask != null)
                    {
                        RunningTask runningTask= zingitTask.Run();
                        Thread.Sleep(1000);
                        if (runningTask.State == TaskState.Running)
                        {
                            MessageBox.Show("Task started successfully.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Task is not created yet. Please create the task through scheduler configuration screen and try again.");
                    }
                }
                catch(Exception ex)
                {
                    if (ex.Message.Contains("0x80070002") == true)
                    {
                        string errMessage = ex.Message + Environment.NewLine +
                                            "Looks like the task is already running. Please check.";
                        MessageBox.Show(errMessage);
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void PopulateSendReminderList()
        {
            IList<SendReminderEntry> list = new List<SendReminderEntry>();

            list.Add(new SendReminderEntry("Same Day", 0));
            list.Add(new SendReminderEntry("Next Day", 1));
            list.Add(new SendReminderEntry("+2 Days", 2));
            list.Add(new SendReminderEntry("+3 Days", 3));
            list.Add(new SendReminderEntry("+4 Days", 4));
            list.Add(new SendReminderEntry("+5 Days", 5));
            list.Add(new SendReminderEntry("+6 Days", 6));
            list.Add(new SendReminderEntry("+7 Days", 7));

            _sendReminderList = new CollectionView(list);
            if (AppConfig.SendReminderFor >= 0 && AppConfig.SendReminderFor <= 7)
            {
                SelectedReminderEntry = list[AppConfig.SendReminderFor];
            }
            else
            {
                SelectedReminderEntry = list[0];
            }
        }

        private void OpenLogFileFolder()
        {
            string strInstallDir = App.Current.Properties["InstallDir"] as string;
            if (string.IsNullOrEmpty(strInstallDir) == false)
            {
                string logFileFolderPath = strInstallDir + "\\" + "logs";
                OpenFolder(logFileFolderPath);
            }
        }

        private void OpenDataFileFolder()
        {
            OpenFolder(Path.GetTempPath() + "zingitmobile");
        }

        private void OpenFolder(string folder)
        {
            try
            {
                Process.Start("explorer", folder);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
