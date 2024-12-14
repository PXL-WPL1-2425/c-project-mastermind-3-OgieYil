﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace Mastermind_PE_Oguzhan_Yilmaz_1PROA
{
    public partial class MainWindow : Window
    {
        private string[] generatedCode;
        private int attemptsLeft = 10;
        private int totalPenaltyPoints = 0;
        private List<string> playerNames = new List<string>(); // Lijst van spelers
        private int currentPlayerIndex = 0; // Houdt bij wie de huidige speler is
        private string playerName;  // Naam van de huidige speler
        private string[] highscores = new string[15]; // Array om de highscores op te slaan

        public MainWindow()
        {
            InitializeComponent();
            playerNames = StartGame();  // Vraag de namen van meerdere spelers
            GenerateRandomCode();
            OpvullenComboBoxes();
            UpdateTitle();
        }

        // Methode om meerdere spelersnamen in te voeren
        private List<string> StartGame()
        {
            List<string> names = new List<string>();
            string name;

            do
            {
                // Vraag om de naam van de speler
                name = Microsoft.VisualBasic.Interaction.InputBox(
                    "Voer de naam van de speler in:",
                    "Speler Naam",
                    ""
                );

                if (!string.IsNullOrEmpty(name))
                {
                    names.Add(name);
                }

                // Vraag of er nog een speler toegevoegd moet worden
                MessageBoxResult result = MessageBox.Show(
                    "Wil je nog een speler toevoegen?",
                    "Nieuwe speler?",
                    MessageBoxButton.YesNo
                );
            }
            while (names.Count == 0 || MessageBox.Show("Wil je nog een speler toevoegen?", "Nieuwe speler?", MessageBoxButton.YesNo) == MessageBoxResult.Yes);

            return names;
        }

        private void GenerateRandomCode()
        {
            Random random = new Random();
            string[] Colors = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
            generatedCode = Enumerable.Range(0, 4).Select(_ => Colors[random.Next(Colors.Length)]).ToArray();
        }

        private void OpvullenComboBoxes()
        {
            string[] Colors = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
            ComboBox1.ItemsSource = Colors;
            ComboBox2.ItemsSource = Colors;
            ComboBox3.ItemsSource = Colors;
            ComboBox4.ItemsSource = Colors;
        }

        private void UpdateTitle()
        {
            this.Title = $"MasterMind - {playerName} - Pogingen over: {attemptsLeft}";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Label1.Background = GetBrushFromColorName(ComboBox1.SelectedItem as string ?? "default");
            Label2.Background = GetBrushFromColorName(ComboBox2.SelectedItem as string ?? "default");
            Label3.Background = GetBrushFromColorName(ComboBox3.SelectedItem as string ?? "default");
            Label4.Background = GetBrushFromColorName(ComboBox4.SelectedItem as string ?? "default");
        }

        private SolidColorBrush GetBrushFromColorName(string colorName)
        {
            return colorName switch
            {
                "Rood" => Brushes.Red,
                "Geel" => Brushes.Yellow,
                "Oranje" => Brushes.Orange,
                "Wit" => Brushes.White,
                "Groen" => Brushes.Green,
                "Blauw" => Brushes.Blue,
                _ => Brushes.Transparent
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (attemptsLeft <= 0)
            {
                ShowEndGameMessage(false);
                return;
            }

            string[] userCode = {
                ComboBox1.SelectedItem != null ? ComboBox1.SelectedItem.ToString() : "default",
                ComboBox2.SelectedItem != null ? ComboBox2.SelectedItem.ToString() : "default",
                ComboBox3.SelectedItem != null ? ComboBox3.SelectedItem.ToString() : "default",
                ComboBox4.SelectedItem != null ? ComboBox4.SelectedItem.ToString() : "default"
            };

            int score = CalculateScore(userCode);
            totalPenaltyPoints += score;
            DisplayScore(score);
            string feedback = GenerateFeedback(userCode);
            LogAttempt(userCode, feedback);

            CheckColor(Label1, userCode[0], 0);
            CheckColor(Label2, userCode[1], 1);
            CheckColor(Label3, userCode[2], 2);
            CheckColor(Label4, userCode[3], 3);

            if (userCode.SequenceEqual(generatedCode))
            {
                AddToHighscores(playerName, attemptsLeft, totalPenaltyPoints);
                ShowEndGameMessage(true);
                return;
            }

            attemptsLeft--;
            UpdateTitle();

            if (attemptsLeft == 0)
            {
                AddToHighscores(playerName, attemptsLeft, totalPenaltyPoints);
                ShowEndGameMessage(false);
            }
        }

        private int CalculateScore(string[] userCode)
        {
            int score = 0;

            for (int i = 0; i < 4; i++)
            {
                if (userCode[i] == generatedCode[i])
                {
                    continue;
                }
                else if (generatedCode.Contains(userCode[i]))
                {
                    score += 1;
                }
                else
                {
                    score += 2;
                }
            }

            return score;
        }

        private void DisplayScore(int score)
        {
            ScoreLabel.Content = $"Score: {score} Strafpunten | Totale strafpunten: {totalPenaltyPoints}";
        }

        private string GenerateFeedback(string[] userCode)
        {
            int correctPosition = 0;
            int correctColorWrongPosition = 0;

            for (int i = 0; i < 4; i++)
            {
                if (userCode[i] == generatedCode[i])
                {
                    correctPosition++;
                }
                else if (generatedCode.Contains(userCode[i]))
                {
                    correctColorWrongPosition++;
                }
            }

            return $"Rood: {correctPosition}, Wit: {correctColorWrongPosition}";
        }

        private void LogAttempt(string[] userCode, string feedback)
        {
            string attempt = $"Poging: {string.Join(", ", userCode)} | Feedback: {feedback}";
            AttemptsListBox.Items.Add(attempt);
        }

        private void CheckColor(Label label, string selectedColor, int position)
        {
            if (selectedColor == generatedCode[position])
            {
                label.BorderBrush = new SolidColorBrush(Colors.DarkRed);
                label.BorderThickness = new Thickness(3);
            }
            else if (generatedCode.Contains(selectedColor))
            {
                label.BorderBrush = new SolidColorBrush(Colors.Wheat);
                label.BorderThickness = new Thickness(3);
            }
            else
            {
                label.BorderBrush = Brushes.Transparent;
                label.BorderThickness = new Thickness(0);
            }
        }

        private void ShowEndGameMessage(bool isWinner)
        {
            string message;
            if (isWinner)
            {
                message = $"Code is gekraakt in {10 - attemptsLeft} pogingen!\nNu is speler {playerNames[(currentPlayerIndex + 1) % playerNames.Count]} aan de beurt.";
            }
            else
            {
                message = $"You failed! De correcte code was {string.Join(" ", generatedCode)}.\nNu is speler {playerNames[(currentPlayerIndex + 1) % playerNames.Count]} aan de beurt.";
            }

            MessageBox.Show(message, isWinner ? "Code gekraakt!" : "Game Over", MessageBoxButton.OK);

            // Wissel naar de volgende speler
            currentPlayerIndex = (currentPlayerIndex + 1) % playerNames.Count;
            playerName = playerNames[currentPlayerIndex];  // Zet de naam van de volgende speler
            UpdateTitle();  // Werk de titel bij met de nieuwe speler
        }

        private void ResetGame()
        {
            GenerateRandomCode();
            attemptsLeft = 10;
            totalPenaltyPoints = 0;
            UpdateTitle();

            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;

            Label1.BorderBrush = Brushes.Transparent;
            Label2.BorderBrush = Brushes.Transparent;
            Label3.BorderBrush = Brushes.Transparent;
            Label4.BorderBrush = Brushes.Transparent;

            Label1.Background = Brushes.LightGray;
            Label2.Background = Brushes.LightGray;
            Label3.Background = Brushes.LightGray;
            Label4.Background = Brushes.LightGray;

            AttemptsListBox.Items.Clear();
            ScoreLabel.Content = "Score: 0 Strafpunten | Totale strafpunten: 0";
        }

        // Methode om de highscores toe te voegen
        private void AddToHighscores(string playerName, int attemptsLeft, int score)
        {
            string newHighscore = $"{playerName} - {10 - attemptsLeft} pogingen - {score / 100.0:F2}";

            // Voeg de nieuwe score toe aan de lijst
            bool added = false;
            for (int i = 0; i < highscores.Length; i++)
            {
                if (highscores[i] == null || GetScoreValue(highscores[i]) < score)
                {
                    // Verschuif de scores naar beneden en voeg de nieuwe score toe
                    for (int j = highscores.Length - 1; j > i; j--)
                    {
                        highscores[j] = highscores[j - 1];
                    }
                    highscores[i] = newHighscore;
                    added = true;
                    break;
                }
            }

            // Als de highscore is toegevoegd, sla het op in de array
            if (added)
            {
                MessageBox.Show("Je hebt een nieuwe highscore!");
            }
        }

        // Hulp functie om de score uit de string highscore te halen voor vergelijking
        private int GetScoreValue(string highscore)
        {
            var scoreString = highscore.Split('-').Last().Trim();
            var score = scoreString.Split(' ')[0]; // Haal het getal eruit
            return (int)(double.Parse(score) * 100); // Zet het terug naar een int
        }

        // Methode om de highscores weer te geven
        private void ShowHighscores()
        {
            string highscoresDisplay = "Highscores:\n";
            foreach (var score in highscores)
            {
                if (score != null)
                {
                    highscoresDisplay += score + "\n";
                }
            }
            MessageBox.Show(highscoresDisplay, "Highscores");
        }

        // Event handler voor 'Highscores' menu item
        private void MenuHighscores_Click(object sender, RoutedEventArgs e)
        {
            ShowHighscores(); // Weergeven van de highscores
        }

        // Event handler voor 'Afsluiten' menu item
        private void MenuAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();  // Laat de applicatie sluiten
        }

        // Event handler voor 'Aantal pogingen' menu item
        private void MenuAantalPogingen_Click(object sender, RoutedEventArgs e)
        {
            // Vraag het aantal pogingen in te stellen
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Voer het aantal pogingen in:",
                "Aantal pogingen instellen",
                attemptsLeft.ToString()
            );

            if (int.TryParse(input, out int newAttempts) && newAttempts > 0)
            {
                attemptsLeft = newAttempts;
                UpdateTitle();
                MessageBox.Show($"Aantal pogingen is ingesteld op {attemptsLeft}.", "Instellingen");
            }
            else
            {
                MessageBox.Show("Ongeldig aantal pogingen ingevoerd.", "Fout");
            }
        }

        private void MenuNieuwSpel_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();  // Reset het spel
            playerNames = StartGame();  // Vraag de naam van de speler opnieuw
            currentPlayerIndex = 0; // Begin met de eerste speler
            playerName = playerNames[currentPlayerIndex];
            MessageBox.Show("Een nieuw spel is begonnen!");  // Optioneel bericht
        }

    }
}