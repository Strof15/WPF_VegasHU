using MySql.Data.MySqlClient;
using Mysqlx.Session;
using MySqlX.XDevAPI.Common;
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
using static VegasHU.LoginPage;

namespace VegasHU
{
    public partial class PlaceBetPage : Window
    {
        private double odds;
        private double balance;
        private int eventId;
        private readonly string connectionString = "Server=localhost;Database=vegashu;Uid=root;Pwd=;";
        private MainPage mainPage;

        public PlaceBetPage(MainPage mainPage, string eventName, double odds, string result, double balance, int eventId)
        {
            InitializeComponent();
            this.mainPage = mainPage;
            this.odds = odds;
            this.balance = balance;
            this.eventId = eventId;
            LoadDatas(eventName, odds, result, balance);
            RefreshBalance();
        }

        private void LoadDatas(string eventName, double odds, string result, double balance)
        {
            lblTicketName.Text = $"{eventName}";
            lblBoolean.Text = result;
            lblOdds.Text = $"@{odds}";

            lblBalance.Text = $"Egyenleg: {Session.CurrentBettor.Balance} ft";

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tbAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(tbAmount.Text, out double amount))
            {
                if (amount > Session.CurrentBettor.Balance)
                {
                    tbAmount.Text = Session.CurrentBettor.Balance.ToString();
                    tbAmount.SelectionStart = tbAmount.Text.Length;
                }
                else
                {
                    lblPossibleWin.Text = $"Várható nyeremény: {amount * odds} ft";
                }
            }
            else
            {
                lblPossibleWin.Text = "Várható nyeremény: 0 ft";
            }
        }

        private void btnPlaceBet_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                PlaceBet();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(tbAmount.Text))
            {
                ShowErrorMessage("Tét megadása kötelező!");
                return false;
            }

            if (!double.TryParse(tbAmount.Text, out double amount))
            {
                ShowErrorMessage("Csak számokat lehet megadni a tét mezőben!");
                return false;
            }

            if (amount <= 0)
            {
                ShowErrorMessage("A tétnek nagyobbnak kell lennie, mint 0!");
                return false;
            }

            return true;
        }


        private void PlaceBet()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Bets (BetDate, Odds, Amount, BettorsID, EventID, Status)
                                    SELECT @betdate, @odds, @amount, @bettorsid, @eventid, @status
                                    FROM Bettors b
                                    INNER JOIN Events e ON b.BettorsID = @bettorsid AND e.EventID = @eventid
                                    WHERE b.BettorsID = @bettorsid AND e.EventID = @eventid";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@betdate", DateTime.Now);
                    command.Parameters.AddWithValue("@odds", odds);
                    command.Parameters.AddWithValue("@amount", double.Parse(tbAmount.Text));
                    command.Parameters.AddWithValue("@bettorsid", Session.CurrentBettor.BettorsId);
                    command.Parameters.AddWithValue("@eventid", eventId);
                    command.Parameters.AddWithValue("@status", 0);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        UpdateBalance();
                        CloseBets();
                        MessageBox.Show("A fogadás sikeres volt!", "VegasHU System", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    else
                    {
                        ShowErrorMessage("A fogadás nem sikerült!");
                    }
                }
            }
        }
        private void CloseBets()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"UPDATE Bets b
                                JOIN Events e ON b.EventID = e.EventID
                                SET b.Status = CASE 
                                    WHEN e.EventDate < NOW() AND b.Status = 0 THEN
                                        CASE WHEN RAND() < 0.5 THEN 1 ELSE 2 END
                                    WHEN e.EventDate < NOW() AND b.Status = 2 THEN 3 -- mark as paid out
                                    ELSE b.Status
                                END
                                WHERE e.EventDate < NOW() AND b.Status IN (0, 2);";

                using (var command = new MySqlCommand(query, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string payoutQuery = @"SELECT SUM(b.Odds * b.Amount) AS TotalWinnings
                                                FROM Bets b
                                                WHERE b.BettorsID = @bettorsID AND b.Status = 2;";

                        using (var payoutCommand = new MySqlCommand(payoutQuery, connection))
                        {
                            payoutCommand.Parameters.AddWithValue("@bettorsID", Session.CurrentBettor.BettorsId);

                            object result = payoutCommand.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                int totalWinnings = Convert.ToInt32(result);

                                string updateBalanceQuery = "UPDATE Bettors SET Balance = Balance + @amount WHERE BettorsID = @bettorsid";
                                using (var updateBalanceCommand = new MySqlCommand(updateBalanceQuery, connection))
                                {
                                    updateBalanceCommand.Parameters.AddWithValue("@amount", totalWinnings);
                                    updateBalanceCommand.Parameters.AddWithValue("@bettorsid", Session.CurrentBettor.BettorsId);
                                    updateBalanceCommand.ExecuteNonQuery();

                                    Session.CurrentBettor.Balance += totalWinnings;

                                    RefreshBalance();
                                }
                            }
                        }
                    }
                }
            }
        }
        private void UpdateBalance()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Bettors SET Balance = Balance - @amount WHERE BettorsID = @bettorsid";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@amount", double.Parse(tbAmount.Text));
                    command.Parameters.AddWithValue("@bettorsid", Session.CurrentBettor.BettorsId);

                    command.ExecuteNonQuery();

                    Session.CurrentBettor.Balance -= int.Parse(tbAmount.Text);
                }
            }
            lblBalance.Text = $"Egyenleg: {Session.CurrentBettor.Balance} ft";
            mainPage.lblBalance.Text = $"Egyenleg: {Session.CurrentBettor.Balance} ft";
        }

        private void RefreshBalance()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Balance FROM Bettors WHERE BettorsID = @bettorsid";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bettorsid", Session.CurrentBettor.BettorsId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Session.CurrentBettor.Balance = reader.GetInt32("Balance");
                            lblBalance.Text = $"Egyenleg: {Session.CurrentBettor.Balance} ft";
                            mainPage.lblBalance.Text  = $"Egyenleg: {Session.CurrentBettor.Balance} ft";
                        }
                    }
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "VegasHU System", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}