using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sample_LINQLogin
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SampleLINQDataContext db = new SampleLINQDataContext(Properties.Settings.Default.LoginPracticeConnectionString);

        public MainWindow()
        {
            InitializeComponent();

        }

        private void login_btn_Click(object sender, RoutedEventArgs e)
        {
            if(user_txt.Text == "" || pass_txt.Text == "")
            {
                MessageBox.Show("Please enter username and password");
                return;
            }
            else
            {
                if(passComparison(getPassword())==0)
                {
                    MessageBox.Show("Login Successful", "Welcome Back!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    insertLog(user_txt.Text, "Successful login");

                    // Create an instance of the UserManagementWindow
                    UserManagementWindow userManagementWindow = new UserManagementWindow();

                    // Show the UserManagementWindow
                    userManagementWindow.Show();

                    // Optionally, close the MainWindow after successful login
                    this.Close();
                }
                else if(passComparison(getPassword()) == -1)
                {
                    MessageBox.Show("Invalid username or password","Login Failed",MessageBoxButton.OK,MessageBoxImage.Warning);
                    insertLog(null, "Brute Force attempt");
                }
                else
                    MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    insertLog(user_txt.Text, "Incorrect Password");
            }
        }

        private string getPassword()
        {
            string uPass = "";
            
            var users = (from u in db.user_Tables
                               where u.UserID == user_txt.Text
                                           select u).FirstOrDefault();

            if (users == null)
            {
                MessageBox.Show("User not found.");
                return "";
            }

            uPass = users.UserPass;
            return uPass;
        }

        private int passComparison(string uPass) 
        {
            if (pass_txt.Text == uPass)
            {
                return 0;
            }
            else if (uPass == null)
            {
                return -1;
            }
            else
                return 1;
        }

        private void insertLog(string user, string message)
        {
            var log = new table_Log
            {
                UserID = user,
                LogMessage = message,
                LogDate = DateTime.Now
            };
            db.table_Logs.InsertOnSubmit(log);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }    
        }
    }
}
