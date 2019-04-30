using Rocket.API;
using System.Collections.Generic;

namespace Game4Freak.Inkognito
{
    public class InkognitoConfiguration : IRocketPluginConfiguration
    {
        public List<string> InkognitoNames;

        public void LoadDefaults()
        {
            InkognitoNames = new List<string> { "Fancy Name", "a Player" };
        }
    }
}
