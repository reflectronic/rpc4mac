using System;
using System.Runtime.CompilerServices;
using MonoDevelop.Core;

namespace Rpc4Mac
{
    public static class DiscordRichPresenceOptions
    {
        public static readonly ConfigurationProperty<string> ApplicationId = ConfigurationProperty.Create(CreateName(), "700501887937544262");
        public static readonly ConfigurationProperty<bool> EnableRichPresence = ConfigurationProperty.Create(CreateName(), true);
        public static readonly ConfigurationProperty<bool> ShowCurrentFile = ConfigurationProperty.Create(CreateName(), true);
        public static readonly ConfigurationProperty<bool> ShowCurrentProject = ConfigurationProperty.Create(CreateName(), true);
        public static readonly ConfigurationProperty<bool> ShowElapsed = ConfigurationProperty.Create(CreateName(), true);

        static string CreateName([CallerMemberName] string name = "") => "Rpc4Mac." + name;
    }
}
