using RimWorld;
using Verse;

namespace PestilentWorlds
{
    [DefOf]
    public class PWDefOf
    {
        public static IncidentDef PW_GeneralFeeders;
        
        static PWDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PWDefOf));
        }
    }   
}