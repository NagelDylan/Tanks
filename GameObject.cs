// Author: Dylan Nagel
// File Name: GameObject.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Runs the flow of the program

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GameUtility;
using System.IO;
using System;

namespace NagelD_PASS3
{
    public class GameObject
    {
        //stores the maximum number of bullets on teh screen at once
        private const int MAX_SCREEN_BUL = 5;

        //stores the length of a wall
        public static int WALL_LENGTH = 50;

        //stores if a value was not found
        public static int NOT_FOUND = -1;

        // stores the costs for pathfinding movement
        public static float HV_COST = 10f;
        public static float DIAG_COST = 14f;

        //stores the number of rows and columns
        public static int NUM_ROWS = 9;
        public static int NUM_COL = 14;

        //stores the number of sorted data
        private const int NUM_SORTED_DATA = 3;

        //stores index location of scores
        private const int SORT_TIME = 0;
        private const int SORT_ACC = 1;
        private const int SORT_AV_SHOT = 2;

        //stores the colour for level info option based on hovor status
        private readonly Color[] LEV_INFO_OPTION_COL = new Color[] { Color.Khaki, Color.Goldenrod };

        //stores the level stats location info
        private const int NUM_LEV_STATS = 6;
        private const int LEVEL_STAT_X_LOC = 15;
        private const int LEVEL_STAT_Y_LOC = 200;
        private const int LEVEL_STAT_Y_SPACING = 123;
        private const int LEVEL_STAT_X_SPACING = 350;

        //stores the statistics index location
        private const int ATTEMPTS = 0;
        private const int LEVEL_NUM = 0;
        private const int BULLETS_SHOT = 1;
        private const int BULL_HIT = 2;
        private const int TIME = 3;
        private const int ACCURACY = 4;
        private const int AV_SHOTS_PER_SEC = 5;

        //stores the index location for next level
        const int NEXT_LEVEL = 4;

        //stores the file error screen location
        private const int ERROR_MESSAGE_START_Y = 260;
        private const int ERROR_MES_SPACE = 50;
        private readonly int[] FILE_ERROR_NEXT_STATE = new int[] { Game1.EXIT, Game1.MENU };

        //stores the location of endgame stars 
        private const int END_STAR_LENGTH = 100;
        private const int END_STAR_SPACING = 135;
        private const int END_STAR_START_X_POS = 188;
        private const int END_STAR_START_Y_POS = 160;
        private const int END_TIMER_LENGTH = 2000;
        private const int END_BUT_Y_SPACE = 110;

        //stores the level info information
        private const int LEVEL_INFO_BUT_X = 50;
        private const int LEVEL_INFO_BUT_SPACE = 350;
        private const int LEVEL_INFO_BUT_WIDTH = 300;
        private const int LEVEL_INFO_BUT_HEIGHT = 80;

        //stores the index of next gamestate in level info
        private const int LEV_INFO_BACK = 0;
        private const int LEV_INFO_PLAY = 1;

        //stores the index for each button in endgame screen
        private const int END_PLAY_STATS = 1;
        private const int END_TRY_AGAIN = 2;
        private const int END_NEXT_LEVEL = 4;

        //stores the index for each button in play stat screen
        private const int PLAY_STAT_BACK = 0;
        private const int PLAY_STAT_MENU = 1;

        //stores the minimum number of nodes for each tank type
        public const int PURP_TANK_MIN_NODE = 2;
        public const int RED_TANK_MIN_NODE = 1;
        public const int BLUE_TANK_MIN_NODE = 1;

        //stores the constant text
        private const string PLAY_STATS_TITLE = "Play Stats";
        private readonly string[] ENDGAME_MESSAGES = new string[] { " Failed!", " Cleared!" };
        private readonly string[] LEVEL_INFO_BUT_TEXT = new string[] { "Back", "Play" };
        private readonly string[] LEVEL_STAT_PREFIXES = new string[] { "Attempts:", "Bullets Shot:", "Bullets Hit:", "Av Time (s):", "Av Accuracy (%):", "Av Shots Per Sec:" };
        private readonly string MED_PREFIX = "Median:";
        private readonly string[] PLAY_STAT_PREFIXES = new string[] { "Level #:", "Bullets Shot:", "Bullets Hit:", "Time (s):", "Accuracy (%):", "Av Shots Per Sec:" };
        private readonly string[] PLAY_STAT_BUTTON_TEXTS = new string[] { "Back", "Menu" };
        private readonly string[] FILE_ERROR_BUT_TEXTS = new string[] { " Exit", "Menu" };
        private readonly string ERROR_TITLE = "File Error";
        private readonly string[] END_BUT_PROMPTS = new string[] { "Menu", "Play Stats", "Try again", "Level Info", "Next Level" };

        //stores the endgame next button states
        private readonly int[] END_BUT_NEXT_STATES = new int[] { Game1.MENU, Game1.PLAY_STATS, Game1.GAMEPLAY, Game1.LEVEL_INFO, Game1.GAMEPLAY };

        //stores the arena images
        private Texture2D arenaImg;
        private Texture2D overhangImg;

        //stores the terrain images
        private Texture2D wallImg;
        private Texture2D waterImg;

        //stores the button image
        private Texture2D buttonImg;

        //stores the fonts
        private SpriteFont menuFont;
        private SpriteFont titleFont;
        private SpriteFont levelStatFont;
        private SpriteFont errorScreenFont;

        //stores the tile map
        Node[,] tileMap = new Node[NUM_COL, NUM_ROWS];

        //stores the grid info
        private Rectangle[,] gridRecs;
        private int[,,] gridInfo;

        //stores the level number
        private int levelNum = 0;

        //stores the bullets
        private List<Bullet> playBullets = new List<Bullet>();
        private List<Bullet> enBullets = new List<Bullet>();

        //stores the player
        private Player player = new Player();

        //stores the spawn location for the player
        private Vector2[] playerSpawnGrid = new Vector2[Game1.NUM_LEV];

        //stores the rectangels for the arena
        private Rectangle arenaRec;
        private Rectangle overhandRec;

        //stores the enemy tanks
        private List<Tank> enemyTanks = new List<Tank>();

        //stores the game timer
        private Timer gameTimer = new Timer(Timer.INFINITE_TIMER, false);

        //stores the sorted score values
        private SortedScore[,] scores = new SortedScore[3, Game1.NUM_LEV];

        //stores if the player won
        private bool didWin;

        //stores the level info information
        private Vector2 levelInfoTitleShadPos;
        private Vector2 levelInfoTitlePos;
        private Rectangle[] levelInfoButtonRecs = new Rectangle[2];
        private Vector2[] levelInfoButtonTextPos = new Vector2[2];
        private Vector2[] levelInfoButtonShadTextPos = new Vector2[2];
        private bool[] isLevelInfoButHover = new bool[2];

        //stores the level stats information
        private float[,] levelStats = new float[NUM_LEV_STATS, Game1.NUM_LEV];
        private Vector2[] levelStatsLoc = new Vector2[NUM_LEV_STATS];
        private Vector2[] levelStatsShadLoc = new Vector2[NUM_LEV_STATS];
        private float[] medLevelStats = new float[3];
        private Vector2[] levelStatMedianLocs = new Vector2[3];
        private Vector2[] levelStatMedianShadLocs = new Vector2[3];

        //stores the play stats information
        private float[] playStats = new float[NUM_LEV_STATS];
        private Vector2 playStatsTitleShadPos;
        private Vector2 playStatsTitlePos;
        private Vector2[] playStatsButtonTextPos = new Vector2[2];
        private Vector2[] playStatsButtonShadTextPos = new Vector2[2];

        //stores the error screen information
        private Vector2 errorTitlePos;
        private Vector2 errorTitleShadPos;
        private string[] errorMessage = new string[3];
        private Vector2[] errorMessageloc = new Vector2[3];
        private Vector2[] errorMessageShadloc = new Vector2[3];

        //stores the endgame information
        private Vector2 endgameMesLoc;
        private Vector2 endgameMesShadLoc;
        private Rectangle[] endStarRecs = new Rectangle[3];
        private Timer endgameTimer = new Timer(END_TIMER_LENGTH, false);
        private Rectangle[] starRecs = new Rectangle[Game1.NUM_LEV * Game1.NUM_STAR_PER_LEV];
        private Texture2D[] starImgs;
        private Rectangle[] endgameButRecs = new Rectangle[5];
        private Vector2[] endButPromptLocs = new Vector2[5];
        private Vector2[] endButPromptShadLocs = new Vector2[5];
        private bool[] isEndButHover = new bool[5];
        private SpriteFont endTitleFont;

        //stores the if star is achieved information
        private bool[] isStarAcheived = new bool[Game1.NUM_LEV * Game1.NUM_STAR_PER_LEV];
        private bool[] isEndStarAcheived = new bool[Game1.NUM_STAR_PER_LEV];

        //stores prev gamestate variable
        private int prevGameState = Game1.LEVEL_SELECT;

        //stores file reading information
        private StreamWriter outFile;
        private StreamReader inFile;
        private string[] line;

        //stores the explosion images
        private Texture2D tankExploadImg;
        private Texture2D bulletExploadImg;

        //stores the animations
        private List<Animation> tankExplodAnims = new List<Animation>();
        private List<Animation> bulletExplodAnims = new List<Animation>();

        //stores if the level is unlocked
        private bool[] isLevelUnlocked = new bool[Game1.NUM_LEV];

        //stores the sound effects
        SoundEffect bulletExploadSnd;
        SoundEffect buttonClickSnd;
        SoundEffect gunShotSnd;
        SoundEffect tankExploadSnd;
        SoundEffect noAmmoSnd;

