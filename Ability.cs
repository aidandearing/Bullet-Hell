using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Xml;
using System;

namespace ActionGame
{
    class Ability
    {
        // The Ability class, outlines basic behavior
        XmlDocument doc;
        List<StatusEffect> Effects = new List<StatusEffect>();
        List<Projectile> Projectiles = new List<Projectile>();
        List<Sound> Sounds = new List<Sound>();

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
                    string name = "";
                    float range = 0;
                    float speed = 0;
                    float angle = 0;
                    bool isOriented = true;
                    float lifetime = 1;
                    float scale = 1;
                    float scaleend = 1;
                    float opacity = 1;
                    float opacityend = 1;

                    name = node.Attributes.Item(0).InnerText;
                    float.TryParse(node.Attributes.Item(1).InnerText, out range);
                    float.TryParse(node.Attributes.Item(2).InnerText, out speed);
                    float.TryParse(node.Attributes.Item(3).InnerText, out angle);
                    bool.TryParse(node.Attributes.Item(4).InnerText.ToLower(), out isOriented);
                    float.TryParse(node.Attributes.Item(5).InnerText, out lifetime);
                    float.TryParse(node.Attributes.Item(6).InnerText, out scale);
                    float.TryParse(node.Attributes.Item(7).InnerText, out scaleend);
                    float.TryParse(node.Attributes.Item(8).InnerText, out opacity);
                    float.TryParse(node.Attributes.Item(9).InnerText, out opacityend);

                    scale = WorldSpace.TILESIZE / 32 * scale;
                    Vector2 scaleStart = new Vector2(scale, scale);

                    scaleend = WorldSpace.TILESIZE / 32 * scaleend;
                    Vector2 scaleEnd = new Vector2(scaleend, scaleend);

                    Projectile projectile = new Projectile(name, range, speed, angle, isOriented, lifetime, scaleStart, scaleEnd, true, opacity, opacityend, true, Parent);
                    XmlNodeList nodes = node.ChildNodes;
                    AddEmitters(projectile, nodes);
                    AddEffects(projectile, nodes);
                    AddLights(projectile, nodes);
                    AddSounds(projectile, nodes);
                    Projectiles.Add(projectile);
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
                Effects.Clear();
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
                    XmlNodeList nodes = node.ChildNodes;
                    AddEmitters(effect, nodes);
                    AddLights(effect, nodes);
                    Effects.Add(effect);
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
                SoundManager soundManager = SoundManager.Instance();

                List<LightSource> lights = new List<LightSource>();
                List<ParticleEmitter> emitters = new List<ParticleEmitter>();
                List<StatusEffect> statusEffects = new List<StatusEffect>();
                List<Sound> sounds = new List<Sound>();

                #region Effects
                foreach (StatusEffect effect in Effects)
                {
                    if (effect.Parent != this.Parent)
                        effect.Parent = this.Parent;

                    StatusEffect statusEffect = new StatusEffect(effect);

                    #region Emitters
                    emitters = new List<ParticleEmitter>();
                    foreach (ParticleEmitter emitter in effect.Emitters)
                    {
                        ParticleEmitter emit = new ParticleEmitter(emitter);
                        emit.Parent = statusEffect.Parent;
                        emitters.Add(emit);
                    }
                    statusEffect.Emitters = emitters;
                    #endregion

                    #region Sounds
                    foreach (Sound sound in effect.Sounds)
                    {
                        Sound soundtoplay = new Sound(sound);
                        soundtoplay.Parent = this.Parent;
                        soundManager.PlaySFX(soundtoplay);
                    }
                    #endregion

                    #region Lights
                    lights = new List<LightSource>();
                    foreach (LightSource light in effect.Lights)
                    {
                        LightSource lightSource = new LightSource(light);
                        lightSource.Parent = this.Parent;
                        lightSource.Position = this.Parent.Position;
                        lights.Add(lightSource);
                    }
                    statusEffect.Lights.AddRange(lights);
                    #endregion

                    foreach (Sound sound in effect.Sounds)
                        soundManager.PlaySFX(sound);

                    Parent.effectManager.AddEffect(statusEffect);
                }
                #endregion

                #region Sounds
                foreach (Sound sound in Sounds)
                {
                    soundManager.PlaySFX(sound);
                }
                #endregion

