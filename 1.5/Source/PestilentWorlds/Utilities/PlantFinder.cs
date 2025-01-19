using System;
using RimWorld;
using Verse;

namespace PestilentWorlds
{
    public static class PlantFinder
    {
        /// <summary>
        /// Should check if a certain type of plant is present on the map.
        /// </summary>
        /// <param name="map">The map to search within for the target plant.</param>
        /// <param name="plantCriteria">A predicate defining the plant criteria.</param>
        /// <returns>True if at least one matching plant is found on the map; otherwise false.</returns>
        public static Plant TryGetSpecificPlantWithCriteriaOnMap(
            Map map, Predicate<ThingDef> plantCriteria)
        {
            try
            {
                if (map == null)
                    throw new ArgumentNullException(nameof(map), 
                        "The provided map is null.");

                if (plantCriteria == null)
                    throw new ArgumentNullException(nameof(plantCriteria), 
                        "The provided plant criteria is null.");

                foreach (Thing thing in map.listerThings
                             .ThingsInGroup(ThingRequestGroup.Plant))
                {
                    if (thing is Plant plant 
                        && plant.IsOutside()
                        && plantCriteria(plant.def))
                    {
                        return plant;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                PWLog.Error($"[PlantFinder] An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
