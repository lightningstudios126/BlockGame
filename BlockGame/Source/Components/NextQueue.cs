using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockGame.Source.Blocks;
using Microsoft.Xna.Framework;
using Nez;

namespace BlockGame.Source.Components {
	class NextQueue : UIPanel {
		private readonly int capacity;
		private readonly IEnumerator<int> generator;
		private readonly PieceDefinition[] options;
		private readonly Queue<PieceDefinition> queue;

		public NextQueue(Randomizers.Randomizer randomizer, int capacity = 5, params PieceDefinition[] options) {
			this.capacity = capacity;
			this.options = options;
			this.generator = randomizer(options.Length).GetEnumerator();
			this.queue = new Queue<PieceDefinition>(capacity);
			this.BackgroundColour = new Color(40, 40, 40);
			InitialLoad();
		}

		public IEnumerator<PieceDefinition> Generate() {
			while (true) yield return GetNext();
		}

		public PieceDefinition GetNext() {
			generator.MoveNext();
			queue.Enqueue(options[generator.Current]);
			return queue.Dequeue();
		}

		private void InitialLoad() {
			for (int i = 0; i < capacity; i++) {
				generator.MoveNext();
				queue.Enqueue(options[generator.Current]);
			}
		}

		public override float Width => 4 * Constants.pixelsPerTile + 2 * padding;
		public override float Height => padding + capacity * (3 * Constants.pixelsPerTile) + Constants.pixelsPerTile + padding;
		private readonly int padding = 10;
		public override void Render(Batcher batcher, Camera camera) {
			batcher.DrawHollowRect(Transform.Position, Width, Height, OutlineColour, 6);
			batcher.DrawRect(Transform.Position, Width, Height, BackgroundColour);
			Point offset = new Point(0, -2);
			foreach (var pieceDef in queue) {
				offset.Y += pieceDef.height + 1;
				foreach (Point point in pieceDef.shape) {
					Utilities.DrawTile(batcher, point - offset, new Point(padding + Constants.pixelsPerTile, padding + Constants.pixelsPerTile) + Transform.Position.ToPoint(), pieceDef.type);
				}
			}
			batcher.DrawCircle(Transform.Position, 3, Color.Red);
		}
	}
}
