using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Xml;
using System;

namespace ActionGame
{
    class Ability
    {
        // The Ability class, outlines basic behavior
        XmlDocument doc;
        List<StatusEffect> effects = new List<StatusEffect>();
        List<Projectile> projectiles = new List<Projectile>();

        public static Dictionary<string, Ability> Abilities = new Dictionary<string, Ability>();

        public string Name { get; set; }
        public string Description { get; set; }
        public uint Level { get; set; }
        public float Cooldown { get; set; }
        public float CooldownBase { get; set; }
        public float CooldownValue { get; set; }
        private float CooldownCurrent { get; set; }
        public Entity Parent { get; set; }
        public string ID { get; set; }

        public Ability(XmlDocument xmlref)
        {
            doc = xmlref;
            Level = 0;

            #region XmlNode Name
            try 
            {
                Name = doc.SelectSingleNode("/ability/name").InnerText;
            }
            catch
            {
                Name = "ability/name node could not be found";
            }
            finally
            {
                Name = doc.SelectSingleNode("/ability/name").InnerText;
            }
            #endregion

            #region XmlNode Description
            try 
            {
                Description = doc.SelectSingleNode("/ability/description").InnerText;
            }
            catch
            {
                Description = "ability/description node could not be found";
            }
            finally
            {
                Description = doc.SelectSingleNode("/ability/description").InnerText;
            }
            #endregion

            #region XmlNode Cooldown
            float cooldown = 10;
            float.TryParse(doc.SelectSingleNode("/ability/cooldown").InnerText, out cooldown);
            CooldownBase = cooldown;
            Cooldown = CooldownBase;
            #endregion 

            #region XmlNode Projectile
            XmlNodeList nodes_projectile;
            try
            {
                nodes_projectile = doc.SelectNodes("/ability/projectile");
            }
            catch
            {}
            finally
            {
                nodes_projectile = doc.SelectNodes("/ability/projectile");
                foreach (XmlNode node in nodes_projectile)
                {
                    string name = node.Attributes.Item(0).InnerText;
                    float range = 0;
                    float.TryParse(node.Attributes.Item(1).InnerText, out range);
                    float speed = 0;
                    float.TryParse(node.Attributes.Item(2).InnerText, out speed);
                    float angle = 0;
                    float.TryParse(node.Attributes.Item(3).InnerText, out angle);
                    bool isOriented = true;
                    if (node.Attributes.Item(4).InnerText.ToLower() == "true")
                        isOriented = true;
                    else
                        isOriented = false;
                    float lifetime = 1;
                    float.TryParse(node.Attributes.Item(5).InnerText, out lifetime);
                    float scale = 1;
                    Vector2 scaleStart = new Vector2(scale, scale);
                    float scaleend = 1;
                    float.TryParse(node.Attributes.Item(6).InnerText, out scaleend);
                    Vector2 scaleEnd = new Vector2(scaleend, scaleend);
                    float opacity = 1;
                    float.TryParse(node.Attributes.Item(7).InnerText, out opacity);
                    float opacityend = 1;
                    float.TryParse(node.Attributes.Item(8).InnerText, out opacityend);
                    bool isHostile = true;
                    if (node.Attributes.Item(10).InnerText.ToLower() == "true")
                        isHostile = true;
                    else
                        isHostile = false;

                    Projectile projectile = new Projectile(name, isHostile, range, speed, angle, isOriented, lifetime, scaleStart, scaleEnd, false, opacity, opacityend, false, Parent);
                    XmlNodeList emitternodes = node.ChildNodes;
                    AddEmitters(projectile, emitternodes);
                    projectiles.Add(projectile);
                }
            }
            #endregion

            #region XmlNodes Effect
            XmlNodeList nodes_effect;
            try
            {
                nodes_effect = doc.SelectNodes("/ability/effect");
            }
            catch
            {
                effects.Clear();
            }
            finally
            {
                nodes_effect = doc.SelectNodes("/ability/effect");
                foreach (XmlNode node in nodes_effect)
                {
                    StatusEffect.Status name = (StatusEffect.Status)Enum.Parse(typeof(StatusEffect.Status),node.Attributes.Item(0).InnerText.ToLower());
                    string gamename = node.Attributes.Item(1).InnerText;
                    string description = node.Attributes.Item(2).InnerText;
                    // Not guaranteed to exist after this point
                    float value = 0;
                    float lifetime = 0;
                    int tick = 0;
                    StatusEffect.StatusUpdate when = StatusEffect.StatusUpdate.update;
                    bool ishostile = false;

                    switch (name)
                    {
                        case StatusEffect.Status.changehealth:
                            float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(4).InnerText, out lifetime);
                            int.TryParse(node.Attributes.Item(5).InnerText, out tick);
                            Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(6).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(7).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.changehealthmax:
                            float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(4).InnerText, out lifetime);
                            //Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(5).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(5).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.changehealthregen:
                            float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(4).InnerText, out lifetime);
                            //Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(5).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(5).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.changespeed:
                            float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(4).InnerText, out lifetime);
                            //Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(5).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(5).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.lifetime:
                            //float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(3).InnerText, out lifetime);
                            Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(4).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(5).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.noability:
                            //float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(3).InnerText, out lifetime);
                            //Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(5).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(4).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.noattack:
                            //float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(3).InnerText, out lifetime);
                            //Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(5).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(4).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.nodamage:
                            //float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(3).InnerText, out lifetime);
                            //Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(5).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(4).InnerText, out ishostile);
                            break;
                        case StatusEffect.Status.noheal:
                            //float.TryParse(node.Attributes.Item(3).InnerText, out value);
                            float.TryParse(node.Attributes.Item(3).InnerText, out lifetime);
                            //Enum.TryParse<StatusEffect.StatusUpdate>(node.Attributes.Item(5).InnerText.ToLower(), out when);
                            bool.TryParse(node.Attributes.Item(4).InnerText, out ishostile);
                            break;
                        default:
                            break;
                    }
                    StatusEffect effect = new StatusEffect(name, gamename, description, value, lifetime, tick, when, ishostile, Parent); 
                    XmlNodeList emitternodes = node.ChildNodes;
                    AddEmitters(effect, emitternodes);
                    effects.Add(effect);
                }
            }
            #endregion
        }

        public void Update(GameTime gameTime)
        {
            if (CooldownCurrent > 0)
            {
                CooldownCurrent -= gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            }
            if (CooldownCurrent < 0)
                CooldownCurrent = 0;
        }

        public bool Use()
        {
            if (this.CooldownCurrent == 0)
            {
                ProjectileManager projectileManager = ProjectileManager.Instance();

                List<StatusEffect> projEffectsHostile = new List<StatusEffect>();
                List<StatusEffect> projEffectsNonHostile = new List<StatusEffect>();

                foreach (StatusEffect effect in effects)
                {
                    if (effect.Parent != this.Parent)
                        effect.Parent = this.Parent;

                    StatusEffect statusEffect = new StatusEffect(effect);

                    if (statusEffect.Hostile)
                        projEffectsHostile.Add(statusEffect);
                    else
                        projEffectsNonHostile.Add(statusEffect);
                }

                foreach (Projectile projectile in projectiles)
                {
                    Projectile proj = new Projectile(projectile);

                    if (proj.Parent != this.Parent)
                        proj.Parent = this.Parent;

                    proj.Position = this.Parent.Position;

                    if (proj.Velocity.Length() > 0)
                        proj.PositionOrigin = proj.Position = this.Parent.Position;

                    if (proj.isHostile)
                        proj.effectManager.AddEffect(projEffectsHostile);
                    else
                        proj.effectManager.AddEffect(projEffectsNonHostile);

                    projectileManager.CreateProj(proj);
                }

                this.CooldownCurrent = this.Cooldown;
                return true;
            }
            return false;
        }

        //public void LevelUp()
        //{
        //    Level++;
        //    Cooldown = MathHelper.Clamp(CooldownBase + CooldownValue * Level,0.1f,120);
        //    foreach (StatusEffect effect in effects)
        //    {
        //        effect.Value += effect.Curve;
        //        effect.Lifetime += effect.LifetimeCurve;
        //    }
        //}

        public void AddEmitters(Object obj, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                float lifetime = 0.1f;
                float emitradius = 0;
                int emitnumber = 1;
                float emitangle = 0;
                float emittoangle = MathHelper.TwoPi;
                float chase = 0;
                bool chaseelastic = false;

                float.TryParse(node.Attributes.Item(0).InnerText, out lifetime);
                float.TryParse(node.Attributes.Item(1).InnerText, out emitradius);
                int.TryParse(node.Attributes.Item(2).InnerText, out emitnumber);
                float.TryParse(node.Attributes.Item(3).InnerText, out emitangle);
                float.TryParse(node.Attributes.Item(4).InnerText, out emittoangle);
                float.TryParse(node.Attributes.Item(5).InnerText, out chase);
                bool.TryParse(node.Attributes.Item(6).InnerText, out chaseelastic);

                XmlNode node_particle = node.FirstChild;
                string name = "spark";
                float particlelifetime = 0.1f;
                float particlechase = 0;
                float rotation = 0;
                float rotationend = 0;
                float rotationchase = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                int re = 0;
                int ge = 0;
                int be = 0;
                Color colour = Color.White;
                Color colourend = Color.White;
                float scale = 0;
                float scaleend = 1;
                float opacity = 1;
                float opacityend = 0;

                name = node_particle.Attributes.Item(0).InnerText.ToLower();
                float.TryParse(node_particle.Attributes.Item(1).InnerText, out particlelifetime);
                float.TryParse(node_particle.Attributes.Item(2).InnerText, out particlechase);
                float.TryParse(node_particle.Attributes.Item(3).InnerText, out rotation);
                float.TryParse(node_particle.Attributes.Item(4).InnerText, out rotationend);
                float.TryParse(node_particle.Attributes.Item(5).InnerText, out rotationchase);
                int.TryParse(node_particle.Attributes.Item(6).InnerText, out r);
                int.TryParse(node_particle.Attributes.Item(7).InnerText, out g);
                int.TryParse(node_particle.Attributes.Item(8).InnerText, out b);
                colour = new Color(r, g, b);
                int.TryParse(node_particle.Attributes.Item(9).InnerText, out re);
                int.TryParse(node_particle.Attributes.Item(10).InnerText, out ge);
                int.TryParse(node_particle.Attributes.Item(11).InnerText, out be);
                colourend = new Color(re, ge, be);
                float.TryParse(node_particle.Attributes.Item(12).InnerText, out scale);
                float.TryParse(node_particle.Attributes.Item(13).InnerText, out scaleend);
                float.TryParse(node_particle.Attributes.Item(14).InnerText, out opacity);
                float.TryParse(node_particle.Attributes.Item(11).InnerText, out opacityend);

                Particle particle = new Particle(name, Parent, particlelifetime, Vector2.Zero, Vector2.Zero, particlechase, rotation, rotationend, rotationchase, colour, colourend, new Vector2(scale, scale), new Vector2(scaleend, scaleend), opacity, opacityend);
                ParticleEmitter emitter = new ParticleEmitter(Vector2.Zero, particle, lifetime, emitradius, emitnumber, Parent, chase, chaseelastic);

                if (obj is StatusEffect)
                    ((StatusEffect)obj).emitters.Add(emitter);
                else if (obj is Projectile)
                    ((Projectile)obj).emitters.Add(emitter);
            }
        }

        public static void LoadContent(ContentManager Content)
        {
            XmlDocument doc = new XmlDocument();
            Ability ability;

            doc.Load("Content/Abilities/abilityroot.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();
        }
    }
}