        //pre: a valid texture2d arena image, a valid texture2d overhang arena image, a valid texture2d wall image, a valid texture2d button image, a valid spritefont menu font, a valid spritefont title font, a valid spritefont level stat font, a valid texture2d[] star img, a valid recangle[] star rectangles, a valid spritefont end title font, a valid spritefont error screen font, a valid texture2d tank explosion image, a valid texture2d bullet explosio nimage, a valid soundeffect bullet explosion sound, a valid sound effect button click sound, a valid sound effect gun shot sound, a valid sound effect tank explosion sound, a valid sound effect for when you have no ammo left
        //post: none
        //description: creates a ninstance of the gameObject object
        public GameObject(Texture2D arenaImg, Texture2D overhangImg, Texture2D wallImg, Texture2D waterImg, Texture2D buttonImg, SpriteFont menuFont, SpriteFont titleFont, SpriteFont levelStatFont, Texture2D[] starImgs, Rectangle[] starRecs, SpriteFont endTitleFont, SpriteFont errorScreenFont, Texture2D tankExploadImg, Texture2D bulletExploadImg, SoundEffect bulletExploadSnd, SoundEffect buttonClickSnd, SoundEffect gunShotSnd, SoundEffect tankExploadSnd, SoundEffect noAmmoSnd)
        {

            //sets sound variables
            this.bulletExploadSnd = bulletExploadSnd;
            this.buttonClickSnd = buttonClickSnd;
            this.gunShotSnd = gunShotSnd;
            this.tankExploadSnd = tankExploadSnd;
            this.noAmmoSnd = noAmmoSnd;

            //sets arena image variabels
            this.arenaImg = arenaImg;
            this.overhangImg = overhangImg;

            //sets terain images
            this.wallImg = wallImg;
            this.waterImg = waterImg;

            //sets button image
            this.buttonImg = buttonImg;

            //sets fonts
            this.menuFont = menuFont;
            this.levelStatFont = levelStatFont;
            this.titleFont = titleFont;
            this.endTitleFont = endTitleFont;
            this.errorScreenFont = errorScreenFont;

            //sets star images and rectangles
            this.starImgs = starImgs;
            this.starRecs = starRecs;

            //sets explosion images
            this.tankExploadImg = tankExploadImg;
            this.bulletExploadImg = bulletExploadImg;

            //loops through the level info button rectangles 
            for (int i = 0; i < levelInfoButtonRecs.Length; i++)
            {
                //sets up the level info button at index i
                levelInfoButtonRecs[i] = new Rectangle(LEVEL_INFO_BUT_X + LEVEL_INFO_BUT_SPACE * i, Game1.screenHeight - LEVEL_INFO_BUT_HEIGHT - 25, LEVEL_INFO_BUT_WIDTH, LEVEL_INFO_BUT_HEIGHT);
                levelInfoButtonTextPos[i] = new Vector2((int)(levelInfoButtonRecs[i].X + 0.5 * (levelInfoButtonRecs[i].Width - menuFont.MeasureString(LEVEL_INFO_BUT_TEXT[i]).X)), (int)(levelInfoButtonRecs[i].Y + 0.5 * (levelInfoButtonRecs[i].Height - menuFont.MeasureString(LEVEL_INFO_BUT_TEXT[i]).Y)));
                levelInfoButtonShadTextPos[i] = new Vector2(levelInfoButtonTextPos[i].X + 3, levelInfoButtonTextPos[i].Y + 3);

                //sets up the play stats button prompt at index i
                playStatsButtonTextPos[i] = new Vector2((int)(levelInfoButtonRecs[i].X + 0.5 * (levelInfoButtonRecs[i].Width - menuFont.MeasureString(PLAY_STAT_BUTTON_TEXTS[i]).X)), (int)(levelInfoButtonRecs[i].Y + 0.5 * (levelInfoButtonRecs[i].Height - menuFont.MeasureString(PLAY_STAT_BUTTON_TEXTS[i]).Y)));
                playStatsButtonShadTextPos[i] = new Vector2(playStatsButtonTextPos[i].X + 3, playStatsButtonTextPos[i].Y + 3);
            }

            // loops through the scores x length
            for(int i = 0; i < scores.GetLength(0); i++)
            {
                //loops through the score y length
                for (int x = 0; x < scores.GetLength(1); x++)
                {
                    //sets an instance of the sorted score object
                    scores[i, x] = new SortedScore();
                }
            }

            //sets the endgame button rectangles
            endgameButRecs[1] = new Rectangle(levelInfoButtonRecs[0].X, levelInfoButtonRecs[0].Y - END_BUT_Y_SPACE, levelInfoButtonRecs[0].Width, levelInfoButtonRecs[0].Height);
            endgameButRecs[2] = new Rectangle(levelInfoButtonRecs[1].X, levelInfoButtonRecs[1].Y - END_BUT_Y_SPACE, levelInfoButtonRecs[1].Width, levelInfoButtonRecs[1].Height);
            endgameButRecs[3] = levelInfoButtonRecs[0];
            endgameButRecs[4] = levelInfoButtonRecs[1];
            endgameButRecs[0] = new Rectangle((int)(0.5 * (Game1.screenWidth - endgameButRecs[1].Width)), endgameButRecs[1].Y - END_BUT_Y_SPACE, endgameButRecs[1].Width, endgameButRecs[1].Height);

            //loops through the endgame button rectangles
            for(int i = 0; i < endgameButRecs.Length; i++)
            {
                //sets up the endgame button prompt locations
                endButPromptLocs[i] = new Vector2(endgameButRecs[i].X + 0.5f * (endgameButRecs[i].Width - menuFont.MeasureString(END_BUT_PROMPTS[i]).X), endgameButRecs[i].Y + 0.5f * (endgameButRecs[i].Height - menuFont.MeasureString(END_BUT_PROMPTS[i]).Y));
                endButPromptShadLocs[i] = new Vector2(endButPromptLocs[i].X + 3, endButPromptLocs[i].Y + 3);
            }

            //loops through half the level stats locations
            for (int i = 0; i < levelStatsLoc.Length * 0.5; i ++)
            {
                //sets up the level stats information locations at index i 
                levelStatsLoc[i] = new Vector2(LEVEL_STAT_X_LOC, LEVEL_STAT_Y_LOC + LEVEL_STAT_Y_SPACING * i);
                levelStatsShadLoc[i] = new Vector2(levelStatsLoc[i].X + 1, levelStatsLoc[i].Y + 1);

                //sets up the level stats information locations at index i + half quantity
                levelStatsLoc[i + (int)(levelStatsLoc.Length * 0.5)] = new Vector2(LEVEL_STAT_X_LOC + LEVEL_STAT_X_SPACING, LEVEL_STAT_Y_LOC + LEVEL_STAT_Y_SPACING * i);
                levelStatsShadLoc[i + (int)(levelStatsLoc.Length * 0.5)] = new Vector2(levelStatsLoc[i + (int)(levelStatsLoc.Length * 0.5)].X + 1, levelStatsLoc[i + (int)(levelStatsLoc.Length * 0.5)].Y + 1);

                //sets up the level stat median locs
                levelStatMedianLocs[i] = new Vector2(levelStatsLoc[i + (int)(levelStatsLoc.Length * 0.5)].X, levelStatsLoc[i + (int)(levelStatsLoc.Length * 0.5)].Y + 45);
                levelStatMedianShadLocs[i] = new Vector2(levelStatMedianLocs[i].X + 1, levelStatMedianLocs[i].Y + 1);
            }

            //sets up the level info title position
            levelInfoTitlePos = new Vector2((int)(0.5 * (Game1.screenWidth - titleFont.MeasureString("Level " + (levelNum + 1) + " Info").X)), 40);
            levelInfoTitleShadPos = new Vector2(levelInfoTitlePos.X + 5, levelInfoTitlePos.Y + 5);

            //sets up teh play stats title positoin
            playStatsTitlePos = new Vector2((int)(0.5 * (Game1.screenWidth - titleFont.MeasureString(PLAY_STATS_TITLE).X)), 40);
            playStatsTitleShadPos = new Vector2(playStatsTitlePos.X + 5, playStatsTitlePos.Y + 5);

            //sets up the error screen title position
            errorTitlePos = new Vector2((int)(0.5 * (Game1.screenWidth - titleFont.MeasureString(ERROR_TITLE).X)), 40);
            errorTitleShadPos = new Vector2(errorTitlePos.X + 5, errorTitlePos.Y + 5);

            //sets up the grid rectangle size
            gridRecs = new Rectangle[(Game1.RIGHT_PLAY_X - Game1.LEFT_PLAY_X) / WALL_LENGTH, (Game1.BOT_PLAY_Y - Game1.TOP_PLAY_Y) / WALL_LENGTH];

            //loops through the grid rectangles x length
            for (int i = 0; i < gridRecs.GetLength(0); i++)
            {
                //loops through the grid rectangles y length
                for (int x = 0; x < gridRecs.GetLength(1); x++)
                {
                    //sets up the grid rectangles at index i, x
                    gridRecs[i, x] = new Rectangle(Game1.LEFT_PLAY_X + WALL_LENGTH * i, Game1.TOP_PLAY_Y + WALL_LENGTH * x, WALL_LENGTH, WALL_LENGTH);
                }
            }

            //loops through the endgame star rectangles
            for (int i = 0; i < endStarRecs.Length; i++)
            {
                //sets up the endgame star rectangle at index i
                endStarRecs[i] = new Rectangle(END_STAR_START_X_POS + END_STAR_SPACING * i, END_STAR_START_Y_POS, END_STAR_LENGTH, END_STAR_LENGTH);
            }

            //sets up teh endgame message locations
            endgameMesLoc = new Vector2(0, 15);
            endgameMesShadLoc = new Vector2(0, endgameMesLoc.Y + 3);

            //sets up the grid info 2d array
            gridInfo = new int[NUM_COL, NUM_ROWS, Game1.NUM_LEV];

            //sets up the grid for level 1
            gridInfo[3, 3, 0] = Game1.WALL;
            gridInfo[3, 5, 0] = Game1.WALL;
            gridInfo[7, 1, 0] = Game1.WALL;
            gridInfo[7, 2, 0] = Game1.WALL;
            gridInfo[7, 3, 0] = Game1.WALL;
            gridInfo[7, 4, 0] = Game1.WALL;
            gridInfo[7, 5, 0] = Game1.WALL;
            gridInfo[7, 6, 0] = Game1.WALL;
            gridInfo[7, 7, 0] = Game1.WALL;
            gridInfo[11, 4, 0] = Game1.YELLOW_TANK;
            playerSpawnGrid[0] = new Vector2(1, 4);

            //sets up the grid for level 2
            gridInfo[2, 2, 1] = Game1.WALL;
            gridInfo[3, 2, 1] = Game1.WALL;
            gridInfo[4, 2, 1] = Game1.WALL;
            gridInfo[5, 2, 1] = Game1.WALL;
            gridInfo[6, 2, 1] = Game1.WALL;
            gridInfo[7, 2, 1] = Game1.WALL;
            gridInfo[8, 2, 1] = Game1.WALL;
            gridInfo[9, 2, 1] = Game1.WALL;
            gridInfo[4, 6, 1] = Game1.WALL;
            gridInfo[5, 6, 1] = Game1.WALL;
            gridInfo[6, 6, 1] = Game1.WALL;
            gridInfo[7, 6, 1] = Game1.WALL;
            gridInfo[8, 6, 1] = Game1.WALL;
            gridInfo[9, 6, 1] = Game1.WALL;
            gridInfo[10, 6, 1] = Game1.WALL;
            gridInfo[11, 6, 1] = Game1.WALL;
            gridInfo[12, 1, 1] = Game1.PURPLE_TANK;
            playerSpawnGrid[1] = new Vector2(1, 7);

            //sets up the grid for level 3
            gridInfo[2, 2, 2] = Game1.WALL;
            gridInfo[3, 2, 2] = Game1.WALL;
            gridInfo[4, 2, 2] = Game1.WALL;
            gridInfo[5, 2, 2] = Game1.WALL;
            gridInfo[6, 2, 2] = Game1.WALL;
            gridInfo[7, 2, 2] = Game1.WALL;
            gridInfo[6, 3, 2] = Game1.WALL;
            gridInfo[7, 3, 2] = Game1.WALL;
            gridInfo[6, 4, 2] = Game1.WALL;
            gridInfo[7, 4, 2] = Game1.WALL;
            gridInfo[6, 5, 2] = Game1.WALL;
            gridInfo[7, 5, 2] = Game1.WALL;
            gridInfo[6, 6, 2] = Game1.WALL;
            gridInfo[7, 6, 2] = Game1.WALL;
            gridInfo[8, 6, 2] = Game1.WALL;
            gridInfo[9, 6, 2] = Game1.WALL;
            gridInfo[10, 6, 2] = Game1.WALL;
            gridInfo[11, 6, 2] = Game1.WALL;
            gridInfo[3, 1, 2] = Game1.PURPLE_TANK;
            gridInfo[10, 7, 2] = Game1.BLUE_TANK;
            gridInfo[12, 1, 2] = Game1.YELLOW_TANK;
            playerSpawnGrid[2] = new Vector2(4, 4);

            //sets up the grid for for level 4
            gridInfo[4, 0, 3] = Game1.WATER;
            gridInfo[4, 1, 3] = Game1.WATER;
            gridInfo[4, 2, 3] = Game1.WATER;
            gridInfo[4, 3, 3] = Game1.WATER;
            gridInfo[4, 4, 3] = Game1.WATER;
            gridInfo[0, 6, 3] = Game1.WATER;
            gridInfo[1, 6, 3] = Game1.WATER;
            gridInfo[2, 6, 3] = Game1.WATER;
            gridInfo[3, 6, 3] = Game1.WATER;
            gridInfo[4, 6, 3] = Game1.WATER;
            gridInfo[5, 6, 3] = Game1.WATER;
            gridInfo[6, 6, 3] = Game1.WATER;
            gridInfo[9, 0, 3] = Game1.WATER;
            gridInfo[9, 1, 3] = Game1.WATER;
            gridInfo[7, 3, 3] = Game1.WATER;
            gridInfo[8, 3, 3] = Game1.WATER;
            gridInfo[9, 3, 3] = Game1.WATER;
            gridInfo[10, 3, 3] = Game1.WATER;
            gridInfo[11, 3, 3] = Game1.WATER;
            gridInfo[12, 3, 3] = Game1.WATER;
            gridInfo[13, 3, 3] = Game1.WATER;
            gridInfo[9, 5, 3] = Game1.WALL;
            gridInfo[9, 6, 3] = Game1.WALL;
            gridInfo[9, 7, 3] = Game1.WALL;
            gridInfo[9, 8, 3] = Game1.WALL;
            gridInfo[6, 1, 3] = Game1.YELLOW_TANK;
            gridInfo[13, 0, 3] = Game1.PURPLE_TANK;
            gridInfo[11, 6, 3] = Game1.BLUE_TANK;
            playerSpawnGrid[3] = new Vector2(0, 8);

            //sets up the grid for level 5
            gridInfo[1, 4, 4] = Game1.WALL;
            gridInfo[2, 4, 4] = Game1.WALL;
            gridInfo[3, 4, 4] = Game1.WALL;
            gridInfo[4, 4, 4] = Game1.WALL;
            gridInfo[4, 5, 4] = Game1.WALL;
            gridInfo[4, 6, 4] = Game1.WALL;
            gridInfo[4, 7, 4] = Game1.WALL;
            gridInfo[9, 1, 4] = Game1.WALL;
            gridInfo[9, 2, 4] = Game1.WALL;
            gridInfo[9, 3, 4] = Game1.WALL;
            gridInfo[9, 4, 4] = Game1.WALL;
            gridInfo[10, 4, 4] = Game1.WALL;
            gridInfo[11, 4, 4] = Game1.WALL;
            gridInfo[12, 4, 4] = Game1.WALL;
            gridInfo[10, 3, 4] = Game1.RED_TANK;
            gridInfo[3, 5, 4] = Game1.RED_TANK;
            playerSpawnGrid[4] = new Vector2(7, 4);

            //sets up the grid for level 6
            gridInfo[0, 2, 5] = Game1.WALL;
            gridInfo[1, 2, 5] = Game1.WALL;
            gridInfo[2, 2, 5] = Game1.WALL;
            gridInfo[2, 3, 5] = Game1.WALL;
            gridInfo[2, 4, 5] = Game1.WALL;
            gridInfo[2, 5, 5] = Game1.WALL;
            gridInfo[2, 6, 5] = Game1.WALL;
            gridInfo[5, 1, 5] = Game1.WALL;
            gridInfo[5, 2, 5] = Game1.WALL;
            gridInfo[5, 3, 5] = Game1.WALL;
            gridInfo[5, 4, 5] = Game1.WALL;
            gridInfo[5, 5, 5] = Game1.WALL;
            gridInfo[5, 6, 5] = Game1.WALL;
            gridInfo[6, 6, 5] = Game1.WALL;
            gridInfo[7, 6, 5] = Game1.WALL;
            gridInfo[8, 6, 5] = Game1.WALL;
            gridInfo[8, 7, 5] = Game1.WALL;
            gridInfo[8, 1, 5] = Game1.WALL;
            gridInfo[8, 2, 5] = Game1.WALL;
            gridInfo[8, 3, 5] = Game1.WALL;
            gridInfo[9, 3, 5] = Game1.WALL;
            gridInfo[10, 3, 5] = Game1.WALL;
            gridInfo[11, 3, 5] = Game1.WALL;
            gridInfo[11, 4, 5] = Game1.WALL;
            gridInfo[11, 5, 5] = Game1.WALL;
            gridInfo[11, 6, 5] = Game1.WALL;
            gridInfo[11, 7, 5] = Game1.WALL;
            gridInfo[3, 1, 5] = Game1.PURPLE_TANK;
            gridInfo[9, 1, 5] = Game1.RED_TANK;
            gridInfo[5, 7, 5] = Game1.YELLOW_TANK;
            gridInfo[5, 8, 5] = Game1.YELLOW_TANK;
            gridInfo[13, 8, 5] = Game1.BLUE_TANK;
            playerSpawnGrid[5] = new Vector2(1, 3);

            //sets up the grid for level 7
            gridInfo[2, 1, 6] = Game1.WALL;
            gridInfo[2, 2, 6] = Game1.WALL;
            gridInfo[2, 3, 6] = Game1.WALL;
            gridInfo[2, 4, 6] = Game1.WALL;
            gridInfo[2, 5, 6] = Game1.WALL;
            gridInfo[2, 6, 6] = Game1.WALL;
            gridInfo[2, 7, 6] = Game1.WALL;
            gridInfo[11, 2, 6] = Game1.WALL;
            gridInfo[11, 3, 6] = Game1.WALL;
            gridInfo[11, 4, 6] = Game1.WALL;
            gridInfo[11, 5, 6] = Game1.WALL;
            gridInfo[11, 6, 6] = Game1.WALL;
            gridInfo[7, 3, 6] = Game1.WATER;
            gridInfo[7, 4, 6] = Game1.WATER;
            gridInfo[7, 5, 6] = Game1.WATER;
            gridInfo[7, 4, 6] = Game1.WATER;
            gridInfo[7, 5, 6] = Game1.WATER;
            gridInfo[8, 3, 6] = Game1.WATER;
            gridInfo[8, 4, 6] = Game1.WATER;
            gridInfo[8, 5, 6] = Game1.WATER;
            gridInfo[4, 1, 6] = Game1.YELLOW_TANK;
            gridInfo[4, 7, 6] = Game1.YELLOW_TANK;
            gridInfo[1, 1, 6] = Game1.PURPLE_TANK;
            gridInfo[13, 2, 6] = Game1.BLUE_TANK;
            gridInfo[13, 6, 6] = Game1.BLUE_TANK;
            gridInfo[1, 4, 6] = Game1.RED_TANK;
            gridInfo[1, 7, 6] = Game1.PURPLE_TANK;
            playerSpawnGrid[6] = new Vector2(10, 3);

            //sets up the grid for level 8
            gridInfo[1, 1, 7] = Game1.RED_TANK;
            gridInfo[12, 1, 7] = Game1.BLUE_TANK;
            gridInfo[6, 1, 7] = Game1.YELLOW_TANK;
            gridInfo[7, 1, 7] = Game1.YELLOW_TANK;
            gridInfo[1, 7, 7] = Game1.BLUE_TANK;
            gridInfo[12, 7, 7] = Game1.RED_TANK;
            gridInfo[6, 7, 7] = Game1.YELLOW_TANK;
            gridInfo[7, 7, 7] = Game1.YELLOW_TANK;
            playerSpawnGrid[7] = new Vector2(6, 4);

            //loops through the grid info x length
            for (int i = 0; i < gridInfo.GetLength(0); i++)
            {
                //loops through the grid info y length
                for (int x = 0; x < gridInfo.GetLength(1); x++)
                {
                    //sets up the tile map at location i, x
                    tileMap[i, x] = new Node(i, x, gridInfo[i, x, levelNum]);
                }
            }

            //loops through the grid for x length
            for(int i = 0; i < gridInfo.GetLength(0); i++)
            {
                //loops through the grid info y length
                for(int x = 0; x < gridInfo.GetLength(1); x++)
                {
                    //sets up the adjacencies at tile map location x,i
                    tileMap[i, x].SetAdjacencies(tileMap);
                }
            }

            //sets up the arena rectangle
            arenaRec = new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight);
            overhandRec = new Rectangle((int)(0.5 * (Game1.screenWidth - overhangImg.Width * 0.68)), 0, (int)(overhangImg.Width * 0.68), (int)(overhangImg.Height * 0.61));

