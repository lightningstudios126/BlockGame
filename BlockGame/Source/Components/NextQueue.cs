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
			this.generator = randomizer(options.Length).GetEnumerator();
			this.capacity = capacity;
			this.options = options;
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


		public override float Width => 1000;
		public override float Height => 1000;
		public override void Render(Batcher batcher, Camera camera) {
			//batcher.DrawRect(Transform.Position, 120, 360, Color.Linen);
			Point offset = new Point(0, 0);
			foreach (var def in queue) {
				foreach (Point point in def.shape) {
					DrawTile(batcher, point - offset, def.type);
				}
				offset.Y += 3;
			}
			batcher.DrawCircle(Transform.Position, 3, Color.Red);
		}

		public void DrawTile(Batcher batcher, Point gridLocation, Tile tile) {
			Point offset = new Point(Constants.pixelsPerTile * gridLocation.X, -Constants.pixelsPerTile * gridLocation.Y);
			var texture = Entity.Scene.Content.LoadTexture(tile.spriteLocation);
			batcher.Draw(texture, new Rectangle((offset.ToVector2() + Transform.Position).RoundToPoint(), new Point(Constants.pixelsPerTile)), tile.color);
		}
	}
}
