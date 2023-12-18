// Author: Dylan Nagel
// File Name: Game1.cs
// Project Name: NagelD_PASS3
// Description: Runs a similar game to "Wii Tanks"

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace NagelD_PASS3
{
    public class Game1 : Game
    {
        //stores the graphics and spritebatch
        private GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        //stores the dimensions of the arena field
        public const int TOP_PLAY_Y = 114;
        public const int BOT_PLAY_Y = 564;
        public const int LEFT_PLAY_X = 24;
        public const int RIGHT_PLAY_X = 724;

        //stores the length of each grid square in arena
        public static int GRID_SIDE_LEN = 50;

        //stores the different terrain types
        public const int SAND = 0;
        public const int WALL = 1;
        public const int WATER = 2;

        //stores the bullet information
        public const float BULLET_SPEED = 3.8f;
        public const float FAST_BULLET_SPEED = 5f;
        public const int BULLET_SIZE = 9;
        public const int BULLET_EXPLOAD_SIZE = 25;
        public const int NORM_BUL_MAX_RIC = 1;
        public const int RED_BUL_MAX_RIC = 2;

        //stores the index location of each gamestate
        public const int GAMEPLAY = 0;
        public const int MENU = 1;
        private const int INSTRUCTIONS = 2;
        public const int EXIT = 3;
        public const int LEVEL_SELECT = 4;
        public const int ENDGAME = 5;
        public const int PLAY_STATS = 6;
        public const int LEVEL_INFO = 7;
        public const int FILE_ERROR = 8;

        //stores the shaft size info
        private const int SHAFT_DISPLACE = 25;
        public const int SHAFT_LENGTH = 40;

        //stores the value of extra space
        public const int EXTRA_SPACE = 1;

        //stores the size of the tank
        public const int TANK_LENGTH = 38;

        //stores the yellow tank information
        public const int YEL_MIN_SHOOT_TIME = 3000;
        public const int YEL_MAX_SHOOT_TIME = 6000;
        public const int YEL_TURN_MIN_TIME = 1000;
        public const int YEL_TURN_MAX_TIME = 5000;

        //stores the shaft turn speed
        public const float SHAFT_TURN_SPEED = 0.017f;

        //stores the tank information
        public const int PLAYER_TANK = 3;
        public const int YELLOW_TANK = 4;
        public const int PURPLE_TANK = 5;
        public const int BLUE_TANK = 6;
        public const int RED_TANK = 7;
        public const int NUM_TANKS = 5;
        public const int ARRAY_LOC_SUB = 3;

        //stores direction values for up, down, left and right
        public const int RIGHT = 0;
        public const int DOWN = 1;
        public const int LEFT = 2;
        public const int UP = 3;

        //stores direction values for x and y direction
        public const int X_DIR = 0;
        public const int Y_DIR = 1;

        //stores information for the moving background
        private const float BACK_HOR_MOVE = -.662f;
        private const float BACK_VERT_MOVE = -0.455f;
        private const int NEG_HOR_LIM = -134;
        private const int NEG_VERT_LIM = -85;

        //stores the menu button spacing
        private const int MENU_BUTTON_SPACING = 150;

        //stores the next gamestte from the menu buttons
        private readonly int[] MENU_GAMESTATE_SWITCH = new int[] { LEVEL_SELECT, INSTRUCTIONS, EXIT };

        //stores the offset quantity for shadow
        private const int SHADOW_OFFSET = 3;

        //stores the value for which team owns the bullet
        public static int EN_BUL = 0;
        public static int PLAY_BUL = 1;

        //stores numerical information about levels
        public const int NUM_LEV = 8;
        public const int NUM_STAR_PER_LEV = 3;

        //stores the values for the level choice button
        private const int LEV_BUT_START_X = 20;
        private const int LEV_BUT_START_Y = 170;
        private const int LEV_BUT_SIZE = 155;
        private const int LEV_BUT_HOR_SPACE = 184;
        private const int LEV_BUT_VERT_SPACE = 185;

        //stores the values for the star achievements
        private const int STAR_SIZE = 35;
        private const int STAR_SPACING = 40;
        private const int STAR_START_X_SPACE = 20;
        private const int STAR_START_Y_SPACE = 95;

        //stores index of if the user attained the star
        private const int YES_STAR = 1;
        private const int NO_STAR = 0;

        //stores the tank speeds
        public const float TANK_ROT_SPEED = 0.05f;
        public const float EN_TANK_SPEED = 1.5f;
        public static float PLAY_TANK_SPEED = 1.9f;

        //stores the shaft speeds
        public const float BLUE_SHAFT_TURN_SPEED = 0.04f;
        public const float RED_SHAFT_TURN_SPEED = 0.038f;
        public const float PURP_SHAFT_TURN_SPEED = 0.025f;

        //stores the timer length for the tanks
        public const int PURP_TANK_TIME = 3000;
        public const int RED_TANK_TIME = 900;
        public const int BLUE_TANK_TIME = 700;

        //stores the range if angle is at correct placement enough
        public const float ANGLE_BOUNDS = 0.05f;

        //stores the size of tank explosion
        public const int EXPLOAD_SIZE = 60;

        //stores the index of each type of star achievement
        public const int COMPLETION = 0;
        public const int ACCURACY = 1;
        public const int TIME = 2;

        //stores the requirements to get stars
        public const int MAX_TIMER_STAR = 15000;
        public const float MIN_ACC = 0.65f;

        //stores the colour of the bullets
        public static readonly Color[] BUL_COL_TYPE = new Color[] { Color.White, Color.CornflowerBlue };

        public static Vector2[] MOVEMENT_SPEED = new Vector2[] { new Vector2(EN_TANK_SPEED, 0), new Vector2(0, EN_TANK_SPEED), new Vector2(-EN_TANK_SPEED, 0), new Vector2(0, -EN_TANK_SPEED) };

        //stores the gamestate
        private int gameState = MENU;

        //stores the random variable
        public static Random rng = new Random();

        //stores the keyboard states
        public static KeyboardState kb;
        public static KeyboardState prevKb;

        //stores the mouse states
        public static MouseState mouse;
        public static MouseState prevMouse;

        //stores the arena images
        private Texture2D arenaImg;
        private Texture2D overhangImg;

        //stores the information for shading out objects
        private Texture2D pixelImg;
        private Rectangle fullScreenRec;

        //stores the tank background
        private Texture2D tankBackImg;
        private Rectangle tankBackRec;
        private Vector2 tankBackPos;

        //stores the button image
        private Texture2D buttonImg;

        //stores the tank and shaft images
        public static Texture2D[] tankImgs = new Texture2D[NUM_TANKS];
        public static Texture2D[] shaftImgs = new Texture2D[NUM_TANKS];

        //stores the terrain images
        private Texture2D wallImg;
        private Texture2D waterImg;

        //stores the bullet images
        public static Texture2D bulletImg;
        public static Texture2D fastBulletImg;

        //stores the fonts
        private SpriteFont levelStatFont;
        private SpriteFont endTitleFont;
        private SpriteFont errorScreenFont;
        private static SpriteFont levelNumFont;
        private static SpriteFont menuFont;
        private SpriteFont titleFont;

        //stores the information for the locks
        private Texture2D lockImg;
        private Rectangle[] lockRec = new Rectangle[NUM_LEV];

        //stores the game music
        private Song generalMusic;
        private Song gamePlayMusic;

        //stores the game sounds
        private SoundEffect bulletExploadSnd;
        private SoundEffect buttonClickSnd;
        private SoundEffect gunShotSnd;
        private SoundEffect tankExploadSnd;
        private SoundEffect noAmmoSnd;

        //stores the origin of the tank and shaft
        public static Vector2 tankOrigin;
        public static Vector2 shaftOrigin;

        //stores information for the menu
        private Rectangle[] menuButRecs = new Rectangle[3];
        private Vector2[] menuOptionLocs = new Vector2[3];
        private Vector2[] menuOptionShadLocs = new Vector2[3];
        private string[] menuOptions = new string[] { "Play", "Instructions", "Exit" };
        private bool[] isMenuButsHover = new bool[3];
        private Color[] menuOptionCol = new Color[] { Color.Khaki, Color.Goldenrod };

        //stores the level button information
        private Texture2D levelButImg;
        private Rectangle[] levButRecs = new Rectangle[NUM_LEV];

        //stores the title image information
        private Texture2D tanksTitleImg;
        private Rectangle tanksTitleRec;
        private Rectangle tanksTitleShadRec;

        //stores the level information
        private Texture2D levelSelImg;
        private Rectangle levelSelRec;
        private Rectangle levelSelShadRec;
        private Vector2[] levelNumPos = new Vector2[NUM_LEV];
        private Vector2[] levelNumShadPos = new Vector2[NUM_LEV];
        private bool[] isLevelSelHover = new bool[NUM_LEV];
        private Rectangle levelSelButRec;
        private string levelSelButPrompt = "Back";
        private Vector2 levelSelButPromptLoc;
        private Vector2 levelSelButShadPromptLoc;
        private bool islevelSelButHover;

        //stores the explosion images
        private Texture2D tankExploadImg;
        private Texture2D bulletExploadImg;

        //stores the star information
        private Texture2D[] starImg = new Texture2D[2];
        private Rectangle[] starRecs = new Rectangle[NUM_LEV * NUM_STAR_PER_LEV];

        //stores the instructions screen information
        private Texture2D instructImg;
        private Rectangle instructRec;
        private Rectangle instructBtnRec;
        private bool isInstrucButHover;
        private string instrucButPrompt = "Menu";
        private Vector2 instrucButPropmtLoc;
        private Vector2 instrucButPropmtShadLoc;

        //stores an instance of the gameonject
        private GameObject gameObject;

        //stores the screen width and height
        public static int screenWidth;
        public static int screenHeight;

        //pre: none
        //post: none
        //description: sets game iinfo
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        //pre: none
        //post: none
        //description: initializes game info
        protected override void Initialize()
        {
            //sets size of screen
            this.graphics.PreferredBackBufferWidth = 748;
            this.graphics.PreferredBackBufferHeight = 678;

            //applies changes
            this.graphics.ApplyChanges();

            //sets screen width and height variables
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        //pre: none
        //post: none
        //description: loads content
        protected override void LoadContent()
        {
            //sets spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //sets fonts
            menuFont = Content.Load<SpriteFont>("Fonts/MenuFont");
            levelNumFont = Content.Load<SpriteFont>("Fonts/LevelNumFont");
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            levelStatFont = Content.Load<SpriteFont>("Fonts/LevelStatFont");
            endTitleFont = Content.Load<SpriteFont>("Fonts/EndgameTitleFont");
            errorScreenFont = Content.Load<SpriteFont>("Fonts/FileErrorFont");

            //sets arena image
            arenaImg = Content.Load<Texture2D>("Images/Backgrounds/Arena");
            overhangImg = Content.Load<Texture2D>("Images/Backgrounds/Overhang");

            //sets pixel image
            pixelImg = Content.Load<Texture2D>("Images/Sprites/BlankPixel");
            fullScreenRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //sets bullet images
            bulletImg = Content.Load<Texture2D>("Images/Sprites/NormBullet");
            fastBulletImg = Content.Load<Texture2D>("Images/Sprites/FastBullet");

            //sets terrain images
            wallImg = Content.Load<Texture2D>("Images/Sprites/Wall");
            waterImg = Content.Load<Texture2D>("Images/Sprites/SpikeFloor");

            //sets tank images
            tankImgs[PLAYER_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Tanks/PlayerTank");
            tankImgs[YELLOW_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Tanks/YellowTank");
            tankImgs[PURPLE_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Tanks/PurpleTank");
            tankImgs[BLUE_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Tanks/BlueTank");
            tankImgs[RED_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Tanks/RedTank");

            //sets shaft images
            shaftImgs[PLAYER_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Shafts/PlayerShaft");
            shaftImgs[YELLOW_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Shafts/YellowShaft");
            shaftImgs[PURPLE_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Shafts/PurpleShaft");
            shaftImgs[BLUE_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Shafts/BlueShaft");
            shaftImgs[RED_TANK - ARRAY_LOC_SUB] = Content.Load<Texture2D>("Images/Sprites/Shafts/RedShaft");

            //sets the tank and shaft origin
            tankOrigin = new Vector2(tankImgs[PLAYER_TANK - ARRAY_LOC_SUB].Width * 0.5f, tankImgs[PLAYER_TANK - ARRAY_LOC_SUB].Height * 0.5f);
            shaftOrigin = new Vector2(SHAFT_DISPLACE, SHAFT_DISPLACE);

            //sets game title image and rec
            tanksTitleImg = Content.Load<Texture2D>("Images/Titles/TanksTitle");
            tanksTitleRec = new Rectangle((int)(0.5 * (screenWidth - tanksTitleImg.Width * 0.5)), 50, (int)(tanksTitleImg.Width * 0.5), (int)(tanksTitleImg.Height * 0.5));
            tanksTitleShadRec = new Rectangle(tanksTitleRec.X + SHADOW_OFFSET * 2, tanksTitleRec.Y + SHADOW_OFFSET * 2, tanksTitleRec.Width, tanksTitleRec.Height);

            //sets level seletion title image and rec
            levelSelImg = Content.Load<Texture2D>("Images/Titles/LevelSelectionTitle");
            levelSelRec = new Rectangle((int)(0.5 * (screenWidth - levelSelImg.Width * 0.3)), 40, (int)(levelSelImg.Width * 0.3), (int)(levelSelImg.Height * 0.3));
            levelSelShadRec = new Rectangle(levelSelRec.X + SHADOW_OFFSET * 2, levelSelRec.Y + SHADOW_OFFSET * 2, levelSelRec.Width, levelSelRec.Height);

            //sets level selection button info
            levelSelButRec = new Rectangle((int)(0.5 * (screenWidth - 300)), screenHeight - 110, 300, 80);
            levelSelButPromptLoc = new Vector2(levelSelButRec.X + 0.5f * (levelSelButRec.Width - menuFont.MeasureString(levelSelButPrompt).X), levelSelButRec.Y + 0.5f * (levelSelButRec.Height - menuFont.MeasureString(levelSelButPrompt).Y));
            levelSelButShadPromptLoc = new Vector2(levelSelButPromptLoc.X + SHADOW_OFFSET, levelSelButPromptLoc.Y + SHADOW_OFFSET);

            //sets tank background image and rec
            tankBackImg = Content.Load<Texture2D>("Images/Backgrounds/TankBack");
            tankBackRec = new Rectangle(0, 0, tankBackImg.Width, tankBackImg.Height);

            //sets button images
            buttonImg = Content.Load<Texture2D>("Images/Sprites/Button");
            levelButImg = Content.Load<Texture2D>("Images/Sprites/LevelButton");

            //sets star images
            starImg[YES_STAR] = Content.Load<Texture2D>("Images/Sprites/Star");
            starImg[NO_STAR] = Content.Load<Texture2D>("Images/Sprites/NoStar");

            //sets intructions screen images and rectangles
            instructImg = Content.Load<Texture2D>("Images/Backgrounds/InstructionsImg");
            instructRec = new Rectangle(0, 0, screenWidth, (int)(instructImg.Height * 0.6));
            instructBtnRec = new Rectangle((int)(screenWidth * 0.25 * 0.5), instructRec.Bottom - 120 - 10, (int)(screenWidth * 0.75), 120);
            instrucButPropmtLoc = new Vector2(instructBtnRec.X + (instructBtnRec.Width - menuFont.MeasureString(instrucButPrompt).X) * 0.5f, instructBtnRec.Y + (instructBtnRec.Height - menuFont.MeasureString(instrucButPrompt).Y) * 0.5f);
            instrucButPropmtShadLoc = new Vector2(instrucButPropmtLoc.X + SHADOW_OFFSET, instrucButPropmtLoc.Y + SHADOW_OFFSET);

            //sets explosion images
            tankExploadImg = Content.Load<Texture2D>("Images/SpriteSheets/ExplosionSpriteSheet");
            bulletExploadImg = Content.Load<Texture2D>("Images/SpriteSheets/GunfireSpriteSheet");

            //sets music
            generalMusic = Content.Load<Song>("Audio/Music/GeneralMusic");
            gamePlayMusic = Content.Load<Song>("Audio/Music/GameplayMusic");

            //sets sounds
            bulletExploadSnd = Content.Load<SoundEffect>("Audio/Sound/BulletExpload");
            buttonClickSnd = Content.Load<SoundEffect>("Audio/Sound/ButtonClick");
            gunShotSnd = Content.Load<SoundEffect>("Audio/Sound/GunShot");
            tankExploadSnd = Content.Load<SoundEffect>("Audio/Sound/TanKExpload");
            noAmmoSnd = Content.Load<SoundEffect>("Audio/Sound/NoAmmo");

            //sets lock image
            lockImg = Content.Load<Texture2D>("Images/Sprites/Lock");

            //sets the gameobject variable
            gameObject = new GameObject(arenaImg, overhangImg, wallImg, waterImg, buttonImg, menuFont, titleFont, levelStatFont, starImg, starRecs, endTitleFont, errorScreenFont, tankExploadImg, bulletExploadImg, bulletExploadSnd, buttonClickSnd, gunShotSnd, tankExploadSnd, noAmmoSnd);

            //sets the gamestate and attempts to save data from file
            gameState = gameObject.SaveDataFromFile();

            //loops through each of the level selction button rectangles
            for (int i = 0; i < levButRecs.Length * 0.5; i++)
            {
                //creates intergers for actual values
                int starActVal;
                int butActVal;

                //sets level button rectangle at index i
                levButRecs[i] = new Rectangle(LEV_BUT_START_X + LEV_BUT_HOR_SPACE * i, LEV_BUT_START_Y, LEV_BUT_SIZE, LEV_BUT_SIZE);

                //sets level number position at index i
                levelNumPos[i] = new Vector2(levButRecs[i].X + 0.5f * (levButRecs[i].Width - levelNumFont.MeasureString(Convert.ToString(i)).X), levButRecs[i].Y + 5);
                levelNumShadPos[i] = new Vector2(levelNumPos[i].X + SHADOW_OFFSET, levelNumPos[i].Y + SHADOW_OFFSET);

                //sets loc rectangle at index i
                lockRec[i] = new Rectangle((int)(levButRecs[i].X + 0.5 * (levButRecs[i].Width - 70)), (int)(levButRecs[i].Y + 0.5 * (levButRecs[i].Height - 100)), 70, 100);

                //loops through each of the stars at level i
                for (int x = 0; x < NUM_STAR_PER_LEV; x++)
                {
                    //sets star actual value
                    starActVal = i * NUM_STAR_PER_LEV + x;

                    //sets rectangle for star rectangle at actual value and level i
                    starRecs[starActVal] = new Rectangle(STAR_START_X_SPACE + levButRecs[i].X + STAR_SPACING * x, levButRecs[i].Y + STAR_START_Y_SPACE, STAR_SIZE, STAR_SIZE);
                }

                //sets button actual value
                butActVal = i + (int)(levButRecs.Length * 0.5);

                //sets level button rectangle at button actual value
                levButRecs[butActVal] = new Rectangle(LEV_BUT_START_X + LEV_BUT_HOR_SPACE * i, LEV_BUT_START_Y + LEV_BUT_VERT_SPACE, LEV_BUT_SIZE, LEV_BUT_SIZE);

                //sets level number position at button actual value
                levelNumPos[butActVal] = new Vector2(levButRecs[butActVal].X + 0.5f * (levButRecs[butActVal].Width - levelNumFont.MeasureString(Convert.ToString(butActVal + 1)).X), levButRecs[butActVal].Y + 5);
                levelNumShadPos[butActVal] = new Vector2(levelNumPos[butActVal].X + SHADOW_OFFSET, levelNumPos[butActVal].Y + SHADOW_OFFSET);

                //sets loc rectangle at button actual value
                lockRec[butActVal] = new Rectangle((int)(levButRecs[butActVal].X + 0.5 * (levButRecs[butActVal].Width - 70)), (int)(levButRecs[butActVal].Y + 0.5 * (levButRecs[butActVal].Height - 100)), 70, 100);

                //loops through each star at level number button actual value
                for (int x = 0; x < NUM_STAR_PER_LEV; x++)
                {
                    //sets star acutal value
                    starActVal = butActVal * NUM_STAR_PER_LEV + x;

                    //sets star rectangels at level number button actual value and star number star actual value
                    starRecs[starActVal] = new Rectangle(STAR_START_X_SPACE + levButRecs[butActVal].X + STAR_SPACING * x, levButRecs[butActVal].Y + STAR_START_Y_SPACE, STAR_SIZE, STAR_SIZE);
                }
            }

            //loops through the menu button rectangles
            for (int i = 0; i < menuButRecs.Length; i++)
            {
                //sets the menu button rectangle at index i
                menuButRecs[i] = new Rectangle((int)((screenWidth - screenWidth * 0.75) * 0.5), 230 + MENU_BUTTON_SPACING * i, (int)(screenWidth * 0.75), 120);

                //sets the menu option locations at index i
                menuOptionLocs[i] = new Vector2(0.5f * (screenWidth - menuFont.MeasureString(menuOptions[i]).X), 0.5f * menuFont.MeasureString(menuOptions[i]).Y + menuButRecs[i].Y);
                menuOptionShadLocs[i] = new Vector2(menuOptionLocs[i].X + SHADOW_OFFSET, menuOptionLocs[i].Y + SHADOW_OFFSET);
            }
        }

        //pre: a valid gametime
        //post: none
        //description: updates the game
        protected override void Update(GameTime gameTime)
        {
            //updates keybaord input
            prevKb = kb;
            kb = Keyboard.GetState();

            //updates moues input
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //checks if gamestate is not gameplay
            if(gameState != GAMEPLAY)
            {
                //checks if the media state is not currenlty playing
                if (MediaPlayer.State != MediaState.Playing)
                {
                    //plays music
                    MediaPlayer.Play(generalMusic);
                }
            }
            else
            {
                //checks if the media state is not currently playing
                if (MediaPlayer.State != MediaState.Playing)
                {
                    //plays music
                    MediaPlayer.Play(gamePlayMusic);
                }
            }

            //updates game based on gamestate
            switch(gameState)
            {
                case GAMEPLAY:
                    //updates the gameplay
                    UpdateGameplay(gameTime);
                    break;

                case MENU:
                    //updates the menu
                    UpdateMenu();
                    break;

                case INSTRUCTIONS:
                    //updates the instructions screen
                    UpdateInstructions();
                    break;

                case LEVEL_SELECT:
                    //updates the level selection screen
                    UpdateLevelSelect();
                    break;

                case PLAY_STATS:
                    //updates the play stats screen
                    UpdatePlayStats();
                    break;

                case ENDGAME:
                    //updates the endgame screen
                    UpdateEndgame(gameTime);
                    break;

                case LEVEL_INFO:
                    //updates the level info screen
                    UpdateLevelInfo();
                    break;

                case EXIT:
                    //exits the game
                    Exit();
                    break;

                case FILE_ERROR:
                    //updates the file error screen
                    UpdateFileError();
                    break;
            }

            base.Update(gameTime);
        }

        //pre: a valid gametime
        //post: none
        //description: draws the game
        protected override void Draw(GameTime gameTime)
        {
            //begins the spritebatch
            spriteBatch.Begin();

            //draw the game based on the gamestate
            switch (gameState)
            {
                case GAMEPLAY:
                    //draws the gameplay
                    DrawGameplay();
                    break;

                case MENU:
                    //draws the menu screen
                    DrawMenu();
                    break;

                case INSTRUCTIONS:
                    //draws the instructions screen
                    DrawInstructions();
                    break;

                case LEVEL_SELECT:
                    //draws the level selection screen
                    DrawLevelSelect();
                    break;

                case ENDGAME:
                    //draws the endgame screen
                    DrawEndgame();
                    break;

                case PLAY_STATS:
                    //draws the play stats
                    DrawPlayStats();
                    break;

                case LEVEL_INFO:
                    //draws the level info
                    DrawLevelInfo();
                    break;

                case FILE_ERROR:
                    //draws the file error
                    DrawFileError();
                    break;
            }

            //ends the spritebatch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //pre: a valid gametime
        //post: none
        //description: updates the gameplay screen
        private void UpdateGameplay(GameTime gameTime)
        {
            //Updates gameplay and checks if game is over
            if (!gameObject.UpdateGameplay(gameTime))
            {
                //sets gamestate to endgame
                gameState = ENDGAME;

                //pauses media player
                MediaPlayer.Pause();
            }
        }

        //pre: none
        //post: none
        //description: updates the menu screen
        private void UpdateMenu()
        {
            //updates the menu background
            UpdateTankBackground();

            //loops through the menu button rectangles
            for (int i = 0; i < menuButRecs.Length; i++)
            {
                //checks if the menu button at index i contains the mouse position
                if (menuButRecs[i].Contains(mouse.Position))
                {
                    //sets the is hover variabel at index i to true
                    isMenuButsHover[i] = true;

                    //checks if the user clicked the left button
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //creates a button click sound
                        buttonClickSnd.CreateInstance().Play();

                        //sets gamestate to menu gamestate at index i
                        gameState = MENU_GAMESTATE_SWITCH[i];
                    }
                }
                else
                {
                    //sets the is hover variabel at index i to false
                    isMenuButsHover[i] = false;
                }
            }
        }

        //pre: none
        //post: none
        //description: updates the instructions menu
        private void UpdateInstructions()
        {
            //updates the menu background
            UpdateTankBackground();

            //checks if instructions is above arcade top
            if (instructRec.Top + (mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025 < 0)
            {
                //changes instructions rectangle location depending on mouse scrolling
                instructRec.Y += (int)((mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025);
            }
            else
            {
                //sets instructions rectangle to top of screen
                instructRec.Y = 0;
            }

            //checks if the instructions rectangle is farther than the bottom
            if (instructRec.Bottom + (mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025 > screenHeight)
            {
                //changes instructions rectangle location depending on mouse scrolling
                instructRec.Y += (int)((mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025);
            }
            else
            {
                //sets instructions rectangle to top of screen
                instructRec.Y = screenHeight - instructRec.Height;
            }

            //set instructions button to the bottom of instructions screen
            instructBtnRec.Y = instructRec.Bottom - instructBtnRec.Height - 60;

            //sets the prompt on button location
            instrucButPropmtLoc = new Vector2(instructBtnRec.X + (instructBtnRec.Width - menuFont.MeasureString(instrucButPrompt).X) * 0.5f, instructBtnRec.Y + (instructBtnRec.Height - menuFont.MeasureString(instrucButPrompt).Y) * 0.5f);
            instrucButPropmtShadLoc = new Vector2(instrucButPropmtLoc.X + SHADOW_OFFSET, instrucButPropmtLoc.Y + SHADOW_OFFSET);

            //checks if the user is hovering the button
            if (instructBtnRec.Contains(mouse.Position))
            {
                //sets the is hovering variable to true
                isInstrucButHover = true;

                //checks if the left button was clicked
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    //creates a button click sound
                    buttonClickSnd.CreateInstance().Play();

                    //sets the instructions rectangle y position to top of screen
                    instructRec.Y = 0;

                    //sets the button position
                    instructBtnRec.Y = instructRec.Bottom - instructBtnRec.Height - 60;

                    //changes gamestate to menu
                    gameState = MENU;
                }
            }
            else
            {
                //sets the is hovering variable to false
                isInstrucButHover = false;
            }
        }

        //pre: none
        //post: none
        //description: updates the level selection screen
        private void UpdateLevelSelect()
        {
            //updates the menu background
            UpdateTankBackground();

            //loops through each of the levels that are unlocked
            for (int i = 0; i < gameObject.GetFirstLevLocked(); i++)
            {
                //checks if the user is hovering the button for level at index i
                if (levButRecs[i].Contains(mouse.Position))
                {
                    //sets the is hover variabel at index i to true
                    isLevelSelHover[i] = true;

                    //checks if the user clicked the left click button
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //creates a button click sound
                        buttonClickSnd.CreateInstance().Play();

                        //chnge when you add a back button you loser breath butt face
                        gameObject.ChangeLevelNum(i);
                        gameObject.ChangePrevState(LEVEL_SELECT);
                        gameObject.SetMedianValues();

                        //sets gamestate to level info
                        gameState = LEVEL_INFO;
                    }
                }
                else
                {
                    //sets the is hover variabel at index i to false
                    isLevelSelHover[i] = false;
                }
            }

            //checks if the level selection back button is hovered
            if (levelSelButRec.Contains(mouse.Position))
            {
                //sets the is hover variable to true
                islevelSelButHover = true;

                //checks if the user clicked the left click
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    //makes a button click cound
                    buttonClickSnd.CreateInstance().Play();

                    //sets gamestate to menu
                    gameState = MENU;
                }
            }
            else
            {
                //sets the is hover variable to false
                islevelSelButHover = false;

            }
        }

        //pre: none
        //post: none
        //description: updates the play stats screen
        private void UpdatePlayStats()
        {
            //updates the menu background
            UpdateTankBackground();

            //updates the play stats screen and sets gameplay to its value
            gameState = gameObject.UpdatePlayStats();
        }

        //pre: a valid gametime
        //post: none
        //description: updates the endgame screen
        private void UpdateEndgame(GameTime gameTime)
        {
            //updates the endgame screen and sets gamestate to its value
            gameState = gameObject.UpdateEndgame(gameTime);

            //checks if gamestate is gameplay
            if (gameState == GAMEPLAY)
            {
                //pauses the music
                MediaPlayer.Pause();
            }
        }

        //pre: none
        //post: none
        //description: updates the level info screen
        private void UpdateLevelInfo()
        {
            //updates the menu background
            UpdateTankBackground();

            //updates the level info screen and sets gamestate to its value
            gameState = gameObject.UpdateLevelInfo();

            //checks if gamestate is gameplay
            if (gameState == GAMEPLAY)
            {
                //sets up the level
                gameObject.SetUpLevel(gameObject.GetLevelNum() - 1);

                //pauses the music
                MediaPlayer.Pause();
            }
        }

        //pre: none
        //post: none
        //description:
        private void UpdateFileError()
        {
            //updates the menu background
            UpdateTankBackground();

            //updates the error screen and sets gamestate to its value
            gameState = gameObject.UpdateErrorScreen();
        }

        //pre: none
        //post: none
        //description: draws the gameplay
        private void DrawGameplay()
        {
            //draws gameplay
            gameObject.DrawGameplay();
        }

        //pre: none
        //post: none
        //description: draws the menu screen
        private void DrawMenu()
        {
            //draws the tank background
            spriteBatch.Draw(tankBackImg, tankBackRec, Color.White);

            //draws the game title image
            spriteBatch.Draw(tanksTitleImg, tanksTitleShadRec, Color.Black * 0.8f);
            spriteBatch.Draw(tanksTitleImg, tanksTitleRec, Color.White);

            //loops through the menu button rectangles
            for (int i = 0; i < menuButRecs.Length; i++)
            {
                //draws the button at index i
                spriteBatch.Draw(buttonImg, menuButRecs[i], Color.White);
                spriteBatch.DrawString(menuFont, menuOptions[i], menuOptionShadLocs[i], Color.Black * 0.5f);
                spriteBatch.DrawString(menuFont, menuOptions[i], menuOptionLocs[i], menuOptionCol[Convert.ToInt32(isMenuButsHover[i])]);
            }
        }

        //pre: none
        //post: none
        //description: draws the instructions screen
        private void DrawInstructions()
        {
            //draws the tank background image
            spriteBatch.Draw(tankBackImg, tankBackRec, Color.White);

            //draws the instructions screen image
            spriteBatch.Draw(instructImg, instructRec, Color.White);

            //draws the button
            spriteBatch.Draw(buttonImg, instructBtnRec, Color.White);
            spriteBatch.DrawString(menuFont, instrucButPrompt, instrucButPropmtShadLoc, Color.Black * 0.5f);
            spriteBatch.DrawString(menuFont, instrucButPrompt, instrucButPropmtLoc, menuOptionCol[Convert.ToInt32(isInstrucButHover)]);
        }

        //pre: none
        //post: none
        //description: draws the level select screen
        private void DrawLevelSelect()
        {
            //draws the tank image background
            spriteBatch.Draw(tankBackImg, tankBackRec, Color.White);

            //draws the level select title image
            spriteBatch.Draw(levelSelImg, levelSelShadRec, Color.Black * 0.8f);
            spriteBatch.Draw(levelSelImg, levelSelRec, Color.White);

            //loops through the level button rectangles
            for (int i = 0; i < levButRecs.Length; i++)
            {
                //draws the level buttons
                spriteBatch.Draw(levelButImg, levButRecs[i], Color.White);
            }

            //loops through all levels up to the first locked one
            for (int i = 0; i < gameObject.GetFirstLevLocked(); i++)
            {
                //draws the level number
                spriteBatch.DrawString(levelNumFont, Convert.ToString(i + 1), levelNumShadPos[i], Color.Black * 0.5f);
                spriteBatch.DrawString(levelNumFont, Convert.ToString(i + 1), levelNumPos[i], menuOptionCol[Convert.ToInt32(isLevelSelHover[i])]);
            }

            //loops through all rectangles including and passed the first locked one
            for (int i = gameObject.GetFirstLevLocked(); i < NUM_LEV; i++)
            {
                //draws a faded square and lock over it
                spriteBatch.Draw(pixelImg, levButRecs[i], Color.Black * 0.5f);
                spriteBatch.Draw(lockImg, lockRec[i], Color.White * 0.5f);
            }

            //draws the star images
            gameObject.DrawLevelSelStar();

            //draws the back button
            spriteBatch.Draw(buttonImg, levelSelButRec, Color.White);
            spriteBatch.DrawString(menuFont, levelSelButPrompt, levelSelButShadPromptLoc, Color.Black * 0.5f);
            spriteBatch.DrawString(menuFont, levelSelButPrompt, levelSelButPromptLoc, menuOptionCol[Convert.ToInt32(islevelSelButHover)]);
        }

        //pre: none
        //post: none
        //description: draws the endgame screen
        private void DrawEndgame()
        {
            //draws gameplay
            gameObject.DrawGameplay();

            //draws faded square over screen
            spriteBatch.Draw(pixelImg, fullScreenRec, Color.Black * 0.5f);

            //draw the endgame screen
            gameObject.DrawEndgame();
        }

        //pre: none
        //post: none
        //desctipion: draws the play stats
        private void DrawPlayStats()
        {
            //draw tank background
            spriteBatch.Draw(tankBackImg, tankBackRec, Color.White);

            //draws the play stast
            gameObject.DrawPlayStats();
        }

        //pre: none
        //post: none
        //description: draw the level info screen
        private void DrawLevelInfo()
        {
            //draws teh tank background
            spriteBatch.Draw(tankBackImg, tankBackRec, Color.White);

            //draws the level info
            gameObject.DrawLevelInfo();
        }

        //pre: none
        //post: none
        //description: draws the file error screen
        private void DrawFileError()
        {
            //draws the tank background image
            spriteBatch.Draw(tankBackImg, tankBackRec, Color.White);

            //draws the error screen
            gameObject.DrawErrorScreen();
        }

        //pre: none
        //post: none
        //description: updates the tank background
        private void UpdateTankBackground()
        {
            //updates the background locations
            tankBackPos.X += BACK_HOR_MOVE;
            tankBackPos.Y += BACK_VERT_MOVE;
            tankBackRec.X = (int)tankBackPos.X;
            tankBackRec.Y = (int)tankBackPos.Y;

            //checks if the image is farther than max limit
            if (tankBackRec.X <= NEG_HOR_LIM || tankBackRec.X <= NEG_VERT_LIM)
            {
                //resets position of tank back image
                tankBackPos.X = 0;
                tankBackPos.Y = 0;
                tankBackRec.X = (int)tankBackPos.X;
                tankBackRec.Y = (int)tankBackPos.Y;
            }
        }
    }
}