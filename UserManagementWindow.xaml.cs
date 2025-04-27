using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace sample_LINQLogin
{
    public partial class UserManagementWindow : Window
    {
        SampleLINQDataContext db = new SampleLINQDataContext(Properties.Settings.Default.LoginPracticeConnectionString);
        private user_Table _selectedUser;

        public UserManagementWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                var users = from u in db.user_Tables select u;
                userDataGrid.ItemsSource = users.ToList();
                statusTextBlock.Text = "Users loaded successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
                statusTextBlock.Text = "Error loading users.";
            }
        }

        private void addNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            string userId = addUserIdTextBox.Text.Trim();
            string password = addPasswordTextBox.Text.Trim(); // Remember to hash this!
            string userName = addUserNameTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userName))
            {
                if (!db.user_Tables.Any(u => u.UserID == userId))
                {
                    var newUser = new user_Table
                    {
                        UserID = userId,
                        UserPass = password, // In reality, store the hashed password
                        UserName = userName
                    };

                    db.user_Tables.InsertOnSubmit(newUser);

                    try
                    {
                        db.SubmitChanges();
                        LoadUsers();
                        statusTextBlock.Text = $"User '{userId}' added successfully.";
                        addUserIdTextBox.Clear();
                        addPasswordTextBox.Clear();
                        addUserNameTextBox.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error adding user: " + ex.Message);
                        statusTextBlock.Text = "Error adding user.";
                    }
                }
                else
                {
                    MessageBox.Show($"User with ID '{userId}' already exists.");
                    statusTextBlock.Text = "User already exists.";
                }
            }
            else
            {
                MessageBox.Show("User ID, password, and user name cannot be empty.");
                statusTextBlock.Text = "User ID, password, and user name cannot be empty.";
            }
        }

        private void userDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (userDataGrid.SelectedItem != null && userDataGrid.SelectedItem is user_Table selectedUser)
            {
                _selectedUser = selectedUser;
                updateUserIdTextBox.Text = _selectedUser.UserID;
                updatePasswordTextBox.Text = _selectedUser.UserPass;
                updateUserNameTextBox.Text = _selectedUser.UserName;
                viewUserIdTextBox.Text = _selectedUser.UserID;
                viewPasswordTextBox.Text = _selectedUser.UserPass;
                viewUserNameTextBox.Text = _selectedUser.UserName;
                saveUserButton.IsEnabled = true;
                deleteSelectedUserButton.IsEnabled = true;
            }
            else
            {
                _selectedUser = null;
                updateUserIdTextBox.Clear();
                updatePasswordTextBox.Clear();
                updateUserNameTextBox.Clear();
                viewUserIdTextBox.Clear();
                viewPasswordTextBox.Clear();
                viewUserNameTextBox.Clear();
                saveUserButton.IsEnabled = false;
                deleteSelectedUserButton.IsEnabled = false;
            }
        }

        private void saveUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser != null)
            {
                _selectedUser.UserPass = updatePasswordTextBox.Text.Trim(); // Remember to hash this!
                _selectedUser.UserName = updateUserNameTextBox.Text.Trim();

                try
                {
                    db.SubmitChanges();
                    LoadUsers();
                    statusTextBlock.Text = $"User '{_selectedUser.UserID}' updated successfully.";
                    updateUserIdTextBox.Clear();
                    updatePasswordTextBox.Clear();
                    updateUserNameTextBox.Clear();
                    viewUserIdTextBox.Clear();
                    viewPasswordTextBox.Clear();
                    viewUserNameTextBox.Clear();
                    saveUserButton.IsEnabled = false;
                    _selectedUser = null; // Clear the selected user
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating user: " + ex.Message);
                    statusTextBlock.Text = "Error updating user.";
                }
            }
            else
            {
                MessageBox.Show("No user selected for update.");
                statusTextBlock.Text = "No user selected for update.";
            }
        }

        private void deleteSelectedUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser != null)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete user '{_selectedUser.UserID}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string deletedUserID = _selectedUser.UserID; // Store the UserID before setting _selectedUser to null

                        db.user_Tables.DeleteOnSubmit(_selectedUser);
                        db.SubmitChanges();

                        // Recreate the DataContext to clear its cache
                        db = new SampleLINQDataContext(Properties.Settings.Default.LoginPracticeConnectionString);

                        LoadUsers();
                        statusTextBlock.Text = $"User '{deletedUserID}' deleted successfully."; // Use the stored UserID
                        deleteSelectedUserButton.IsEnabled = false;
                        _selectedUser = null;
                        updateUserIdTextBox.Clear();
                        updatePasswordTextBox.Clear();
                        updateUserNameTextBox.Clear();
                        viewUserIdTextBox.Clear();
                        viewPasswordTextBox.Clear();
                        viewUserNameTextBox.Clear();
                        saveUserButton.IsEnabled = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        statusTextBlock.Text = "Error deleting user.";
                        // Optionally log the exception for more detailed debugging
                        // System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("No user selected for deletion.");
                statusTextBlock.Text = "No user selected for deletion.";
            }
        }
    }
}