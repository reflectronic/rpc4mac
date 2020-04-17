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
            IdeApp.Workbench.ActiveDocumentChanged += (sender, e) => UpdateRichPresence(e.Document, e.Document?.Owner ?? IdeApp.Workspace);

            IdeApp.FocusIn += delegate
            {
                var doc = IdeApp.Workbench.ActiveDocument;
                UpdateRichPresence(doc, doc?.Owner);
            };

            IdeApp.Workspace.FirstWorkspaceItemOpened += (sender, e) => startSession = DateTime.UtcNow;
            IdeApp.Workspace.LastWorkspaceItemClosed += delegate
            {
                UpdateRichPresence(null, null);
                startSession = DateTime.UtcNow;
            };

            startSession = DateTime.UtcNow;

            client = new DiscordRpcClient("700501887937544262");
            client.Initialize();

            timer = new Timer(_ => client.SetPresence(presence), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            IdeApp.Exited += delegate
            {
                client.Dispose();
            };

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
                        ".cs" => "csharp",
                        ".fs" => "fsharp",
                        _ => "unknown"
                    },
                    SmallImageKey = "vs",
                    SmallImageText = $"Visual Studio for Mac {IdeApp.Version}"
                },
                Details = document?.FileName.FileName,
                State = workspace is null ? null : "Developing " + workspace.Name,
                Timestamps = new Timestamps(startSession)
            };

            client.SetPresence(presence);
        }
    }
}
