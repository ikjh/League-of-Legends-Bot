﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueBot;
using LeagueBot.Patterns;
using LeagueBot.Game.Enums;
using LeagueBot.Game.Misc;

namespace LeagueBot
{
    public class Coop : PatternScript
    {

        private Point CastTargetPoint
        {
            get;
            set;
        }
        private int AllyIndex
        {
            get;
            set;
        }

        private Item[] Items = new Item[]
        {
            new Item("Dorans Blade",450),
            new Item("Boots of Speed",300),
            new Item("Bf Sword",1300),
            new Item("Dagger",300),
            new Item("Berserkers Greaves",500),
            new Item("Long Sword",350),
            new Item("Vampiric Scepter",900),
            new Item("Bilgewater Cutlass",350),
	    new Item("Dagger",300),
	    new Item("Dagger",300),
	    new Item("Recurve Bow",400),
	    new Item("Blade of the Ruined King",700),
        };

        public override bool ThrowException 
        {
            get
            {
                return false;
            }
        }

        public override void Execute()
        {
            bot.log("Waiting for league of legends process...");

            bot.waitProcessOpen(Constants.GameProcessName); 

            bot.waitUntilProcessBounds(Constants.GameProcessName, 1030, 797);

            bot.wait(200);

            bot.log("Waiting for game to load.");

            bot.bringProcessToFront(Constants.GameProcessName);
            bot.centerProcess(Constants.GameProcessName);

            game.waitUntilGameStart();

            bot.log("Game Started");

            bot.bringProcessToFront(Constants.GameProcessName);
            bot.centerProcess(Constants.GameProcessName);

            bot.wait(3000);

            if (game.getSide() == SideEnum.Blue)
            {
                CastTargetPoint = new Point(1084, 398);
                bot.log("We are blue side !");
            }
            else
            {
                CastTargetPoint = new Point(644, 761);
                bot.log("We are red side !");
            }

            game.player.upgradeSpellOnLevelUp();

            OnSpawnJoin();

            bot.log("Playing...");

            GameLoop();

            this.End();
        }
        private void BuyItems()
        {
            int golds = game.player.getGolds();

            game.shop.toogle();

            foreach (Item item in Items)
            {
                if (item.Cost > golds)
                {
                    break;
                }
                if (!item.Buyed)
                {
                    game.shop.searchItem(item.Name);

                    game.shop.buySearchedItem();

                    item.Buyed = true;

                    golds -= item.Cost;
                }
            }

            game.shop.toogle();

        }
        private void GameLoop()
        {
            int level = game.player.getLevel();

            bool dead = false;

            bool isRecalling = false;

            while (bot.isProcessOpen(Constants.GameProcessName))
            {
                bot.bringProcessToFront(Constants.GameProcessName);

                bot.centerProcess(Constants.GameProcessName);

                int newLevel = game.player.getLevel();

                if (newLevel != level)
                {
                    level = newLevel;
                    game.player.upgradeSpellOnLevelUp();
                }


                if (game.player.dead())
                {
                    if (!dead)
                    {
                        dead = true;
                        isRecalling = false;
                        OnDie();
                    }

                    bot.wait(4000);
                    continue;
                }

                if (dead)
                {
                    dead = false;
                    OnRevive();
                    continue;
                }

                if (isRecalling)
                {
                    game.player.recall();
                    bot.wait(4000);

                    if (game.player.getManaPercent() == 1)
                    {
                        OnSpawnJoin();
                        isRecalling = false;
                    }
                    continue;
                }



                if (game.player.getManaPercent() <= 0.10d)
                {
                    //isRecalling = true;
                    continue;
                }


                CastAndMove();


            }
        }
        private void OnDie()
        {
            //BuyItems();
        }
        private void OnSpawnJoin()
        {
            //BuyItems();
            AllyIndex = game.getAllyIdToFollow();
            game.camera.lockAlly(AllyIndex);
        }
        private void OnRevive()
        {
            AllyIndex = game.getAllyIdToFollow();
            game.camera.lockAlly(AllyIndex);
        }

        private void CastAndMove() // Replace this by Champion pattern script.
        {
            game.moveCenterScreen();

            game.player.tryCastSpellOnTarget(3); // veigar cage

            game.moveCenterScreen();

            game.player.tryCastSpellOnTarget(2); // Z

            game.moveCenterScreen();

            game.player.tryCastSpellOnTarget(1); // Q

            game.moveCenterScreen();

            game.player.tryCastSpellOnTarget(4); // ult 
        }

        public override void End()
        {
            bot.executePattern("EndCoop");
            base.End();
        }
    }
}
