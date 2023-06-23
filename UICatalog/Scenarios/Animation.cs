using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using Terminal.Gui.Windows;
using Attribute = Terminal.Gui.Attribute;

namespace UICatalog.Scenarios {
	[ScenarioMetadata (Name: "Animation", Description: "Demonstration of how to render animated images with threading.")]
	[ScenarioCategory ("Colors")]
	public class Animation : Scenario
	{
		private bool isDisposed;

		public override void Setup ()
		{
			base.Setup ();


			var imageView = new ImageView () {
				Width = Dim.Fill(),
				Height = Dim.Fill()-2,
			};

			Win.Add (imageView);

			var lbl = new Label("Image by Wikiscient"){
				Y = Pos.AnchorEnd(2)
			};
			Win.Add(lbl);

			var lbl2 = new Label("https://commons.wikimedia.org/wiki/File:Spinning_globe.gif"){
				Y = Pos.AnchorEnd(1)
			};
			Win.Add(lbl2);

			var dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
			
			var f = new FileInfo(
				Path.Combine(dir.FullName,"Scenarios","Spinning_globe_dark_small.gif"));
			if(!f.Exists)
			{
				MessageBox.ErrorQuery("Could not find gif","Could not find "+ f.FullName,"Ok");
				return;
			}

			imageView.SetImage(Image.Load<Rgba32> (File.ReadAllBytes (f.FullName)));

			//Task.Run(()=>{
			//	while(!isDisposed)
			//	{
			//		// When updating from a Thread/Task always use Invoke
			//		Application.MainLoop.Invoke(()=>
			//		{
			//			imageView.NextFrame();
			//			imageView.SetNeedsDisplay();
			//		});

			//		Task.Delay(100).Wait();
			//	}
			//});
		}

		protected override void Dispose(bool disposing)
		{
			isDisposed = true;
			base.Dispose(disposing);
		}

		// This is a C# port of https://github.com/andraaspar/bitmap-to-braille by Andraaspar

		/// <summary>
		/// Renders an image as unicode Braille.
		/// </summary>
		public class BitmapToBraille
		{

			public const int CHAR_WIDTH = 2;
			public const int CHAR_HEIGHT = 4;

			const string CHARS = " ⠁⠂⠃⠄⠅⠆⠇⡀⡁⡂⡃⡄⡅⡆⡇⠈⠉⠊⠋⠌⠍⠎⠏⡈⡉⡊⡋⡌⡍⡎⡏⠐⠑⠒⠓⠔⠕⠖⠗⡐⡑⡒⡓⡔⡕⡖⡗⠘⠙⠚⠛⠜⠝⠞⠟⡘⡙⡚⡛⡜⡝⡞⡟⠠⠡⠢⠣⠤⠥⠦⠧⡠⡡⡢⡣⡤⡥⡦⡧⠨⠩⠪⠫⠬⠭⠮⠯⡨⡩⡪⡫⡬⡭⡮⡯⠰⠱⠲⠳⠴⠵⠶⠷⡰⡱⡲⡳⡴⡵⡶⡷⠸⠹⠺⠻⠼⠽⠾⠿⡸⡹⡺⡻⡼⡽⡾⡿⢀⢁⢂⢃⢄⢅⢆⢇⣀⣁⣂⣃⣄⣅⣆⣇⢈⢉⢊⢋⢌⢍⢎⢏⣈⣉⣊⣋⣌⣍⣎⣏⢐⢑⢒⢓⢔⢕⢖⢗⣐⣑⣒⣓⣔⣕⣖⣗⢘⢙⢚⢛⢜⢝⢞⢟⣘⣙⣚⣛⣜⣝⣞⣟⢠⢡⢢⢣⢤⢥⢦⢧⣠⣡⣢⣣⣤⣥⣦⣧⢨⢩⢪⢫⢬⢭⢮⢯⣨⣩⣪⣫⣬⣭⣮⣯⢰⢱⢲⢳⢴⢵⢶⢷⣰⣱⣲⣳⣴⣵⣶⣷⢸⢹⢺⢻⢼⢽⢾⢿⣸⣹⣺⣻⣼⣽⣾⣿";

