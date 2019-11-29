using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockGame.Source.Blocks;
using Microsoft.Xna.Framework;
using Nez;

namespace BlockGame.Source.Components {
	class HoldQueue : RenderableComponent {
		PieceDefinition heldPiece;
		bool isLocked = false;

		public bool Swap(PieceDefinition toHold, out PieceDefinition swapped) {
			if (isLocked) {
				swapped = null;
				return false;
			} else {
				swapped = heldPiece;
				heldPiece = toHold;
				isLocked = true;
				return true;
			}
		}

		public void Unlock() {
			isLocked = false;
		}
		public override float Width => 4 * Constants.pixelsPerTile + 2 * padding;
		public override float Height => padding + (5 * Constants.pixelsPerTile) + padding;
		int padding = 10;
		public override void Render(Batcher batcher, Camera camera) {
			batcher.DrawRect(Transform.Position, Width, Height, Color.Linen);
			Point offset = new Point(0, 0);
			if (heldPiece != null) {
				foreach (Point point in heldPiece.shape) {
					Utilities.DrawTile(batcher, point - offset, new Point(padding + Constants.pixelsPerTile, padding + Constants.pixelsPerTile) + Transform.Position.ToPoint(), heldPiece.type);				
				}
			}

			batcher.DrawCircle(Transform.Position, 3, Color.Red);
		}
	}
}
