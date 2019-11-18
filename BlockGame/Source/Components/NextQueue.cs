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
	class NextQueue : RenderableComponent {
		int capacity;
		IEnumerator<int> generator;
		TileGroupDefinition[] options;
		Queue<TileGroupDefinition> queue;

		public NextQueue(Randomizers.Randomizer randomizer, int capacity = 5, params TileGroupDefinition[] options) {
			this.capacity = capacity;
			this.options = options;
			this.generator = randomizer(options.Length).GetEnumerator();
			this.queue = new Queue<TileGroupDefinition>(capacity);
			InitialLoad();
		}

		public IEnumerator<TileGroupDefinition> Generate() {
			while (true) yield return GetNext();
		}

		public TileGroupDefinition GetNext() {
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
		public override float Height => padding + capacity * (5 * Constants.pixelsPerTile) + padding;
		int padding = 10;
		public override void Render(Batcher batcher, Camera camera) {
			batcher.DrawRect(Transform.Position, Width, Height, Color.Linen);
			Point offset = new Point(0, -2);
			foreach (var def in queue) {
				offset.Y += def.height + 2;
				foreach (Point point in def.shape) {
					DrawTile(batcher, point - offset, new Point(padding + Constants.pixelsPerTile, padding + Constants.pixelsPerTile), def.type);
				}
			}
			batcher.DrawCircle(Transform.Position, 3, Color.Red);
		}

		public void DrawTile(Batcher batcher, Point gridOffset, Point worldOffset, Tile tile) {
			Point offset = new Point(Constants.pixelsPerTile * gridOffset.X, -Constants.pixelsPerTile * gridOffset.Y);
			var texture = Entity.Scene.Content.LoadTexture(tile.spriteLocation);
			batcher.Draw(texture, new Rectangle(Transform.Position.RoundToPoint() + worldOffset + offset, new Point(Constants.pixelsPerTile)), tile.color);
		}
	}
}
