using Microsoft.Xna.Framework;
using System;

namespace ActionGame
{
    class ParticleEmitter
    {
        public Particle Particle { get; set; }
        public float Lifetime { get; set; }
        private float LifetimeElapsed { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 PositionOrigin { get; set; }
        public float EmitRadius { get; set; }
        public float EmitNumber { get; set; }
        /// <summary>
        /// Ensures that for emitNumbers that would return a value less than 0.5 they still emit
        /// </summary>
        private float emittedThisSecond { get; set; }
        private float secondTimer { get; set; }
        private float emitTimer { get; set; }
        public StaticObject Parent { get; set; }
        /// <summary>
        /// How much of the parents position the emitter inherits each update
        /// 0.0f = 0%
        /// 1.0f = 100%
        /// </summary>
        public float PositionInheritance { get; set; }
        /// <summary>
        /// Does PositionInheritance chase the Parent, or get left behind
        /// Does the position of the emitter chase the position of the Parent by the PositionInheritance value, or does it get left behind
        /// </summary>
        public bool PositionInheritanceElastic { get; set; }

        /// <summary>
        /// ParticleEmitter constructor
        /// </summary>
        /// <param name="position">The position of the emitter at its start</param>
        /// <param name="emitRadius">The radius in metres that the particles can emit from around the emitter position</param>
        /// <param name="parent">The parent of the emitter, can be any simulated object</param>
        /// <param name="positionInheritance">How much of the parents position the emitter inherits each update (0.0f = 0% | 1.0f = 100%)</param>
        /// <param name="positionInheritanceElastic">Does the position of the emitter chase the position of the Parent by the PositionInheritance value, or does it get left behind</param>
        public ParticleEmitter(Vector2 position, Particle particle, float lifetime, float emitRadius, float emitNumber, StaticObject parent, float positionInheritance, bool positionInheritanceElastic)
        {
            this.Lifetime = lifetime;
            this.Particle = particle;
            this.Position = position;
            this.PositionOrigin = position;
            this.EmitRadius = emitRadius;
            this.EmitNumber = MathHelper.Clamp(emitNumber, 1, 60);
            this.Particle.Parent = parent;
            this.Parent = parent;
            this.PositionInheritance = MathHelper.Clamp(positionInheritance,0,1);
            this.PositionInheritanceElastic = positionInheritanceElastic;
        }

        public ParticleEmitter(ParticleEmitter emitter)
        {
            this.Lifetime = emitter.Lifetime;
            this.Particle = emitter.Particle;
            this.Position = emitter.Position;
            this.PositionOrigin = emitter.Position;
            this.EmitRadius = emitter.EmitRadius;
            this.EmitNumber = emitter.EmitNumber;
            this.Parent = emitter.Parent;
            this.PositionInheritance = emitter.PositionInheritance;
            this.PositionInheritanceElastic = emitter.PositionInheritanceElastic;
        }

        public void Update(GameTime gameTime)
        {
            float timeLapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            secondTimer += timeLapse;

            if (PositionInheritance > 0)
            {
                if (PositionInheritanceElastic)
                    Position += Parent.Position * PositionInheritance;
                else
                    Position = Position + (Parent.Position - Position) * PositionInheritance;
            }

            if (Lifetime > 0)
            {
                LifetimeElapsed += timeLapse;
                if (LifetimeElapsed > Lifetime)
                    ParticleManager.Instance().RemoveEmitter(this);
            }

            emitTimer += timeLapse;
            float emitTimerGoal = 1 / EmitNumber;

            if (emitTimer > emitTimerGoal && emittedThisSecond < EmitNumber)
            {
                Particle particle = new Particle(Particle);
                float num = (float)Game1.random.NextDouble() * EmitRadius;
                float angle = (float)Game1.random.NextDouble() * MathHelper.TwoPi;
                particle.Position = this.Position + new Vector2(num * (float)Math.Cos((double)angle), num * (float)Math.Sin((double)angle));
                ParticleManager.Instance().AddParticle(particle);
                emittedThisSecond++;
                emitTimer = 0;
            }

            if (secondTimer > 1)
            {
                secondTimer = 0;
                emittedThisSecond = 0;
            }
        }
    }
}
