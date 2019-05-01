using Rocket.API;
using System.Collections.Generic;

namespace Game4Freak.Inkognito
{
    public class InkognitoConfiguration : IRocketPluginConfiguration
    {
        public bool InkognitoInGlobalChat;
        public bool InkognitoUseGroupPrefixAndSuffix;
        public List<string> InkognitoNames;

        public void LoadDefaults()
        {
            InkognitoInGlobalChat = true;
            InkognitoUseGroupPrefixAndSuffix = false;
            InkognitoNames = new List<string> { "Fancy Name", "a Player" };
        }
    }
}
