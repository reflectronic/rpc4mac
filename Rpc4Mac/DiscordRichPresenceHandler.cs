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
                if (Options.EnableRichPresence)
                {
                    client = new DiscordRpcClient(Options.ApplicationId.Value);
                    client.Initialize();

                    timer = new Timer(delegate
                    {
                        if (Options.EnableRichPresence.Value)
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
            Options.EnableRichPresence.Changed += OnEnabledChanged;

            Options.ApplicationId.Changed += (s, e) =>
            {
                if (client is null)
                    return;

                var newClient = new DiscordRpcClient(Options.ApplicationId.Value);
                newClient.Initialize();

                var oldClient = Interlocked.Exchange(ref client, newClient);
                oldClient.Dispose();
            };

            IdeApp.FocusIn += Update;
            Options.ShowCurrentFile.Changed += Update;
            Options.ShowCurrentProject.Changed += Update;
            Options.ShowElapsed.Changed += Update;

            IdeApp.Exited += (s, e) => client.Dispose();

            UpdateRichPresence(null, null);
        }

        void UpdateRichPresence(Document document, WorkspaceObject workspace)
        {
            presence = new RichPresence()
            {
                Assets = new Assets()
                {
                    LargeImageKey = document?.FileName.Extension switch
                    {
                        var _ when !Options.ShowCurrentFile.Value => "unknown",
                        ".cs" => "csharp",
                        ".fs" => "fsharp",
                        _ => "unknown"
                    },
                    SmallImageKey = "vs",
                    SmallImageText = $"Visual Studio for Mac {IdeApp.Version}"
                },
                Details = Options.ShowCurrentFile.Value ? document?.FileName.FileName : null,
                State = Options.ShowCurrentProject.Value ? workspace?.Name.Insert(0, "Developing ") : null,
                Timestamps = Options.ShowElapsed ? new Timestamps(startSession) : null
            };

            client.SetPresence(presence);
        }
    }
}
