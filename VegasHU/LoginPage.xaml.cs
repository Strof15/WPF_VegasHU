using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
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
using VegasHU;
using VegasHU.Models;

namespace VegasHU
{
    public partial class LoginPage : Window
    {
        private readonly string connectionString = "Server=localhost;Database=vegashu;Uid=root;Pwd=;";
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password = tbPassword.Password;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    using (var sqlConn = new MySqlConnection(connectionString))
                    {
                        sqlConn.Open();

                        string query = "SELECT BettorsId, Username, Password FROM Bettors WHERE username=@username";

                        using (var sqlCommand = new MySqlCommand(query, sqlConn))
                        {
                            sqlCommand.Parameters.AddWithValue("@username", username);
                            
                            using (var reader = sqlCommand.ExecuteReader())
                            {
                                if (reader.HasRows && reader.Read())
                                {
                                    string storedHashedPassword = reader["password"].ToString();

                                    if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                                    {
                                        var loggedInBettor = new Bettors
                                        {
                                            BettorsId = Convert.ToInt32(reader["BettorsID"]),
                                            Username = reader["Username"].ToString()
                                        };

                                        Session.CurrentBettor = loggedInBettor;
                                        sqlConn.Close();
                                        new RegisterPage().Show();
                                        Close();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Hibás jelszó!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Hibás felhasználónév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
            else
            {
                MessageBox.Show("Adj meg felhasználónevet és jelszót!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnToRegister_Click(object sender, RoutedEventArgs e)
        {
            new RegisterPage().Show();
            Close();
        }
        public static class Session
        {
            public static Bettors CurrentBettor { get; set; }
        }
    }
}
