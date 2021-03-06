using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System;
using Helper;
using control;
using XNAnimation;

namespace MyGame
{
    public class Game1 : Microsoft.Xna.Framework.Game, IEvent
    {
        List<Event> events;

        GraphicsDeviceManager graphics;

        public DifficultyConstants difficultyConstants;
        public Camera camera;
        public Controller controller;
        public Mediator mediator;
        public Player player;
        public bool paused = false;
        public bool gameOver = false;

        public CameraMode cameraMode = CameraMode.thirdPerson;

        public enum CameraMode
        {
            thirdPerson = 0,
            firstPersonWithWeapon ,
            firstPersonWithoutWeapon ,
        }


        private Sky sky;
        private FirstAidManager firstAidManger;
        private Terrain terrain;
        private MonstersManager monsters;
        private DelayedAction delayedAction;
        private DelayedAction delayedAction2;
        private ScoreBoard scoreBoard;
        private Weapon weapon;
        private BulletsManager bullets;
        private StateManager stateManager;
        private AudioManager audioManager;
        //assal

        // Shot variables
        //int keyDelay = 800;
        //int keyCountdown = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            controller = new Controller(Constants.LEFT_HAND);

            //DONT Remove i need this.--Mahmoud Bahaa
            if (System.IO.File.Exists("fbDeprofiler.dll"))
                fbDeprofiler.DeProfiler.Run();

            Window.AllowUserResizing = true;

            mediator = new Mediator();
            events = new List<Event>();
            delayedAction = new DelayedAction(800);
            delayedAction2 = new DelayedAction();
            mediator.register(this, MyEvent.G_StartGame, MyEvent.G_StartScreen, MyEvent.G_HelpScreen, MyEvent.G_Exit);
            //mediator.fireEvent(MyEvent.G_StartGame);
        }

        private Player initializePlayer()
        {
            SkinnedModel pmodel = Content.Load<SkinnedModel>(@"model/PlayerMarine");
            //SkinningData skinnedData = pmodel.Tag as SkinningData;
            PlayerUnit playerUnit = new PlayerUnit(this, new Vector3(0, terrain.GetHeightAtPosition(0, 0) + 5, 0),
                new Vector3(0, 0, 0),
                Constants.PLAYER_SCALE);
            Player player = new Player(this, pmodel,playerUnit);
            return player;
        }

        private Sky intitializeSky()
        {
            TextureCube tc = Content.Load<TextureCube>("clouds");
            Model pmodel = Content.Load<Model>("skysphere_mesh");
            SkyUnit skyUnit = new SkyUnit(this, Vector3.Zero, Vector3.Zero, new Vector3(10000));
            Sky sky = new Sky(this, pmodel, skyUnit, tc);

            return sky;
        }

        private void initializeGame2()
        {
            Components.Clear();

            Components.Add(camera);
            Components.Add(sky);
            Components.Add(terrain);
            Components.Add(monsters);
            Components.Add(firstAidManger);
            Components.Add(bullets);
            Components.Add(weapon);
            //Components.Add(test);
            Components.Add(player);
            Components.Add(scoreBoard);
            Components.Add(stateManager);
            Components.Add(audioManager);
        }

        private void initializeGame1()
        {
            switch (StartScreen.Difficulty)
            {
                case Constants.Difficulties.Novice: difficultyConstants = new NoviceConstants(); break ;
                case Constants.Difficulties.Advanced: difficultyConstants = new AdvancedConstants(); break;
                case Constants.Difficulties.Xtreme: difficultyConstants = new XtremeConstants(); break;

            }
            //camera = new FreeCamera(this, new Vector3(0, 0, 0), 0, 0, 0 , 0);
            //camera = new FreeCamera(new Vector3(400, 600, 400), MathHelper.ToRadians(45), MathHelper.ToRadians(-30), GraphicsDevice);
            camera = new ChaseCamera(this, Constants.CAMERA_POSITION, Constants.CAMERA_TARGET, Vector3.Zero);
            terrain = new Terrain(this, camera, Content.Load<Texture2D>("terrain"), Constants.TERRAIN_CELL_SIZE,
                Constants.TERRAIN_HEIGHT, Content.Load<Texture2D>("grass"), Constants.TERRAIN_TEXTURE_TILING, new Vector3(1, -1, 0));

            player = initializePlayer();
            sky = intitializeSky();


            
            weapon = new Weapon(this, player, Content.Load<Model>("model//WeaponMachineGun"),
                new Unit(this, Vector3.Zero, Vector3.Zero, Vector3.One));
            bullets = new BulletsManager(this);
            scoreBoard = new ScoreBoard(this);
            monsters = new MonstersManager(this);
           firstAidManger = new FirstAidManager(this);

            stateManager = new StateManager(this);
            audioManager = new AudioManager(this);

            //CDrawableComponent test = new CDrawableComponent(this,
            //    new Unit(this, new Vector3(0, 80, 0), Vector3.Zero, Vector3.One * .5f),
            //    new CModel(this, Content.Load<Model>(@"model/First Aid Kit2")));

           
        }


        private void initializeStartMenu()
        {
            Components.Clear();
            StartScreen startScreen = new StartScreen(this);

            Components.Add(startScreen);
        }

        private void initializeHelpScreen()
        {
            Components.Clear();
            HelpScreen helpScreen = new HelpScreen(this);

            Components.Add(helpScreen);
        }

        // Called when the game should load its content
        protected override void LoadContent()
        {
            initializeStartMenu();
            initializeGame1();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            foreach (Event ev in events)
            {
                switch (ev.EventId)
                {
                    case (int)MyEvent.G_Exit: Exit(); break;
                    case (int)MyEvent.G_StartGame: initializeGame2(); break;
                    case (int)MyEvent.G_StartScreen: initializeStartMenu(); break;
                    case (int)MyEvent.G_HelpScreen: initializeHelpScreen(); break;
                }
            }
            events.Clear();

            if (delayedAction.eventHappened(gameTime, keyState.IsKeyDown(Keys.RightAlt) &&
                                                    keyState.IsKeyDown(Keys.Enter)))
            {
                graphics.ToggleFullScreen();
            }

            if (delayedAction2.eventHappened(gameTime, keyState,Keys.C))
            {
                if ((int)cameraMode == 2)
                    cameraMode = CameraMode.thirdPerson;
                else
                    cameraMode++;
                
            }

            base.Update(gameTime);
        }

        public void addEvent(Helper.Event ev)
        {
            events.Add(ev);
        }

        public bool checkCollisionWithBullet(Unit unit)
        {
            return (monsters.checkCollisionWithBullet(unit)||firstAidManger.checkCollisionWithBullet(unit));
        }

        protected override void EndRun()
        {
            GestureManager.running = false;
            base.EndRun();
        }

        public float GetHeightAtPosition(float X, float Z)
        {
            return terrain.GetHeightAtPosition(X, Z);
        }

        public float GetHeightAtPosition2(float X, float Z)
        {
            float steepness;
            return terrain.GetHeightAtPosition(X, Z, out steepness);
        }
    }
}
