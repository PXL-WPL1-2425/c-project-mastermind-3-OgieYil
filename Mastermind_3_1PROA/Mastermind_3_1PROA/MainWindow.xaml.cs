using System;
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
        private List<string> playerNames = new List<string>();  // List to store multiple player names
        private int currentPlayerIndex = 0; // To track the current player
        private string playerName;  // Current player name
        private string[] highscores = new string[15]; // Array to store high scores

        public MainWindow()
        {
            InitializeComponent();
            playerName = StartGame();  // Get the first player's name
            GenerateRandomCode();
            OpvullenComboBoxes();
            UpdateTitle();
        }

        // Method to start the game and add players
        private string StartGame()
        {
            string name = string.Empty;
            // Add the first player
            while (true)
            {
                name = Microsoft.VisualBasic.Interaction.InputBox("Voer de naam van de speler in:", "Speler Naam", "");
                if (!string.IsNullOrEmpty(name))
                {
                    playerNames.Add(name);  // Add the player's name to the list
                }
                else
                {
                    MessageBox.Show("Naam mag niet leeg zijn.");
                    continue;
                }

                var result = MessageBox.Show("Wilt u nog een speler toevoegen?", "Nieuwe speler?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    break;  // Exit loop when no more players
                }
            }

            return playerNames[currentPlayerIndex];  // Return the first player
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
            this.Title = $"MasterMind - {playerName} - Pogingen over: {attemptsLeft}";  // Show the current player in the title
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
                ShowEndGameMessage(false);  // Game over
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
                AddToHighscores(playerName, attemptsLeft, totalPenaltyPoints); // Add score to the highscores
                ShowEndGameMessage(true);  // Game won
                return;
            }

            attemptsLeft--;
            UpdateTitle();

            if (attemptsLeft == 0)
            {
                AddToHighscores(playerName, attemptsLeft, totalPenaltyPoints); // Add score to the highscores
                ShowEndGameMessage(false);  // Game over
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
                // Message for winning
                message = $"Code is gekraakt in {10 - attemptsLeft} pogingen!\n" +
                          $"Nu is speler {playerNames[(currentPlayerIndex + 1) % playerNames.Count]} aan de beurt.";
            }
            else
            {
                // Message for losing
                message = $"You failed! De correcte code was {string.Join(" ", generatedCode)}.\n" +
                          $"Nu is speler {playerNames[(currentPlayerIndex + 1) % playerNames.Count]} aan de beurt.";
            }

            // Show the message in a MessageBox
            MessageBox.Show(message, isWinner ? "Code gekraakt!" : "Game Over", MessageBoxButton.OK);

            // Switch to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % playerNames.Count;
            playerName = playerNames[currentPlayerIndex];  // Set the next player's name
            ResetGame();  // Reset the game for the next player
            UpdateTitle();  // Update the title with the new player
        }

        private void ResetGame()
        {
            // Generate a new random code
            GenerateRandomCode();

            // Reset the attempts to the set value
            attemptsLeft = 10;  // Or use the dynamically set value
            totalPenaltyPoints = 0;
            UpdateTitle();  // Update the title with the new player

            // Reset the ComboBoxes to no selection
            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;

            // Reset the background color of the labels
            Label1.Background = Brushes.LightGray;
            Label2.Background = Brushes.LightGray;
            Label3.Background = Brushes.LightGray;
            Label4.Background = Brushes.LightGray;

            // Reset the border color of the labels
            Label1.BorderBrush = Brushes.Transparent;
            Label2.BorderBrush = Brushes.Transparent;
            Label3.BorderBrush = Brushes.Transparent;
            Label4.BorderBrush = Brushes.Transparent;

            // Clear the attempt list
            AttemptsListBox.Items.Clear();
            ScoreLabel.Content = "Score: 0 Strafpunten | Totale strafpunten: 0";
        }

        // Method to add highscores
        private void AddToHighscores(string playerName, int attemptsLeft, int score)
        {
            string newHighscore = $"{playerName} - {10 - attemptsLeft} pogingen - {score / 100.0:F2}";

            // Add the new score to the list
            bool added = false;
            for (int i = 0; i < highscores.Length; i++)
            {
                if (highscores[i] == null || GetScoreValue(highscores[i]) < score)
                {
                    // Shift the scores down and add the new score
                    for (int j = highscores.Length - 1; j > i; j--)
                    {
                        highscores[j] = highscores[j - 1];
                    }
                    highscores[i] = newHighscore;
                    added = true;
                    break;
                }
            }

            // If the highscore was added, show a message
            if (added)
            {
                MessageBox.Show("Je hebt een nieuwe highscore!");
            }
        }

        // Helper function to get the score value from the highscore string
        private int GetScoreValue(string highscore)
        {
            var scoreString = highscore.Split('-').Last().Trim();
            var score = scoreString.Split(' ')[0]; // Get the number from the score
            return (int)(double.Parse(score) * 100); // Convert back to an int
        }

        // Method to show the highscores
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

        // Event handler for 'Highscores' menu item
        private void MenuHighscores_Click(object sender, RoutedEventArgs e)
        {
            ShowHighscores(); // Show the highscores
        }

        // Event handler for 'Afsluiten' menu item
        private void MenuAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();  // Close the application
        }

        // Event handler for 'Nieuw Spel' menu item
        private void MenuNieuwSpel_Click(object sender, RoutedEventArgs e)
        {
            // Reset the game
            ResetGame();

            // Ask for the name of the new player
            playerName = StartGame();  // New player name is set
            MessageBox.Show($"Een nieuw spel is begonnen voor {playerName}!");  // Optional message

            // Ensure the new player's turn is set
            UpdateTitle();  // Ensure the title is correct
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

        private const int MaxPenaltyPoints = 100; // Limiet voor strafpunten
        private int penaltyPoints = 0; // Strafpunten variabele voor de huidige speler

        // Start een nieuwe spelerbeurt en reset de strafpunten
        private void StartNewPlayerTurn()
        {
            penaltyPoints = 0; // Reset de strafpunten naar 0
            MessageBox.Show("Nieuwe speler aan de beurt! Strafpunten zijn gereset.", "Nieuwe Beurt");
        }

        // Event om de beurt te wisselen naar de volgende speler
        private void NextPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewPlayerTurn(); // Reset de strafpunten voor de nieuwe speler
            ResetGameBoard();     // Optioneel: Reset het bord voor de nieuwe speler
        }

        // Hint Button Click Event
        private void HintButton_Click(object sender, RoutedEventArgs e)
        {
            // Controleer of de strafpuntenlimiet wordt overschreden vóór de hintoptie
            if (penaltyPoints >= MaxPenaltyPoints)
            {
                MessageBox.Show($"Je kunt geen hints meer kopen, want je hebt {MaxPenaltyPoints} of meer strafpunten bereikt!", "Limiet Bereikt");
                return; // Stop hier als limiet is bereikt
            }

            // Vraag de gebruiker welke hint ze willen
            MessageBoxResult result = MessageBox.Show(
                "Kies een hint:\n" +
                "1. Juiste kleur, verkeerde plaats (kost 15 strafpunten).\n" +
                "2. Juiste kleur en juiste plaats (kost 25 strafpunten).",
                "Hint Opties",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                if (penaltyPoints + 15 > MaxPenaltyPoints)
                {
                    MessageBox.Show("Je hebt niet genoeg strafpunten om deze hint te kopen!", "Limiet Bereikt");
                    return;
                }
                penaltyPoints += 15;
                ShowCorrectColorWrongPosition();
            }
            else if (result == MessageBoxResult.No)
            {
                if (penaltyPoints + 25 > MaxPenaltyPoints)
                {
                    MessageBox.Show("Je hebt niet genoeg strafpunten om deze hint te kopen!", "Limiet Bereikt");
                    return;
                }
                penaltyPoints += 25;
                ShowCorrectColorRightPosition();
            }

            // Toon bijgewerkte strafpunten aan de gebruiker
            MessageBox.Show($"Strafpunten: {penaltyPoints}", "Hint Resultaat");
        }

        // Hint: juiste kleur, verkeerde plaats
        private void ShowCorrectColorWrongPosition()
        {
            Random random = new Random();
            int correctIndex = random.Next(0, 4); // Kies een willekeurige index
            int wrongIndex;

            do
            {
                wrongIndex = random.Next(0, 4);
            } while (wrongIndex == correctIndex);

            string correctColor = generatedCode[correctIndex];
            UpdateComboBox(wrongIndex, correctColor);
        }

        // Hint: juiste kleur en juiste plaats
        private void ShowCorrectColorRightPosition()
        {
            Random random = new Random();
            int correctIndex;

            do
            {
                correctIndex = random.Next(0, 4);
            } while (GetComboBoxValue(correctIndex) == generatedCode[correctIndex]);

            string correctColor = generatedCode[correctIndex];
            UpdateComboBox(correctIndex, correctColor);
        }

        // Reset het bord voor de volgende speler (optioneel)
        private void ResetGameBoard()
        {
            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;
        }

        // Haal de waarde van een ComboBox
        private string GetComboBoxValue(int index)
        {
            switch (index)
            {
                case 0: return ComboBox1.SelectedItem as string ?? string.Empty;
                case 1: return ComboBox2.SelectedItem as string ?? string.Empty;
                case 2: return ComboBox3.SelectedItem as string ?? string.Empty;
                case 3: return ComboBox4.SelectedItem as string ?? string.Empty;
                default: return string.Empty;
            }
        }


        // Update de waarde van een ComboBox
        private void UpdateComboBox(int index, string value)
        {
            switch (index)
            {
                case 0: ComboBox1.SelectedItem = value; break;
                case 1: ComboBox2.SelectedItem = value; break;
                case 2: ComboBox3.SelectedItem = value; break;
                case 3: ComboBox4.SelectedItem = value; break;
            }
        }

    }
}
