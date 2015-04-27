using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ActionGame
{
    class ParticleManager
    {
        List<ParticleEmitter> Emitters;
        List<ParticleEmitter> deadEmitters;
        List<Particle> Particles;
        List<Particle> deadParticles;

        public const int PARTICLES_MAX = 2000;

        private static ParticleManager instance { get; set; }

        private ParticleManager()
        {
            Emitters = new List<ParticleEmitter>();
            deadEmitters = new List<ParticleEmitter>();
            Particles = new List<Particle>();
            deadParticles = new List<Particle>();
        }

        public static ParticleManager Instance()
        {
            if (instance == null)
                instance = new ParticleManager();
            return instance;
        }

        public void Update(GameTime gameTime)
        {
            foreach (ParticleEmitter emitter in Emitters)
                emitter.Update(gameTime);

            foreach (Particle particle in Particles)
                particle.Update(gameTime);

            foreach (Particle particle in deadParticles)
                Particles.Remove(particle);

            foreach (ParticleEmitter emitter in deadEmitters)
                Emitters.Remove(emitter);

            deadParticles = new List<Particle>();
            deadEmitters = new List<ParticleEmitter>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in Particles)
            {
                particle.Draw(spriteBatch);
            }
        }

        public void AddEmitter(ParticleEmitter emitter)
        {
            Emitters.Add(emitter);
        }

        public void RemoveEmitter(ParticleEmitter emitter)
        {
            deadEmitters.Add(emitter);
        }

        public void AddParticle(Particle particle)
        {
            if (Particles.Count < PARTICLES_MAX)
                Particles.Add(particle);
        }

        public void RemoveParticle(Particle particle)
        {
            deadParticles.Add(particle);
        }
    }
}
