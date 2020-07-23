﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

class RoundCalcer
{
    public int allyDamage = 0;
    public int enemyDamage = 0;

    public List<Combatant> Combatants = new List<Combatant>();
    public List<Combatant> listAllies = new List<Combatant>();
    public List<Combatant> listEnemies = new List<Combatant>();
    /*
        populate combatants for fight
        */

    public RoundCalcer()
    {
    }
    
    /*
        Calculates overall damage, returns the damage as a sum for one side. On final round returns negative if the hostiles win, Posative if the allies win. Returns 0 if fight is ongoing.
    
        TODO: calculate combat results
    
        @param distanceBetween, DistanceBetweenParties // assume 0 for now
        @param isFirst, is this the first round of combat?
    
     */
    public int damageCalculator(int distanceBetween, bool isFirst)
    {
        if (Combatants.Count == 0) 
        {
            return 0;
        }

        Combatant[] combatantArray = new Combatant[Combatants.Count];

        int enemies = 1;
        int allies = 1;
        /*
         roll initiative and sort based on the rolls, populate sides of the battle
         */
        int indexer = 0;
        foreach (Combatant c in Combatants)
        {
            c.rollInit();
            combatantArray[indexer] = c;
            if (c.isFriendly)             
            {
                listAllies.Add(c);
                allies++;
            }
            else 
            {
                listEnemies.Add(c);
                enemies++;
            }
            indexer++;
        }

        listAllies.Sort();
        listEnemies.Sort();

        Array.Sort(combatantArray);
        /*
            Initialize first Round, establist targets and distance
         */
        if (isFirst)
        {
            // Set allies targets
            foreach (Combatant c in listAllies)
            {
                c.rangeToFocus = distanceBetween;
                // Randomize the targets
                int x = 0;
                System.Threading.Thread.Sleep(0); 
                Random rnd = new Random();

                x = rnd.Next(0, (listEnemies.Count + 1)); // get random index of enemy list               

                c.focusedTar = listEnemies[x];
            }
            // Set enemy targets
            foreach (Combatant c in listEnemies)
            {
                c.rangeToFocus = distanceBetween;
                // Randomize the targets
                int x = 0;
                System.Threading.Thread.Sleep(0); 
                Random rnd = new Random();

                x = rnd.Next(0, (listAllies.Count + 1)); // get random index of ally list               

                c.focusedTar = listAllies[x];
            }
        }

        /*
        Simulate combat round
        */          

            allies = 0;
            enemies = 0;

        // Count enemies and allies still standing
        foreach (Combatant c in Combatants)
        {
            if (!c.isUnconcious || !c.isDead)
            {
                if (c.isFriendly)
                {
                    allies++;
                }
                else
                {
                    enemies++;
                }
            }
        }
        /*
            Check win condition
         */
        if (allies == 0)
        {
            return 0 - enemyDamage;
        }
        else if (enemies == 0) 
        {
            return allyDamage;
        }
        /*
            Check Target Status and deal damage REMEMBER Combatants is sorted by initiative
         */
        foreach (Combatant c in Combatants)
        {
            // get a new target if previous target is dead or unconcious, for testing purposes we assume the monsters aren't intentionally checking to kill heroes
            if (c.focusedTar.isUnconcious || c.focusedTar.isDead)
            {
                if (c.isFriendly)
                {
                    foreach (Combatant t in listEnemies)
                    {
                        bool newTar = false;
                        while (!newTar)
                        {
                            if (t.isDead || t.isUnconcious)
                            {

                            }
                            else
                            {
                                newTar = true;
                                c.focusedTar = t;
                            }
                        }

                    }
                }
                else
                {
                    foreach (Combatant t in listAllies)
                    {
                        bool newTar = false;
                        while (!newTar)
                        {
                            if (t.isDead || t.isUnconcious)
                            {

                            }
                            else
                            {
                                newTar = true;
                                c.focusedTar = t;
                            }
                        }

                    }
                }
            }

            /*
            Run Damage
            */

            int[] damageTaken = c.calcRound(c.focusedTar);
            int damageActual = c.focusedTar.takeDamage(damageTaken);

            if (c.isFriendly) 
            {
                allyDamage += damageActual;
            }
            else
            {
                enemyDamage += damageActual;
            }

            // Count enemies and allies still standing (as a fight can end mid round)

            allies = 0;
            enemies = 0;

            foreach (Combatant s in Combatants)
            {
                if (!s.isUnconcious || !s.isDead)
                {
                    if (s.isFriendly)
                    {
                        allies++;
                    }
                    else
                    {
                        enemies++;
                    }
                }
            }

            if (allies == 0)
            {
                return 0 - enemyDamage;
            }
            else if (enemies == 0)
            {
                return allyDamage;
            }


        }

        return 0;
    }

    /*
        Add a Combatant to the combat Note: if there are no enemies or allies one side will automatically win
        if the ally is == true the combatant will be considered ally.
    */
    public void addCombatant(Combatant c, bool ally)
    {
        c.setAlly(ally);
        Combatants.Add(c);
    }
}

