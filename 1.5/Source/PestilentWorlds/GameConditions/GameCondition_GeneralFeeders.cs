

namespace PestilentWorlds
{
    public class GameCondition_GeneralFeeders : GameCondition_InsectInfestation
    {
        protected override InfestationCategory InfestationCategory => InfestationCategory.GeneralFeeders;

        public override string LetterText => 
            TryGetRandomLetterText(LetterTextFlare.GeneralFeedersOptions) + "\n\n" + base.LetterText;
    }
}