                #region Projectiles
                foreach (Projectile projectile in Projectiles)
                {
                    Projectile proj = new Projectile(projectile);

                    if (proj.Parent != this.Parent)
                        proj.Parent = this.Parent;

                    #region Emitters
                    emitters = new List<ParticleEmitter>();
                    foreach(ParticleEmitter emitter in projectile.Emitters)
                    {
                        ParticleEmitter emit = new ParticleEmitter(emitter);
                        emit.Parent = proj;
                        emitters.Add(emit);
                    }
                    proj.Emitters = emitters;
                    #endregion Emitters

                    proj.Position = this.Parent.Position;

                    if (proj.Velocity.Length() > 0)
                        proj.PositionOrigin = proj.Position = this.Parent.Position;

                    #region Effects
                    statusEffects = new List<StatusEffect>();
                    foreach (StatusEffect effect in projectile.Effects)
                    {
                        StatusEffect statusEffect = new StatusEffect(effect);

                        #region Emitters
                        emitters = new List<ParticleEmitter>();
                        foreach (ParticleEmitter emitter in effect.Emitters)
                        {
                            ParticleEmitter emit = new ParticleEmitter(emitter);
                            emit.Parent = statusEffect.Parent;
                            emitters.Add(emit);
                        }
                        statusEffect.Emitters = emitters;
                        #endregion

                        #region Lights
                        lights = new List<LightSource>();
                        foreach (LightSource light in projectile.Lights)
                        {
                            LightSource lightSource = new LightSource(light);
                            lightSource.Parent = proj;
                            lights.Add(lightSource);
                        }
                        statusEffect.Lights.AddRange(lights);
                        #endregion

                        statusEffects.Add(statusEffect);
                    }
                    proj.Effects.AddRange(statusEffects);
                    #endregion

                    #region Lights
                    lights = new List<LightSource>();
                    foreach(LightSource light in projectile.Lights)
                    {
                        LightSource lightSource = new LightSource(light);
                        lightSource.Parent = proj;
                        lights.Add(lightSource);
                    }
                    proj.Lights.AddRange(lights);
                    #endregion

                    #region Sounds
                    foreach (Sound sound in projectile.Sounds)
                    {
                        Sound soundtoplay = new Sound(sound);
                        soundtoplay.Parent = this.Parent;
                        soundManager.PlaySFX(soundtoplay);
                    }
                    #endregion

                    projectileManager.CreateProj(proj);
                }
                #endregion

                this.CooldownCurrent = this.Cooldown;
                return true;
            }
            return false;
        }

        public void AddEmitters(Object obj, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "emitter")
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
                        ((StatusEffect)obj).Emitters.Add(emitter);
                    else if (obj is Projectile)
                        ((Projectile)obj).Emitters.Add(emitter);
                }
            }
        }

        public void AddEffects(Projectile obj, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "effect")
                {
                    StatusEffect.Status name = (StatusEffect.Status)Enum.Parse(typeof(StatusEffect.Status), node.Attributes.Item(0).InnerText.ToLower());
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
                    XmlNodeList import_nodes = node.ChildNodes;
                    AddEmitters(effect, import_nodes);
                    AddLights(effect, import_nodes);
                    AddSounds(effect, import_nodes);
                    obj.Effects.Add(effect);
                }
            }
        }

        public void AddLights(Object obj, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "light")
                {
                    LightSource.Light type = LightSource.Light.circle;
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    Color colour = new Color();
                    float intensity = 0;
                    float range = 0;

                    Enum.TryParse<LightSource.Light>(node.Attributes.Item(0).InnerText, out type);
                    int.TryParse(node.Attributes.Item(1).InnerText, out r);
                    int.TryParse(node.Attributes.Item(2).InnerText, out g);
                    int.TryParse(node.Attributes.Item(3).InnerText, out b);
                    colour = new Color(r, g, b);
                    float.TryParse(node.Attributes.Item(4).InnerText, out intensity);
                    float.TryParse(node.Attributes.Item(5).InnerText, out range);

                    LightSource light = new LightSource(type, intensity, range, colour, null);
                    if (obj is Projectile)
                    {
                        light.Parent = (Projectile)obj;
                        ((Projectile)obj).Lights.Add(light);
                    }
                    else if (obj is StatusEffect)
                    {
                        light.Parent = ((StatusEffect)obj).Parent;
                        ((StatusEffect)obj).Lights.Add(light);
                    }
                }
            }
        }

        public void AddSounds(Object obj, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "sound")
                {
                    string name = "";
                    float volume = 1;
                    float range = 1;
                    float pitch = 0;
                    bool looped = false;

                    name = node.Attributes.Item(0).InnerText.ToLower();
                    float.TryParse(node.Attributes.Item(1).InnerText, out volume);
                    float.TryParse(node.Attributes.Item(2).InnerText, out range);
                    float.TryParse(node.Attributes.Item(3).InnerText, out pitch);
                    bool.TryParse(node.Attributes.Item(4).InnerText, out looped);

                    range = WorldSpace.MetresToPixels(range);

                    Sound sound = new Sound(name, volume, range, pitch, looped, null);

                    if (obj is Projectile)
                        ((Projectile)obj).Sounds.Add(sound);
                    if (obj is StatusEffect)
                        ((StatusEffect)obj).Sounds.Add(sound);
                }
            }
        }

        public static void LoadContent(ContentManager Content)
        {
            XmlDocument doc = new XmlDocument();
            Ability ability;

            #region Fire
            doc.Load("Content/Abilities/fire_firebolt.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/fire_fireball.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/fire_combust.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/fire_innervate.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/fire_dash.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/fire_firepulse.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();
            #endregion

            #region Divine
            doc.Load("Content/Abilities/divine_heal.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/divine_consecratedGround.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/divine_divineFury.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/divine_slash.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/divine_stronghold.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/divine_godsWrath.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            #endregion

            #region Ranger
            doc.Load("Content/Abilities/ranger_shoot.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/ranger_ClusterArrow.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/ranger_LightningArrow.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/ranger_RainofVengeance.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/ranger_ShadowStep.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();

            doc.Load("Content/Abilities/ranger_BolaStrike.xml");
            ability = new Ability(doc);
            Abilities.Add(ability.Name, ability);
            doc = new XmlDocument();
            #endregion
        }
    }
}
