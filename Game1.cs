using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml;
using System.Collections.Generic;

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
        public static Random random;

        #region Shaders
        //ShaderBloom shaderBloom;
        #endregion

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            //shaderBloom = new ShaderBloom(this);

            //Components.Add(shaderBloom);
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
            camera = Camera.Instance();
            Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
            Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;
            Camera.ZoomMin = Camera.ViewportHeight / (float)ObjectManager.COLLISIONBOUNDS;
            Console.WriteLine(Camera.ZoomMin);

            // Object Dictionaries
            #region Object Dictionaries
            Dictionary<string, XmlDocument> dictionary_ability_xml = new Dictionary<string, XmlDocument>();
            Dictionary<string, Ability> dictionary_ability = new Dictionary<string, Ability>();
            #endregion

            // TODO: use this.Content to load your game content here
            StaticObject.LoadContent(Content);
            Scene.LoadContent(Content);
            Projectile.LoadContent(Content);
            Ability.LoadContent(Content);
            WorldSpace.LoadContent(Content);
            Entity.LoadContent(Content);

            #region Debug and Tests

            Player player = new Player("wizard", PlayerIndex.One);
            player.TextureOrigin = new Vector2(8, 8);
            player.Scale = new Vector2(4, 4);

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

            player.abilityManager.AddNewAbility("Root");
            player.Position = Vector2.Zero;
            //player.Velocity = new Vector2(0, -WorldSpace.CHUNKSIZE/16);
            objectManager.AddPlayer(player);
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

            //players[0].abilityManager.UseAbility(0);

            objectManager.Update(gameTime);

            camera.Update(objectManager.players, Camera.Focus.None);
            objectManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //shaderBloom.BeginDraw();
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.FrontToBack,BlendState.NonPremultiplied,SamplerState.PointClamp,DepthStencilState.Default,RasterizerState.CullNone,null,Camera.Instance().TranslationMatrix);
            projectileManager.Draw(spriteBatch);
            objectManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
