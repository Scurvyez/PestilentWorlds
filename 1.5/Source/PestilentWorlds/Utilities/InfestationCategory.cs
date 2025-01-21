

namespace PestilentWorlds
{
    public enum InfestationCategory : byte
    {
        None,
        WoodBorers,         // Insects like emerald ash borers and pine beetles that tunnel into wood
        SapFeeders,         // Insects like aphids and spotted lanternflies that feed on plant sap
        LeafDefoliators,    // Insects like gypsy moths that strip leaves from plants
        RootFeeders,        // Pests like Japanese beetle larvae that feed on roots
        LeafMiners,         // Insects that create "mines" in leaves
        FungusSpreaders,    // Insects that introduce fungal pathogens (e.g., pine beetles)
        GeneralFeeders      // Broad-spectrum feeders like Japanese beetles adults
    }
}
