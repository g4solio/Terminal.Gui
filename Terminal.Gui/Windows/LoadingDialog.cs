using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Terminal.Gui.Windows {
	public struct LoadingPayload {
		public Task WorkToLoadFor;

		public IAnimation AnimationToDisplay;

	};

	public class LoadingDialog : Dialog {

		readonly LoadingPayload _loadingPayload;

		Label lblLoadingMessage;

		public LoadingDialog (LoadingPayload loadingPayload)
		{
			this._loadingPayload = loadingPayload;
			Init ();
		}

		public LoadingDialog (ustring title, LoadingPayload loadingPayload) : base (title, Array.Empty<Button>())
		{
			this._loadingPayload = loadingPayload;
			Init ();
		}

		public LoadingDialog (ustring title, int width, int height, LoadingPayload loadingPayload) : base (title, width, height, Array.Empty<Button> ())
		{
			this._loadingPayload = loadingPayload;
			Init ();
		}

		public void UpdateLblMessage(ustring newMessage)
		{
			lblLoadingMessage.Text = newMessage;
		}

		private void Init()
		{
			BindDialogLifeToWorkerTask ();
			InitMessageLabel ();
			BindAnimationFrames ();
		}

		private void BindDialogLifeToWorkerTask ()
		{
			var dialogReference = this;
			_loadingPayload.WorkToLoadFor.ContinueWith (t => Application.RequestStop (dialogReference));
		}

		private void InitMessageLabel()
		{
			lblLoadingMessage = new Label () {
				LayoutStyle = LayoutStyle.Computed,
				TextAlignment = TextAlignment.Centered,
				X = Pos.Center (),
				Y = Pos.Center (),
				Width = Dim.Fill (),
				Height = Dim.Fill (1),
				AutoSize = false
			};
			this.Add (lblLoadingMessage);
		}

		private void BindAnimationFrames()
		{
			Application.MainLoop.AddIdle (_loadingPayload.AnimationToDisplay.Tick);
		}


	}
}
