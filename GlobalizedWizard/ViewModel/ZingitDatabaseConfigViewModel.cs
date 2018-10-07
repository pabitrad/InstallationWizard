using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Data;
using System.Windows.Input;
using ZingitWizard.Command;
using System.Data.SqlClient;
using System.Windows.Forms;
using ZingitWizard.Model;

namespace ZingitWizard.ViewModel
{
    public class ServerNameEntry
    {
        public string Name { get; set; }
        public ServerNameEntry(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class ZingitDatabaseConfigViewModel : ZingitWizardPageViewModelBase
    {
        private string _serverNameEntry;
        private CollectionView _sqlServerList;

        RelayCommand _testConnectionCommand;

        public ZingitDatabaseConfigViewModel()
        {
        }

        public CollectionView SQLServerList
        {
            get
            {
                if (_sqlServerList == null)
                {
                    PopulateSQLServerList();
                }
    
                return _sqlServerList;
            }
        }

        public string ServerName
        {
            get { return _serverNameEntry; }
            set
            {
                if (_serverNameEntry != value)
                {
                    _serverNameEntry = value;
                    AppConfig.DbServerName = value;
                    OnPropertyChanged("ServerName");
                }
            }
        }


        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return "Database Configuration"; }
        }

        public ICommand TestConnectionCommand
        {
            get
            {
                if (_testConnectionCommand == null)
                    _testConnectionCommand = new RelayCommand(() => this.TestDatabaseConnection(ServerName, true));

                return _testConnectionCommand;
            }
        }

        public override bool CanMoveToNextPage()
        {
            AppConfig.EncryptAndSaveData();

            return true;
        }
 
        /// <summary>
        /// List of Sql servers in a network
        /// https://stackoverflow.com/questions/13462120/how-do-i-get-a-list-of-sql-servers-available-on-my-network
        /// if not working then try following
        /// https://msdn.microsoft.com/en-us/library/dd981032.aspx
        /// </summary>
        public void PopulateSQLServerList()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            IList<ServerNameEntry> list = new List<ServerNameEntry>();
            AppConfigModel appConfig = AppConfigModel.Instance;

            DataTable table = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();
            bool sqlDBConnected = false;
            foreach (DataRow server in table.Rows)
            {
                string serverName = server[table.Columns["ServerName"]].ToString();
                ServerNameEntry serverEntry = new ServerNameEntry(serverName);
                list.Add(serverEntry);

                if (!sqlDBConnected && TestDatabaseConnection(serverName, false))
                {
                    ServerName = serverName;
                    sqlDBConnected = true;
                }
            }
            _sqlServerList = new CollectionView(list);

            if (!sqlDBConnected)
            {
                ServerName = "(local)";
                if (TestDatabaseConnection(ServerName, false))
                {
                    sqlDBConnected = true;
                }
            }

            if (sqlDBConnected)
            {
                MessageBox.Show("SQL server " + ServerName + " is connected.");
            }
            else
            {
                MessageBox.Show("None of the SQL server found connected with credential provided in AppConfig.\nTo test the SQL server connection, type server name in \"DB Server Name\" drop down list.");
            }

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        private bool TestDatabaseConnection(string sqlServerName, bool displayMessage)
        {
            string server = sqlServerName.Trim();
            if (displayMessage && string.IsNullOrEmpty(server) == true)
            {
                MessageBox.Show("Please select server name.");
                return false;
            }

            if (displayMessage && string.IsNullOrEmpty(AppConfig.DbName.Trim()) == true)
            {
                MessageBox.Show("Please enter database name.");
                return false;
            }

            if (displayMessage && string.IsNullOrEmpty(AppConfig.DbUserName.Trim()) == true)
            {
                MessageBox.Show("Please enter user name.");
                return false;
            }

            try
            {
                SqlConnection conn = new SqlConnection();
                if (AppConfig.DbUserName.ToUpper().Trim() == "WINDOWSAUTH")
                {
                    conn.ConnectionString =
                        "Data Source=" + server +
                        ";Initial Catalog=" + AppConfig.DbName.Trim() +
                        ";Integrated Security=SSPI;";
                }
                else
                {
                    conn.ConnectionString =
                    "Data Source=" + server + ";" +
                    "Initial Catalog=" + AppConfig.DbName.Trim() + ";" +
                    "User id=" + AppConfig.DbUserName.Trim() + ";" +
                    "Password=" + AppConfig.DbPassword.Trim() + ";" +
                    "Connection Timeout=10";
                }

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                conn.Open(); //Open the Sql connection
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

                if (displayMessage)
                {
                    MessageBox.Show("Connection successful.");
                }
            }
            catch (Exception ex)
            {
                if (displayMessage)
                {
                    MessageBox.Show(ex.Message);
                }
                return false;
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            return true;
        }
    }
}
