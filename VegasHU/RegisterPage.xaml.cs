using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VegasHU
{
    public partial class RegisterPage : Window
    {
        private readonly string connectionString = "Server=localhost;Database=vegashu;Uid=root;Pwd=;";
        public RegisterPage()
        {
            InitializeComponent();
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsRegistrationValid())
                {
                    RegisterUser(tbUsername.Text, tbPassword.Password, tbEmail.Text);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void RegisterUser(string username, string password, string email)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                const string query = @"INSERT INTO Bettors (username, password, email, joindate) VALUES (@username, @password, @mail, @joindate)";

                using (var command = new MySqlCommand(query, connection))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                    command.Parameters.AddWithValue("@username", username.ToLower());
                    command.Parameters.AddWithValue("@password", hashedPassword);
                    command.Parameters.AddWithValue("@mail", email);
                    command.Parameters.AddWithValue("@joindate", DateTime.Now);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sikeres regisztráció!", "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
                        OpenMainWindow();
                    }
                    else
                    {
                        ShowErrorMessage("A regisztráció sikertelen. Próbáld újra.");
                    }
                }
            }
        }

        private bool IsRegistrationValid()
        {
            return IsValidUsername(tbUsername.Text) &&
                   IsValidPassword(tbPassword.Password) &&
                   IsValidEmail(tbEmail.Text);
        }

        private bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowErrorMessage("A felhasználónév nem lehet üres.");
                return false;
            }

            if (username.Length < 3 || username.Length > 15 || !Regex.IsMatch(username, @"^[a-zA-Z]+$"))
            {
                ShowErrorMessage("Betűkből kell állnia és legalább 3, maximum 15 karakter hosszú.");
                return false;
            }

            return true;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("A jelszó nem lehet üres!");
                return false;
            }

            if (password.Length < 6 || password.Length > 16 || !Regex.IsMatch(password, @"[A-Z]"))
            {
                ShowErrorMessage("A jelszónak tartalmaznia kell legalább 1 nagy betűt és legalább 6, maximum 16 karakter hosszúnak kell lennie.");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowErrorMessage("Az email nem lehet üres!");
                return false;
            }

            if (!email.Contains("@"))
            {
                ShowErrorMessage("Az emailnek valósnak kell lennie!");
                return false;
            }

            return true;
        }
        private void OpenMainWindow()
        {
            new LoginPage().Show();
            Close();
        }
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnToLogin_Click(object sender, RoutedEventArgs e)
        {
            new LoginPage().Show();
            Close();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
