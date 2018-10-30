using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PGAParser
{
    class Program
    {
        public const int numOfPlayers = 10;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                WriteHelpLines();
            }
            else
            {
                switch (args[0])
                {
                    case "-g":
                        GenerateScoreData();
                        break;
                    case "-t":
                        TopPlayers();
                        break;
                    case "-f":
                        BestFrontNinePlayers();
                        break;
                    case "-r":
                        BestBackNinePlayers();
                        break;
                    case "-p":
                        BestParsOrBetterPlayers();
                        break;
                    case "-b":
                        BestBirdiesOrBetterPlayers();
                        break;
                }
            }
        }

        public static void WriteHelpLines()
        {
            System.Console.WriteLine("PGA Parser v1.0");
            System.Console.WriteLine("PGAParser.exe [options]");
            System.Console.WriteLine("");
            System.Console.WriteLine("Options:");
            System.Console.WriteLine("  -g   Generates a new set of score data");
            System.Console.WriteLine("  -t   Return the top 10 players");
            System.Console.WriteLine("  -f   Return the top 10 players on the front nine");
            System.Console.WriteLine("  -r   Return the top 10 players on the back nine");
            System.Console.WriteLine("  -p   Return the top 10 players with the most consecutive pars or better");
            System.Console.WriteLine("  -b   Return the top 10 players with the most consecutive birdies or better");
        }

        public static void GenerateScoreData()
        {
            string[] players = System.IO.File.ReadAllLines(@"players.csv");

            using (StreamWriter file = new StreamWriter(@"scores.csv"))
            {
                foreach (string player in players)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(player);
                    sb.Append(",");
                    sb.Append(Scorecard.Scores(PGAParser.CourseCard.AugustaScoreCard()));

                    file.WriteLine(sb.ToString());
                }
            }
        }

        public static List<PlayerCard> GetReturnedCards()
        {
            string[] lines = System.IO.File.ReadAllLines(@"scores.csv");
            List<PlayerCard> ReturnedCards = new List<PlayerCard>();

            foreach (string line in lines)
            {
                PlayerCard playerCard = new PlayerCard(line, PGAParser.CourseCard.AugustaScoreCard());
                ReturnedCards.Add(playerCard);
            }

            return ReturnedCards;
        }

        public static void TopPlayers()
        {
            List<PlayerCard> ReturnedCards = GetReturnedCards();
            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                return x.Score.CompareTo(y.Score);
            });

            int playerCount = 0;

            foreach (PlayerCard ReturnedCard in ReturnedCards)
            {
                playerCount++;
                if (playerCount <= numOfPlayers) {
                    Console.WriteLine("{0}, {1}", ReturnedCard.Name, ReturnedCard.Score);
                }
            }
        }

        public static void BestFrontNinePlayers()
        {
            List<PlayerCard> ReturnedCards = GetReturnedCards();
            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                return x.Front.CompareTo(y.Front);
            });

            int playerCount = 0;

            foreach (PlayerCard ReturnedCard in ReturnedCards)
            {
                playerCount++;
                if (playerCount <= numOfPlayers)
                {
                    Console.WriteLine("{0}, {1}", ReturnedCard.Name, ReturnedCard.Front);
                }
            }
        }

        public static void BestBackNinePlayers()
        {
            List<PlayerCard> ReturnedCards = GetReturnedCards();
            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                return x.Front.CompareTo(y.Rear);
            });

            int playerCount = 0;

            foreach (PlayerCard ReturnedCard in ReturnedCards)
            {
                playerCount++;
                if (playerCount <= numOfPlayers)
                {
                    Console.WriteLine("{0}, {1}", ReturnedCard.Name, ReturnedCard.Rear);
                }
            }
        }


        public static void BestParsOrBetterPlayers()
        {
            List<PlayerCard> ReturnedCards = GetReturnedCards();
            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                return y.ParsOrBetter.CompareTo(x.ParsOrBetter);
            });

            int playerCount = 0;

            foreach (PlayerCard ReturnedCard in ReturnedCards)
            {
                playerCount++;
                if (playerCount <= numOfPlayers)
                {
                    Console.WriteLine("{0}, {1}", ReturnedCard.Name, ReturnedCard.ParsOrBetter);
                }
            }
        }

        public static void BestBirdiesOrBetterPlayers()
        {
            List<PlayerCard> ReturnedCards = GetReturnedCards();
            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                return y.BirdiesOrBetter.CompareTo(x.BirdiesOrBetter);
            });

            int playerCount = 0;

            foreach (PlayerCard ReturnedCard in ReturnedCards)
            {
                playerCount++;
                if (playerCount <= numOfPlayers)
                {
                    Console.WriteLine("{0}, {1}", ReturnedCard.Name, ReturnedCard.BirdiesOrBetter);
                }
            }
        }
    }

    public static class CourseCard
    {
        public static int[] AugustaScoreCard()
        {
            return new int[] { 4, 5, 4, 3, 4, 3, 4, 5, 4, 4, 4, 3, 5, 4, 5, 3, 4, 4 };
        }
    }

    class PlayerCard : IComparable<PlayerCard>
    {
        public string Name;
        public string Country;
        public int Score;
        public int Front;
        public int Rear;
        public int ParsOrBetter;
        public int BirdiesOrBetter;

        public PlayerCard(string data, int[] CourseCard)
        {
            string[] PlayerInfo = data.Split(",");
            Name = PlayerInfo[0];
            Country = PlayerInfo[1];
            Score = TotalScore(PlayerInfo);
            Front = PartialScore(PlayerInfo, 2, 10);
            Rear = PartialScore(PlayerInfo, 11, 19);
            ParsOrBetter = CalculateParsOrBetter(PlayerInfo, CourseCard);
            BirdiesOrBetter = CalculateBirdiesOrBetter(PlayerInfo, CourseCard);
        }

        public int CompareTo(PlayerCard comparePlayerCard)
        {
            if (comparePlayerCard == null)
                return 1;
            else
                return this.Name.CompareTo(comparePlayerCard.Name);
        }

        private int TotalScore(string[] PlayerInfo)
        {
            int Score = 0;

            for (int i = 2; i < PlayerInfo.Length; i += 1)
            {
                Score += Convert.ToInt32(PlayerInfo[i]);
            }

            return Score;
        }

        private int PartialScore(string[] PlayerInfo, int Start, int Finish)
        {
            int Score = 0;

            for (int i = Start; i <= Finish; i += 1)
            {
                Score += Convert.ToInt32(PlayerInfo[i]);
            }

            return Score;
        }

        private int CalculateParsOrBetter(string[] PlayerInfo, int[] CourseCard)
        {
            int NumOfHoles = 0;
            int CurrentStreak = 0;

            for (int i = 0; i < 18; i += 1)
            {
                if (Convert.ToInt32(PlayerInfo[i + 2]) <= CourseCard[i]) {
                    CurrentStreak++;
                } else {
                    CurrentStreak = 0;
                }

                if (CurrentStreak > NumOfHoles) {
                    NumOfHoles = CurrentStreak;
                }
            }

            return NumOfHoles;
        }

        private int CalculateBirdiesOrBetter(string[] PlayerInfo, int[] CourseCard)
        {
            int NumOfHoles = 0;
            int CurrentStreak = 0;

            for (int i = 0; i < 18; i += 1)
            {
                if (Convert.ToInt32(PlayerInfo[i + 2]) < CourseCard[i])
                {
                    CurrentStreak++;
                }
                else
                {
                    CurrentStreak = 0;
                }

                if (CurrentStreak > NumOfHoles)
                {
                    NumOfHoles = CurrentStreak;
                }
            }

            return NumOfHoles;
        }

    }

    class Scorecard
    {
        public static string Scores(int[] scoreCard)
        {
            int[] scores = new int[18];
            Random rnd = new Random();

            for (int i = 0; i < scoreCard.Length; i += 1)
            {
                scores[i] = AdjustScore(scoreCard[i], rnd);
            }

            return string.Join(",", scores);
        }

        public static int AdjustScore(int score, Random randomizer)
        {
            int adjuster = randomizer.Next(1, 100);
            switch (adjuster)
            {
                case int n when (adjuster <= 5):
                    score = score - 2;
                    break;
                case int n when (adjuster > 5 && adjuster <= 20):
                    score = score - 1;
                    break;
                case int n when (adjuster > 80 && adjuster <= 95):
                    score = score + 1;
                    break;
                case int n when (adjuster > 95):
                    score = score + 2;
                    break;
            }
            return score;
        }
    }
}