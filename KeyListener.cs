using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    class KeyListener
    {
        GamePadState pad;
        PlayerIndex player;

        //Constructor
        public KeyListener(PlayerIndex player)
        {
            pad = GamePad.GetState(player, GamePadDeadZone.Circular);
            this.player = player;
        }

        public GamePadState Update()
        {
            pad = GamePad.GetState(player, GamePadDeadZone.Circular);
            return pad;
        }
    }
}
