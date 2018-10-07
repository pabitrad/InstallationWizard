using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using System.Security.Cryptography;

namespace ZingitWizard.Model
{
    public class AppConfigModel : INotifyPropertyChanged
    {
        private static TripleDESCryptoServiceProvider decProvider = new TripleDESCryptoServiceProvider();
        private static readonly AppConfigModel instance = new AppConfigModel();

        public static AppConfigModel Instance
        {
            get
            {
                return instance;
            }
        }

        const string txtKey = "ZINGITMOBILE";
        private Dictionary<string, string> _configDict = new Dictionary<string, string>();
        private string _configFile = string.Empty;

        private bool quoteRemovedFromAccountNumber = false;

        private AppConfigModel()
        {
            try
            {
                decProvider.Key = TruncateHash(txtKey, decProvider.KeySize / 8);
                decProvider.IV = TruncateHash("", decProvider.BlockSize / 8);
            }
            catch(Exception ex)
            {
                string errMsg = ex.Message;
            }
        }

        //Label changed to account key word
        public string AccountNumber
        {
            get
            {
                return GetConfigValue("ACCOUNT_NUMBER");
            }

            set
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    _configDict["ACCOUNT_NUMBER"] = value;
                    OnPropertyChanged("AccountNumber");
                }
            }
        }

        public string ClientName
        {
            get
            {
                return GetConfigValue("CLIENT_NAME");
            }

            set
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    _configDict["CLIENT_NAME"] = value;
                    OnPropertyChanged("ClientName");
                }
            }
        }

        public string Customization
        {
            get { return GetConfigValue("CUSTOMIZATION"); }

            set
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    _configDict["CUSTOMIZATION"] = value;
                    OnPropertyChanged("Customization");
                }
            }
        }

        public string FilterFirstAppt
        {
            get
            {
                return GetConfigValue("FILTER_FIRST_APPT");
            }

            set
            {
                _configDict["FILTER_FIRST_APPT"] = value;
            }
        }

        public bool FilterFirstApptChecked
        {
            get { return FilterFirstAppt == "Y"; }

            set
            {
                if (value == true)
                {
                    _configDict["FILTER_FIRST_APPT"] = "Y";
                }
                else
                {
                    _configDict["FILTER_FIRST_APPT"] = "N";
                }
                OnPropertyChanged("FilterFirstApptChecked");
            }
        }

        public string DbServerName
        {
            get
            {
                return GetConfigValue("DATABASE_SERVER_NAME");
            }

            set
            {
                if (value != _configDict["DATABASE_SERVER_NAME"])
                {
                    _configDict["DATABASE_SERVER_NAME"] = value;
                    OnPropertyChanged("DbServerName");
                }
            }
        }

        public string DbName
        {
            get { return GetConfigValue("DATABASE_NAME"); }

            set
            {
                if (value != _configDict["DATABASE_NAME"])
                {
                    _configDict["DATABASE_NAME"] = value;
                    OnPropertyChanged("DbName");
                }
            }
        }

        public string DbUserName
        {
            get { return GetConfigValue("DATABASE_USERNAME"); }

            set
            {
                if (value != _configDict["DATABASE_USERNAME"])
                {
                    _configDict["DATABASE_USERNAME"] = value;
                    OnPropertyChanged("DbUserName");
                }
            }
        }

        public string DbPassword
        {
            get { return GetConfigValue("DATABASE_PASSWORD"); }

            set
            {
                if (value != _configDict["DATABASE_PASSWORD"])
                {
                    _configDict["DATABASE_PASSWORD"] = value;
                    OnPropertyChanged("DbPassword");
                }
            }
        }

        public string FTPHost
        {
            get { return GetConfigValue("FTP_HOST"); }

            set
            {
                //if (value != GetConfigValue("FTP_HOST"))
                {
                    _configDict["FTP_HOST"] = value;
                    OnPropertyChanged("FTPHost");
                }
            }
        }

        public string FTPUserName
        {
            get { return GetConfigValue("FTP_USERNAME"); }

            set
            {
                //if (value != GetConfigValue("FTP_USERNAME"))
                {
                    _configDict["FTP_USERNAME"] = value;
                    OnPropertyChanged("FTPUserName");
                }
            }
        }

        public string FTPPassword
        {
            get { return GetConfigValue("FTP_PASSWORD"); }

            set
            {
                //if (value != GetConfigValue("FTP_PASSWORD"))
                {
                    _configDict["FTP_PASSWORD"] = value;
                    OnPropertyChanged("FTPPassword");
                }
            }
        }

        public string FTPUploadFilePrefix
        {
            get { return GetConfigValue("FTP_UPLOAD_FILE_PREFIX"); }

            set
            {
                if (value != GetConfigValue("FTP_UPLOAD_FILE_PREFIX"))
                {
                    _configDict["FTP_UPLOAD_FILE_PREFIX"] = value;
                    OnPropertyChanged("FTPUploadFilePrefix");
                }
            }
        }

        public string HeadlessMode
        {
            get { return GetConfigValue("HEADLESS_MODE"); }

            set
            {
                _configDict["HEADLESS_MODE"] = value;
            }
        }

        public bool HeadlessModeChecked
        {
            get { return HeadlessMode.Equals("TRUE"); }

            set
            {
                HeadlessMode = (value == true) ? "TRUE" : "FALSE";
                OnPropertyChanged("HeadlessModeChecked");
            }
        }

        public int TestRecordCount
        {
            get
            {
                string strRecordCount = GetConfigValue("TEST_MODE_RECORD_COUNT");
                if (string.IsNullOrEmpty(strRecordCount) == false)
                {
                    return Convert.ToInt32(strRecordCount);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                _configDict["TEST_MODE_RECORD_COUNT"] = value.ToString();
                OnPropertyChanged("TestRecordCount");
            }
        }

        public string TestPhoneNumber
        {
            get { return GetConfigValue("TEST_MODE_PHONE_NUMBER"); }

            set
            {
                _configDict["TEST_MODE_PHONE_NUMBER"] = value;
                OnPropertyChanged("TestPhoneNumber");
            }
        }

        public int SendReminderFor
        {
            get
            {
                string strSendReminderFor = GetConfigValue("APPT_DATE_OFFSET");
                if (string.IsNullOrEmpty(strSendReminderFor) == false)
                {
                    return Convert.ToInt32(strSendReminderFor);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                _configDict["APPT_DATE_OFFSET"] = value.ToString();
                OnPropertyChanged("ApptDateOffset");
            }
        }

        public string ExecutionMode
        {
            get { return _configDict["EXECUTION_MODE"]; }

            set
            {
                _configDict["EXECUTION_MODE"] = value;
            }
        }

        public string StopFTP
        {
            get { return GetConfigValue("STOP_FTP"); }

            set
            {
                _configDict["STOP_FTP"] = value;
                OnPropertyChanged("StopFTP");
            }
        }

        public bool StopFTPChecked
        {
            get { return StopFTP.Equals("Y"); }

            set
            {
                StopFTP = (value == true) ? "Y" : "N";
                OnPropertyChanged("StopFTPChecked");
            }
        }

        public string SMSContent
        {
            get; set;
        }

        public bool EncryptAndSaveData()
        {
            try
            {
                if (string.IsNullOrEmpty(_configFile))
                {
                    return false;
                }

                string plaintext = GetConfigDataString();
                if (string.IsNullOrEmpty(plaintext))
                {
                    return false;
                }

                //  Convert the plaintext string to a byte array. 
                byte[] plaintextBytes = Encoding.Unicode.GetBytes(plaintext);
                //  Create the stream. 
                MemoryStream ms = new MemoryStream();
                //  Create the encoder to write to the stream. 
                CryptoStream encStream = new CryptoStream(ms, decProvider.CreateEncryptor(), CryptoStreamMode.Write);
                //  Use the crypto stream to write the byte array to the stream.
                encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                encStream.FlushFinalBlock();
                //  Convert the encrypted stream to a printable string. 
                string encryptedData = Convert.ToBase64String(ms.ToArray());

                File.WriteAllText(_configFile, encryptedData);
            }
            catch(CryptographicException crEx)
            {
                string strMessage = "The config data could not be encrypted before writting to AppConfig.dat." +
                                    crEx.Message;
                MessageBox.Show(strMessage);
                return false;
            }
            catch (FileNotFoundException /*fnfEx*/)
            {
                string strMessage = _configFile + " could not be found.\n" +
                                    "Download the file from \"Copy Files Screen\" and try again."; ;
                MessageBox.Show(strMessage);
                return false;
            }
            catch (IOException ioEx)
            {
                MessageBox.Show("There is some problem writting the content in AppConfig.dat file.\n" + ioEx.Message);
                return false;
            }
            catch (UnauthorizedAccessException uaeEx)
            {
                string strMessage = "You do not have access to write in AppConfig.dat file.\n" + uaeEx.Message;
                MessageBox.Show(strMessage);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }
 
        public bool DecryptData(string installDir)
        {
            try
            {
                _configFile = installDir + "\\AppConfig.dat";
                string encryptedtext = File.ReadAllText(_configFile);

                //  Convert the encrypted text string to a byte array. 
                byte[] encryptedBytes = Convert.FromBase64String(encryptedtext);
                //  Create the stream. 
                MemoryStream ms = new MemoryStream();
                //  Create the decoder to write to the stream. 
                CryptoStream decStream = new CryptoStream(ms, decProvider.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                //  Use the crypto stream to write the byte array to the stream.
                decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                decStream.FlushFinalBlock();
                //  Convert the plaintext stream to a string. 
                string strSettings = Encoding.Unicode.GetString(ms.ToArray());

                string[] strTemp = strSettings.Split('\n'); //store all config entries in an temp array
                foreach (string configEntry in strTemp)
                {
                    string[] strLine = configEntry.Split('='); // Split key and value
                    _configDict.Remove(strLine[0]); //remove old entry from dictionary
                    _configDict.Add(strLine[0], strLine[1]); // add new entry into dictionary
                }
            }
            catch(DirectoryNotFoundException /*dnfEx*/)
            {
                string strMessage = "Directory " + installDir + " could not be found.\n" +
                                    "Download the installation files from \"Copy Files Screen\" and try again.";
                MessageBox.Show(strMessage);

                return false;
            }
            catch (FileNotFoundException /*fnfEx*/)
            {
                string strMessage = "File " + _configFile + " could not be found.\n" +
                                    "Download the file from \"Copy Files Screen\" and try again."; ;
                MessageBox.Show(strMessage);

                return false;
            }
            catch(IOException ioEx)
            {
                MessageBox.Show("There is some problem in reading the AppConfig.dat file.\n" + ioEx.Message);
                return false;
            }
            catch(UnauthorizedAccessException uaeEx)
            {
                string strMessage = "You do not have access to read AppConfig.dat file.\n" + uaeEx.Message;
                MessageBox.Show(strMessage);
                return false;
            }
            catch (CryptographicException crEx)
            {
                string strMessage = "The config data could not be decrypted after reading from AppConfig.dat." +
                                    crEx.Message;
                MessageBox.Show(strMessage);
                return false;
            }
            catch (Exception ex)
            {
                string strMessage = "There is some problem in reading or decrypting AppConfig.dat file content.\n" +
                                    ex.Message;
                MessageBox.Show(strMessage);
                return false;
            }

            string accountNumber = _configDict["ACCOUNT_NUMBER"].Trim();
            if (accountNumber.StartsWith("'"))
            {
                accountNumber = accountNumber.Remove(0, 1);
                if (accountNumber.EndsWith("'"))
                {
                    accountNumber = accountNumber.Remove(accountNumber.Length - 1);
                }
                quoteRemovedFromAccountNumber = true;
                _configDict["ACCOUNT_NUMBER"] = accountNumber;
            }

            return true;
        }

        public void SetClientSettings()
        {
            AccountNumber = GetConfigValue("ACCOUNT_NUMBER");
            ClientName = GetConfigValue("CLIENT_NAME");
            Customization = GetConfigValue("CUSTOMIZATION");

            FilterFirstAppt = GetConfigValue("FILTER_FIRST_APPT");
            FilterFirstApptChecked = (FilterFirstAppt == "Y");
        }

        public void SetDbConfigData()
        {
            DbServerName = GetConfigValue("DATABASE_SERVER_NAME");
            DbName = GetConfigValue("DATABASE_NAME");
            DbUserName = GetConfigValue("DATABASE_USERNAME");
            DbPassword = GetConfigValue("DATABASE_PASSWORD");
        }
        
        public void SetFTPConfigData()
        {
            FTPHost = GetConfigValue("FTP_HOST");
            FTPUserName = GetConfigValue("FTP_USERNAME");
            FTPPassword = GetConfigValue("FTP_PASSWORD");
            FTPUploadFilePrefix = GetConfigValue("FTP_UPLOAD_FILE_PREFIX");
            //FTPTimeOut = GetConfigValue("FTP_TIMEOUT");

            StopFTP = GetConfigValue("STOP_FTP");
            StopFTPChecked = (StopFTP == "Y");
        }

        public void SetTestInstallationData()
        {
            HeadlessMode = GetConfigValue("HEADLESS_MODE");

            string strRecordCount = GetConfigValue("TEST_MODE_RECORD_COUNT");
            if (string.IsNullOrEmpty(strRecordCount) == false)
            {
                TestRecordCount = Convert.ToInt32(strRecordCount);
            }

            TestPhoneNumber = GetConfigValue("TEST_MODE_PHONE_NUMBER");

            string strApptDateOffset = GetConfigValue("APPT_DATE_OFFSET");
            if (string.IsNullOrEmpty(strApptDateOffset) == false)
            {
                SendReminderFor = Convert.ToInt32(strApptDateOffset);
            }

            StopFTP = GetConfigValue("STOP_FTP");
            StopFTPChecked = (StopFTP == "Y");
        }

        private string GetConfigDataString()
        {
            if (quoteRemovedFromAccountNumber)
            {
                string accountNumber = _configDict["ACCOUNT_NUMBER"].Trim();
                if (!string.IsNullOrEmpty(accountNumber) && accountNumber[0] != '\'')
                {
                    accountNumber = "'" + accountNumber + "'";
                    _configDict["ACCOUNT_NUMBER"] = accountNumber;
                }
            }
 
            return _configDict.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + "\n" + s2);
        }

        private string GetConfigValue(string dictKey)
        {
            if (_configDict.ContainsKey(dictKey))
            {
                return _configDict[dictKey];
            }

            return string.Empty;
        }
  
        private byte[] TruncateHash(string key, int length)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            //  Hash the key. 
            byte[] keyBytes = Encoding.Unicode.GetBytes(key);
            byte[] hash = sha1.ComputeHash(keyBytes);

            Array.Resize(ref hash, length);

            return hash;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
