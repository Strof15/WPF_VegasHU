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
using MySql.Data;
using MySql.Data.MySqlClient;
namespace VegasHU
{
    public partial class EventCreationPage : Window
    {
        private readonly string connectionString = "Server=localhost;Database=vegashu;Uid=root;Pwd=;";
        public EventCreationPage()
        {
            InitializeComponent();
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

                const string query = @"INSERT INTO Events (EventName, EventDate, Category, Location) 
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
            EventListPanel.Visibility = Visibility.Visible;
            NewEventPanel.Visibility = Visibility.Collapsed;
        }

        private void btnCreateEvents_Click(object sender, RoutedEventArgs e)
        {
            EventListPanel.Visibility = Visibility.Collapsed;
            NewEventPanel.Visibility = Visibility.Visible;
        }
    }
}
