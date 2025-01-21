using RimWorld;
using Verse;

namespace PestilentWorlds
{
    public class IncidentWorker_InsectInfestation : IncidentWorker_MakeGameCondition
    {
        protected virtual InfestationCategory InfestationCategory => InfestationCategory.None;
        
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (parms.target is not Map map)
                return false;
            
            Plant matchingPlant = PlantFinder.TryGetSpecificPlantWithCriteriaOnMap(
                map, plantDef =>
                {
                    ModExtension_InsectDamage mE_ID = plantDef
                        .GetModExtension<ModExtension_InsectDamage>();
                    
                    if (mE_ID?.insectThreats == null)
                        return false;
                    
                    foreach (InsectDamage threat in mE_ID.insectThreats)
                    {
                        if (threat.infestationCategory == InfestationCategory)
                            return true;
                    }
                    
                    return false;
                });
            
            if (matchingPlant == null) 
                return false;
            
            // debugging, remove later
            Logging.LogIncidentCauseData(def.defName, matchingPlant);
            
            return true;
        }
    }
}