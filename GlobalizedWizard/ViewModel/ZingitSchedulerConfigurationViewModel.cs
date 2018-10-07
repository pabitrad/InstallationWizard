using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

using Microsoft.Win32.TaskScheduler;

using Zingit.TaskManagerUtil;
using ZingitWizard.Command;
using System.Diagnostics;

namespace ZingitWizard.ViewModel
{
    class ZingitSchedulerConfigurationViewModel : ZingitWizardPageViewModelBase
    {
        private const string _taskFolder = "ZingitInstallation";
        private const string TASK_NAME = "ZingitTask";
        private const string _ZingitAutoUploaderEXE = "Zingit_Auto_Uploader.exe";

        private int _pushTimeHour = 12;
        private int _pushTimeMinute = 0;

        private Period _PeriodType = Period.AM;
        private Frequency _frequencyType = Frequency.Daily;

        private RelayCommand _openTaskSchedulerCommand;

        public enum Frequency { Daily = 1, OneTime, Weekly, Monthly };
        public enum Period { AM = 1, PM};

        /// <summary>
        /// To populate frequency types Daily, weekly, etc
        /// </summary>
        public IList<Frequency> FrequencyTypes
        {
            get
            {
                return Enum.GetValues(typeof(Frequency)).
                            Cast<Frequency>().
                            ToList();
            }
        }

        /// <summary>
        /// To populate drop down list AM and PM
        /// </summary>
        public IList<Period> PeriodTypes
        {
            get
            {
                return Enum.GetValues(typeof(Period)).
                            Cast<Period>().
                            ToList();
            }
        }

        /// <summary>
        /// Task trigger daily, weekly etc
        /// </summary>
        public Frequency FrequencyType
        {
            get { return _frequencyType; }

            set
            {
                if (value != _frequencyType)
                {
                    _frequencyType = value;
                    OnPropertyChanged("FrequencyType");
                }
            }
        }
        
        /// <summary>
        /// AM or PM
        /// </summary>
        public Period PeriodType
        {
            get { return _PeriodType; }

            set
            {
                if (value != _PeriodType)
                {
                    _PeriodType = value;
                    OnPropertyChanged("PeriodType");
                }
            }
        }

