using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VegasHU.Models;
using static VegasHU.LoginPage;
using System.Windows.Controls.Primitives;

namespace VegasHU
{
    public partial class MainPage : Window
    {
        private readonly string connectionString = "Server=localhost;Database=vegashu;Uid=root;Pwd=;";

        public MainPage()
        {
            InitializeComponent();
            LoadEventCards();
            LoadMoreEventsCards();
            LoadUserData();
            HomePanelShow();
            RefreshBalance();
        }

        private void LoadEventCards()
        {
            List<Event> events = GetEventsFromDatabase();
            Random random = new Random();

            List<Event> randomEvents = events.OrderBy(e => random.Next()).Take(5).ToList();

            foreach (var bettingEvent in randomEvents)
            {
                Border eventCardBorder = new Border
                {
                    Style = (Style)FindResource("ColoredStackPanelBorder")
                };

                StackPanel eventCardStack = new StackPanel();

                StackPanel horizontalStack = new StackPanel { Orientation = Orientation.Horizontal };
                Label sportLabel = new Label
                {
                    Style = (Style)FindResource("SmallLabels"),
                    Content = bettingEvent.Category
                };
                Label dateLabel = new Label
                {
                    Style = (Style)FindResource("SmallLabels"),
                    Content = bettingEvent.EventDate.ToString("MM.dd"),
                    Margin = new Thickness(240, 0, 0, 0)
                };
                horizontalStack.Children.Add(sportLabel);
                horizontalStack.Children.Add(dateLabel);

                Label eventNameLabel = new Label
                {
                    Style = (Style)FindResource("MediumLabels"),
                    Content = bettingEvent.EventName
                };

                Button oddsButton = new Button
                {
                    Margin = new Thickness(0, 20, 0, 0),
                    Width = 90,
                    Height = 60
                };

                StackPanel oddsButtonStack = new StackPanel { Orientation = Orientation.Vertical };

                double odds = Math.Round(random.NextDouble() * (4.0 - 1.5) + 1.5, 2);
                string result = random.Next(2) == 0 ? "Igen" : "Nem";

                Label oddsLabel = new Label
                {
                    Style = (Style)FindResource("LargeLabels"),
                    Content = odds.ToString()
                };
                Label resultLabel = new Label
                {
                    Style = (Style)FindResource("SmallLabels"),
                    Content = result
                };
                oddsButton.Click += (s, e) =>
                {
                    var placeBetPage = new PlaceBetPage(this,
                        bettingEvent.EventName,
                        odds,
                        result,
                        Session.CurrentBettor.Balance,
                        bettingEvent.EventID
                    );
                    placeBetPage.ShowDialog();
                };

                oddsButtonStack.Children.Add(oddsLabel);
                oddsButtonStack.Children.Add(resultLabel);
                oddsButton.Content = oddsButtonStack;

                eventCardStack.Children.Add(horizontalStack);
                eventCardStack.Children.Add(eventNameLabel);
                eventCardStack.Children.Add(oddsButton);
                eventCardBorder.Child = eventCardStack;

                mainStackPanel.Children.Add(eventCardBorder);
            }
        }

