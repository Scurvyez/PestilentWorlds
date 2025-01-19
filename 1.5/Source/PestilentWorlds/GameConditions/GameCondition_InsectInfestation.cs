using System.Collections.Generic;
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

                    PWLog.Message($"{plant.LabelCap}, " +
                                  $"at {plant.Position}, " +
                                  $"base damage: {baseDamage:F5}, " +
                                  $"growth factor: {growthDamageFactor:F2}, " +
                                  $"health factor: {healthDamageFactor:F2}, " +
                                  $"damaged for: {Mathf.CeilToInt(finalDamageAmount * 
                                                                  Constants.PlantTickLong)}");
                    plant.TakeDamage(new DamageInfo(
                        DamageDefOf.Bite, finalDamageAmount * Constants.PlantTickLong));
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