        public ICommand OpenTaskSchedulerCommand
        {
            get
            {
                if (_openTaskSchedulerCommand == null)
                    _openTaskSchedulerCommand = new RelayCommand(() => this.OpenTaskScheduler());

                return _openTaskSchedulerCommand;
            }
        }

        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return "Scheduler Configuration"; }
        }

        public int DailyPushTimeHour
        {
            get { return _pushTimeHour; }

            set
            {
                if (value != _pushTimeHour)
                {
                    _pushTimeHour = value;
                    OnPropertyChanged("DailyPushTimeHour");
                }
            }
        }
   
        public int DailyPushTimeMinute
        {
            get { return _pushTimeMinute; }

            set
            {
                if (value != _pushTimeMinute)
                {
                    _pushTimeMinute = value;
                    OnPropertyChanged("DailyPushTimeMinute");
                }
            }
        }

        public override bool CanMoveToNextPage()
        {
            if (IsValidHourMinute() == false || IsValidTime() == false)
            {
                return false;
            }

            CreateModifyTask();

            return true;
        }

        private void OpenTaskScheduler()
        {
            try
            {
                Process.Start("taskschd.msc");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// If the task exists the load the trigger(frequency) and push time
        /// </summary>
        public void LoadTaskParams()
        {
            using (TaskService ts = new TaskService())
            {
                Task zingitTask = TaskManager.getTask(TASK_NAME, ts);
                if (zingitTask != null)
                {
                    if (zingitTask.Definition.Triggers.Count > 0)
                    {
                        Trigger frequency = zingitTask.Definition.Triggers[0];
                        switch(frequency.TriggerType)
                        {
                            case TaskTriggerType.Time:
                                FrequencyType = Frequency.OneTime;
                                break;

                            case TaskTriggerType.Daily:
                                FrequencyType = Frequency.Daily;
                                break;

                            case TaskTriggerType.Weekly:
                                FrequencyType = Frequency.Weekly;
                                break;

                            case TaskTriggerType.Monthly:
                                FrequencyType = Frequency.Monthly;
                                break;
                        }
                        int hour = frequency.StartBoundary.Hour;

                        // Convert 24 hour format to AM/PM
                        if (hour == 0)
                        {
                            hour = 12; // 12 AM
                        }
                        else if (hour >= 12)
                        {
                            PeriodType = Period.PM;
                            if (hour != 12) // 12 is 12 PM
                            {
                                hour -= 12;
                            }
                        }

                        DailyPushTimeHour = hour;
                        DailyPushTimeMinute = frequency.StartBoundary.Minute;
                    }
                }
            }

            IsPropertyChanged = false;
        }

        /// <summary>
        /// Creates new task or modify existing
        /// </summary>
        /// <returns></returns>
        private bool CreateModifyTask()
        {
            if (IsPropertyChanged == false && IsTaskExist() == true) // Do not modify task if nothing in screen has been changed
            {
                return false;
            }

            string machineName = Environment.MachineName;
            Trigger trigger = GetTrigger();


            if (trigger != null)
            {
                try
                {
                    List<Trigger> triggerList = new List<Trigger>();
                    triggerList.Add(trigger);

                    string installDir = App.Current.Properties["InstallDir"] as string;
                    if (string.IsNullOrEmpty(installDir))
                    {
                        return false;
                    }
                    List<Microsoft.Win32.TaskScheduler.Action> listActions = new List<Microsoft.Win32.TaskScheduler.Action>();
                    listActions.Add(new ExecAction(_ZingitAutoUploaderEXE, null, installDir));

                    TaskManager.createTask(machineName, TASK_NAME, null, triggerList, listActions);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }

            return true;
        }

        private Trigger GetTrigger()
        {
            Trigger trigger = null;
            switch (FrequencyType)
            {
                case Frequency.OneTime:
                    trigger = new TimeTrigger();
                    break;

                case Frequency.Daily:
                    trigger = new DailyTrigger();
                    break;

                case Frequency.Weekly:
                    trigger = new WeeklyTrigger();
                    break;

                case Frequency.Monthly:
                    trigger = new MonthlyTrigger();
                    break;
            }

            if (trigger != null)
            {
                trigger.StartBoundary = GetDateTime();
            }

            return trigger;
        }

        private DateTime GetDateTime()
        {
            DateTime today = DateTime.Today;

            //Convert AM/PM to 24 hour format
            int hour = (PeriodType == Period.AM) ? (_pushTimeHour % 12) : (_pushTimeHour % 12) + 12;

            return new DateTime(today.Year, today.Month, today.Day, hour, _pushTimeMinute, 0);
        }

        private bool IsValidHourMinute()
        {
            if (PeriodType == Period.AM)
            {
                if (DailyPushTimeHour < 1 || DailyPushTimeHour > 12)
                {
                    MessageBox.Show("Please enter valid Hour value.\nIt should be between 0 to 11 when AM is selected.");
                    return false;
                }
            } else if (PeriodType == Period.PM) {
                if (DailyPushTimeHour < 1 || DailyPushTimeHour > 12)
                {
                    MessageBox.Show("Please enter valid Hour value.\nIt should be between 1 to 12 when PM is selected.");
                    return false;
                }
            }

            if (DailyPushTimeMinute < 0 || DailyPushTimeMinute > 59)
            {
                MessageBox.Show("Please enter valid Minute value.\nIt should be between 0 to 59.");
                return false;
            }

            return true;
        }

        private bool IsTaskExist()
        {
            using (TaskService ts = new TaskService())
            {
                Task zingitTask = TaskManager.getTask(TASK_NAME, ts);
                if (zingitTask != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValidTime()
        {
            TimeSpan t1 = new TimeSpan(7, 0, 0);
            TimeSpan t2 = new TimeSpan(21, 0, 0);

            int taskHour = (PeriodType == Period.AM) ? DailyPushTimeHour : DailyPushTimeHour + 12;
            TimeSpan taskTime = new TimeSpan(taskHour, DailyPushTimeMinute, 0);
            if (taskTime < t1 || taskTime > t2)
            {
                MessageBox.Show("Task can only be created between 7.00 AM and 9.00 PM.");
                return false;
            }

            return true;
        }
    }
}
