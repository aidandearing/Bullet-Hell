using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ActionGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        WorldSpace worldSpace;
        ProjectileManager projectileManager;
        ObjectManager objectManager;
        ParticleManager particleManager;
        LightManager lightManager;
        SoundManager soundManager;
        public static Random random;

        UIHealthBar[] healthBars = new UIHealthBar[3];

        #region Shaders
        ShaderBloom shaderBloom;
        #endregion

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";

            shaderBloom = new ShaderBloom(this);
            shaderBloom.Settings = BloomSettings.PresetSettings[0];
            shaderBloom.ShowBuffer = ShaderBloom.IntermediateBuffer.FinalResult;

            Components.Add(shaderBloom);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Camera.Screen = GraphicsDevice.Viewport.Bounds;

            lightManager = LightManager.Instance();
            lightManager.Initialize(GraphicsDevice);

            random = new Random();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            worldSpace = WorldSpace.Instance();
            projectileManager = ProjectileManager.Instance();
            objectManager = ObjectManager.Instance();
            particleManager = ParticleManager.Instance();
            soundManager = SoundManager.Instance();
            camera = Camera.Instance();
            Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
            Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

            // Object Dictionaries
            #region Object Dictionaries
            Dictionary<string, XmlDocument> dictionary_ability_xml = new Dictionary<string, XmlDocument>();
            Dictionary<string, Ability> dictionary_ability = new Dictionary<string, Ability>();
            #endregion

            // TODO: use this.Content to load your game content here
            SoundManager.LoadContent(Content);
            StaticObject.LoadContent(Content);
            Particle.LoadContent(Content);
            LightSource.LoadContent(Content);
            Scene.LoadContent(Content);
            Projectile.LoadContent(Content);
            Ability.LoadContent(Content);
            WorldSpace.LoadContent(Content);
            Entity.LoadContent(Content);

            #region Debug and Tests
            Player player = new Player("wizard", PlayerIndex.One);
            player.TextureOrigin = new Vector2(8, 8);
            player.Scale = new Vector2(8, 8);

            Animation animation = new Animation();
            animation.Sheet = player.Texture;
            animation.Frame = new Rectangle(0, 0, 16, 16);
            animation.Name = "idle";
            animation.FramesAcross = 0;
            player.AddAnimation(animation);

            animation = new Animation();
            animation.Sheet = player.Texture;
            animation.Frame = new Rectangle(0, 0, 16, 16);
            animation.Name = "walking";
            animation.FramesAcross = 3;
            player.AddAnimation(animation);

            animation = new Animation();
            animation.Sheet = player.Texture;
            animation.Frame = new Rectangle(0, 48, 16, 16);
            animation.Name = "dead";
            animation.FramesAcross = 3;
            player.AddAnimation(animation);

            Player player2 = new Player("paladin", PlayerIndex.Two);
            player2.TextureOrigin = new Vector2(8, 8);
            player2.Scale = new Vector2(8, 8);

            Animation animation2 = new Animation();
            animation2.Sheet = player2.Texture;
            animation2.Frame = new Rectangle(0, 0, 16, 16);
            animation2.Name = "idle";
            animation2.FramesAcross = 0;
            player2.AddAnimation(animation2);

            animation2 = new Animation();
            animation2.Sheet = player2.Texture;
            animation2.Frame = new Rectangle(0, 0, 16, 16);
            animation2.Name = "walking";
            animation2.FramesAcross = 3;
            player2.AddAnimation(animation2);

            animation2 = new Animation();
            animation2.Sheet = player2.Texture;
            animation2.Frame = new Rectangle(48, 48, 16, 16);
            animation2.Name = "dead";
            animation2.FramesAcross = 0;
            player2.AddAnimation(animation2);

            Player player3 = new Player("archer", PlayerIndex.Three);
            player3.TextureOrigin = new Vector2(8, 8);
            player3.Scale = new Vector2(8, 8);

            Animation animation3 = new Animation();
            animation3.Sheet = player3.Texture;
            animation3.Frame = new Rectangle(0, 0, 16, 16);
            animation3.Name = "idle";
            animation3.FramesAcross = 0;
            player3.AddAnimation(animation3);

            animation3 = new Animation();
            animation3.Sheet = player3.Texture;
            animation3.Frame = new Rectangle(0, 0, 16, 16);
            animation3.Name = "walking";
            animation3.FramesAcross = 3;
            player3.AddAnimation(animation3);

            animation3 = new Animation();
            animation3.Sheet = player3.Texture;
            animation3.Frame = new Rectangle(48, 48, 16, 16);
            animation3.Name = "dead";
            animation3.FramesAcross = 0;
            player3.AddAnimation(animation3);

            player.abilityManager.AddNewAbility("Firebolt");
            player.abilityManager.AddNewAbility("Dash");
            player.abilityManager.AddNewAbility("Combust");
            player.abilityManager.AddNewAbility("Fireball");
            player.abilityManager.AddNewAbility("Innervate");
            player.abilityManager.AddNewAbility("Fire Burst");

            player2.abilityManager.AddNewAbility("Slash");
            player2.abilityManager.AddNewAbility("Heal");
            player2.abilityManager.AddNewAbility("Consecrated Ground");
            player2.abilityManager.AddNewAbility("Stronghold");
            player2.abilityManager.AddNewAbility("Divine Fury");
            player2.abilityManager.AddNewAbility("Gods Wrath");

            player3.abilityManager.AddNewAbility("Shoot");
            player3.abilityManager.AddNewAbility("Shadow Step");
            player3.abilityManager.AddNewAbility("Lightning Arrow");
            player3.abilityManager.AddNewAbility("Rain of Vengeance");
            player3.abilityManager.AddNewAbility("Bola Strike");
            player3.abilityManager.AddNewAbility("Cluster Arrow");

            player.Position = Vector2.Zero;
            player2.Position = new Vector2(0,128);
            player2.Faction = FactionManager.Faction.Bandit;
            player3.Position = new Vector2(128, 0);
            player3.Faction = FactionManager.Faction.Goblin;
            //player.Speed = WorldSpace.MetresToPixels(5);

            //Particle particle = new Particle("spark", player, 1f, Vector2.Zero, Vector2.Zero, 0, 0, 0, 0, Color.LightYellow, Color.White, new Vector2(32, 32), new Vector2(32, 32), 1, 0);
            //ParticleEmitter emitter = new ParticleEmitter(player.Position, particle, 100, 0, 60, player, 1, false);
            //particleManager.AddEmitter(emitter);
            objectManager.AddPlayer(player);
            objectManager.AddPlayer(player2);
            objectManager.AddPlayer(player3);

            //light = new LightSource(LightSource.Light.circle, 1f, 5f, Color.White, player);
            //lightManager.AddLight(light);

            healthBars[0] = new UIHealthBar(Scene.Textures["textures_smooth1"], new Rectangle(), Color.Red, player);
            healthBars[1] = new UIHealthBar(Scene.Textures["textures_smooth1"], new Rectangle(), Color.Red, player2);
            healthBars[2] = new UIHealthBar(Scene.Textures["textures_smooth1"], new Rectangle(), Color.Red, player3);
            #endregion
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            worldSpace.Update(gameTime);
            objectManager.Update(gameTime);
            camera.Update(objectManager.players, Camera.Focus.None);
            objectManager.Update(gameTime);
            particleManager.Update(gameTime);
            lightManager.Update();

            foreach (UIHealthBar healthbar in healthBars)
                healthbar.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //shaderBloom.BeginDraw();
            lightManager.Begin(spriteBatch, GraphicsDevice);

            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.Instance().TranslationMatrix);
            projectileManager.Draw(spriteBatch);
            objectManager.Draw(spriteBatch);
            particleManager.Draw(spriteBatch);
            spriteBatch.End();

            lightManager.Draw(spriteBatch, GraphicsDevice);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Camera.Instance().TranslationMatrix);
            foreach (UIHealthBar healthbar in healthBars)
                healthbar.Draw(spriteBatch);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
