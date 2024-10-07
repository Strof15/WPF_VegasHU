using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VegasHU.Models;
using static VegasHU.LoginPage;

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
                    var placeBetPage = new PlaceBetPage(
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
                string query = "SELECT EventID,EventName, EventDate, Category, Location FROM Events";
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
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveUserDatas();
        }
        private void HomePanelShow() {
            HomePanel.Visibility = Visibility.Visible;
            BetPanel.Visibility = Visibility.Collapsed;
            MyBetsPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;
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
        }

        private void btnMyBetsPanel_Click(object sender, RoutedEventArgs e)
        {
            MyBetsPanelShow();
        }

        private void MyBetsPanelShow()
        {
            HomePanel.Visibility = Visibility.Collapsed;
            BetPanel.Visibility = Visibility.Collapsed;
            MyBetsPanel.Visibility = Visibility.Visible;
            ProfilePanel.Visibility = Visibility.Collapsed;
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
        }
    }
}