			public int WidthPixels {get; }
			public int HeightPixels { get; }

			public Func<int,int,bool> PixelIsLit {get;}

			public BitmapToBraille (int widthPixels, int heightPixels, Func<int, int, bool> pixelIsLit)
			{
				WidthPixels = widthPixels;
				HeightPixels = heightPixels;
				PixelIsLit = pixelIsLit;
			}

			public string GenerateImage() {
				int imageHeightChars = (int) Math.Ceiling((double)HeightPixels / CHAR_HEIGHT);
				int imageWidthChars = (int) Math.Ceiling((double)WidthPixels / CHAR_WIDTH);

				var result = new StringBuilder();

				for (int y = 0; y < imageHeightChars; y++) {
					
					for (int x = 0; x < imageWidthChars; x++) {
						int baseX = x * CHAR_WIDTH;
						int baseY = y * CHAR_HEIGHT;

						int charIndex = 0;
						int value = 1;

						for (int charX = 0; charX < CHAR_WIDTH; charX++) {
							for (int charY = 0; charY < CHAR_HEIGHT; charY++) {
								int bitmapX = baseX + charX;
								int bitmapY = baseY + charY;
								bool pixelExists = bitmapX < WidthPixels && bitmapY < HeightPixels;

								if (pixelExists && PixelIsLit(bitmapX, bitmapY)) {
									charIndex += value;
								}
								value *= 2;
							}
						}

						result.Append(CHARS[charIndex]);
					}
					result.Append('\n');
				}
				return result.ToString().TrimEnd();
			}  
		}

		class AnimationRgba32 : Animation<Image<Rgba32>> {
			public AnimationRgba32 (Image<Rgba32> [] frames, float timeToFullAnimate, Animate animateAction) : base (frames, timeToFullAnimate, animateAction)
			{
			}
		}

		class ImageView : View {

			AnimationRgba32 animation;
			private Hashtable brailleCache;

			Rect oldSize = Rect.Empty;


			internal void SetImage (Image<Rgba32> image)
			{
				var frameCount = image.Frames.Count;

				var fullResImages = new Image<Rgba32>[frameCount];
				brailleCache = new Hashtable ();

				for(int i=0;i<frameCount-1;i++)
				{
					fullResImages[i] = image.Frames.ExportFrame(0);
				}
				fullResImages[frameCount-1] = image;

				animation = new AnimationRgba32 (fullResImages, 2.4f, UpdateBraileFrame);
				Application.MainLoop.AddIdle (animation.Tick);

			}

			private string BraileFrame = string.Empty;

			void UpdateBraileFrame(Image<Rgba32> frame)
			{

				if (oldSize != this.Bounds) {
					// Invalidate cached images now size has changed
					brailleCache.Clear ();
					oldSize = this.Bounds;
				}

				if (brailleCache.ContainsKey (frame)) {
					BraileFrame = brailleCache [frame].ToString ();
				}else {
					// keep aspect ratio
					var newSize = Math.Min (this.Bounds.Width, this.Bounds.Height);

					// generate one
					var scaledImg = frame.Clone (
						x => x.Resize (
								newSize * BitmapToBraille.CHAR_HEIGHT,
								newSize * BitmapToBraille.CHAR_HEIGHT));

					brailleCache [frame] = BraileFrame = GetBraille (scaledImg);
				}




				this.SetChildNeedsDisplay ();
				this.SetNeedsDisplay ();
			}

			public override void Redraw (Rect bounds)
			{
				base.Redraw (bounds);

				var lines = BraileFrame.Split ('\n');

				for (int y = 0; y < lines.Length; y++) {
					var line = lines [y];
					for (int x = 0; x < line.Length; x++) {
						AddRune (x, y, line [x]);
					}
				}

			}

			private string GetBraille (Image<Rgba32> img)
			{
				var braille = new BitmapToBraille(
					img.Width,
					img.Height,
					(x,y)=>IsLit(img,x,y));

				return braille.GenerateImage();
			}

			private bool IsLit (Image<Rgba32> img, int x, int y)
			{
				var rgb = img[x,y];
				return rgb.R + rgb.G + rgb.B > 50;
			}
		}
	}
}