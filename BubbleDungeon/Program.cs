namespace BubbleDungeon
{
    internal class Program
    {
        /// <summary>
        /// The current map in [column, row] format origin is at the top left of the map
        /// </summary>
        private static char[,] map;

        /// <summary>
        /// The blocks that the player can see in [column, row] format
        /// </summary>
        private static char[,] playerView = new char[3,4];

        /// <summary>
        /// The column of the map cell that the player is in
        /// </summary>
        private static int playerX;

        /// <summary>
        /// The row of the map cell that the player is in
        /// </summary>
        private static int playerY;

        /// <summary>
        /// Which direction the player is looking
        /// </summary>
        private static Heading playerHeading = Heading.North;

        /// <summary>
        /// How many bubbles have been found so far
        /// </summary>
        private static int bubblesFound = 0;

        /// <summary>
        /// How manytimes has the player succesfully moved
        /// </summary>
        private static int numberOfMoves = 0;


        /// <summary>
        /// How many bubbles there are total in the dungeon
        /// </summary>
        private static int numberOfBubbles = 0;

        private static void Main(string[] args)
        {
            InitializeConsoleWindow();
            PlayIntro();
            InitializeUI();
            LoadMap("./map.txt");
            UpdatePlayerView();
            StartGameLoop();
            PlayEnding();
        }

        private static void PlayEnding()
        {
            Console.Clear();
            Console.Write("You managed to collect all the Bubbles and seal the Bubble\r\nDungeon. Thanks to you, peace has been restored to the Bubble\r\nKingdom once again!\r\n\r\nThank you for Playing! Press any key to quit the game...");
            Console.ReadKey();
        }

        private static void PlayIntro()
        {
            Console.Write("It has been 1000 years since the Great Hero Bubbles of the\r\nBubble Kingdom ventured into the Bubble Dungeon, \r\ncollected all the Bubbles and sealed the dungeon.\r\nUnfortunately, the prophecy left behind by the Great Bubble\r\nSages say that the seal would only last for a millennium and\r\nyesterday the seal was broken and the Dungeon once again\r\nsucked all the bubbles into its depths. As the last\r\nremaining descendant of the Great Hero Bubbles, it now falls\r\non to you to venture into the Bubble Dungeon and collect all\r\nthe Bubbles.\r\n\r\nPress any key to start your Adventure!");
            Console.ReadKey();
            Console.Clear();
        }

        /// <summary>
        /// Check if the supplied position can be moved into
        /// </summary>
        /// <param name="x">X coordinate of the position we want to check</param>
        /// <param name="y">Y coordinate of the position we want to check</param>
        /// <returns>True if the position can be moved into</returns>
        private static bool CanMove(int x, int y)
        {
            return GetCharacter(x, y) != (char)CharacterType.Wall;
        }

        private static void DrawBubble()
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (playerView[1, 1] == (char)CharacterType.Bubble)
            {
                WriteAtPosition("____", 11, 11);
                WriteAtPosition("/    \\", 10, 12);
                WriteAtPosition("|      |", 9, 13);
                WriteAtPosition("\\____/", 10, 14);
            }
            else if (playerView[1, 2] == (char)CharacterType.Bubble && playerView[1, 1] != (char)CharacterType.Wall)
            {
                WriteAtPosition("()", 12, 12);
            }
            Console.ForegroundColor = originalColor;
        }

        private static bool CheckBubbleCollision()
        {
            if (map[playerX, playerY] == (char)CharacterType.Bubble)
            {
                bubblesFound++;
                map[playerX, playerY] = (char)CharacterType.Hallway;
                return true;
            }
            return false;
        }

        private static void StartGameLoop()
        {
            PrintMessage("Welcome to the Bubble Dungeon!");
            while (true)
            {
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        MoveForward();
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        MoveBackward();
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        TurnLeft();
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        TurnRight();
                        break;
                }
                if (bubblesFound == numberOfBubbles)
                {
                    break;
                }
            }
        }

        private static void MoveForward()
        {
            int x = playerX;
            int y = playerY;
            switch (playerHeading)
            {
                case Heading.North:
                    y -= 1;
                    break;
                case Heading.East:
                    x += 1;
                    break;
                case Heading.South:
                    y += 1;
                    break;
                case Heading.West:
                    x -= 1;
                    break;
            }
            if (CanMove(x, y))
            {
                playerX = x;
                playerY = y;
                numberOfMoves++;
                UpdateStepCounter();
                UpdatePlayerView();
                if (CheckBubbleCollision())
                {
                    UpdateBubbleCounter();
                    PrintMessage("You move forward and find a Bubble!");
                }
                else
                {
                    PrintMessage("You move forward.");
                }
            }
            else
            {
                PrintMessage("You try to move forward but hit a wall.");
                Console.Beep();
            }
        }

        private static void UpdateBubbleCounter()
        {
            WriteAtPosition(bubblesFound.ToString(), 29, 13);
        }

        private static void MoveBackward()
        {
            int x = playerX;
            int y = playerY;
            switch (playerHeading)
            {
                case Heading.North:
                    y += 1;
                    break;
                case Heading.East:
                    x -= 1;
                    break;
                case Heading.South:
                    y -= 1;
                    break;
                case Heading.West:
                    x += 1;
                    break;
            }
            if (CanMove(x, y))
            {
                playerX = x;
                playerY = y;
                numberOfMoves++;
                UpdateStepCounter();
                UpdatePlayerView();
                if (CheckBubbleCollision())
                {
                    UpdateBubbleCounter();
                    PrintMessage("You move back and find a Bubble!");
                }
                else
                {
                    PrintMessage("You move back.");
                }
            }
            else
            {
                PrintMessage("You try to move back but hit a wall.");
                Console.Beep();
            }
        }

        private static void TurnLeft()
        {
            string message = "You turn left. ";
            switch (playerHeading)
            {
                case Heading.North:
                    playerHeading = Heading.West;
                    message += "You are now facing West.";
                    break;
                case Heading.East:
                    playerHeading = Heading.North;
                    message += "You are now facing North.";
                    break;
                case Heading.South:
                    playerHeading= Heading.East;
                    message += "You are now facing East.";
                    break;
                case Heading.West:
                    playerHeading= Heading.South;
                    message += "You are now facing South.";
                    break;
            }
            PrintMessage(message);
            UpdatePlayerView();
            UpdateCompass();
        }

        private static void UpdateStepCounter()
        {
            WriteAtPosition(numberOfMoves.ToString(), 29, 9);
        }

        private static void TurnRight()
        {
            string message = "You turn right. ";
            switch (playerHeading)
            {
                case Heading.North:
                    playerHeading = Heading.East;
                    message += "You are now facing East.";
                    break;
                case Heading.East:
                    playerHeading = Heading.South;
                    message += "You are now facing South.";
                    break;
                case Heading.South:
                    playerHeading = Heading.West;
                    message += "You are now facing West.";
                    break;
                case Heading.West:
                    playerHeading = Heading.North;
                    message += "You are now facing North.";
                    break;
            }
            PrintMessage(message);
            UpdatePlayerView();
            UpdateCompass();
        }

        private static void LoadMap(string path)
        {
            // Load map data from the map file
            string[] lines = File.ReadAllLines(path);

            // Determine the bounds of the map
            int largestLine = 0;
            foreach (var line in lines)
            {
                if (line.Length > largestLine)
                {
                    largestLine = line.Length;
                }
            }

            // Intialize the map array and parse the map data to it
            map = new char[largestLine, lines.Length];
            for (int row = 0; row < lines.Length; row++)
            {
                string line = lines[row];
                for (int column = 0; column < line.Length; column++)
                {
                    char c = line[column];
                    if (c == (char)CharacterType.Player)
                    {
                        playerX = column;
                        playerY = row;
                        map[column, row] = (char)CharacterType.Hallway;
                    }
                    else if (c == ' ')
                    {
                        map[column, row] = (char)CharacterType.Wall;
                    }
                    else if (c == 'o')
                    {
                        numberOfBubbles++;
                        map[column, row] = c;
                    }
                    else
                    {
                        map[column, row] = c;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the First Person Viewport
        /// </summary>
        private static void UpdatePlayerView()
        {
            int x = playerX;
            int y = playerY;
            switch (playerHeading)
            {
                case Heading.North:
                    playerView[0, 0] = GetCharacter(x - 1, y);
                    playerView[1, 0] = GetCharacter(x, y);
                    playerView[2, 0] = GetCharacter(x + 1, y);
                    playerView[0, 1] = GetCharacter(x - 1, y - 1);
                    playerView[1, 1] = GetCharacter(x, y - 1);
                    playerView[2, 1] = GetCharacter(x + 1, y - 1);
                    playerView[0, 2] = GetCharacter(x - 1, y - 2);
                    playerView[1, 2] = GetCharacter(x, y - 2);
                    playerView[2, 2] = GetCharacter(x + 1, y - 2);
                    playerView[0, 3] = GetCharacter(x - 1, y - 3);
                    playerView[1, 3] = GetCharacter(x, y - 3);
                    playerView[2, 3] = GetCharacter(x + 1, y - 3);
                    break;
                case Heading.East:
                    playerView[0, 0] = GetCharacter(x, y - 1);
                    playerView[1, 0] = GetCharacter(x, y);
                    playerView[2, 0] = GetCharacter(x, y + 1);
                    playerView[0, 1] = GetCharacter(x + 1, y - 1);
                    playerView[1, 1] = GetCharacter(x + 1, y);
                    playerView[2, 1] = GetCharacter(x + 1, y + 1);
                    playerView[0, 2] = GetCharacter(x + 2, y - 1);
                    playerView[1, 2] = GetCharacter(x + 2, y);
                    playerView[2, 2] = GetCharacter(x + 2, y + 1);
                    playerView[0, 3] = GetCharacter(x + 3, y - 1);
                    playerView[1, 3] = GetCharacter(x + 3, y);
                    playerView[2, 3] = GetCharacter(x + 3, y + 1);
                    break;
                case Heading.South:
                    playerView[0, 0] = GetCharacter(x + 1, y);
                    playerView[1, 0] = GetCharacter(x, y);
                    playerView[2, 0] = GetCharacter(x - 1, y);
                    playerView[0, 1] = GetCharacter(x + 1, y + 1);
                    playerView[1, 1] = GetCharacter(x, y + 1);
                    playerView[2, 1] = GetCharacter(x - 1, y + 1);
                    playerView[0, 2] = GetCharacter(x + 1, y + 2);
                    playerView[1, 2] = GetCharacter(x, y + 2);
                    playerView[2, 2] = GetCharacter(x - 1, y + 2);
                    playerView[0, 3] = GetCharacter(x + 1, y + 3);
                    playerView[1, 3] = GetCharacter(x, y + 3);
                    playerView[2, 3] = GetCharacter(x - 1, y + 3);
                    break;
                case Heading.West:
                    playerView[0, 0] = GetCharacter(x, y + 1);
                    playerView[1, 0] = GetCharacter(x, y);
                    playerView[2, 0] = GetCharacter(x, y - 1);
                    playerView[0, 1] = GetCharacter(x - 1, y + 1);
                    playerView[1, 1] = GetCharacter(x - 1, y);
                    playerView[2, 1] = GetCharacter(x - 1, y - 1);
                    playerView[0, 2] = GetCharacter(x - 2, y + 1);
                    playerView[1, 2] = GetCharacter(x - 2, y);
                    playerView[2, 2] = GetCharacter(x - 2, y - 1);
                    playerView[0, 3] = GetCharacter(x - 3, y + 1);
                    playerView[1, 3] = GetCharacter(x - 3, y);
                    playerView[2, 3] = GetCharacter(x - 3, y - 1);
                    break;
            }
            ConsoleColor originalColor = Console.BackgroundColor;
            ClearPlayerView();
            DrawHorizontalLines();
            DrawVerticalLines();
            DrawAngularLines();
            DrawBubble();
            Console.BackgroundColor = originalColor;
        }

        /// <summary>
        /// Clears the First Person Viewport
        /// </summary>
        private static void ClearPlayerView()
        {
            for (int row = 1; row <= 22; row++)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                WriteAtPosition("                        ", 1, row);
                Thread.Sleep(5);
            }
        }

        private static void DrawVerticalLines()
        {
            // First Row
            if (playerView[0, 0] == (char)CharacterType.Wall || playerView[0, 1] == (char)CharacterType.Wall || playerView[1,1] == (char)CharacterType.Wall)
            {
                for (int row = 3; row <= 20; row++)
                {
                    WriteAtPosition('|', 2, row);
                }
            }
            if (playerView[2, 0] == (char)CharacterType.Wall || playerView[2, 1] == (char)CharacterType.Wall ||  playerView[1, 1] == (char)CharacterType.Wall)
            {
                for (int row = 3; row <= 20; row++)
                {
                    WriteAtPosition('|', 23, row);
                }
            }

            // Second Row
            if (playerView[1,1] != (char)CharacterType.Wall)
            {
                if (playerView[0, 1] == (char)CharacterType.Wall || playerView[0, 2] == (char)CharacterType.Wall || playerView[1, 2] == (char)CharacterType.Wall)
                {
                    for (int row = 7; row <= 16; row++)
                    {
                        WriteAtPosition('|', 6, row);
                    }
                }
                if (playerView[2, 1] == (char)CharacterType.Wall || playerView[2, 2] == (char)CharacterType.Wall || playerView[1, 2] == (char)CharacterType.Wall)
                {
                    for (int row = 7; row <= 16; row++)
                    {
                        WriteAtPosition('|', 19, row);
                    }
                }
            }

            // Third row
            if (playerView[1, 1] != (char)CharacterType.Wall && playerView[1, 2] != (char)CharacterType.Wall)
            {
                if (playerView[0, 2] == (char)CharacterType.Wall || playerView[0, 3] == (char)CharacterType.Wall || playerView[1, 3] == (char)CharacterType.Wall)
                {
                    for (int row = 10; row <= 13; row++)
                    {
                        WriteAtPosition('|', 9, row);
                    }
                }
                if (playerView[2, 2] == (char)CharacterType.Wall || playerView[2, 3] == (char)CharacterType.Wall || playerView[1, 3] == (char)CharacterType.Wall)
                {
                    for (int row = 10; row <= 13; row++)
                    {
                        WriteAtPosition('|', 16, row);
                    }
                }
            }
        }

        private static void DrawHorizontalLines()
        {
            // First Row Left
            if (playerView[0, 0] == (char)CharacterType.Wall || playerView[0, 1] != (char)CharacterType.Wall)
            {
                //WriteAtPosition("  ", 1, 2);
                //WriteAtPosition(' ', 1, 20);
            }
            else
            {
                WriteAtPosition("__", 1, 2);
                WriteAtPosition('_', 1, 20);
            }

            // First Row Middle
            if (playerView[1,1] == (char)CharacterType.Wall)
            {
                WriteAtPosition("____________________", 3, 2);
                WriteAtPosition("____________________", 3, 20);
            }

            // First Row Right
            if (playerView[2, 0] == (char)CharacterType.Wall || playerView[2, 1] != (char)CharacterType.Wall)
            {
                //WriteAtPosition("  ", 23, 2);
                //WriteAtPosition(' ', 24, 20);
            }
            else
            {
                WriteAtPosition("__", 23, 2);
                WriteAtPosition('_', 24, 20);
            }

            // Second Row
            if (playerView[1, 1] != (char)CharacterType.Wall)
            {
                // Left
                if (playerView[0, 1] == (char)CharacterType.Wall)
                {
                    //WriteAtPosition("        ", 1, 6);
                    //WriteAtPosition("        ", 1, 16);
                }
                else if (playerView[0, 2] == (char)CharacterType.Wall)
                {
                    if (playerView[0, 0] == (char)CharacterType.Wall)
                    {
                        WriteAtPosition("____", 3, 6);
                        WriteAtPosition("____", 3, 16);
                    }
                    else
                    {
                        WriteAtPosition("______", 1, 6);
                        WriteAtPosition("______", 1, 16);
                    }
                }

                // Middle
                if (playerView[1, 2] == (char)CharacterType.Wall)
                {
                    WriteAtPosition("____________", 7, 6);
                    WriteAtPosition("____________", 7, 16);
                }

                // Right
                if (playerView[2, 1] == (char)CharacterType.Wall)
                {
                    //WriteAtPosition("      ", 19, 6);
                    //WriteAtPosition("      ", 19, 16);
                }
                else if (playerView[2, 2] == (char)CharacterType.Wall)
                {
                    if (playerView[2, 0] == (char)CharacterType.Wall)
                    {
                        WriteAtPosition("____", 19, 6);
                        WriteAtPosition("____", 19, 16);
                    }
                    else
                    {
                        WriteAtPosition("______", 19, 6);
                        WriteAtPosition("______", 19, 16);
                    }
                }
            }

            // Third row
            if (playerView[1, 1] != (char)CharacterType.Wall && playerView[1, 2] != (char)CharacterType.Wall)
            {
                // Left
                if (playerView[0, 3] == (char)CharacterType.Wall && playerView[0, 2] != (char)CharacterType.Wall)
                {
                    if (playerView[0, 1] == (char)CharacterType.Wall)
                    {
                        WriteAtPosition("__", 7, 9);
                        WriteAtPosition("__", 7, 13);
                    }
                    else if (playerView[0, 0] == (char)CharacterType.Wall)
                    {
                        WriteAtPosition("_______", 3, 9);
                        WriteAtPosition("_______", 3, 13);
                    }
                    else
                    {
                        WriteAtPosition("_________", 1, 9);
                        WriteAtPosition("_________", 1, 13);
                    }
                }

                // Middle
                if (playerView[1, 3] == (char)CharacterType.Wall)
                {
                    WriteAtPosition("______", 10, 9);
                    WriteAtPosition("______", 10, 13);
                }

                // Right
                if (playerView[2, 3] == (char)CharacterType.Wall && playerView[2, 2] != (char)CharacterType.Wall)
                {
                    if (playerView[2, 1] == (char)CharacterType.Wall)
                    {
                        WriteAtPosition("__", 17, 9);
                        WriteAtPosition("__", 17, 13);
                    }
                    else if (playerView[2, 0] == (char)CharacterType.Wall)
                    {
                        WriteAtPosition("_______", 16, 9);
                        WriteAtPosition("_______", 16, 13);
                    }
                    else
                    {
                        WriteAtPosition("_________", 16, 9);
                        WriteAtPosition("_________", 16, 13);
                    }
                }
            }
        }

        private static void DrawAngularLines()
        {
            // First Row Left
            if (playerView[0,0]== (char)CharacterType.Wall)
            {
                WriteAtPosition('\\', 1, 1);
                WriteAtPosition('\\', 2, 2);
                WriteAtPosition('/', 1, 22);
                WriteAtPosition('/', 2, 21);
            }

            // First Row Right
            if (playerView[2, 0] == (char)CharacterType.Wall)
            {
                WriteAtPosition('/', 24, 1);
                WriteAtPosition('/', 23, 2);
                WriteAtPosition('\\', 24, 22);
                WriteAtPosition('\\', 23, 21);
            }

            // Second Row
            if (playerView[1, 1] != (char)CharacterType.Wall)
            {
                // Left
                if (playerView[0, 1] == (char)CharacterType.Wall)
                {
                    WriteAtPosition('\\', 3, 3);
                    WriteAtPosition('\\', 4, 4);
                    WriteAtPosition('\\', 5, 5);
                    WriteAtPosition('\\', 6, 6);
                    WriteAtPosition('/', 6, 17);
                    WriteAtPosition('/', 5, 18);
                    WriteAtPosition('/', 4, 19);
                    WriteAtPosition('/', 3, 20);
                }

                // Right
                if (playerView[2, 1] == (char)CharacterType.Wall)
                {
                    WriteAtPosition('/', 22, 3);
                    WriteAtPosition('/', 21, 4);
                    WriteAtPosition('/', 20, 5);
                    WriteAtPosition('/', 19, 6);
                    WriteAtPosition('\\', 19, 17);
                    WriteAtPosition('\\', 20, 18);
                    WriteAtPosition('\\', 21, 19);
                    WriteAtPosition('\\', 22, 20);
                }
            }

            // Third row
            if (playerView[1, 1] != (char)CharacterType.Wall && playerView[1, 2] != (char)CharacterType.Wall)
            {
                // Left
                if (playerView[0, 2] == (char)CharacterType.Wall)
                {
                    WriteAtPosition('\\', 7, 7);
                    WriteAtPosition('\\', 8, 8);
                    WriteAtPosition('\\', 9, 9);
                    WriteAtPosition('/', 9, 14);
                    WriteAtPosition('/', 8, 15);
                    WriteAtPosition('/', 7, 16);
                }

                // Right
                if (playerView[2, 2] == (char)CharacterType.Wall)
                {
                    WriteAtPosition('/', 18, 7);
                    WriteAtPosition('/', 17, 8);
                    WriteAtPosition('/', 16, 9);
                    WriteAtPosition('\\', 16, 14);
                    WriteAtPosition('\\', 17, 15);
                    WriteAtPosition('\\', 18, 16);
                }
            }
        }

        private static void PrintMessage(string message)
        {
            Console.SetCursorPosition(3, 26);
            if (message.Length < 54)
            {
                int difference = 54 - message.Length;
                message += new string(' ', difference);
            }
            Console.Write(message);
        }

        private static char GetCharacter(int x, int y)
        {
            if (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1))
            {
                return map[x, y];
            }
            else
            {
                return (char)CharacterType.Wall;
            }
        }

        private static void WriteAtPosition(string text, int column, int row)
        {
            Console.SetCursorPosition(column, row);
            Console.Write(text);
        }

        private static void WriteAtPosition(char c, int column, int row)
        {
            Console.SetCursorPosition(column, row);
            Console.Write(c);
        }

        private static void InitializeConsoleWindow()
        {
            Console.SetWindowSize(60, 30);
            Console.Title = "Bubble Dungeon";
            Console.CursorVisible = false;
        }

        private static void InitializeUI()
        {

            Console.Write("############################################################\r\n#                        #                                 #\r\n#                        #                 N               #\r\n#                        #                 ^               #\r\n#                        #              W< o >E            #\r\n#                        #                 v               #\r\n#                        #                 S               #\r\n#                        #                                 #\r\n#                        #   Steps taken:                  #\r\n#                        #   0                             #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #   Bubbles Found:                #\r\n#                        #   0                             #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #                                 #\r\n#                        #                                 #\r\n############################################################\r\n#                                                          #\r\n#                                                          #\r\n#                                                          #\r\n#                                                          #\r\n#                                                          #\r\n############################################################");
        }


        private static void UpdateCompass()
        {
            switch (playerHeading)
            {
                case Heading.North:
                    WriteAtPosition('N', 43, 2);
                    WriteAtPosition('E', 46, 4);
                    WriteAtPosition('S', 43, 6);
                    WriteAtPosition('W', 40, 4);
                    break;
                case Heading.East:
                    WriteAtPosition('E', 43, 2);
                    WriteAtPosition('S', 46, 4);
                    WriteAtPosition('W', 43, 6);
                    WriteAtPosition('N', 40, 4);
                    break;
                case Heading.South:
                    WriteAtPosition('S', 43, 2);
                    WriteAtPosition('W', 46, 4);
                    WriteAtPosition('N', 43, 6);
                    WriteAtPosition('E', 40, 4);
                    break;
                case Heading.West:
                    WriteAtPosition('W', 43, 2);
                    WriteAtPosition('N', 46, 4);
                    WriteAtPosition('E', 43, 6);
                    WriteAtPosition('S', 40, 4);
                    break;
            }
        }

        private enum CharacterType
        {
            Player = '@',
            Bubble = 'o',
            Hallway = '#',
            Wall = '\0'
        }

        private enum Heading
        {
            North, East, South, West
        }
    }
}