            //sets is level unlocked at 0 to true
            isLevelUnlocked[0] = true;
        }

        //pre: a valid gametime
        //post: a boolean representing if the game is over or not
        //description: updates the gameplay
        public bool UpdateGameplay(GameTime gameTime)
        {
            //updates the gametimer
            gameTimer.Update(gameTime.ElapsedGameTime.Milliseconds);

            //moves the player
            player.Move();

            //loops through the enemy tanks
            for(int i = 0; i < enemyTanks.Count; i++)
            {
                //move the enemy tanks
                enemyTanks[i].Move(player.GetPos());

                //ccheck for enemy tank world collision
                EnWorldCol(enemyTanks[i]);

                //add enemy bullet based on attack update
                AddEnBulletCheck(enemyTanks[i].Attack(gameTime, player.GetPos()));
            }

            //check for player world collision
            PlayerWorldCol();

            //add player bullet based on attack update
            AddPlayBulletCheck(player.Attack());

            //update bullets
            UpdateBullets(playBullets);
            UpdateBullets(enBullets);

            //check if the enemy bullet collides with player
            if (!EnBullPlayerCol())
            {
                //set did win to false
                didWin = false;

                //set endgame star achievments to false
                isEndStarAcheived[Game1.COMPLETION] = false;
                isEndStarAcheived[Game1.ACCURACY] = false;
                isEndStarAcheived[Game1.TIME] = false;

                //set up endgame
                SetUpEndgame();

                //seve date to file
                SaveDataToFile();

                //return false
                return false;
            }

            //check for player bullet and enemy collision
            PlayBullEnCol();

            //set remove index to player bullet enemy bullet collision
            Vector2 removeInd = PlayBullEnBullCol();

            //check if remove index at x is not equal to 1
            if(removeInd.X != -1)
            {
                //add new explosion animations
                bulletExplodAnims.Add(new Animation(bulletExploadImg, 3, 3, 8, 0, 0, Animation.ANIMATE_ONCE, 600, enBullets[(int)removeInd.X].GetExploadLoc(), true));
                bulletExplodAnims.Add(new Animation(bulletExploadImg, 3, 3, 8, 0, 0, Animation.ANIMATE_ONCE, 600, playBullets[(int)removeInd.Y].GetExploadLoc(), true));

                //create bullet explosion sound effect
                bulletExploadSnd.CreateInstance().Play();
                bulletExploadSnd.CreateInstance().Play();

                //remove bullets
                enBullets.RemoveAt((int)removeInd.X);
                playBullets.RemoveAt((int)removeInd.Y);
            }

            //update the animations
            UpdateAnimation(tankExplodAnims, gameTime);
            UpdateAnimation(bulletExplodAnims, gameTime);

            //check if there are not enemy tanks remaining
            if (enemyTanks.Count == 0)
            {
                //set did win variable to true
                didWin = true;

                //set up endgame screen
                SetUpEndgame();

                //set is star acheived at completion to true
                isEndStarAcheived[Game1.COMPLETION] = true;
                isStarAcheived[levelNum * Game1.NUM_STAR_PER_LEV + Game1.COMPLETION] = true;

                //check if accuracy is greater than minimum accuracy for star
                if (playStats[ACCURACY] * 0.01 >= Game1.MIN_ACC)
                {
                    //set is star acheived at accuracy to true
                    isEndStarAcheived[Game1.ACCURACY] = true;
                    isStarAcheived[levelNum * Game1.NUM_STAR_PER_LEV + Game1.ACCURACY] = true;
                }

                //check if time passed is less than the max amount of time for star
                if (gameTimer.GetTimePassed() < Game1.MAX_TIMER_STAR)
                {
                    //set is star acheived at completion to true
                    isEndStarAcheived[Game1.TIME] = true;
                    isStarAcheived[levelNum * Game1.NUM_STAR_PER_LEV + Game1.TIME] = true;
                }

                //check if the levelnum does not equal to the last level
                if(levelNum != Game1.NUM_LEV - 1)
                {
                    //set next level to unlocked
                    isLevelUnlocked[levelNum + 1] = true;
                }

                //save data to file
                SaveDataToFile();

                //return false
                return false;
            }

            //return true
            return true;
        }

