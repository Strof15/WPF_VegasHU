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
                                    IsActive = reader.GetInt32(4),
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
        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is Bettors selectedBettor)
            {
                tbUsername.Text = selectedBettor.Username;
                tbEmail.Text = selectedBettor.Email;
                tbBalance.Text = selectedBettor.Balance.ToString();
                tbPassword.Clear();
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbUsername.Text) && !string.IsNullOrEmpty(tbEmail.Text) && !string.IsNullOrEmpty(tbPassword.Text) && !string.IsNullOrEmpty(tbBalance.Text))
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Bettors (Username, Password, Email, Balance, JoinDate, IsActive) VALUES (@Username, @Password, @Email, @Balance, @JoinDate, @IsActive)";

                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", tbUsername.Text);
                            command.Parameters.AddWithValue("@Email", tbEmail.Text);
                            command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(tbPassword.Text));
                            command.Parameters.AddWithValue("@Balance", int.Parse(tbBalance.Text));
                            command.Parameters.AddWithValue("@JoinDate", DateTime.Now);
                            command.Parameters.AddWithValue("@IsActive", 1);

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadUsers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            else
            {
                ShowErrorMessage("Nem lehet mező üresen!");
            }
        }

        private void btnModifyUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is Bettors selectedBettor)
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "UPDATE Bettors SET Username = @Username, Email = @Email, Balance = @Balance, IsActive = @IsActive";

                        if (!string.IsNullOrEmpty(tbPassword.Text))
                        {
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tbPassword.Text);
                            query += ", Password = @Password";
                        }

                        query += " WHERE BettorsID = @BettorsID";

                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", tbUsername.Text);
                            command.Parameters.AddWithValue("@Email", tbEmail.Text);
                            command.Parameters.AddWithValue("@Balance", int.Parse(tbBalance.Text));
                            command.Parameters.AddWithValue("@IsActive", 1);
                            command.Parameters.AddWithValue("@BettorsID", selectedBettor.BettorsId);

                            if (!string.IsNullOrEmpty(tbPassword.Text))
                            {
                                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tbPassword.Text);
                                command.Parameters.AddWithValue("@Password", hashedPassword);
                            }

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadUsers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            else
            {
                ShowErrorMessage("Válassz ki egy felhasználót a szerkesztéshez!");
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

                    LoadUsers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            else
            {
                ShowErrorMessage("Válassz ki egy felhasználót a törléshez!");
            }
        }

        private void ClearFields()
        {
            tbUsername.Clear();
            tbEmail.Clear();
            tbPassword.Clear();
            tbBalance.Clear();
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
