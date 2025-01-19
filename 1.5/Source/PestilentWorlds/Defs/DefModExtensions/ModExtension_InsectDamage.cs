using System.Collections.Generic;
using Verse;

namespace PestilentWorlds
{
    public class ModExtension_InsectDamage : DefModExtension
    {
        public List<InsectDamage> insectThreats;
    }
    
    public struct InsectDamage()
    {
        public InfestationCategory infestationCategory = InfestationCategory.None;
        public FloatRange beetleDamage = FloatRange.One;
        public float beetleDamageChance = 0.05f;
    }
}
