using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace Rpc4Mac
{
    public class DiscordRichPresenceHandler : CommandHandler
    {
        protected override void Run()
        {
            IdeApp.Workbench.ActiveDocumentChanged += (sender, e) => UpdateRichPresence(e.Document);

            IdeApp.FocusIn += (sender, e) => UpdateRichPresence(IdeApp.Workbench.ActiveDocument);

            IdeApp.Workspace.LastWorkspaceItemClosed += (sender, e) => ShowNoWorkspaceRichPresence();

            ShowNoWorkspaceRichPresence();
        }

        void ShowNoWorkspaceRichPresence()
        {

        }

        void UpdateRichPresence(Document currentDocument)
        {
            if (currentDocument is null)
            {
                // Show generic workspace info
            }

            MessageService.ShowMessage("Rich Presence Update", $"Your current document is {currentDocument?.Name ?? "None"}");
        }
    }
}
