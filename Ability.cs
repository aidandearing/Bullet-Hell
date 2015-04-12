using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using System.Collections.Generic;
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
        public int Damage { get; set; }
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
                //Console.WriteLine("Name:\t\t\t\t" + Name);
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
                //Console.WriteLine("Description:\t\t\t" + Description);
            }
            #endregion

            #region XmlNode Cooldown
            float cooldown = 10;
            float.TryParse(doc.SelectSingleNode("/ability/cooldown").InnerText, out cooldown);
            CooldownBase = cooldown;
            Cooldown = CooldownBase;
            float cooldownValue = CooldownBase;
            float.TryParse(doc.SelectSingleNode("/ability/cooldown").Attributes.Item(0).InnerText, out cooldownValue);
            CooldownValue = cooldownValue;
            CooldownCurrent = CooldownBase;
            //Console.WriteLine("Cooldown:\t\t\t" + CooldownBase);
            //Console.WriteLine("Cooldown Value:\t\t\t" + CooldownValue);
            #endregion 

            #region XmlNode Damage
            try 
            {
                Damage = int.Parse(doc.SelectSingleNode("/ability/damage").InnerText);
            }
            catch
            {
                Damage = 0;
            }
            finally
            {
                Damage = int.Parse(doc.SelectSingleNode("/ability/damage").InnerText);
                //Console.WriteLine("Damage:\t\t\t\t" + Damage);
            }
            #endregion

            #region XmlNode Projectile
            XmlNodeList nodes_projectile;
            try
            {
                nodes_projectile = doc.SelectNodes("/ability/projectile");
            }
            catch
            {
                projectiles.Clear();
            }
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
                    float scalex = 1;
                    float.TryParse(node.Attributes.Item(6).InnerText, out scalex);
                    float scaley = 1;
                    float.TryParse(node.Attributes.Item(7).InnerText, out scaley);
                    Vector2 scale = new Vector2(scalex, scaley);
                    float scaleendx = 1;
                    float.TryParse(node.Attributes.Item(8).InnerText, out scaleendx);
                    float scaleendy = 1;
                    float.TryParse(node.Attributes.Item(9).InnerText, out scaleendy);
                    Vector2 scaleend = new Vector2(scaleendx, scaleendy);
                    bool scalecurved = false;
                    if (node.Attributes.Item(10).InnerText.ToLower() == "true")
                        scalecurved = true;
                    else
                        scalecurved = false;
                    float opacity = 1;
                    float.TryParse(node.Attributes.Item(11).InnerText, out opacity);
                    float opacityend = 1;
                    float.TryParse(node.Attributes.Item(12).InnerText, out opacityend);
                    bool opacitycurved = true;
                    if (node.Attributes.Item(13).InnerText.ToLower() == "true")
                        opacitycurved = true;
                    else
                        opacitycurved = false;

                    projectiles.Add(new Projectile(name, range, speed, angle, isOriented, lifetime, scale, scaleend, scalecurved, opacity, opacityend, opacitycurved, Parent));
                    /*Console.WriteLine("Projectile Name:\t\t" + name);
                    Console.WriteLine("\tRange:\t\t\t" + range);
                    Console.WriteLine("\tSpeed:\t\t\t" + speed);
                    Console.WriteLine("\tAngle:\t\t\t" + angle);
                    Console.WriteLine("\tisOriented:\t\t" + isOriented);
                    Console.WriteLine("\tLifetime:\t\t" + lifetime);
                    Console.WriteLine("\tScale:\t\t\t" + scale);
                    Console.WriteLine("\tScale End:\t\t" + scaleend);
                    Console.WriteLine("\tScale Curve:\t\t" + scalecurved);
                    Console.WriteLine("\tOpacity:\t\t" + opacity);
                    Console.WriteLine("\tOpacity End:\t\t" + opacityend);
                    Console.WriteLine("\tOpacity Curve:\t\t" + opacitycurved);
                    Console.WriteLine("\tParent:\t\t\t" + Parent);*/
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
                    string name = node.Attributes.Item(0).InnerText;
                    string gamename = node.Attributes.Item(1).InnerText;
                    string description = node.Attributes.Item(2).InnerText;
                    float value = 0;
                    float.TryParse(node.Attributes.Item(3).InnerText, out value);
                    float lifetime = 1;
                    float.TryParse(node.Attributes.Item(4).InnerText, out lifetime);
                    bool oncaster;
                    if (node.Attributes.Item(5).InnerText.ToLower() == "true")
                        oncaster = true;
                    else
                        oncaster = false;
                    bool onupdate;
                    if (node.Attributes.Item(6).InnerText.ToLower() == "true")
                        onupdate = true;
                    else
                        onupdate = false;
                    bool onmove;
                    if (node.Attributes.Item(7).InnerText.ToLower() == "true")
                        onmove = true;
                    else
                        onmove = false;
                    bool oncast;
                    if (node.Attributes.Item(8).InnerText.ToLower() == "true")
                        oncast = true;
                    else
                        oncast = false;
                    bool onattack;
                    if (node.Attributes.Item(9).InnerText.ToLower() == "true")
                        onattack = true;
                    else
                        onattack = false;
                    float curve = 0;
                    float.TryParse(node.Attributes.Item(10).InnerText, out curve);
                    bool ishostile = true;
                    if (node.Attributes.Item(11).InnerText.ToLower() == "true")
                        ishostile = true;
                    else
                        ishostile = false;
                    float lifetimecurve = 0;
                    float.TryParse(node.Attributes.Item(12).InnerText, out lifetimecurve);

                    effects.Add(new StatusEffect(name,gamename,description,value,lifetime,oncaster,onupdate,onmove,oncast,onattack,curve,ishostile,lifetimecurve,Parent));
                    /*Console.WriteLine("Effect Name:\t\t\t" + name);
                    Console.WriteLine("\tGame Name:\t\t" + gamename);
                    Console.WriteLine("\tDescription:\t\t" + description);
                    Console.WriteLine("\tValue:\t\t\t" + value);
                    Console.WriteLine("\tLifetime:\t\t" + lifetime);
                    Console.WriteLine("\tLifetime Curve:\t\t" + lifetimecurve);
                    Console.WriteLine("\tOn Caster:\t\t" + oncaster);
                    Console.WriteLine("\tOn Update:\t\t" + onupdate);
                    Console.WriteLine("\tOn Move:\t\t" + onmove);
                    Console.WriteLine("\tOn Cast:\t\t" + oncast);
                    Console.WriteLine("\tOn Attack:\t\t" + onattack);
                    Console.WriteLine("\tCurve:\t\t\t" + curve);
                    Console.WriteLine("\tIs Hostile:\t\t" + ishostile);
                    Console.WriteLine("\tParent:\t\t\t" + Parent);*/
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

        public void Use()
        {
            if (this.CooldownCurrent == 0)
            {
                ProjectileManager projectileManager = ProjectileManager.Instance();

                List<StatusEffect> projEffects = new List<StatusEffect>();
                foreach (StatusEffect effect in effects)
                {
                    if (effect.Caster)
                        Parent.effectManager.AddEffect(effect);
                    else
                        projEffects.Add(effect);
                }

                foreach (Projectile projectile in projectiles)
                {
                    Projectile proj = new Projectile(projectile);
                    if (proj.Parent != this.Parent)
                        proj.Parent = this.Parent;
                    if (proj.Velocity.Length() > 0)
                        proj.PositionOrigin = proj.Position = this.Parent.Position;
                    proj.effectManager.AddEffect(projEffects);
                    projectileManager.CreateProj(proj);
                    Console.WriteLine(this + " created: " + proj);
                }

                this.CooldownCurrent = this.Cooldown;
            }
        }

        public void LevelUp()
        {
            Level++;
            Cooldown = MathHelper.Clamp(CooldownBase + CooldownValue * Level,0.1f,120);
            foreach (StatusEffect effect in effects)
            {
                effect.Value += effect.Curve;
                effect.Lifetime += effect.LifetimeCurve;
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
