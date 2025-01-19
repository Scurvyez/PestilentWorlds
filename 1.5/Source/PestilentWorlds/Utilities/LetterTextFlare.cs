using System.Collections.Generic;
using Verse;

namespace PestilentWorlds
{
    [StaticConstructorOnStartup]
    public static class LetterTextFlare
    {
        public static readonly HashSet<string> GeneralFeedersOptions =
        [
            "PW_GeneralFeeders_LT1".Translate(),
            "PW_GeneralFeeders_LT2".Translate(),
            "PW_GeneralFeeders_LT3".Translate(),
            "PW_GeneralFeeders_LT4".Translate(),
            "PW_GeneralFeeders_LT5".Translate()
        ];
    }
}
