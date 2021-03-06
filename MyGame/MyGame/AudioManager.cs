﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Helper;
using control;

namespace MyGame
{
    public class AudioManager : GameComponent,IEvent
    {
        protected List<Event> events;

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        // Shot variables
        int musicDelay = 800;
        int musicCountdown = 0;

        private Game1 myGame;

        public AudioManager(Game1 game)
            :base(game)
        {
            myGame = game;
            events = new List<Event>();
            game.mediator.register(this, MyEvent.C_ATTACK_BULLET_END, MyEvent.M_BITE, MyEvent.G_GameOver);


            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");

            trackCue =  soundBank.GetCue("Cowboy");
            trackCue.Play();
            trackCue.Pause();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            musicCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            if (musicCountdown <= 0)
            {
                if (keyboard.IsKeyDown(Keys.M))
                {
                    if (trackCue.IsPaused)
                        trackCue.Resume();
                    else
                        trackCue.Pause();
                    musicCountdown = musicDelay;
                }
                else
                    musicCountdown = 0;
                
            }

            foreach (Event ev in events)
            {
                switch (ev.EventId)
                {
                    case (int)MyEvent.C_ATTACK_BULLET_END:  soundBank.PlayCue("shot"); break;
                    case (int)MyEvent.M_BITE:               soundBank.PlayCue("Bite"); break;
                    case (int)MyEvent.G_GameOver:           soundBank.PlayCue("ScreamAndDie"); break;
                }
            }

            events.Clear();
            base.Update(gameTime);
        }

        public void addEvent(Event ev)
        {
            events.Add(ev);
        }
    }
}
