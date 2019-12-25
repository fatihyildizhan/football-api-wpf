using System;
using System.Windows;
using System.Net.Http;
using System.Collections.Generic;
// Newtonsoft
using Newtonsoft.Json;

namespace football_api_wpf.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FetchTeams();

            selectTeamComboBox.SelectionChanged += SelectTeamComboBox_SelectionChanged;
        }

        public void FetchTeams()
        {
            TeamResponse teamResponse = FetchJson<TeamResponse>("https://api.football-data.org/v2/competitions/2021/teams");

            selectTeamComboBox.DisplayMemberPath = "Name";
            selectTeamComboBox.ItemsSource = teamResponse.Teams;
        }

        private void SelectTeamComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int id = ((Team)e.AddedItems[0]).Id;

            PlayerResponse playerResponse = FetchJson<PlayerResponse>("https://api.football-data.org/v2/teams/" + id);

            playersListBox.Items.Clear();
            foreach (Player p in playerResponse.Squad)
            {
                playersListBox.Items.Add(p.ToString());
            }
        }

        private static T FetchJson<T>(string url) where T : new()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Auth-Token", "get-your-free-api-key");

                var jsonString = string.Empty;
                try
                {
                    var jsonResponse = client.GetAsync(url).Result;
                    jsonString = jsonResponse.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {

                }

                return !string.IsNullOrEmpty(jsonString)
                           ? JsonConvert.DeserializeObject<T>(jsonString)
                           : new T();
            }
        }

        public class Team
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
        }

        public class TeamResponse
        {
            public List<Team> Teams { get; set; }
        }

        public class Player
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string Nationality { get; set; }
            public string Role { get; set; }

            public override string ToString()
            {
                return $"{Name} {Position} {Nationality}";
            }
        }

        public class PlayerResponse
        {
            public List<Player> Squad { get; set; }
        }
    }
}