        //pre: a valid gametime
        //post: none
        //description: updates the endgame screen
        public int UpdateEndgame(GameTime gameTime)
        {
            //updates the endgame timer
            endgameTimer.Update(gameTime.ElapsedGameTime.Milliseconds);

            //update animations
            UpdateAnimation(bulletExplodAnims, gameTime);
            UpdateAnimation(tankExplodAnims, gameTime);

            //check if the endgame timer is finished
            if (endgameTimer.IsFinished())
            {
                //loop through the engame button rectangles
                for (int i = 0; i < endgameButRecs.Length; i++)
                {
                    //check if the user is hovering the endgame button at index i
                    if (endgameButRecs[i].Contains(Game1.mouse.Position))
                    {
                        //check if 
                        if (i != END_NEXT_LEVEL)
                        {
                            //set is hover variable at index i to true
                            isEndButHover[i] = true;

                            //check if the user clicked the mouse button
                            if (Game1.mouse.LeftButton == ButtonState.Pressed && Game1.prevMouse.LeftButton != ButtonState.Pressed)
                            {
                                //create a click sound effect
                                buttonClickSnd.CreateInstance().Play();

                                //change info based on button
                                switch (i)
                                {
                                    case END_PLAY_STATS:
                                        //set level numebr to level number plus 1
                                        playStats[LEVEL_NUM] = levelNum + 1;
                                        break;

                                    case END_TRY_AGAIN:
                                        //set up level
                                        SetUpLevel(levelNum);
                                        break;
                                }

                                //loop through the is end button hover variabels
                                for (int z = 0; z < isEndButHover.Length; z++)
                                {
                                    //set is end button hover at index z to false
                                    isEndButHover[z] = false;
                                }

                                //set prev gamestate to endgame
                                prevGameState = Game1.ENDGAME;

                                //return the end button next gamestate at index i
                                return END_BUT_NEXT_STATES[i];
                            }
                        }
                        else
                        {
                            //check if the level number is less than max level number adn the next level is unlocked
                            if (levelNum < Game1.NUM_LEV - 1 && isLevelUnlocked[levelNum + 1])
                            {
                                //set is button hvoer at index to true
                                isEndButHover[END_NEXT_LEVEL] = true;

                                //check if user clicked the left button
                                if (Game1.mouse.LeftButton == ButtonState.Pressed && Game1.prevMouse.LeftButton != ButtonState.Pressed)
                                {
                                    //create a button click sound
                                    buttonClickSnd.CreateInstance().Play();

                                    //increase the level number by 1
                                    levelNum++;

                                    //set up the level
                                    SetUpLevel(levelNum);

                                    ///loop through the is end button hover variabels
                                    for (int z = 0; z < isEndButHover.Length; z++)
                                    {
                                        //set is end button hover at index z to false
                                        isEndButHover[z] = false;
                                    }

                                    //set prev gamestate to endgame
                                    prevGameState = Game1.ENDGAME;

                                    //return the end button next gamestate at index
                                    return END_BUT_NEXT_STATES[END_NEXT_LEVEL];
                                }
                            }
                        }
                    }
                    else
                    {
                        //set is endbutton hover at index i to false
                        isEndButHover[i] = false;
                    }
                }
            }

            //return endgame
            return Game1.ENDGAME;
        }

        //pre: none
        //post: a interger representing the next gamestate
        //description: update the play stats
        public int UpdatePlayStats()
        {
            //loop through the level info button rectangles 
            for (int i = 0; i < levelInfoButtonRecs.Length; i++)
            {
                //check if the player is hovering the button recatngle at index i
                if (levelInfoButtonRecs[i].Contains(Game1.mouse.Position))
                {
                    //set the is hovering variable at index i to true
                    isLevelInfoButHover[i] = true;

                    //check if the user clicked the left click button
                    if (Game1.mouse.LeftButton == ButtonState.Pressed && Game1.prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //create a button click sound effect
                        buttonClickSnd.CreateInstance().Play();

                        //change gamestate based on index value
                        switch (i)
                        {
                            case PLAY_STAT_BACK:
                                //set temporary variabel to previous gamestate
                                int temp = prevGameState;

                                //set previous gamesatte to elvel selction
                                prevGameState = Game1.LEVEL_SELECT;

                                //return the temporary variable
                                return temp;

                            case PLAY_STAT_MENU:
                                //return the menu gamestate
                                return Game1.MENU;
                        }
                    }
                }
                else
                {
                    //set the is hover variable at index i to false
                    isLevelInfoButHover[i] = false;
                }
            }

            //return the play stats gamestate
            return Game1.PLAY_STATS;
        }

