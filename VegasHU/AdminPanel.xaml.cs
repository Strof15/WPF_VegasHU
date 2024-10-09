using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;
using VegasHU.Models;

namespace VegasHU
{
    public partial class AdminPanel : Window
    {
        private readonly string connectionString = "Server=localhost;Database=vegashu;Uid=root;Pwd=;";
        public AdminPanel()
        {
            InitializeComponent();
            NewEventPanelShow();
        }
        private void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                EventCreation();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(tbEventName.Text) || string.IsNullOrWhiteSpace(tbEventLocation.Text) || cbCategories.SelectedIndex == -1)
            {
                ShowErrorMessage("Minden mezőt ki kell tölteni!");
                return false;
            }

            if (dpEventDate.SelectedDate == null)
            {
                ShowErrorMessage("Válasszon érvényes dátumot az eseményhez!");
                return false;
            }

            if (dpEventDate.SelectedDate < DateTime.Today)
            {
                ShowErrorMessage("Az esemény dátuma nem lehet korábbi, mint a mai nap!");
                return false;
            }

            return true;
        }

        private void EventCreation()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Events (EventName, EventDate, Category, Location) 
                               VALUES (@eventname, @eventdate, @category, @location)";

                using (var command = new MySqlCommand(query, connection))
                {
                    var comboBoxItem = cbCategories.SelectedItem as ComboBoxItem;

                    command.Parameters.AddWithValue("@eventname", tbEventName.Text);
                    command.Parameters.AddWithValue("@eventdate", dpEventDate.SelectedDate);
                    command.Parameters.AddWithValue("@category", comboBoxItem?.Content.ToString());
                    command.Parameters.AddWithValue("@location", tbEventLocation.Text);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sikeres esemény létrehozás!", "VegasHU System", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        ShowErrorMessage("Esemény létrehozása sikertelen!");
                    }
                }
            }
        }
        private void LoadEvents()
        {
            try
            {
                List<Event> events = new List<Event>();

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT EventID, EventName, EventDate, Category, Location FROM events";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                events.Add(new Event
                                {
                                    EventID = reader.GetInt32(0),
                                    EventName = reader.GetString(1),
                                    EventDate = reader.GetDateTime(2),
                                    Category = reader.GetString(3),
                                    Location = reader.GetString(4),
                                });
                            }
                        }
                    }
                }

                dgEvents.ItemsSource = events;
            }
            catch (Exception ex) { ShowErrorMessage(ex.Message); }
        }
        private void LoadUsers()
        {
            try
            {
                List<Bettors> bettors = new List<Bettors>();

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT BettorsID, Username, Email, Balance, IsActive FROM Bettors";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bettors.Add(new Bettors
                                {
                                    BettorsId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Balance = reader.GetInt32(3),
                                    IsActive = reader.GetBoolean(4),
                                });
                            }
                        }
                    }
                }

                dgUsers.ItemsSource = bettors;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newBettor = new Bettors
                {
                    Username = txtUsername.Text,
                    Email = txtEmail.Text,
                    Balance = int.Parse(txtBalance.Text),
                    IsActive = chkIsActive.IsChecked ?? false
                };

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Bettors (Username, Email, Balance, IsActive) VALUES (@Username, @Email, @Balance, @IsActive)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", newBettor.Username);
                        command.Parameters.AddWithValue("@Email", newBettor.Email);
                        command.Parameters.AddWithValue("@Balance", newBettor.Balance);
                        command.Parameters.AddWithValue("@IsActive", newBettor.IsActive);

                        command.ExecuteNonQuery();
                    }
                }

                LoadUsers(); // Refresh the DataGrid
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is Bettors selectedBettor)
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Bettors WHERE BettorsID = @BettorsID";

                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@BettorsID", selectedBettor.BettorsId);
                            command.ExecuteNonQuery();
                        }
                    }

                    LoadUsers(); // Refresh the DataGrid
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            else
            {
                ShowErrorMessage("Please select a user to delete.");
            }
        }
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "VegasHU System", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnListEvents_Click(object sender, RoutedEventArgs e)
        {
            EventListPanelShow();
            LoadEvents();
        }

        private void btnCreateEvents_Click(object sender, RoutedEventArgs e)
        {
            NewEventPanelShow();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            UsersPanelShow();
            LoadUsers();
        }
        private void NewEventPanelShow()
        {
            NewEventPanel.Visibility = Visibility.Visible;
            EventListPanel.Visibility = Visibility.Collapsed;
            UsersListPanel.Visibility = Visibility.Collapsed;
        }

        private void EventListPanelShow()
        {
            NewEventPanel.Visibility = Visibility.Collapsed;
            EventListPanel.Visibility = Visibility.Visible;
            UsersListPanel.Visibility = Visibility.Collapsed;
        }
        private void UsersPanelShow()
        {
            NewEventPanel.Visibility = Visibility.Collapsed;
            EventListPanel.Visibility = Visibility.Collapsed;
            UsersListPanel.Visibility = Visibility.Visible;
        }
        private void tbEventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLabelContent(lblName, tbEventName.Text);
        }
        private void cbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCategories.SelectedItem is ComboBoxItem comboBoxItem)
            {
                UpdateLabelContent(lblCategory, comboBoxItem.Content.ToString());
            }
        }
        private void tbEventLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLabelContent(lblLocation, tbEventLocation.Text);
        }
        private void dpEventDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpEventDate.SelectedDate.HasValue)
            {
                UpdateLabelContent(lblDate, dpEventDate.SelectedDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                UpdateLabelContent(lblDate, "");
            }
        }
        private void UpdateLabelContent(Label label, string content)
        {
            label.Content = content;
        }
    }
}
