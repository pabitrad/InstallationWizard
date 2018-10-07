using System;
using System.Text;
using System.Net.Mail;
using System.Windows.Forms;

using Microsoft.Win32.TaskScheduler;
using ZingitWizard.Model;
using Zingit.TaskManagerUtil;

namespace ZingitWizard.ViewModel
{
    class ZingitFinalizeSettingsViewModel : ZingitWizardPageViewModelBase
    {
        private const string EMAIL_HOST = "mail.zingitsolutions.com";
        private const string USER_NAME = "installs@zingitsolutions.com";
        private const string PASSWORD = "[s+gC5hs{\"/eZ<";
        private const string SENDER_ADDRESS = USER_NAME;
        //private const string ACCOUNT_MANAGER_ADDRESS = SENDER_ADDRESS;
        private const int PORT = 26;
        private const string TASK_NAME = "ZingitTask";

        private string _recepientEmailAddress = SENDER_ADDRESS + ";";
        private string _subject;
        private string _message;
        //private bool _isNotificationChecked = true;

        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return "Finalize Settings"; }
        }

        public string RecepientEmailAddress
        {
            get { return _recepientEmailAddress; }

            set
            {
                if (value != _recepientEmailAddress)
                {
                    _recepientEmailAddress = value;
                    OnPropertyChanged("RecepientEmailAddress");
                }
            }
        }

        public string Subject
        {
            get { return _subject; }

            set
            {
                if (value != _subject)
                {
                    _subject = value;
                    OnPropertyChanged("Subject");
                }
            }
        }

        public string Message
        {
            get { return _message; }

            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public override bool CanMoveToNextPage()
        {
            SendEmail();
            
            //Close main window after sending email
            System.Windows.Application.Current.Shutdown();

            return false;
        }

        public void LoadParams()
        {
            AppConfigModel appConfig = AppConfig;
            Subject = @"Installation Completed for " + appConfig.ClientName;

            Message = @"Installation completed." + Environment.NewLine +
                        "Client Name: " + appConfig.ClientName + Environment.NewLine +
                        "PMS Name: CHIROTOUCH" + Environment.NewLine +
                        "Send Time: " + getTaskRunTime() + Environment.NewLine +
                        "Keyword : " + appConfig.AccountNumber;
        }

        private string getTaskRunTime()
        {
            using (TaskService ts = new TaskService())
            {
                Task zingitTask = TaskManager.getTask(TASK_NAME, ts);
                if (zingitTask != null)
                {
                    if (zingitTask.Definition.Triggers.Count > 0)
                    {
                        Trigger frequency = zingitTask.Definition.Triggers[0];

                        return frequency.StartBoundary.ToShortTimeString();
                    }
                }
            }

            return string.Empty;
        }

        private void SendEmail()
        {
            if (IsValidate() == false)
            {
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = PORT;
                client.Host = EMAIL_HOST;
                client.EnableSsl = false;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(USER_NAME, PASSWORD);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(SENDER_ADDRESS);

                char[] charSeparators = new char[] { ';' };
                string [] recepientList = RecepientEmailAddress.Trim().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                foreach(string recepient in recepientList)
                {
                    mailMessage.To.Add(new MailAddress(recepient));
                }

                //if (IsNotificationChecked) //send to account manager
                //{
                //    mailMessage.To.Add(new MailAddress(ACCOUNT_MANAGER_ADDRESS));
                //}
                mailMessage.Subject = Subject;
                mailMessage.Body = Message + Environment.NewLine + Environment.NewLine +
                                   "SMS Content : " + AppConfig.SMSContent;

                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                mailMessage.SubjectEncoding = UTF8Encoding.UTF8;

                string strInstallDir = App.Current.Properties["InstallDir"] as string;
                if (string.IsNullOrEmpty(strInstallDir) == false)
                {
                    string fileName = strInstallDir + "\\AppConfig.dat";

                    Attachment attachment = new Attachment(fileName);
                    mailMessage.Attachments.Add(attachment);
                }

                // send to account manager also
                client.Send(mailMessage);

                MessageBox.Show("Email Sent!");
            }
            catch(FormatException fEx)
            {
                MessageBox.Show("Email id entered in \"Email Address\" box is not valid.\n" + fEx.Message);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nEmail could not be sent.");
            }

            Cursor.Current = Cursors.Default;
        }

        private bool IsValidate()
        {
            if (string.IsNullOrEmpty(RecepientEmailAddress))
            {
                MessageBox.Show("Recepient email id can not be empty.");
                return false;
            }

            if (string.IsNullOrEmpty(Subject))
            {
                MessageBox.Show("Email Subject can not be empty.");
                return false;
            }

            if (string.IsNullOrEmpty(Message))
            {
                MessageBox.Show("Email body can not be empty.");
                return false;
            }

            return true;
        }
    }
}
