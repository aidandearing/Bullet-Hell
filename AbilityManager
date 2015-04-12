using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ActionGame
{
    class AbilityManager
    {
        // Handles all the abilities for the player / enemy
        List<Ability> abilities = new List<Ability>();

        public Entity Parent { get; set; }

        public AbilityManager(Entity parent)
        {
            this.Parent = parent;
        }

        public string AddNewAbility(string key)
        {
            Ability ability = Ability.Abilities[key];
            Guid id = Guid.NewGuid();
            ability.ID = id.ToString();
            ability.Parent = this.Parent;
            abilities.Add(ability);
            return ability.ID;
        }

        public void RemoveAbility(string id)
        {
            for (int i = 0; i < abilities.Count; ++i)
            {
                Ability ability = abilities[i];
                if (ability.ID == id)
                {
                    abilities.RemoveAt(i);
                    break;
                }
            }
        }

        public void UseAbility(uint index)
        {
            if (index < abilities.Count)
                abilities[(int)index].Use();
        }

        public void LevelAbility(uint index)
        {
            if (index < abilities.Count)
                abilities[(int)index].LevelUp();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Ability ability in abilities)
            {
                if (ability.Parent != this.Parent)
                    ability.Parent = this.Parent;
                ability.Update(gameTime);
            }
        }
    }
}
