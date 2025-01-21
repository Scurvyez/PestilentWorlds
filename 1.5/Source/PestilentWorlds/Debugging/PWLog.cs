using UnityEngine;
using Verse;

namespace PestilentWorlds
{
    public class PWLog
    {
        public static readonly Color ErrorMsgCol = new (0.4f, 0.54902f, 1.0f);
        public static readonly Color WarningMsgCol = new (0.70196f, 0.4f, 1.0f);
        public static readonly Color MessageMsgCol = new (0.4f, 1.0f, 0.54902f);
        
        public static void Error(string msg)
        {
            Log.Error("[Pestilent Worlds] ".Colorize(ErrorMsgCol) + msg);
        }
        
        public static void Warning(string msg)
        {
            Log.Warning("[Pestilent Worlds] ".Colorize(WarningMsgCol) + msg);
        }
        
        public static void Message(string msg)
        {
            Log.Message("[Pestilent Worlds] ".Colorize(MessageMsgCol) + msg);
        }
    }
}