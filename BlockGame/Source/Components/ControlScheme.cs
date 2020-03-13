using Nez;
using System;

namespace BlockGame.Source.Components {
	public abstract class ControlScheme : Component {
		public VirtualButton LMove { get; protected set; }
		public VirtualButton RMove { get; protected set; }
		public VirtualButton LRotate { get; protected set; }
		public VirtualButton RRotate { get; protected set; }
		public VirtualButton Hold { get; protected set; }
		public VirtualButton SoftDrop { get; protected set; }
		public VirtualButton HardDrop { get; protected set; }
		/// <summary>
		/// Negative values correspond to left, positive values correspond to right
		/// </summary>
		public VirtualIntegerAxis MoveAxis { get; protected set; }

		protected ControlScheme() {
			LMove = new VirtualButton();
			RMove = new VirtualButton();
			LRotate = new VirtualButton();
			RRotate = new VirtualButton();
			Hold = new VirtualButton();
			SoftDrop = new VirtualButton();
			HardDrop = new VirtualButton();
			MoveAxis = new VirtualIntegerAxis(new RepeatingButtonAxis(LMove, RMove));
		}

		public class RepeatingButtonAxis : VirtualAxis.Node {
			public override float Value => _value;
			float _value;
			bool towardsPositive;

			public VirtualButton Positive;
			public VirtualButton Negative;

			public RepeatingButtonAxis(VirtualButton negative, VirtualButton positive) {
				Negative = negative;
				Positive = positive;
			}

			public override void Update() {
				if (Positive.IsDown && Negative.IsDown) {
					// new press means change the direction accordingly
					if (Positive.IsPressed && !Positive.IsRepeating)
						towardsPositive = true;
					else if (Negative.IsPressed && !Negative.IsRepeating)
						towardsPositive = false;
					_value = towardsPositive ? Positive.IsPressed ? 1 : 0 : Negative.IsPressed ? -1 : 0;
				} else if (Positive.IsPressed && !Negative.IsDown) {
					_value = 1;
				} else if (Negative.IsPressed && !Positive.IsDown) {
					_value = -1;
				} else {
					_value = 0;
				}
			}
		}
	}
}
