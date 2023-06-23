using NStack;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Terminal.Gui.Windows {
	public interface IAnimation {

		bool Tick ();

	}

	public abstract class Animation<TAnimationFrameType> : IAnimation
	{
		public delegate void Animate (TAnimationFrameType frame);
		readonly TAnimationFrameType [] _frames;
		readonly double _timeToFullAnimateMs;
		readonly Animate _animateAction;

		public Animation (TAnimationFrameType [] frames, float timeToFullAnimateS, Animate animateAction)
		{
			_frames = frames;
			_timeToFullAnimateMs = timeToFullAnimateS * 1000;
			_animateAction = animateAction;

			stopwatch.Start ();
		}

		private Stopwatch stopwatch = new Stopwatch ();
		public bool Tick ()
		{
			Tick (stopwatch.ElapsedMilliseconds);
			return true;
		}

		private int lastFrameIndex = 0;

		private void Tick (long deltaMsTime)
		{

			var frameIndex = (int) Math.Floor ((deltaMsTime % _timeToFullAnimateMs) / _timeToFullAnimateMs * _frames.Length);

			if (frameIndex < 0)
				frameIndex = 0;

			if (frameIndex > _frames.Length - 1)
				frameIndex = _frames.Length - 1; 

			if (lastFrameIndex == frameIndex)
				return;

			lastFrameIndex = frameIndex;
			_animateAction (_frames [lastFrameIndex]);
		}
	}

	public class ASCIIAnimation : Animation<ustring> {
		public ASCIIAnimation (ustring [] frames, float timeToFullAnimate, Animate animateAction) : base (frames, timeToFullAnimate, animateAction)
		{
		}

		public ASCIIAnimation (string [] frames, float timeToFullAnimate, Animate animateAction) : base (frames.Select(ustring.Make).ToArray(), timeToFullAnimate, animateAction)
		{
		}
	}
}