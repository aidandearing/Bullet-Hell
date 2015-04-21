using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    class Animation
    {
        public Texture2D Sheet { get; set; }
        public Rectangle Frame { get; set; }
        public Vector2 FramePos { get; set; }
        public int TimeBetweenFrames { get; set; }
        public int TimeElapsed { get; set; }
        public string Name { get; set; }
        public int FramesAcross { get; set; }

        public void Update(GameTime gameTime)
        {
            int timeLapse = gameTime.ElapsedGameTime.Milliseconds;
            TimeElapsed += timeLapse;

            if (TimeElapsed > TimeBetweenFrames)
            {
                TimeElapsed = 0;

                FramePos = new Vector2(FramePos.X + 1, FramePos.Y);
                
                if (FramePos.X > FramesAcross)
                {
                    FramePos = new Vector2(0, FramePos.Y);
                }

                Frame = new Rectangle((int)FramePos.X * Frame.Width, (int)FramePos.Y * Frame.Height, Frame.Width, Frame.Height);
            }
        }
        public void Draw(SpriteBatch spriteBatch, AnimatedObject parent)
        {
            spriteBatch.Draw(Sheet, parent.Position, Frame, parent.Colour, parent.Rotation, parent.TextureOrigin, 
                parent.Scale, parent.SpriteEffect, parent.Depth);
        }
    }
}