        private void LoadMoreEventsCards()
        {
            List<Event> events = GetEventsFromDatabase();
            Random random = new Random();

            foreach (var bettingEvent in events)
            {
                string result = random.Next(2) == 0 ? "Igen" : "Nem";
                Border eventCardBorder = new Border
                {
                    Width = 800,
                    Height = 100,
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#784599")),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                StackPanel eventCardStack = new StackPanel() { Orientation = Orientation.Horizontal };
                StackPanel horizontalStack = new StackPanel { Orientation = Orientation.Horizontal };

                Label sportLabel = new Label
                {
                    Style = (Style)FindResource("SmallLabels"),
                    Width = 80,
                    Content = bettingEvent.Category,
                    Margin = new Thickness(20, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Label dateLabel = new Label
                {
                    Style = (Style)FindResource("SmallLabels"),
                    Content = bettingEvent.EventDate.ToString("MM.dd"),
                    Margin = new Thickness(5, 0, 20, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                horizontalStack.Children.Add(sportLabel);
                horizontalStack.Children.Add(dateLabel);

                Border imageBorder = new Border
                {
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#784599")),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    CornerRadius = new CornerRadius(5),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A3A"))
                };

                Image eventImage = new Image { Width = 20, Margin = new Thickness(15, 5, 15, 5) };

                switch (bettingEvent.Category)
                {
                    case "Labdarugás":
                        eventImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/001-football.png"));
                        FootballEventsGrid.Children.Add(eventCardBorder);
                        break;

                    case "Darts":
                        eventImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/001-dart.png"));
                        DartsEventsGrid.Children.Add(eventCardBorder);
                        break;

                    case "Röplabda":
                        eventImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/002-volleyball.png"));
                        VolleyballEventsGrid.Children.Add(eventCardBorder);
                        break;

                    case "Jégkorong":
                        eventImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/003-hockey-sticks.png"));
                        HockeyEventsGrid.Children.Add(eventCardBorder);
                        break;

                    case "Kosárlabda":
                        eventImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/002-basketball.png"));
                        BasketballEventsGrid.Children.Add(eventCardBorder);
                        break;

                    default:
                        break;
                }

                imageBorder.Child = eventImage;

                Label eventNameLabel = new Label
                {
                    Width = 400,
                    Height = 40,
                    Content = bettingEvent.EventName,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Style = (Style)FindResource("MediumLabels"),
                    Margin = new Thickness(0, 0, 20, 0)
                };
                Label resultLabel = new Label
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    Style = (Style)FindResource("SmallLabels"),
                    Content = result,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button oddsButton = new Button
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    Width = 120,
                    Height = 60,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };

                StackPanel oddsButtonStack = new StackPanel { Orientation = Orientation.Horizontal };

                double odds = Math.Round(random.NextDouble() * (4.0 - 1.5) + 1.5, 2);

                Label oddsLabel = new Label
                {
                    Style = (Style)FindResource("LargeLabels"),
                    Content = odds.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                oddsButton.Click += (s, e) =>
                {
                    var placeBetPage = new PlaceBetPage(
                        this,
                        bettingEvent.EventName,
                        odds,
                        result,
                        Session.CurrentBettor.Balance,
                        bettingEvent.EventID
                    );
                    placeBetPage.ShowDialog();

                };

                oddsButtonStack.Children.Add(oddsLabel);
                oddsButtonStack.Children.Add(resultLabel);
                oddsButton.Content = oddsButtonStack;

                eventCardStack.Children.Add(horizontalStack);
                eventCardStack.Children.Add(imageBorder);
                eventCardStack.Children.Add(eventNameLabel);
                eventCardStack.Children.Add(oddsButton);

                eventCardBorder.Child = eventCardStack;
            }
        }
        private List<Event> GetEventsFromDatabase()
        {
            var events = new List<Event>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT EventID, EventName, EventDate, Category, Location FROM Events WHERE EventDate >= NOW()";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(new Event
                            {
                                EventID = reader.GetInt32("EventID"),
                                EventName = reader.GetString("EventName"),
                                EventDate = reader.GetDateTime("EventDate"),
                                Category = reader.GetString("Category"),
                                Location = reader.GetString("Location")
                            });
                        }
                    }
                }
            }
            return events;
        }
        private List<Bets> GetBetsForCurrentBettor(int bettorId)
        {
            List<Bets> betsList = new List<Bets>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT b.BetsID, b.BetDate, b.Odds, b.Amount, b.Status, e.EventID, e.EventName, e.EventDate, e.Category, e.Location
                                FROM Bets b 
                                JOIN Events e ON b.EventID = e.EventID 
                                WHERE b.BettorsID = @bettorsID";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bettorsID", bettorId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Bets bet = new Bets
                            {
                                BetsID = reader.GetInt32("BetsID"),
                                BetDate = reader.GetDateTime("BetDate"),
                                Odds = reader.GetFloat("Odds"),
                                Amount = reader.GetInt32("Amount"),
                                Status = reader.GetInt32("Status"),

                                Event = new Event
                                {
                                    EventID = reader.GetInt32("EventID"),
                                    EventName = reader.GetString("EventName"),
                                    EventDate = reader.GetDateTime("EventDate"),
                                    Category = reader.GetString("Category"),
                                    Location = reader.GetString("Location")
                                }
                            };
                            betsList.Add(bet);
                        }
                    }
                }
            }
            return betsList;
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
                                    WHEN e.EventDate < NOW() AND b.Status = 2 THEN 3
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

        private void LoadBets(int bettorId)
        {
            BetsStackPanel.Children.Clear();
            CloseBets();
            RefreshBalance();
            List<Bets> bets = GetBetsForCurrentBettor(bettorId);
            UniformGrid betGrid = new UniformGrid
            {
                
                Columns = 2,
                Background = new SolidColorBrush(Color.FromRgb(50, 50, 50))
            };

            foreach (var bet in bets)
            {
                Border betCard = new Border
                {
                    Height = 250,
                    Style = (Style)FindResource("ColoredStackPanelBorder"),
                    VerticalAlignment = VerticalAlignment.Top
                };

                StackPanel stackPanel = new StackPanel();

                StackPanel eventInfoPanel = new StackPanel { Orientation = Orientation.Horizontal };
                eventInfoPanel.Children.Add(new Label
                {
                    Content = bet.Event.Category,
                    Style = (Style)FindResource("SmallLabels"),
                    Width = 80
                });
                eventInfoPanel.Children.Add(new Label
                {
                    Content = bet.Event.EventDate.ToString("MM.dd"),
                    Style = (Style)FindResource("SmallLabels"),
                    Margin = new Thickness(230, 0, 0, 0)
                });
                stackPanel.Children.Add(eventInfoPanel);

                stackPanel.Children.Add(new TextBlock
                {
                    Text = bet.Event.EventName,
                    Style = (Style)FindResource("MediumLabelsText"),
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                stackPanel.Children.Add(new Label
                {
                    Content = bet.Event.Location,
                    Style = (Style)FindResource("SmallLabels"),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                string statusText;
                if (bet.Status == 0) statusText = "nyitott";
                else if (bet.Status == 1) statusText = "vesztett";
                else statusText = "nyert";

                stackPanel.Children.Add(new Label
                {
                    Content = statusText,
                    Style = (Style)FindResource("SmallLabels"),
                    Foreground = bet.Status == 1 ? Brushes.Red : bet.Status == 2 || bet.Status == 3 ? Brushes.Green : Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                stackPanel.Children.Add(new Label
                {
                    Content = $"Nyeremény: {(bet.Status == 2 || bet.Status == 3 ? (bet.Odds * bet.Amount).ToString("N0") : "0")} ft",
                    Style = (Style)FindResource("SmallLabels"),
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                Border oddsBorder = new Border
                {
                    Width = 70,
                    Height = 48,
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(120, 69, 153)),
                    Background = new SolidColorBrush(Color.FromRgb(120, 69, 153)),
                    CornerRadius = new CornerRadius(5)
                };

                StackPanel oddsPanel = new StackPanel();
                oddsPanel.Children.Add(new Label
                {
                    Content = bet.Odds.ToString("N1"),
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                oddsPanel.Children.Add(new Label
                {
                    Content = "Nem",
                    FontSize = 8,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                oddsBorder.Child = oddsPanel;
                stackPanel.Children.Add(oddsBorder);

                betCard.Child = stackPanel;
                betGrid.Children.Add(betCard);
            }

            BetsStackPanel.Children.Add(betGrid);
        }

        private void LoadUserData()
        {
            using (var sqlConn = new MySqlConnection(connectionString))
            {
                sqlConn.Open();

                string query = @"
                                SELECT Username, Password, Email, JoinDate
                                FROM Bettors
                                WHERE BettorsID=@userId";

                using (var sqlCommand = new MySqlCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@userId", Session.CurrentBettor.BettorsId);

                    using (MySqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            tbUsername.Text = reader["Username"].ToString();

                            tbEmail.Text = reader["Email"].ToString();
                            tbJoinDate.Text = reader["JoinDate"].ToString();
                        }
                        else
                        {
                            tbUsername.Text = "";
                            tbPassword.Text = "";

                            tbEmail.Text = "";
                            tbJoinDate.Text = "";
                        }
                    }
                }
            }
        }
        private void SaveUserDatas()
        {
            using (var sqlConn = new MySqlConnection(connectionString))
            {
                sqlConn.Open();

                string query = @"
                        UPDATE Bettors 
                        SET username=@username, email=@mail
                        WHERE BettorsID=@userId";

                if (!string.IsNullOrWhiteSpace(tbPassword.Text))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tbPassword.Text);

                    query = @"
                        UPDATE Bettors 
                        SET username=@username, email=@mail, password=@password
                        WHERE BettorsID=@userId";
                }

                using (var sqlCommand = new MySqlCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@username", tbUsername.Text);
                    sqlCommand.Parameters.AddWithValue("@mail", tbEmail.Text);
                    sqlCommand.Parameters.AddWithValue("@userId", Session.CurrentBettor.BettorsId);

                    if (query.Contains("password"))
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tbPassword.Text);
                        sqlCommand.Parameters.AddWithValue("@password", hashedPassword);
                    }

                    sqlCommand.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Adatait sikeresen frissítette!", "VegasHU System", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateBalance()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Bettors SET Balance = Balance + @amount WHERE BettorsID = @bettorsid";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@amount", double.Parse(tbBalance.Text));
                    command.Parameters.AddWithValue("@bettorsid", Session.CurrentBettor.BettorsId);

                    command.ExecuteNonQuery();

                    Session.CurrentBettor.Balance += int.Parse(tbBalance.Text);
                }
            }

            lblBalance.Text = $"Egyenleg: {Session.CurrentBettor.Balance} ft";
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
                        }
                    }
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveUserDatas();
        }

        private void btnAddBalance_Click(object sender, RoutedEventArgs e)
        {
            UpdateBalance();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginPage Loginwindow = new();
            Loginwindow.Show();
            this.Close();
        }

        private void btnHomePanel_Click(object sender, RoutedEventArgs e)
        {
            HomePanelShow();
        }
        private void HomePanelShow() {
            HomePanel.Visibility = Visibility.Visible;
            BetPanel.Visibility = Visibility.Collapsed;
            MyBetsPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;
            NavPanel.Visibility = Visibility.Collapsed;
        }

        private void btnBetPanel_Click(object sender, RoutedEventArgs e)
        {
            BetPanelShow();
        }
        private void BetPanelShow()
        {
          
            HomePanel.Visibility = Visibility.Collapsed;
            BetPanel.Visibility = Visibility.Visible;
            MyBetsPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;
            NavPanel.Visibility = Visibility.Visible;
        }
        private void btnMyBetsPanel_Click(object sender, RoutedEventArgs e)
        {
            MyBetsPanelShow();
            LoadBets(Session.CurrentBettor.BettorsId);
        }

        private void MyBetsPanelShow()
        {
            HomePanel.Visibility = Visibility.Collapsed;
            BetPanel.Visibility = Visibility.Collapsed;
            MyBetsPanel.Visibility = Visibility.Visible;
            ProfilePanel.Visibility = Visibility.Collapsed;
            NavPanel.Visibility = Visibility.Visible;
        }

        private void btnProfilePanel_Click(object sender, RoutedEventArgs e)
        {
            ProfilePanelShow();
        }

        private void ProfilePanelShow()
        {
            HomePanel.Visibility = Visibility.Collapsed;
            BetPanel.Visibility = Visibility.Collapsed;
            MyBetsPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Visible;
            NavPanel.Visibility = Visibility.Visible;
        }

        private void NextBet_Click(object sender, RoutedEventArgs e)
        {
            BetPanelShow();
        }

        private void NextMybet_Click(object sender, RoutedEventArgs e)
        {
           
            LoadBets(Session.CurrentBettor.BettorsId);
            MyBetsPanelShow();
        }

        private void NextProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfilePanelShow();
        }
    }
}