        //pre: none
        //post: an interger representing the next gamestate
        //description: update the error screen
        public int UpdateErrorScreen()
        {
            //loop through the level info button rectangles
            for (int i = 0; i < levelInfoButtonRecs.Length; i++)
            {
                //check if the bottom button is being hovered
                if (levelInfoButtonRecs[i].Contains(Game1.mouse.Position))
                {
                    //set the is hover variabel at bottom button to true
                    isLevelInfoButHover[i] = true;

                    //check if the user clicked the left button
                    if (Game1.mouse.LeftButton == ButtonState.Pressed)
                    {
                        buttonClickSnd.CreateInstance().Play();

                        //set gamesatte to menu
                        return FILE_ERROR_NEXT_STATE[i];
                    }
                }
                else
                {
                    //set the hover variable to false
                    isLevelInfoButHover[i] = false;
                }
            }

            //return the file error gamestate
            return Game1.FILE_ERROR;
        }

        //pre: none
        //post: none
        //description: draw gameplay
        public void DrawGameplay()
        {
            //draw the arena
            Game1.spriteBatch.Draw(arenaImg, arenaRec, Color.White);

            //draw teh level
            DrawLevel();

            //loop through the enemy tanks
            for(int i = 0; i < enemyTanks.Count; i++)
            {
                //draw the enemy tank at index i
                enemyTanks[i].Draw();
            }

            //draw the player
            player.Draw();

            //draw the overhang image
            Game1.spriteBatch.Draw(overhangImg, overhandRec, Color.White);

            //draw the bullets
            DrawBullets(playBullets);
            DrawBullets(enBullets);

            //loop through the tank explosions
            for(int i = 0; i < tankExplodAnims.Count; i++)
            {
                //draw the tank explosion at index i
                tankExplodAnims[i].Draw(Game1.spriteBatch, Color.White, Animation.FLIP_NONE);
            }

            //loop through the bullet explisions
            for (int i = 0; i < bulletExplodAnims.Count; i++)
            {
                //draw the bullet explosion at index i
                bulletExplodAnims[i].Draw(Game1.spriteBatch, Color.White, Animation.FLIP_NONE);
            }
        }

        //pre: none
        //post: none
        //description: draw the level information
        public void DrawLevelInfo()
        {
            //draw the level info title
            Game1.spriteBatch.DrawString(titleFont, "Level " + (levelNum + 1) + " Info", levelInfoTitleShadPos, Color.Black * 0.8f);
            Game1.spriteBatch.DrawString(titleFont, "Level " + (levelNum + 1) + " Info ", levelInfoTitlePos, Color.Gray);

            //loop through the level stats x length
            for (int i = 0; i < levelStats.GetLength(0); i++)
            {
                //draw the level stats
                Game1.spriteBatch.DrawString(levelStatFont, LEVEL_STAT_PREFIXES[i] + " " + levelStats[i, levelNum], levelStatsShadLoc[i], Color.Black * 0.8f);
                Game1.spriteBatch.DrawString(levelStatFont, LEVEL_STAT_PREFIXES[i] + " " + levelStats[i, levelNum], levelStatsLoc[i], Color.Brown);
            }

            //loop through the level info button rectangles
            for (int i = 0; i < levelInfoButtonRecs.Length; i++)
            {
                //draw the buttons
                Game1.spriteBatch.Draw(buttonImg, levelInfoButtonRecs[i], Color.White);
                Game1.spriteBatch.DrawString(menuFont, LEVEL_INFO_BUT_TEXT[i], levelInfoButtonShadTextPos[i], Color.Black * 0.5f);
                Game1.spriteBatch.DrawString(menuFont, LEVEL_INFO_BUT_TEXT[i], levelInfoButtonTextPos[i], LEV_INFO_OPTION_COL[Convert.ToInt32(isLevelInfoButHover[i])]);
            }

            //loop through teh median level stats
            for (int i = 0; i < medLevelStats.Length; i++)
            {
                //draw the median level stats
                Game1.spriteBatch.DrawString(levelStatFont, MED_PREFIX + " " + medLevelStats[i], levelStatMedianShadLocs[i], Color.Black * 0.8f);
                Game1.spriteBatch.DrawString(levelStatFont, MED_PREFIX + " " + medLevelStats[i], levelStatMedianLocs[i], Color.Gray);
            }
        }

        //pre: none
        //post: none
        //description: draws the engame screen
        public void DrawEndgame()
        {
            //draws engame screen title
            Game1.spriteBatch.DrawString(endTitleFont, "Mission " + (levelNum + 1) + ENDGAME_MESSAGES[Convert.ToInt32(didWin)], endgameMesShadLoc, Color.Black * 0.8f);
            Game1.spriteBatch.DrawString(endTitleFont, "Mission " + (levelNum + 1) + ENDGAME_MESSAGES[Convert.ToInt32(didWin)], endgameMesLoc, Color.Khaki);

            //sets float transparancy variable
            float trans = (float)endgameTimer.GetTimePassed() / END_TIMER_LENGTH;

            //loops through teh endgame star rectangles
            for (int i = 0; i < endStarRecs.Length; i++)
            {
                //draws the engame star recdtangle at index i
                Game1.spriteBatch.Draw(starImgs[Convert.ToInt32(isEndStarAcheived[i])], endStarRecs[i], Color.White * trans);
            }

            //loop through the endgame button rectangles excpet for the last on
            for (int i = 0; i < endgameButRecs.Length - 1; i++)
            {
                //draw button
                Game1.spriteBatch.Draw(buttonImg, endgameButRecs[i], Color.White * trans);
                Game1.spriteBatch.DrawString(menuFont, END_BUT_PROMPTS[i], endButPromptShadLocs[i], Color.Black * 0.8f * trans);
                Game1.spriteBatch.DrawString(menuFont, END_BUT_PROMPTS[i], endButPromptLocs[i], LEV_INFO_OPTION_COL[Convert.ToInt32(isEndButHover[i])] * trans);
            }

            //check if the level number is less than the last level and the next level is unlcoked
            if (levelNum < Game1.NUM_LEV - 1)
            {
                if(isLevelUnlocked[levelNum + 1])
                {
                    //draw the final button normally
                    Game1.spriteBatch.Draw(buttonImg, endgameButRecs[NEXT_LEVEL], Color.White * trans);
                    Game1.spriteBatch.DrawString(menuFont, END_BUT_PROMPTS[NEXT_LEVEL], endButPromptShadLocs[NEXT_LEVEL], Color.Black * 0.8f * trans);
                    Game1.spriteBatch.DrawString(menuFont, END_BUT_PROMPTS[NEXT_LEVEL], endButPromptLocs[NEXT_LEVEL], LEV_INFO_OPTION_COL[Convert.ToInt32(isEndButHover[NEXT_LEVEL])] * trans);
                }
                else
                {
                    //draw the final button, except darkened
                    Game1.spriteBatch.Draw(buttonImg, endgameButRecs[NEXT_LEVEL], Color.White * 0.4f * trans);
                    Game1.spriteBatch.DrawString(menuFont, END_BUT_PROMPTS[NEXT_LEVEL], endButPromptShadLocs[NEXT_LEVEL], Color.Black * 0.8f * 0.4f * trans);
                    Game1.spriteBatch.DrawString(menuFont, END_BUT_PROMPTS[NEXT_LEVEL], endButPromptLocs[NEXT_LEVEL], LEV_INFO_OPTION_COL[Convert.ToInt32(isEndButHover[NEXT_LEVEL])] * 0.4f * trans);
                }
            }
        }

        //pre: none
        //post: none
        //description: draw the play stats
        public void DrawPlayStats()
        {
            //draw the play stats title
            Game1.spriteBatch.DrawString(titleFont, PLAY_STATS_TITLE, playStatsTitleShadPos, Color.Black * 0.8f);
            Game1.spriteBatch.DrawString(titleFont, PLAY_STATS_TITLE, playStatsTitlePos, Color.Gray);

            //loop through the play stats
            for (int i = 0; i < playStats.Length; i++)
            {
                //draw the play stats at index i
                Game1.spriteBatch.DrawString(levelStatFont, PLAY_STAT_PREFIXES[i] + " " + playStats[i], levelStatsShadLoc[i], Color.Black * 0.8f);
                Game1.spriteBatch.DrawString(levelStatFont, PLAY_STAT_PREFIXES[i] + " " + playStats[i], levelStatsLoc[i], Color.Brown);
            }

            //loop through the level information button rectangles
            for (int i = 0; i < levelInfoButtonRecs.Length; i++)
            {
                //draw the button at index i
                Game1.spriteBatch.Draw(buttonImg, levelInfoButtonRecs[i], Color.White);
                Game1.spriteBatch.DrawString(menuFont, PLAY_STAT_BUTTON_TEXTS[i], playStatsButtonShadTextPos[i], Color.Black * 0.5f);
                Game1.spriteBatch.DrawString(menuFont, PLAY_STAT_BUTTON_TEXTS[i], playStatsButtonTextPos[i], LEV_INFO_OPTION_COL[Convert.ToInt32(isLevelInfoButHover[i])]);
            }
        }

        //pre: none
        //post: none
        //description: checks for player world collision
        private void PlayerWorldCol()
        {
            //stores the offset rectangle
            Rectangle offsetRec = player.GetOffsetRec();

            //sets the y and x node centre positions
            int centreY = (offsetRec.Y - Game1.TOP_PLAY_Y) / WALL_LENGTH;
            int centreX = (offsetRec.X - Game1.LEFT_PLAY_X) / WALL_LENGTH;

            //create variable to store the land type
            int landType;

            //loop through the three rows around centre node
            for (int i = centreY - 1; i <= centreY + 1; i++)
            {
                //loop through the three columns around centre node 
                for (int x = centreX - 1; x <= centreX + 1; x++)
                {
                    //set landtype
                    landType = GetLandType(x, i);

                    //check if land type is wall or water
                    if (landType == Game1.WALL || landType == Game1.WATER)
                    {
                        //push player based on rotated collision with wall/water
                        player.PushPlayer(Util.RotatedCollisionPush(offsetRec, gridRecs[x,i], player.GetTankAngle(), 0f));
                    }
                }
            }

            //loop through the enemy tanks
            for(int i = 0; i < enemyTanks.Count; i++)
            {
                //push the player based on the rotated collision with enemy tanks
                player.PushPlayer(Util.RotatedCollisionPush(offsetRec, enemyTanks[i].GetOffsetRec(), player.GetTankAngle(), enemyTanks[i].GetTankAngle()));
            }
        }

