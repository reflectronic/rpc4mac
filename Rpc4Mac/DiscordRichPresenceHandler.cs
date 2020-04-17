using System;
using System.Threading;
using DiscordRPC;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace Rpc4Mac
{
    public class DiscordRichPresenceHandler : CommandHandler
    {
        DiscordRpcClient client;

        RichPresence presence;
        DateTime startSession;

        Timer timer;

        protected override void Run()
        {
            void Update(object sender, EventArgs e)
            {
                var doc = IdeApp.Workbench.ActiveDocument;
                UpdateRichPresence(doc, doc?.Owner);
            }

            void OnEnabledChanged(object sender, EventArgs e)
            {
                if (DiscordRichPresenceOptions.EnableRichPresence)
                {
                    client = new DiscordRpcClient(DiscordRichPresenceOptions.ApplicationId.Value);
                    client.Initialize();

                    timer = new Timer(delegate
                    {
                        if (DiscordRichPresenceOptions.EnableRichPresence.Value)
                            client.SetPresence(presence);
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
                }
                else
                {
                    timer.Dispose();
                    timer = null;
                    client.ClearPresence();
                    client.Dispose();
                    client = null;
                }
            }

            OnEnabledChanged(null, null);

            startSession = DateTime.UtcNow;

            IdeApp.Workspace.FirstWorkspaceItemOpened += (sender, e) => startSession = DateTime.UtcNow;
            IdeApp.Workspace.LastWorkspaceItemClosed += delegate
            {
                UpdateRichPresence(null, null);
                startSession = DateTime.UtcNow;
            };

            IdeApp.Workbench.ActiveDocumentChanged += (sender, e) => UpdateRichPresence(e.Document, e.Document?.Owner ?? IdeApp.Workspace);
            DiscordRichPresenceOptions.EnableRichPresence.Changed += OnEnabledChanged;

            DiscordRichPresenceOptions.ApplicationId.Changed += (s, e) =>
            {
                if (client is null)
                    return;

                var newClient = new DiscordRpcClient(DiscordRichPresenceOptions.ApplicationId.Value);
                newClient.Initialize();

                var oldClient = Interlocked.Exchange(ref client, newClient);
                oldClient.Dispose();
            };

            IdeApp.FocusIn += Update;
            DiscordRichPresenceOptions.ShowCurrentFile.Changed += Update;
            DiscordRichPresenceOptions.ShowCurrentProject.Changed += Update;
            DiscordRichPresenceOptions.ShowElapsed.Changed += Update;

            IdeApp.Exited += (s, e) => client.Dispose();

            UpdateRichPresence(null, null);
        }

        void UpdateRichPresence(Document document, WorkspaceObject workspace)
        {
            var largeImageKey = (DiscordRichPresenceOptions.ShowCurrentFile.Value, DiscordRichPresenceOptions.ShowCurrentProject.Value) switch
            {
                // If the user wants to display the current file, use that to determine the icon.
                (true, _) => document?.FileName.Extension switch
                {
                    ".cs" => "csharp",
                    ".fs" => "fsharp",
                    _ => "unknown"
                },
                // Otherwise, if the user only wants to display the current project, determine that from the project type.
                (false, true) => document?.Owner.ItemDirectory.Extension switch
                {
                    ".csproj" => "csharp",
                    ".fsproj" => "fsharp",
                    _ => "unknown"
                },
                _ => "unknown"
            };

            presence = new RichPresence()
            {
                Assets = new Assets()
                {
                    LargeImageKey = largeImageKey,
                    SmallImageKey = "vs",
                    SmallImageText = $"Visual Studio for Mac {IdeApp.Version}"
                },
                Details = DiscordRichPresenceOptions.ShowCurrentFile.Value ? document?.FileName.FileName : null,
                State = DiscordRichPresenceOptions.ShowCurrentProject.Value ? workspace?.Name.Insert(0, "Developing ") : null,
                Timestamps = DiscordRichPresenceOptions.ShowElapsed ? new Timestamps(startSession) : null
            };

            client.SetPresence(presence);
        }
    }
}
