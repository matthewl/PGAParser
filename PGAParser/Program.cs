using System;
using System.Collections;
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
                TopPlayers();
            }
            else
            {
                switch (args[0])
                {
                    case "-g":
                        GeneratePlayerData();
                        break;
                    case "-t":
                        TopPlayers();
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

        public static void GeneratePlayerData()
        {
            string[] Players = new string[] { "Tiger Woods, USA", "Rory McIlroy, NI", "Ian Poulter, ENG" };

            using (StreamWriter file = new StreamWriter(@"players.csv"))
            {
                foreach (string player in Players)
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
            string[] lines = System.IO.File.ReadAllLines(@"players.csv");
            SortedList topPlayers = new SortedList();

            foreach (string line in lines)
            {
                PlayerCard playerCard = new PlayerCard(line);
                topPlayers.Add(playerCard.name, playerCard.score);
            }

            for (int i = 0; i < topPlayers.Capacity; i++)
            {
                Console.WriteLine("       {0,-6}: {1}", topPlayers.GetKey(i), topPlayers.GetByIndex(i));
            }
        }

        public static int[] AugustaScoreCard()
        {
            return new int[] { 4, 5, 4, 3, 4, 3, 4, 5, 4, 4, 4, 3, 5, 4, 5, 3, 4, 4 };
        }
    }

    class PlayerCard
    {
        public string name;
        public string country;
        public int score;

        public PlayerCard(string data)
        {
            string[] playerInfo = data.Split(",");
            name = playerInfo[0];
            country = playerInfo[1];
            score = TotalScore(playerInfo);
        }

        private int TotalScore(string[] playerInfo)
        {
            int totalScore = 0;

            for (int i = 2; i < playerInfo.Length; i += 1)
            {
                totalScore += Convert.ToInt32(playerInfo[i]);
            }

            return totalScore;
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