        //pre: a valid tank
        //post: none
        //description: checks for enemy tank world collision
        private void EnWorldCol(Tank tank)
        {
            //stores the tank offset rectangle
            Rectangle offsetRec = tank.GetOffsetRec();

            //stores the centre y and x node positoins
            int centreY = (offsetRec.Y - Game1.TOP_PLAY_Y) / WALL_LENGTH;
            int centreX = (offsetRec.X - Game1.LEFT_PLAY_X) / WALL_LENGTH;

            //creates a variabel to store the land type
            int landType;

            //loop through the columns around the centre node
            for (int i = centreY - 1; i <= centreY + 1; i++)
            {
                //loop through the rows around the centre node
                for (int x = centreX - 1; x <= centreX + 1; x++)
                {
                    //set the landtype
                    landType = GetLandType(x, i);

                    //check if the landtyp is wall or water
                    if (landType == Game1.WALL || landType == Game1.WATER)
                    {
                        //push teh tank based on the wall/water
                        tank.PushTank(Util.RotatedCollisionPush(offsetRec, gridRecs[x, i], tank.GetTankAngle(), 0f));
                    }
                }
            }

            //loop through the enemy tanks
            for(int i = 0; i < enemyTanks.Count; i++)
            {
                //check if the enemy tank id is not the same as current tank id
                if(enemyTanks[i].GetId() != tank.GetId())
                {
                    //push the tank based on rotated collision with enemy tank at index i
                    tank.PushTank(Util.RotatedCollisionPush(offsetRec, enemyTanks[i].GetOffsetRec(), tank.GetTankAngle(), enemyTanks[i].GetTankAngle()));
                }
            }
        }

        //pre: none
        //post: a bool representing if a enemy bullet collides with player
        //description: check for enemy bullet player collision
        private bool EnBullPlayerCol()
        {
            //loops throug hthe enemy bullets
            for (int i = 0;  i < enBullets.Count; i++)
            {
                //checks for collision with player
                if (Util.RotatedCollision(enBullets[i].GetOffsetRec(), player.GetOffsetRec(), enBullets[i].GetAngle(), player.GetTankAngle()))
                {
                    //returns false
                    return false;
                }
            }

            //returns true
            return true;
        }

        //pre: none
        //post: none
        //description: checks for player bullet enemy collision
        private void PlayBullEnCol()
        {
            //loops through the player bullets
            for(int i = 0; i < playBullets.Count; i++)
            {
                //loops through the enemy tanks
                for (int x = 0; x < enemyTanks.Count; x++)
                {
                    //checks if the player bullets at index i collide with enemy tanks at x
                    if (Util.RotatedCollision(playBullets[i].GetOffsetRec(), enemyTanks[x].GetOffsetRec(), playBullets[i].GetAngle(), enemyTanks[x].GetTankAngle()))
                    {
                        //add new explosion animations
                        tankExplodAnims.Add(new Animation(tankExploadImg, 5, 3, 12, 0, 0, Animation.ANIMATE_ONCE, 1000, enemyTanks[x].GetExploadLoc(), true));
                        bulletExplodAnims.Add(new Animation(bulletExploadImg, 3, 3, 8, 0, 0, Animation.ANIMATE_ONCE, 600, playBullets[i].GetExploadLoc(), true));

                        //create a sound effect
                        tankExploadSnd.CreateInstance().Play();

                        //remove enemy tank at index x and player bullet at index i
                        enemyTanks.RemoveAt(x);
                        playBullets.RemoveAt(i);

                        //increase the stat values
                        levelStats[BULL_HIT, levelNum]++;
                        playStats[BULL_HIT]++;

                        //set x and i values to leave the for statement
                        x = enemyTanks.Count;
                        i = playBullets.Count;
                    }
                }
            }
        }

        //pre: a valid bullet
        //post: none
        //description: check for bullet collision
        private void BulletCol(Bullet bullet)
        {
            //stores the bullet location
            Vector2 bulletLoc = bullet.GetLoc();

            //stores the centre node that the bullet is on
            int centreY = ((int)bulletLoc.Y - Game1.TOP_PLAY_Y) / WALL_LENGTH;
            int centreX = ((int)bulletLoc.X - Game1.LEFT_PLAY_X) / WALL_LENGTH;

            //stores the landtype
            int landType;

            //stores the x, y and diagonal distances
            float distX;
            float distY;
            float diagDist;

            //loops through the rows around the centre node
            for (int i = centreY - 1; i <= centreY + 1; i++)
            {
                //loops through the columns around the centre node
                for (int x = centreX - 1; x <= centreX + 1; x++)
                {
                    //stores the landtype
                    landType = GetLandType(x, i);

                    //checks if the landtype is wall
                    if (landType == Game1.WALL)
                    {
                        //finds x and y distance
                        distX = bulletLoc.X - gridRecs[x, i].Center.X;
                        distY = bulletLoc.Y - gridRecs[x, i].Center.Y;

                        //checks if the x and y distance is less than the bullet radius
                        if(Math.Abs(distX) < gridRecs[x, i].Width * 0.5 + Game1.BULLET_SIZE && Math.Abs(distY) < gridRecs[x, i].Height * 0.5 + Game1.BULLET_SIZE)
                        {
                            //sets the diagonal distance
                            diagDist = (float)Math.Sqrt(Math.Pow(Math.Abs(distX) - gridRecs[x, i].Width * 0.5, 2) + Math.Pow(Math.Abs(distY) - gridRecs[x, i].Height * 0.5, 2));

                            //checks if the daigonal distance is less than the bullet radius
                            if (diagDist <= Game1.BULLET_SIZE)
                            {
                                //checks if the magnitude of the x distance is greater than or equal to the magnitude of the y distance
                                if (Math.Abs(distX) >= Math.Abs(distY))
                                {
                                    //change the bullet direction accordingly
                                    bullet.ChangeDirection(-1, 1);

                                    //set x and i values to leave for statement
                                    x = centreX + 2;
                                    i = centreY + 2;
                                }
                                else
                                {
                                    //change the bullet direction accordingly
                                    bullet.ChangeDirection(1, -1);

                                    //set x and i values to leave for statement
                                    x = centreX + 2;
                                    i = centreY + 2;
                                }
                            }
                            else
                            {
                                //checks if the magnitude of the x distance is less than or equal to the half the width of the grid square
                                if (Math.Abs(distX) <= gridRecs[x, i].Width * 0.5)
                                {
                                    //change the bullet direction accordingly
                                    bullet.ChangeDirection(1, -1);

                                    //set x and i values to leave for statement
                                    x = centreX + 2;
                                    i = centreY + 2;
                                }
                                else if (Math.Abs(distY) <= gridRecs[x, i].Height * 0.5)
                                {
                                    //change the bullet direction accordingly
                                    bullet.ChangeDirection(- 1, 1);

                                    //set x and i values to leave for statement
                                    x = centreX + 2;
                                    i = centreY + 2;
                                }
                            }
                        }
                    }
                }
            }
        }

        //pre: none
        //post: a valid vector2 representing the indexes of enemy bullets and play bullets that collide
        private Vector2 PlayBullEnBullCol()
        {
            //loop through the enemy bullets
            for (int i = 0; i < enBullets.Count; i++)
            {
                //loop through the play bullets
                for(int x = 0; x < playBullets.Count; x++)
                {
                    //check if the enemy bullet at index i collides with the play bullet at index x
                    if (Util.RotatedCollision(playBullets[x].GetOffsetRec(), enBullets[i].GetOffsetRec(), playBullets[x].GetAngle(), enBullets[i].GetAngle()))
                    {
                        //changes stats accordingly
                        levelStats[BULL_HIT, levelNum]++;
                        playStats[BULL_HIT]++;

                        //returns index locations
                        return new Vector2(i,x);
                    }
                }
            }

            //returns default vector2
            return new Vector2(-1,-1);
        }

        //pre: a valid bullet
        //post: none
        //description: checks if a play bullet should be added, and adds one if so
        private void AddPlayBulletCheck(Bullet bullet)
        {
            //checks if bullet is not null
            if (bullet != null)
            {
                //checks if there is not the max number of bullets on screen
                if (playBullets.Count != MAX_SCREEN_BUL)
                {
                    //checks if the bullet location is a valid location
                    if (ValidShootLocChecker(bullet.GetOffsetRec()))
                    {
                        //adds a bullet to play bullets
                        playBullets.Add(bullet);

                        //adds a bullet explosion animation to bullet explosion animations
                        bulletExplodAnims.Add(new Animation(bulletExploadImg, 3, 3, 8, 0, 0, Animation.ANIMATE_ONCE, 600, bullet.GetExploadLoc(), true));

                        //creates a gun shot sound
                        gunShotSnd.CreateInstance().Play();

                        //changes stats accordingly
                        levelStats[BULLETS_SHOT, levelNum]++;
                        playStats[BULLETS_SHOT]++;
                    }
                }
                else
                {
                    //create a sound effect to tell user that there is no ammo left
                    noAmmoSnd.CreateInstance().Play();
                }
            }
        }

        //pre: a valid bullet
        //post: none
        //description: check if enemy bullet should be added
        private void AddEnBulletCheck(Bullet bullet)
        {
            //check if bullet is not null
            if (bullet != null)
            {
                //check if bullet is in a valid location
                if (ValidShootLocChecker(bullet.GetOffsetRec()))
                {
                    //adds a bullet to enemy bullets
                    enBullets.Add(bullet);

                    //creates a gun shot sound
                    gunShotSnd.CreateInstance().Play();

                    //adds a bullet explosion animation
                    bulletExplodAnims.Add(new Animation(bulletExploadImg, 3, 3, 8, 0, 0, Animation.ANIMATE_ONCE, 600, bullet.GetExploadLoc(), true));
                }
            }
        }

