using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    public class SoundManager
    {
        #region Var declarations
        // player
        private SoundEffect player_dash;
        public SoundEffectInstance player_dashInstance;
        private SoundEffect player_crushDeath;
        public SoundEffectInstance player_crushDeathInstance;
        private SoundEffect player_fallDeath;
        public SoundEffectInstance player_fallDeathInstance;
        private SoundEffect player_heartbeat;
        public SoundEffectInstance player_heartbeatInstance;
        private SoundEffect player_hurt;
        public SoundEffectInstance player_hurtInstance;

        // soldiers
        List<SoundEffect> soldier_deaths;
        public List<SoundEffectInstance> soldier_deathsInstances;
        List<SoundEffect> soldier_spawns;
        public List<SoundEffectInstance> soldier_spawnsInstances;

        // announcer
        List<SoundEffect> battleCries;
        public List<SoundEffectInstance> battleCriesInstances;
        private SoundEffect ann_congrats;
        public SoundEffectInstance ann_congratsInstance;
        private SoundEffect ann_continue;
        public SoundEffectInstance ann_continueInstance;
        private SoundEffect ann_gameOver;
        public SoundEffectInstance ann_gameOverInstance;
        private SoundEffect ann_go;
        public SoundEffectInstance ann_goInstance;
        private SoundEffect ann_perfect;
        public SoundEffectInstance ann_perfectInstance;
        private SoundEffect ann_ready;
        public SoundEffectInstance ann_readyInstance;
        private SoundEffect ann_youWin;
        public SoundEffectInstance ann_youWinInstance;

        // bgm (loopables)
        private SoundEffect bgm_title;
        public SoundEffectInstance bgm_titleInstance;
        private SoundEffect bgm_inGame;
        public SoundEffectInstance bgm_inGameInstance;
        private SoundEffect bgm_levelEnd;
        public SoundEffectInstance bgm_levelEndInstance;
        private SoundEffect bgm_gameEndDefeat;
        public SoundEffectInstance bgm_gameEndDefeatInstance;
        private SoundEffect bgm_gameEndVictory;
        public SoundEffectInstance bgm_gameEndVictoryInstance;

        // crowd
        private SoundEffect crowd_charge;
        public SoundEffectInstance crowd_chargeInstance;
        private SoundEffect crowd_chatting;
        public SoundEffectInstance crowd_chattingInstance;
        private SoundEffect crowd_marching;
        public SoundEffectInstance crowd_marchingInstance;
        private SoundEffect crowd_cheer;
        public SoundEffectInstance crowd_cheerInstance;

        // menu
        private SoundEffect menu_click;
        public SoundEffectInstance menu_clickInstance;
        private SoundEffect menu_pauseOff;
        public SoundEffectInstance menu_pauseOffInstance;
        private SoundEffect menu_pauseOn;
        public SoundEffectInstance menu_pauseOnInstance;

        // objects
        private SoundEffect item_pickUp;
        public SoundEffectInstance item_pickUpInstance;
        private SoundEffect rock_fall; // loopable?
        public SoundEffectInstance rock_fallInstance;
        private SoundEffect rock_smash;
        public SoundEffectInstance rock_smashInstance;

        Random random;
        bool listsPopulated;
        #endregion

        public SoundManager()
        {
            soldier_deaths = new List<SoundEffect>(6);
            soldier_deathsInstances = new List<SoundEffectInstance>(6);
            soldier_spawns = new List<SoundEffect>(4);
            soldier_spawnsInstances = new List<SoundEffectInstance>(4);
            
            battleCries = new List<SoundEffect>(3);
            battleCriesInstances = new List<SoundEffectInstance>(3);

            listsPopulated = false;
        }

        public void LoadContent(ContentManager Content)
        {
            // player
            player_dash = Content.Load<SoundEffect>("sounds/actors/player/dash");
            player_crushDeath = Content.Load<SoundEffect>("sounds/actors/player/death_crushed");
            player_fallDeath = Content.Load<SoundEffect>("sounds/actors/player/death_fell");
            //player_heartbeat = Content.Load<SoundEffect>("sounds/actors/player/heartBeat"); // argument out of range??
            player_hurt = Content.Load<SoundEffect>("sounds/actors/player/hurt");

            // soldiers
            soldier_deaths.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/death_1"));
            soldier_deaths.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/death_2"));
            soldier_deaths.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/death_3"));
            soldier_deaths.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/death_4"));
            soldier_deaths.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/death_5"));
            soldier_deaths.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/death_6"));

            soldier_spawns.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/spawn_1"));
            soldier_spawns.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/spawn_2"));
            soldier_spawns.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/spawn_3"));
            soldier_spawns.Add(Content.Load<SoundEffect>("sounds/actors/soldiers/spawn_4"));

            // announcer
            battleCries.Add(Content.Load<SoundEffect>("sounds/announcer/attack"));
            battleCries.Add(Content.Load<SoundEffect>("sounds/announcer/charge"));
            battleCries.Add(Content.Load<SoundEffect>("sounds/announcer/forward"));

            ann_congrats = Content.Load<SoundEffect>("sounds/announcer/congratulations");
            ann_continue = Content.Load<SoundEffect>("sounds/announcer/continue");
            ann_gameOver = Content.Load<SoundEffect>("sounds/announcer/game_over");
            ann_go = Content.Load<SoundEffect>("sounds/announcer/go");
            ann_perfect = Content.Load<SoundEffect>("sounds/announcer/perfect");
            ann_ready = Content.Load<SoundEffect>("sounds/announcer/ready");
            ann_youWin = Content.Load<SoundEffect>("sounds/announcer/you_win");

            // bgm
            bgm_title = Content.Load<SoundEffect>("sounds/bgm/bgm_title");
            bgm_inGame = Content.Load<SoundEffect>("sounds/bgm/bgm_attack");
            bgm_levelEnd = Content.Load<SoundEffect>("sounds/bgm/bgm_inbetween");
            bgm_gameEndDefeat = Content.Load<SoundEffect>("sounds/bgm/bgm_death");
            bgm_gameEndVictory = Content.Load<SoundEffect>("sounds/bgm/bgm_victory");

            // crowd
            crowd_charge = Content.Load<SoundEffect>("sounds/crowd/charge");
            crowd_chatting = Content.Load<SoundEffect>("sounds/crowd/chatting");
            crowd_marching = Content.Load<SoundEffect>("sounds/crowd/marching");
            crowd_cheer = Content.Load<SoundEffect>("sounds/crowd/shouting");

            // menu
            menu_click = Content.Load<SoundEffect>("sounds/menu/click");
            menu_pauseOff = Content.Load<SoundEffect>("sounds/menu/pause_off");
            menu_pauseOn = Content.Load<SoundEffect>("sounds/menu/pause_on");

            // objects
            item_pickUp = Content.Load<SoundEffect>("sounds/objects/itempickup");
            rock_fall = Content.Load<SoundEffect>("sounds/objects/rock_fall");
            rock_smash = Content.Load<SoundEffect>("sounds/objects/rock_smash");

        }

        public void UnloadContent()
        {
            cleanUpInGameSounds();
            cleanUpScreenSounds();
        }

        public void createInGameInstances()
        {
            player_dashInstance = player_dash.CreateInstance();
            player_crushDeathInstance = player_crushDeath.CreateInstance();
            player_fallDeathInstance = player_fallDeath.CreateInstance();
            player_hurtInstance = player_hurt.CreateInstance();

            if (!listsPopulated)
            {
                foreach (SoundEffect s in soldier_deaths)
                    soldier_deathsInstances.Add(s.CreateInstance());
                foreach (SoundEffect s in soldier_spawns)
                    soldier_spawnsInstances.Add(s.CreateInstance());

                foreach (SoundEffect s in battleCries)
                    battleCriesInstances.Add(s.CreateInstance());
                
                listsPopulated = true;
            }
            
            ann_goInstance = ann_go.CreateInstance();
            ann_readyInstance = ann_ready.CreateInstance();

            bgm_inGameInstance = bgm_inGame.CreateInstance();
            bgm_inGameInstance.IsLooped = true;

            crowd_chargeInstance = crowd_charge.CreateInstance();
            crowd_marchingInstance = crowd_marching.CreateInstance();

            menu_clickInstance = menu_click.CreateInstance();
            menu_pauseOffInstance = menu_pauseOff.CreateInstance();
            menu_pauseOnInstance = menu_pauseOn.CreateInstance();

            item_pickUpInstance = item_pickUp.CreateInstance();
            rock_fallInstance = rock_fall.CreateInstance();
            rock_smashInstance = rock_smash.CreateInstance();
        }

        public void createScreenInstances()
        {
            ann_congratsInstance = ann_congrats.CreateInstance();
            ann_continueInstance = ann_continue.CreateInstance();
            ann_gameOverInstance = ann_gameOver.CreateInstance();
            ann_perfectInstance = ann_perfect.CreateInstance();
            ann_youWinInstance = ann_youWin.CreateInstance();

            bgm_titleInstance = bgm_title.CreateInstance();
            bgm_levelEndInstance = bgm_levelEnd.CreateInstance();
            bgm_gameEndDefeatInstance = bgm_gameEndDefeat.CreateInstance();
            bgm_gameEndVictoryInstance = bgm_gameEndVictory.CreateInstance();
            bgm_titleInstance.IsLooped = true;
            bgm_levelEndInstance.IsLooped = true;
            bgm_gameEndDefeatInstance.IsLooped = true;
            bgm_gameEndVictoryInstance.IsLooped = true;

            crowd_cheerInstance = crowd_cheer.CreateInstance();
            crowd_chattingInstance = crowd_chatting.CreateInstance();
            crowd_chattingInstance.IsLooped = true;

            menu_clickInstance = menu_click.CreateInstance();
        }

        public void createAllInstances()
        {
            createInGameInstances();
            createScreenInstances();
        }

        // from in game to screen, clear memory
        public void cleanUpInGameSounds()
        {
            player_dashInstance.Dispose();
            player_crushDeathInstance.Dispose();
            player_fallDeathInstance.Dispose();
            player_hurtInstance.Dispose();
            
            foreach (SoundEffectInstance s in soldier_deathsInstances)
            {
                s.Dispose();
            }
            foreach (SoundEffectInstance s in soldier_spawnsInstances)
            {
                s.Dispose();
            }
            foreach (SoundEffectInstance s in battleCriesInstances)
            {
                s.Dispose();
            }
            soldier_deathsInstances.Clear();
            soldier_spawnsInstances.Clear();
            battleCriesInstances.Clear();
            listsPopulated = false;
            
            ann_congratsInstance.Dispose();
            ann_continueInstance.Dispose();
            ann_gameOverInstance.Dispose();
            ann_goInstance.Dispose();
            ann_perfectInstance.Dispose();
            ann_readyInstance.Dispose();
            ann_youWinInstance.Dispose();

            bgm_inGameInstance.Stop();
            bgm_inGameInstance.Dispose();

            crowd_chargeInstance.Dispose();
            crowd_marchingInstance.Dispose();

            menu_clickInstance.Dispose();
            menu_pauseOffInstance.Dispose();
            menu_pauseOnInstance.Dispose();

            item_pickUpInstance.Dispose();
            rock_fallInstance.Dispose();
            rock_smashInstance.Dispose();
        }

        // from screen to in game, clear memory
        public void cleanUpScreenSounds()
        {
            bgm_titleInstance.Stop();
            bgm_levelEndInstance.Stop();
            bgm_gameEndDefeatInstance.Stop();
            bgm_gameEndVictoryInstance.Stop();
            crowd_cheerInstance.Stop();

            bgm_titleInstance.Dispose();
            bgm_levelEndInstance.Dispose();
            bgm_gameEndDefeatInstance.Dispose();
            bgm_gameEndVictoryInstance.Dispose();

            crowd_cheerInstance.Dispose();
            crowd_chattingInstance.Dispose();

            menu_clickInstance.Dispose();
        }

        public SoundEffectInstance getSoldierDeathInstance()
        {
            random = new Random();
            int max = random.Next(soldier_deathsInstances.Capacity);

            return soldier_deathsInstances[max];
        }
        public SoundEffectInstance getSoldierSpawnInstance()
        {
            random = new Random();
            int max = random.Next(soldier_spawnsInstances.Capacity);

            return soldier_spawnsInstances[max];
        }
        public SoundEffectInstance getBattleCryInstance()
        {
            random = new Random();
            int max = random.Next(battleCriesInstances.Capacity);

            return battleCriesInstances[max];
        }


    }
}
