using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            // Iterate over each betting event
            foreach (var bettingEvent in events)
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

                // Add the card to the corresponding tab based on event category
                switch (bettingEvent.Category)
                {
                    case "Labdarugás": // Football
                        ((StackPanel)tabFootball.Content).Children.Add(eventCardBorder);
                        break;
                    case "Darts":
                        ((StackPanel)tabDarts.Content).Children.Add(eventCardBorder);
                        break;
                    case "Röplabda": // Volleyball
                        ((StackPanel)tabVolleyball.Content).Children.Add(eventCardBorder);
                        break;
                    case "Jégkorong": // Hockey
                        ((StackPanel)tabHockey.Content).Children.Add(eventCardBorder);
                        break;
                    case "Kosárlabda": // Basketball
                        ((StackPanel)tabBasketball.Content).Children.Add(eventCardBorder);
                        break;
                    default:
                        break; // Or handle unexpected categories
                }
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
       
    }
}
