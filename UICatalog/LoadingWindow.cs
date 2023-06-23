using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using Terminal.Gui.Windows;

namespace UICatalog {

	[ScenarioMetadata (Name: "Loading Window", Description: "Dialog loading animation.")]
	[ScenarioCategory ("Controls")]
	public class LoadingWindow : Scenario{


		public override void Setup ()
		{
			base.Setup ();

			var textLabel = new Label ("Delay time picker (ms)") {
				X = Pos.Center(),
				Y = Pos.Top(Win) + 3
			};
			Win.Add (textLabel);
			var delayPicker = new Terminal.Gui.TextField ("Delay picker") {
				X = Pos.Center(),
				Y = Pos.Top(Win) + 5,
				Text = "10000"
			};
			Win.Add (delayPicker);

			var defaultButton = new Button ("Pop Dialog") {
				X = Pos.Center (),
				Y = Pos.Bottom (Win) - 9,
				IsDefault = true,
			};
			defaultButton.Clicked += () => ShowLoadingDialog(int.Parse(delayPicker.Text.ToString()));
			Win.Add (defaultButton);

		}

		private void ShowLoadingDialog(int delay)
		{
			MessageBox.LoadingDialog ("10 second loading task", "...loading", Task.Delay (delay), new [] { "←", "↖", "↑", "↗", "→", "↘", "↓", "↙" });
		}

	}
}
