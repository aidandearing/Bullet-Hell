using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class Particle : StaticObject
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public StaticObject Parent { get; set; }
        public float Lifetime { get; set; }
        private float LifetimeEllapsed { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 VelocityEnd { get; set; }
        public float PositionInherited { get; set; }
        public float RotationVelocity { get; set; }
        public float RotationVelocityEnd { get; set; }
        public float RotationInherited { get; set; }
        public Color ColourEnd { get; set; }
        public Vector2 ScaleEnd { get; set; }
        public float OpacityEnd { get; set; }

        /// <summary>
        /// Particle constructor, taking parameters
        /// </summary>
        /// <param name="key">The string key of the texture in Particle.Textures dictionary</param>
        /// <param name="lifetime">The lifetime in seconds the particle is alive for</param>
        /// <param name="velocity">The velocity of the particles at its start</param>
        /// <param name="velocityEnd">The velocity of the particles at its end</param>
        /// <param name="velocityInherited">The velocity inherited from the parent per frame</param>
        /// <param name="rotationVelocity">The rotational velocity in radians at its start</param>
        /// <param name="rotationVelocityEnd">The rotational velocity in radians at its end</param>
        /// <param name="rotationInherited">The rotation inherited from the parent per frame </param>
        /// <param name="colour">The colour of the particle at its start</param>
        /// <param name="colourEnd">The colour of the particle at its end</param>
        /// <param name="scale">The scale of the particle at its start</param>
        /// <param name="scaleEnd">The scale of the particle at its end</param>
        /// <param name="opacity">The opacity of the particle at its start</param>
        /// <param name="opacityEnd">The opacity of the particle at its end</param>
        public Particle(String key, StaticObject Parent, float lifetime ,Vector2 velocity, Vector2 velocityEnd, float positionInherited, float rotationVelocity, float rotationVelocityEnd, float rotationInherited, Color colour, Color colourEnd, Vector2 scale, Vector2 scaleEnd, float opacity, float opacityEnd) : base(Textures[key])
        {
            this.Lifetime = lifetime;
            this.Velocity = velocity;
            this.VelocityEnd = velocityEnd;
            this.PositionInherited = positionInherited;
            this.RotationVelocity = rotationVelocity;
            this.RotationVelocityEnd = rotationVelocityEnd;
            this.RotationInherited = rotationInherited;
            this.Colour = colour;
            this.ColourEnd = colourEnd;
            this.Scale = scale;
            this.ScaleEnd = scaleEnd;
            this.Opacity = opacity;
            this.OpacityEnd = opacityEnd;
            this.Depth = 0.4f + (float)Game1.random.NextDouble() * 0.1f;
        }

        public Particle(Particle particle) : base(particle.Texture)
        {
            this.Parent = particle.Parent;
            this.Lifetime = particle.Lifetime;
            this.Velocity = particle.Velocity;
            this.VelocityEnd = particle.VelocityEnd;
            this.PositionInherited = particle.PositionInherited;
            this.RotationVelocity = particle.RotationVelocity;
            this.RotationVelocityEnd = particle.RotationVelocityEnd;
            this.RotationInherited = particle.RotationInherited;
            this.Scale = particle.Scale;
            this.ScaleEnd = particle.ScaleEnd;
            this.Colour = particle.Colour;
            this.ColourEnd = particle.ColourEnd;
            this.Depth = particle.Depth;
            this.Opacity = particle.Opacity;
            this.OpacityEnd = particle.OpacityEnd;
        }

        public void Update(GameTime gameTime)
        {
            float timeLapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            LifetimeEllapsed += timeLapse;

            // Calculate the percentage of life left
            float lifeLeft = LifetimeEllapsed / Lifetime;

            Velocity = new Vector2(MathHelper.Lerp(Velocity.X, VelocityEnd.X, lifeLeft), MathHelper.Lerp(Velocity.Y, VelocityEnd.Y, lifeLeft));
            Opacity = MathHelper.Lerp(Opacity, OpacityEnd, lifeLeft);
            RotationVelocity = MathHelper.Lerp(RotationVelocity, RotationVelocityEnd, lifeLeft);
            Scale = new Vector2(MathHelper.Lerp(Scale.X, ScaleEnd.X, lifeLeft),MathHelper.Lerp(Scale.Y, ScaleEnd.Y, lifeLeft));

            int r = (int)(MathHelper.Lerp(Colour.R, ColourEnd.R, lifeLeft));
            int g = (int)(MathHelper.Lerp(Colour.G, ColourEnd.G, lifeLeft));
            int b = (int)(MathHelper.Lerp(Colour.B, ColourEnd.B, lifeLeft));
            Colour = new Color(r, g, b);

            if (Parent is DynamicObject)
                Position = new Vector2(MathHelper.Lerp(Position.X, Parent.Position.X, PositionInherited),MathHelper.Lerp(Position.Y, Parent.Position.Y, PositionInherited)); 
            Position += Velocity;
            Rotation += RotationVelocity + Parent.Rotation * RotationInherited;

            if (LifetimeEllapsed > Lifetime)
                ParticleManager.Instance().RemoveParticle(this);
        }

        new public static void LoadContent(ContentManager Content)
        {
            Textures.Add("spark",Content.Load<Texture2D>("Textures/Particles/spark"));
        }
    }
}
