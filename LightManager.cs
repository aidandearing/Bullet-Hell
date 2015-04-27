using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class LightManager
    {
        List<LightSource> lights = new List<LightSource>();
        List<LightSource> deadLights = new List<LightSource>();

        LightSource globalLight;
        LightSource[] playerLights = new LightSource[4];

        RenderTarget2D renderTarget;
        RenderTarget2D normalRender;

        public float Brightness { get; set; }
        private static LightManager instance { get; set; }

        private LightManager()
        {
            Brightness = 0f;
        }

        public static LightManager Instance()
        {
            if (instance == null)
                instance = new LightManager();
            return instance;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            renderTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            normalRender = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }

        public void Update()
        {
            ObjectManager objectManager = ObjectManager.Instance();

            if (globalLight == null)
                globalLight = new LightSource(LightSource.Light.circlesharp, 1, 1, Color.AntiqueWhite, null);

            foreach (LightSource light in lights)
                light.Update();

            foreach (LightSource light in deadLights)
                lights.Remove(light);

            globalLight.Position = Camera.Instance().Position - Camera.Instance().ViewportCenter;
            globalLight.Intensity = Brightness;
            globalLight.Range = 30 / Camera.Instance().Zoom;
            globalLight.Update();

            for (int i = 0; i < objectManager.players.Count; i++)
            {
                if (playerLights[i] == null)
                    playerLights[i] = new LightSource(LightSource.Light.circle, 1, 30, Color.Orange, objectManager.players[i]);
                playerLights[i].Intensity = 0.5f - Brightness / 2;
                playerLights[i].Update();
            }

            deadLights = new List<LightSource>();
        }

        public void Begin(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(normalRender);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(renderTarget);

            Color background = Color.Black * (1 - Brightness);
            graphicsDevice.Clear(background);

            BlendState overlay = new BlendState();
            overlay.ColorSourceBlend = Blend.DestinationColor;
            overlay.ColorDestinationBlend = Blend.SourceColor;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, null, null, null, Camera.Instance().TranslationMatrix);
            foreach (LightSource light in lights)
                light.Draw(spriteBatch);
            globalLight.Draw(spriteBatch);
            foreach (LightSource light in playerLights)
                if (light != null)
                    light.Draw(spriteBatch);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw(normalRender, normalRender.Bounds, Color.White);
            spriteBatch.End();
            
            spriteBatch.Begin(SpriteSortMode.Immediate, overlay, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, null);
            spriteBatch.Draw(renderTarget, renderTarget.Bounds, Color.White);
            spriteBatch.End();
        }

        public void AddLight(LightSource light)
        {
            lights.Add(light);
        }

        public void RemoveLight(LightSource light)
        {
            deadLights.Add(light);
        }
    }
}
