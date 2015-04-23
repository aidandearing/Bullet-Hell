using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class Camera
    {
        public enum Focus { None, Player1, Player2, Player3, Player4 };
        /// <summary>
        /// The padding around the players, gaurantees they stay on screen so long as they do not go beyond the Maximum zoom
        /// </summary>
        private const int EDGE_PADDING = 4;
        /// <summary>
        /// The elasticity of the zoom, the higher the value the longer it takes to zoom to the new value, exponentially
        /// Default: 60
        /// </summary>
        private int ZOOM_ELASTICITY = 60;
        /// <summary>
        /// The elasticity of panning, the higher the value the longer it takes to pan to the desired camera position, exponentially
        /// Default: 15
        /// </summary>
        private int PAN_ELASTICITY = 5;

        private static Camera instance { get; set; }
        public static Rectangle Screen { get; set; }
        public static float ZoomMin { get; set; }
        public static float ZoomMax { get; set; }
        //Centered Position of camera in pixels
        public Vector2 Position { get; private set; }
        //Current Zoom level with 1.0f being standard
        public float Zoom { get; private set; }
        //Current Rotation amount with 0.0f being standard orientation
        public float Rotation { get; private set; }
        /*Height and Width of the viewport window which we need to adjust
        anytime the player resizes the game window*/
        public static int ViewportWidth { get; set; }
        public static int ViewportHeight { get; set; }
        //Center of the viewport which does not account
        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }

        private Camera()
        {
            ZoomMin = 0.25f;
            ZoomMax = 1f;
        }

        public static Camera Instance()
        {
            if (instance == null)
                instance = new Camera();
            return instance;
        }
        
        /*Create a matrix for the camera to offset everything we draw,
        the map and our objects. since the camera coordinates are where
        the camera is, we offset everything by the negative of that to simulate
        a camera moving. We also cast to integers to avoid filtering artifacts.
        */
        public Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                /* Matrix.CreateRotationZ(Rotation) * */
                Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }
        public void Update(List<Player> players, Focus focus)
        {
            Vector2 position = new Vector2();
            Vector2 dCP1 = new Vector2();
            Vector2 dCP2 = new Vector2();
            Vector2 dCP3 = new Vector2();
            Vector2 dCP4 = new Vector2();
            Vector2 max = new Vector2();
            float zoom = 1.0f;

            // Get the average position between all players
            if (focus != Focus.None)
            {
                switch (focus)
                {
                    case Focus.Player1:
                        position = players[0].Position;
                        break;
                    case Focus.Player2:
                        position = players[1].Position;
                        break;
                    case Focus.Player3:
                        position = players[2].Position;
                        break;
                    case Focus.Player4:
                        position = players[3].Position;
                        break;
                    default:
                        position = players[0].Position;
                        break;
                }
            }
            else
            {
                switch (players.Count)
                {
                    case 1:
                        position = players[0].Position;
                        
                        break;
                    case 2:
                        position = (players[0].Position + players[1].Position) / 2;
                        dCP1 = position - players[0].Position;
                        dCP2 = position - players[1].Position;

                        max = new Vector2();

                        if (dCP1.Length() > dCP2.Length())
                            max = dCP1;
                        else
                            max = dCP2;
                        break;
                    case 3:
                        position = (players[0].Position + players[1].Position + players[2].Position) / 3;
                        dCP1 = position - players[0].Position;
                        dCP2 = position - players[1].Position;
                        dCP3 = position - players[2].Position;

                        max = new Vector2();

                        if (dCP1.Length() > dCP2.Length())
                            max = dCP1;
                        else
                            max = dCP2;
                        if (max.Length() < dCP3.Length())
                            max = dCP3;
                        break;
                    case 4:
                        position = (players[0].Position + players[1].Position + players[2].Position + players[3].Position) / 4;
                        dCP1 = position - players[0].Position;
                        dCP2 = position - players[1].Position;
                        dCP3 = position - players[2].Position;
                        dCP4 = position - players[3].Position;

                        max = new Vector2();

                        if (dCP1.Length() > dCP2.Length())
                            max = dCP1;
                        else
                            max = dCP2;
                        if (max.Length() < dCP3.Length())
                            max = dCP3;
                        if (max.Length() < dCP4.Length())
                            max = dCP4;
                        break;
                    default:
                        position = players[0].Position;
                        break;
                }
            }

            max *= EDGE_PADDING;
            Vector2 viewport = new Vector2(ViewportWidth, ViewportHeight);
            max = viewport / max;

            float zoomtemp = MathHelper.Min(Math.Abs(max.X),Math.Abs(max.Y));

            zoom = MathHelper.Clamp(zoomtemp, ZoomMin, ZoomMax);
            zoom = (float)Math.Round(zoom / 0.125f) * 0.125f;

            Position = Position + (position - Position) / PAN_ELASTICITY;
            Position = new Vector2((float)Math.Round((float)Position.X), (float)Math.Round((float)Position.Y));

            Zoom = Zoom + (zoom - Zoom) / ZOOM_ELASTICITY;

            int x = (int)(Position.X - ViewportWidth / Zoom / 2);
            int y = (int)(Position.Y - ViewportHeight / Zoom / 2);
            int w = (int)(ViewportWidth / Zoom * 1.5f);
            int h = (int)(ViewportHeight / Zoom * 1.5f);
            Camera.Screen = new Rectangle(x,y,w,h);
        }
    }
}