        //pre: none
        //post: none
        //description: draws the level
        public void DrawLevel()
        {
            //loops through the x length of the grid info
            for (int i = 0; i < gridInfo.GetLength(0); i++)
            {
                // loops through teh y length of the grid info
                for (int x = 0; x < gridInfo.GetLength(1); x++)
                {
                    //draws the grid based on grid info
                    switch (gridInfo[i, x, levelNum])
                    {
                        case Game1.WALL:
                            //draws a wall at grid rectangle at index i, x
                            Game1.spriteBatch.Draw(wallImg, gridRecs[i, x], Color.White);
                            break;

                        case Game1.WATER:
                            //draws water at grid rectangle at index i, x
                            Game1.spriteBatch.Draw(waterImg, gridRecs[i, x], Color.White * 0.55f);
                            break;
                    }
                }
            }
        }

        //pre: a valid index x and index y
        //post: an interger representing the grid info
        //description: gets the land type at specefied index
        public int GetLandType(int x, int y)
        {
            //checks if the index is not within border
            if (x < 0 || x >= gridRecs.GetLength(0) || y < 0 || y >= gridRecs.GetLength(1))
            {
                //returns -1
                return -1;
            }

            //returns the grid info at x, y and level num location
            return gridInfo[x, y, levelNum];
        }

        //pre: a valid int tank type, a valid vector2 spawn location, a valid interger id number
        private Tank AddNewTank(int tankType, Vector2 spawnLoc, int id)
        {
            //return a tank based on tank type
            switch(tankType)
            {
                case Game1.YELLOW_TANK:
                    //returns a yellow tank
                    return new YellowTank(tankType, spawnLoc, id, tileMap);

                case Game1.PURPLE_TANK:
                    //returns a purple tank
                    return new PurpleTank(tankType, spawnLoc, id, tileMap);

                case Game1.BLUE_TANK:
                    //returns a blue tank
                    return new BlueTank(tankType, spawnLoc, id, tileMap);

                case Game1.RED_TANK:
                    //returns a red tank
                    return new RedTank(tankType, spawnLoc, id, tileMap);
            }

            //returns null
            return null;
        }

        //pre: a valid level number
        //post: none
        //description: sets up the level
        public void SetUpLevel(int levelNum)
        {
            //resets the player stats
            ResetPlayStats();

            //rests the game timer activated
            gameTimer.ResetTimer(true);

            //rests the level
            ResetLevel();

            //sets the level number
            this.levelNum = levelNum;

            //adds one to number of attempts stat
            levelStats[ATTEMPTS, levelNum]++;

            //loop through the x length of grid information
            for (int i = 0; i < gridInfo.GetLength(0); i++)
            {
                //loop through the y length of grid information
                for (int x = 0; x < gridInfo.GetLength(1); x++)
                {
                    //reset the node at index i, x
                    tileMap[i, x].ResetNode(gridInfo[i, x, levelNum]);

                    //check if the grid info is greater than player tank
                    if (gridInfo[i, x, levelNum] > Game1.PLAYER_TANK)
                    {
                        //add enemy tank
                        enemyTanks.Add(AddNewTank(gridInfo[i, x, levelNum], new Vector2(i * WALL_LENGTH + WALL_LENGTH * 0.5f, x * WALL_LENGTH + WALL_LENGTH * 0.5f), enemyTanks.Count));
                    }
                }
            }

            //set player position
            player.SetPos(new Vector2(Game1.LEFT_PLAY_X + (int)playerSpawnGrid[levelNum].X * WALL_LENGTH + WALL_LENGTH * 0.5f, Game1.TOP_PLAY_Y + (int)playerSpawnGrid[levelNum].Y * WALL_LENGTH + WALL_LENGTH * 0.5f));
        }

        //pre: none
        //post: an interger represneing the level number
        //description: gets the level number to be displayed to user
        public int GetLevelNum()
        {
            //returns the level number plus 1
            return levelNum + 1;
        }

        //pre: none
        //post: none
        //description: resets the level
        public void ResetLevel()
        {
            //clears the enemy information
            enBullets.Clear();
            enemyTanks.Clear();

            //clears the player bullets
            playBullets.Clear();

            //resets the player
            player.Reset();

            //clears the expload animations
            tankExplodAnims.Clear();
            bulletExplodAnims.Clear();
        }

