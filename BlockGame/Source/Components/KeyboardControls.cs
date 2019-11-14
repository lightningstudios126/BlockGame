using System;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace BlockGame.Source.Components {
	class KeyboardControls : Controls {
		/// <summary>
		/// The delay before the input starts repeats in milliseconds
		/// </summary>
		public float delayedAutoShift {
			get; private set;
		}
		/// <summary>
		/// The delay between each repeat in milliseconds
		/// </summary>
		public float autoRepeatRate {
			get; private set;
		}

		protected override void _Initialize() {
			delayedAutoShift = 133;
			autoRepeatRate = 5;
			LMove.AddKeyboardKey(Keys.Left);
			RMove.AddKeyboardKey(Keys.Right);
			LRotate.AddKeyboardKey(Keys.Z);
			RRotate.AddKeyboardKey(Keys.X);
			Hold.AddKeyboardKey(Keys.C);
			SoftDrop.AddKeyboardKey(Keys.Down);
			HardDrop.AddKeyboardKey(Keys.Space);
			LMove.SetRepeat(delayedAutoShift / 1000, autoRepeatRate / 1000);
			RMove.SetRepeat(delayedAutoShift / 1000, autoRepeatRate / 1000);
		}
	}
}
