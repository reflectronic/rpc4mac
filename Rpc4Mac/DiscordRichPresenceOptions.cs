using System;
using AppKit;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Dialogs;

namespace Rpc4Mac
{
    public class DiscordRichPresenceOptions : OptionsPanel
    {
        CheckButton enableButton = new CheckButton("Enable rich presence") { Xalign = 0 };
        CheckButton showSolution = new CheckButton("Show current file") { Xalign = 0 };
        CheckButton showProject = new CheckButton("Show current project") { Xalign = 0 };
        CheckButton showElapsed = new CheckButton("Show elapsed time") { Xalign = 0 };
        Entry customId = new Entry("700501887937544262");

        public override void ApplyChanges()
        {

        }

        public override Control CreatePanelWidget()
        {
            var vbox = new VBox { Spacing = 6 };

            var sectionLabel = new Label("<b>General</b>") { Xalign = 0, UseMarkup = true };
            vbox.PackStart(sectionLabel, false, false, 0);

            vbox.PackStart(enableButton, false, false, 0);
            vbox.PackStart(showSolution, false, false, 0);
            vbox.PackStart(showProject, false, false, 0);
            vbox.PackStart(showElapsed, false, false, 0);

            var id = new HBox { BorderWidth = 10, Spacing = 6 };
            var idLabel = new Label("Application ID:") { Xalign = 0 };
            id.PackStart(idLabel, false, false, 0);
            id.PackStart(customId, true, true, 0);

            vbox.PackStart(id, false, false, 0);
            vbox.ShowAll();
            return vbox;
        }
    }
}
