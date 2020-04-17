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

        Timer timer;

        protected override void Run()
        {
            IdeApp.Workbench.ActiveDocumentChanged += (sender, e) => UpdateRichPresence(e.Document, e.Document?.Owner);

            IdeApp.FocusIn += delegate
            {
                var doc = IdeApp.Workbench.ActiveDocument;
                UpdateRichPresence(doc, doc?.Owner);
            };

            IdeApp.Workspace.LastWorkspaceItemClosed += (sender, e) => UpdateRichPresence(null, null);

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
                    SmallImageKey = "vs"
                },
                Details = document?.FileName.FileName ?? "No file",
                State = workspace?.Name ?? "No workspace",
                Timestamps = new Timestamps(DateTime.UtcNow),
            };

            client.SetPresence(presence);
        }
    }
}
