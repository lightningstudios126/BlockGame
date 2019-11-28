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

		public KeyboardControls(Keys lMov = Keys.Left, Keys rMov = Keys.Right, Keys lRot = Keys.Z, Keys rRot = Keys.X, Keys hold = Keys.C, Keys softDrop = Keys.Down, Keys hardDrop = Keys.Space, float delayedAutoShift = 133, float autoRepeatRate = 5) : base() {
			LMove.AddKeyboardKey(lMov);
			RMove.AddKeyboardKey(rMov);
			LRotate.AddKeyboardKey(lRot);
			RRotate.AddKeyboardKey(rRot);
			Hold.AddKeyboardKey(hold);
			SoftDrop.AddKeyboardKey(softDrop);
			HardDrop.AddKeyboardKey(hardDrop);
			LMove.SetRepeat(delayedAutoShift / 1000, autoRepeatRate / 1000);
			RMove.SetRepeat(delayedAutoShift / 1000, autoRepeatRate / 1000);
		}
	}
}
