﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Helper
{
    class DelayedAction
    {
        // Shot variables
        private int keyDelay ;
        private int keyCountdown ;
        public DelayedAction(int keyDelay = 300)
        {
            keyCountdown = this.keyDelay = keyDelay;
        }

        public bool eventHappened(GameTime gameTime, KeyboardState keyState, params Keys[] keys)
        {
            keyCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            if (keyCountdown <= 0)
            {
                foreach (Keys key in keys)
                {
                    if (keyState.IsKeyDown(key))
                    {
                        keyCountdown = keyDelay;
                        return true;
                    }
                }
                keyCountdown = 0;
            }

            return false;
        }

        public bool eventHappened(GameTime gameTime, params bool[] conditions)
        {
            keyCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            if (keyCountdown <= 0)
            {
                foreach (bool condition in conditions)
                {
                    if (condition)
                    {
                        keyCountdown = keyDelay;
                        return true;
                    }
                }
                keyCountdown = 0;
            }

            return false;
        }
    }
}
