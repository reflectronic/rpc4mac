using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Dialogs;

namespace Rpc4Mac
{
    public class GeneralOptionsPanel : OptionsPanel
    {
        CheckButton enableButton = new CheckButton("Enable rich presence") { Xalign = 0, Active = Options.EnableRichPresence.Value };
        CheckButton showFile = new CheckButton("Show current file") { Xalign = 0, Active = Options.ShowCurrentFile.Value };
        CheckButton showProject = new CheckButton("Show current project") { Xalign = 0, Active = Options.ShowCurrentProject.Value };
        CheckButton showElapsed = new CheckButton("Show elapsed time") { Xalign = 0, Active = Options.ShowElapsed.Value };
        Entry appId = new Entry(Options.ApplicationId.Value);

        public override Control CreatePanelWidget()
        {
            var vbox = new VBox { Spacing = 6 };

            var sectionLabel = new Label("<b>General</b>") { Xalign = 0, UseMarkup = true };
            vbox.PackStart(sectionLabel, false, false, 0);

            vbox.PackStart(enableButton, false, false, 0);
            vbox.PackStart(showFile, false, false, 0);
            vbox.PackStart(showProject, false, false, 0);
            vbox.PackStart(showElapsed, false, false, 0);

            var id = new HBox { BorderWidth = 10, Spacing = 6 };
            var idLabel = new Label("Application ID:") { Xalign = 0 };
            id.PackStart(idLabel, false, false, 0);
            id.PackStart(appId, true, true, 0);

            vbox.PackStart(id, false, false, 0);
            vbox.ShowAll();
            return vbox;
        }

        public override void ApplyChanges()
        {
            Options.EnableRichPresence.Value = enableButton.Active;
            Options.ShowCurrentFile.Value = showFile.Active;
            Options.ShowCurrentProject.Value = showProject.Active;
            Options.ShowElapsed.Value = showElapsed.Active;
            Options.ApplicationId.Value = appId.Text;
        }
    }
}
