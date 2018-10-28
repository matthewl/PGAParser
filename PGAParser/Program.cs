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
                // WriteHelpLines();
                BestBackNinePlayers();
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
            System.Console.WriteLine("  -p   Return the top 10 players with the most consecutive pars or better");
            System.Console.WriteLine("  -b   Return the top 10 players with the most consecutive birdies or better");
            System.Console.WriteLine("  -f   Return the top 10 players on the front nine");
            System.Console.WriteLine("  -r   Return the top 10 players on the back nine");
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
                    sb.Append(Scorecard.Scores(AugustaScoreCard()));

                    file.WriteLine(sb.ToString());
                }
            }
        }

        public static void TopPlayers()
        {
            string[] lines = System.IO.File.ReadAllLines(@"scores.csv");
            List<PlayerCard> ReturnedCards = new List<PlayerCard>();

            foreach (string line in lines)
            {
                PlayerCard playerCard = new PlayerCard(line);
                ReturnedCards.Add(playerCard);
            }

            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                if (x.Score == null && y.Score == null) return 0;
                else if (x.Score == null) return -1;
                else if (y.Score == null) return 1;
                else return x.Score.CompareTo(y.Score);
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
            string[] lines = System.IO.File.ReadAllLines(@"scores.csv");
            List<PlayerCard> ReturnedCards = new List<PlayerCard>();

            foreach (string line in lines)
            {
                PlayerCard playerCard = new PlayerCard(line);
                ReturnedCards.Add(playerCard);
            }

            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                if (x.Front == null && y.Front == null) return 0;
                else if (x.Front == null) return -1;
                else if (y.Front == null) return 1;
                else return x.Front.CompareTo(y.Front);
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
            string[] lines = System.IO.File.ReadAllLines(@"scores.csv");
            List<PlayerCard> ReturnedCards = new List<PlayerCard>();

            foreach (string line in lines)
            {
                PlayerCard playerCard = new PlayerCard(line);
                ReturnedCards.Add(playerCard);
            }

            ReturnedCards.Sort(delegate (PlayerCard x, PlayerCard y)
            {
                if (x.Rear == null && y.Rear == null) return 0;
                else if (x.Rear == null) return -1;
                else if (y.Rear == null) return 1;
                else return x.Front.CompareTo(y.Rear);
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

        public PlayerCard(string data)
        {
            string[] playerInfo = data.Split(",");
            Name = playerInfo[0];
            Country = playerInfo[1];
            Score = TotalScore(playerInfo);
            Front = PartialScore(playerInfo, 2, 10);
            Rear = PartialScore(playerInfo, 11, 19);
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