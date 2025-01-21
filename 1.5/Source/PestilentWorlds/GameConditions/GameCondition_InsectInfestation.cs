using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace PestilentWorlds
{
    public class GameCondition_InsectInfestation : GameCondition
    { 
        [TweakValue("Graphics", 0f, 100f)]
        public static bool DrawAffectedPlantLocations;
        
        protected virtual InfestationCategory InfestationCategory => InfestationCategory.None;
        private static bool DrawAffectedPlantLocationsNow => DrawAffectedPlantLocations;
        
        protected HashSet<Plant> AffectedPlants = [];
        protected int LastUpdateTick = -1;
        
        public override void Init()
        {
            base.Init();
            UpdateAffectedPlantList(InfestationCategory);
        }
        
        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame - LastUpdateTick > Constants.PlantListUpdateInterval)
            {
                UpdateAffectedPlantList(InfestationCategory);
                LastUpdateTick = Find.TickManager.TicksGame;
                
                TryProcessPlantDamage(AffectedPlants);
            }
        }
        
        public override void GameConditionDraw(Map map)
        {
            UpdateDebugDrawing();
        }
        
        /// <summary>
        /// Picks a random string to display in addition to the base condition's LetterText.
        /// </summary>
        /// <param name="letterTextOptions">A hashset of options to select from.</param>
        /// <returns>A random bit of flavor text for the condition's letter text.</returns>
        protected static string TryGetRandomLetterText(HashSet<string> letterTextOptions)
        {
            return letterTextOptions.NullOrEmpty() 
                ? null 
                : letterTextOptions.RandomElement();
        }
        
        /// <summary>
        /// Calculates and applies damage to applicable plants on the map.
        /// </summary>
        /// <param name="plants">A collection of all possible plants that can be damaged.</param>
        private static void TryProcessPlantDamage(HashSet<Plant> plants)
        {
            foreach (Plant plant in plants)
            {
                if (plant.DestroyedOrNull())
                    continue;

                ModExtension_InsectDamage mE_ID = plant.def
                    .GetModExtension<ModExtension_InsectDamage>();

                if (mE_ID?.insectThreats == null)
                    return;

                foreach (InsectDamage threat in mE_ID.insectThreats)
                {
                    if (!Rand.Chance(threat.beetleDamageChance))
                        continue;

                    float growthDamageFactor = PlantDamageCalculator
                        .TryGetInsectDamageFactorForPlantGrowth(plant);

                    float healthDamageFactor = PlantDamageCalculator
                        .TryGetDamageFactorForPlantHealth(plant);

                    float baseDamage = threat.beetleDamage.RandomInRange;
                    float finalDamageAmount = PlantDamageCalculator
                        .TryGetInsectDamageForPlantCategory(baseDamage)
                                              * growthDamageFactor * healthDamageFactor;

                    if (!(finalDamageAmount > 0f))
                        continue;
                    
                    // debugging, remove later
                    //Logging.LogPlantDamageData(plant, baseDamage, 
                        //growthDamageFactor, healthDamageFactor, finalDamageAmount);
                    
                    plant.TakeDamage(new DamageInfo(
                        DamageDefOf.Deterioration, finalDamageAmount * Constants.PlantTickLong));
                }
            }
        }
        
        /// <summary>
        /// Determines which plants on the map should be considered for insect damage.
        /// </summary>
        /// <param name="infestationCategory">A filter for all plants on the map.</param>
        private void UpdateAffectedPlantList(InfestationCategory infestationCategory)
        {
            AffectedPlants.Clear();
            
            foreach (Map map in AffectedMaps)
            {
                List<Thing> plants = map?.listerThings?
                    .ThingsInGroup(ThingRequestGroup.Plant);
                
                if (plants == null)
                    continue;

                foreach (Thing plant in plants)
                {
                    if (!plant.IsOutside())
                        continue;
                    
                    if (plant is not Plant specificPlant) 
                        continue;
                    
                    ModExtension_InsectDamage mE_ID = specificPlant.def
                        .GetModExtension<ModExtension_InsectDamage>();
                    
                    if (mE_ID?.insectThreats == null)
                        continue;

                    if (mE_ID.insectThreats.Exists(threat => 
                            threat.infestationCategory == infestationCategory))
                    {
                        AffectedPlants.Add(specificPlant);
                    }
                }
            }
            PWLog.Message($"Plants in collection, 1st pass: {AffectedPlants.Count}");
            RandomlyRemovePercentageOfPlants(AffectedPlants, Constants.RemovalPct);
            PWLog.Message($"Plants in collection, 2nd pass: {AffectedPlants.Count}");
        }
        
        /// <summary>
        /// Removes a portion of a collection of plants.
        /// </summary>
        /// <param name="collection">The collection to remove entries from.</param>
        /// <param name="percentage">The percentage to remove (0 - 100)</param>
        private static void RandomlyRemovePercentageOfPlants(HashSet<Plant> collection, float percentage)
        {
            if (collection == null || collection.Count == 0 || percentage <= 0f)
                return;

            int totalToRemove = Mathf.CeilToInt(collection.Count * (percentage / 100f));
            List<Plant> plantsList = collection.ToList();

            for (int i = 0; i < totalToRemove; i++)
            {
                if (plantsList.Count == 0)
                    break;

                int randomIndex = Rand.Range(0, plantsList.Count);
                Plant plantToRemove = plantsList[randomIndex];

                collection.Remove(plantToRemove);
                plantsList.RemoveAt(randomIndex);
            }
        }
        
        /// <summary>
        /// Draws a colored rect at each affected plant's location (for debugging only).
        /// </summary>
        private void UpdateDebugDrawing()
        {
            if (!DrawAffectedPlantLocationsNow)
                return;
                
            if (AffectedPlants.NullOrEmpty())
                return;
            
            foreach (Plant plant in AffectedPlants)
            {
                if (plant.DestroyedOrNull())
                    continue;
                
                float healthPct = Mathf.Clamp01(plant.HitPoints / (float)plant.MaxHitPoints);
                Color lerpedColor = Color.Lerp(Color.red, Color.green, healthPct);

                GenDraw.DrawFieldEdges([plant.Position], lerpedColor);
            }
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref LastUpdateTick, "LastUpdateTick", -1);
            Scribe_Collections.Look(ref AffectedPlants, "AffectedPlants", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.LoadingVars && AffectedPlants == null)
            {
                AffectedPlants = [];
            }
        }
    }
}