        //pre: a valid list of bullets
        //post: none
        //description: updates an list of bullets
        private void UpdateBullets(List<Bullet> bullets)
        {
            //loops through the bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                //checks if the bullet is ready to be removed and updates bullet
                if (!bullets[i].UpdateBullet())
                {
                    //adds a bullet explosion animation
                    bulletExplodAnims.Add(new Animation(bulletExploadImg, 3, 3, 8, 0, 0, Animation.ANIMATE_ONCE, 600, bullets[i].GetExploadLoc(), true));

                    //adds a bullet explosion sound
                    bulletExploadSnd.CreateInstance().Play();

                    //removes bullets at index i
                    bullets.RemoveAt(i);
                }
                else
                {
                    //checks for bullet collision at index i
                    BulletCol(bullets[i]);
                }
            }
        }

        //pre: a valid  list of bullets
        //post: none
        //descriptino: draws a list of bullets
        private void DrawBullets(List<Bullet> bullets)
        {
            //loops through the bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                //draws the bullet at index i
                bullets[i].Draw();
            }
        }

        //pre: a valid offset rectangle
        //post: a bool representing if the bullet in a valid locaiton
        //description: checks if a bullet being shot is in a valid location
        private bool ValidShootLocChecker(Rectangle offsetRec)
        {
            //sets centre y and x node location
            int centreY = (offsetRec.Y - Game1.TOP_PLAY_Y) / WALL_LENGTH;
            int centreX = (offsetRec.X - Game1.LEFT_PLAY_X) / WALL_LENGTH;

            //stores the landtype
            int landType;

            //loops through the columns around the centre node
            for (int i = centreY - 1; i <= centreY + 1; i++)
            {
                //loops through teh rows around the centre node
                for (int x = centreX - 1; x <= centreX + 1; x++)
                {
                    //sets the landtype
                    landType = GetLandType(x, i);

                    //checks if the landtype is a wall
                    if (landType == Game1.WALL)
                    {
                        //checks if the bullet collides with teh wall
                        if (Util.RotatedCollision(offsetRec, gridRecs[x, i], player.GetTankAngle(), 0f))
                        {
                            //return false
                            return false;
                        }
                    }
                }
            }

            //return true
            return true;
        }

        //pre: none
        //post: a bool representing if the player won the round
        //description: returns if the player won the game
        public bool GetIfWin()
        {
            //returns if the player won the game
            return didWin;
        }

        //pre: none
        //post: none
        //description: draw the level seleciton stars
        public void DrawLevelSelStar()
        {
            //loop through all stars up to the first level taht is still locked
            for (int i = 0; i < GetFirstLevLocked() * Game1.NUM_STAR_PER_LEV; i++)
            {
                //draw star at index i
                Game1.spriteBatch.Draw(starImgs[Convert.ToInt32(isStarAcheived[i])], starRecs[i], Color.White);
            }
        }

        //pre: a valid interger level number
        //post: none
        //description: change the level numebr based on provided level number
        public void ChangeLevelNum(int levelNum)
        {
            //set level number
            this.levelNum = levelNum;
        }

        //pre: none
        //post: a valid interger representing next gamestate
        //description: updates the level information
        public int UpdateLevelInfo()
        {
            //loops through the level information button rectangles
            for (int i = 0; i < levelInfoButtonRecs.Length; i++)
            {
                //checks if the user is hovering the button at index i
                if (levelInfoButtonRecs[i].Contains(Game1.mouse.Position))
                {
                    //set is hovering variable at index i to true
                    isLevelInfoButHover[i] = true;

                    //check if the user clicked the left button
                    if (Game1.mouse.LeftButton == ButtonState.Pressed && Game1.prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //create a button click sound
                        buttonClickSnd.CreateInstance().Play();

                        //change gamestate based on i value
                        switch (i)
                        {
                            case LEV_INFO_BACK:
                                //set temporary variable to previous gamestaet
                                int temp = prevGameState;

                                //set previous gamestate to level selction
                                prevGameState = Game1.LEVEL_SELECT;

                                //return temporary variable
                                return temp;

                            case LEV_INFO_PLAY:
                                //return gameplay
                                return Game1.GAMEPLAY;
                        }
                    }
                }
                else
                {
                    //set is hover variabel to false
                    isLevelInfoButHover[i] = false;
                }
            }

            //return level info gamestate
            return Game1.LEVEL_INFO;
        }

        //pre: none
        //post: none
        //description: sets up the endgame screen
        private void SetUpEndgame()
        {
            //change time stats
            scores[SORT_TIME, levelNum].AddData((float)Math.Round(gameTimer.GetTimePassed() * 0.001, 3));
            medLevelStats[SORT_TIME] = scores[SORT_TIME, levelNum].GetMedian();
            levelStats[TIME, levelNum] = scores[SORT_TIME, levelNum].GetAverage();
            playStats[TIME] = (float)(gameTimer.GetTimePassed() * 0.001);


            //check if bullets were shot in game
            if (playStats[BULLETS_SHOT] == 0)
            {
                //change accuracy stats to add zero
                playStats[ACCURACY] = 0;
                scores[SORT_ACC, levelNum].AddData(0);
            }
            else
            {
                //change accuracy stats to add the accuracy
                playStats[ACCURACY] = (float)Math.Round(playStats[BULL_HIT] / playStats[BULLETS_SHOT] * 100, 2);
                scores[SORT_ACC, levelNum].AddData((float)Math.Round(playStats[BULL_HIT] / playStats[BULLETS_SHOT] * 100, 2));
            }

            //set accuracy stat
            levelStats[ACCURACY, levelNum] = scores[SORT_ACC, levelNum].GetAverage();
            medLevelStats[SORT_ACC] = scores[SORT_ACC, levelNum].GetMedian();

            //add to average shots per second stats
            playStats[AV_SHOTS_PER_SEC] = (float)Math.Round(playStats[BULLETS_SHOT] / playStats[TIME], 3);
            scores[SORT_AV_SHOT, levelNum].AddData(playStats[AV_SHOTS_PER_SEC]);
            levelStats[AV_SHOTS_PER_SEC, levelNum] = scores[SORT_AV_SHOT, levelNum].GetAverage();
            medLevelStats[SORT_AV_SHOT] = scores[SORT_AV_SHOT, levelNum].GetMedian();

            //reset the endgame timer
            endgameTimer.ResetTimer(true);

            //set endgame message x location to centre screen
            endgameMesLoc.X = 0.5f * (Game1.screenWidth - endTitleFont.MeasureString("Mission " + (levelNum + 1) + ENDGAME_MESSAGES[Convert.ToInt32(didWin)]).X);
            endgameMesShadLoc.X = endgameMesLoc.X + 3;
        }

        //pre: none
        //post: none
        //description: resets the play stats
        private void ResetPlayStats()
        {
            //loop through the endgame stars
            for(int i = 0; i < isEndStarAcheived.Length; i++)
            {
                //set is achieved at index i to false
                isEndStarAcheived[i] = false;
            }

            //loop through the play stats
            for(int i = 0; i < playStats.Length; i++)
            {
                //set the play stats at index i to zero
                playStats[i] = 0;
            }
        }

        //pre: a valid interger representing the previous gamestate
        //post: noe
        //description: change the previous game state
        public void ChangePrevState(int prevGameState)
        {
            //set previous gamestate
            this.prevGameState = prevGameState;
        }

        //pre: none
        //post: none
        //description:set the median values
        public void SetMedianValues()
        {
            //loop through the median stats
            for(int i = 0; i < medLevelStats.Length; i++)
            {
                //set the median level stats at index i
                medLevelStats[i] = scores[i, levelNum].GetMedian();
            }
        }

        //pre: none
        //post: none
        //description: save data to a file
        private void SaveDataToFile()
        {
            try
            {
                //create new file to store overall stats
                outFile = File.CreateText("GameInfo.txt");

                //write the numebr of levels unlocked plus 1
                outFile.WriteLine(GetFirstLevLocked());

                //loop through all unlocked levels
                for(int i = 0; i < GetFirstLevLocked(); i++)
                {
                    //loop through stars for level number i
                    for (int x = 0; x < Game1.NUM_STAR_PER_LEV; x++)
                    {
                        //write if the star is achieved at level number at index x
                        outFile.Write(isStarAcheived[i * Game1.NUM_STAR_PER_LEV + x] + ",");
                    }

                    //write the number of attempts at level i to file
                    outFile.WriteLine("\n" + levelStats[ATTEMPTS, i]);

                    //write the sorted scores to file
                    outFile.WriteLine(scores[SORT_TIME, i].ReturnAll());
                    outFile.WriteLine(scores[SORT_ACC, i].ReturnAll());
                    outFile.WriteLine(scores[SORT_AV_SHOT, i].ReturnAll());

                    //write the bullets hit and bullets shot stats to file
                    outFile.WriteLine(levelStats[BULL_HIT, i]);
                    outFile.WriteLine(levelStats[BULLETS_SHOT, i]);
                }

                //close file
                outFile.Close();
            }
            catch (Exception)
            {

            }
        }

        //pre: none
        //post: an interger representing the next gamestate
        //description: saves data from a file
        public int SaveDataFromFile()
        {
            try
            {
                //read and store overall score
                inFile = File.OpenText("GameInfo.txt");

                //reads first level locked from file
                int firstLevLocked = Convert.ToInt32(inFile.ReadLine());

                //loops through all unlocked levels
                for (int i = 0; i < firstLevLocked; i++)
                {
                    //sets if level is unlocked at idnex i to true
                    isLevelUnlocked[i] = true;

                    //read line and split at commas
                    line = inFile.ReadLine().Split(',');

                    //loop through the stars for level number i
                    for (int x = 0; x < Game1.NUM_STAR_PER_LEV; x++)
                    {
                        //set if star is achieved at star number
                        isStarAcheived[i * Game1.NUM_STAR_PER_LEV + x] = Convert.ToBoolean(line[x]);
                    }

                    //set numeber of attempts at level number stat
                    levelStats[ATTEMPTS, i] = Convert.ToInt32(inFile.ReadLine());

                    //loop through all the sorted datas
                    for(int z = 0; z < NUM_SORTED_DATA; z++)
                    {
                        //read line and split at commas
                        line = inFile.ReadLine().Split(',');

                        //loop through the array based on the number of attempts
                        for (int x = 0; x < levelStats[ATTEMPTS, i]; x++)
                        {
                            //add data to sorted score
                            scores[z, i].AddData((float)Convert.ToDouble(line[x]));
                        }
                    }

                    //set accuracy, time and average shots per second level stats 
                    levelStats[ACCURACY, i] = scores[SORT_ACC, i].GetAverage();
                    levelStats[TIME, i] = scores[SORT_TIME, i].GetAverage();
                    levelStats[AV_SHOTS_PER_SEC, i] = scores[SORT_AV_SHOT, i].GetAverage();

                    //read level stats from file for bullets hit and bullets shot
                    levelStats[BULL_HIT, i] = Convert.ToInt32(inFile.ReadLine());
                    levelStats[BULLETS_SHOT, i] = Convert.ToInt32(inFile.ReadLine());
                }

                //check if infile does not equal null
                if (inFile != null)
                {
                    //close infile
                    inFile.Close();
                }

                //return manu gamestate
                return Game1.MENU;
            }
            catch (FileNotFoundException)
            {
                //set the three lines of error messages
                errorMessage[0] = "Your save file could not be found!";
                errorMessage[1] = "If this is first game, have fun!";
                errorMessage[2] = "Otherwise your data was lost, sorry :(";

                //fix up the game to match file error
                return SetUpErrorScreen();
            }
            catch (Exception)
            {
                //set the three lines of error messages
                errorMessage[0] = "The file could not be read!";
                errorMessage[1] = "Sorry for the inconveinience :(";
                errorMessage[2] = "Have fun playing the game!";

                //fix up the game to match file error
                return SetUpErrorScreen();
            }
        }

        //pre:none
        //post: none
        //description: sets up the error screen
        private int SetUpErrorScreen()
        {
            //set error message location
            for (int i = 0; i < errorMessageloc.Length; i++)
            {
                //sets x and y location of error message at index i
                errorMessageloc[i].X = 0.5f * (Game1.screenWidth - errorScreenFont.MeasureString(errorMessage[i]).X);
                errorMessageloc[i].Y = ERROR_MESSAGE_START_Y + ERROR_MES_SPACE * i;

                //set error message shadow location
                errorMessageShadloc[i] = new Vector2(errorMessageloc[i].X + 2, errorMessageloc[i].Y + 2);
            }

            //check if infile does not equal null
            if (inFile != null)
            {
                //close infile
                inFile.Close();
            }

            //reset all data
            ResetAllData();

            //save all data to file
            SaveDataToFile();

            //return file error gamestate
            return Game1.FILE_ERROR;
        }

        //pre: none
        //post: none
        //description: draw the error screen
        public void DrawErrorScreen()
        {
            //draw the error tile
            Game1.spriteBatch.DrawString(titleFont, ERROR_TITLE, errorTitleShadPos, Color.Black * 0.8f);
            Game1.spriteBatch.DrawString(titleFont, ERROR_TITLE, errorTitlePos, Color.Gray);

            //loop through the error messages
            for (int i = 0; i < errorMessage.Length; i++)
            {
                //draw error messag at index i
                Game1.spriteBatch.DrawString(errorScreenFont, errorMessage[i], errorMessageShadloc[i], Color.Black * 0.8f);
                Game1.spriteBatch.DrawString(errorScreenFont, errorMessage[i], errorMessageloc[i], Color.Gray);
            }

            //loop through the level info button rectangles
            for(int i = 0; i < levelInfoButtonRecs.Length; i++)
            {
                //draw the button at index i
                Game1.spriteBatch.Draw(buttonImg, levelInfoButtonRecs[i], Color.White);
                Game1.spriteBatch.DrawString(menuFont, FILE_ERROR_BUT_TEXTS[i], playStatsButtonShadTextPos[i], Color.Black * 0.5f);
                Game1.spriteBatch.DrawString(menuFont, FILE_ERROR_BUT_TEXTS[i], playStatsButtonTextPos[i], LEV_INFO_OPTION_COL[Convert.ToInt32(isLevelInfoButHover[i])]);
            }
        }

        //pre: none
        //post: none
        //description: reset all data
        private void ResetAllData()
        {
            //loop through all teh levels
            for (int i = 0; i < Game1.NUM_LEV; i++)
            {
                //loop through all stars
                for (int x = 0; x < Game1.NUM_STAR_PER_LEV; x++)
                {
                    //set is star achieved value to false
                    isStarAcheived[i * Game1.NUM_STAR_PER_LEV + x] = false;
                }

                //set numebr of attmpts at level number i to false
                levelStats[ATTEMPTS, i] = 0;

                //reset data in sorted scores
                scores[SORT_TIME, i].ResetData();
                scores[SORT_ACC, i].ResetData();
                scores[SORT_AV_SHOT, i].ResetData();

                //rese the bullets hit and bullets shot variables
                levelStats[BULL_HIT, i] = 0;
                levelStats[BULLETS_SHOT, i] = 0;
            }
        }

        //pre: none
        //post: a interger repreenting the first level that is locked
        public int GetFirstLevLocked()
        {
            //loop through the is level unlocked array
            for(int i = 0; i < isLevelUnlocked.Length; i++)
            {
                //check if the level is not unlocked at index i
                if(!isLevelUnlocked[i])
                {
                    //return index i
                    return i;
                }
            }

            //return the number of levels
            return Game1.NUM_LEV;
        }

        //pre: a valid list of animations, a valid gametime
        //post: none
        //description: update animations
        private void UpdateAnimation(List<Animation> animations, GameTime gameTime)
        {
            //loop through the animainos 
            for (int i = 0; i < animations.Count; i++)
            {
                //update the animation at index i
                animations[i].Update(gameTime);

                //check if the animation at index i is animating
                if (!animations[i].IsAnimating())
                {
                    //remove animation from list
                    animations.RemoveAt(i);
                }
            }
        }
    }
}