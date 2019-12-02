using Microsoft.Xna.Framework;
using Nez;
using Nez.Console;

namespace BlockGame.Source.Components {
	class MouseLocator : RenderableComponent, IUpdatable {
		public MouseLocator() {
			RenderLayer = -100;
		}
		public override float Height => 20;
		public override float Width => 20;
		public override void Render(Batcher batcher, Camera camera) {
			batcher.DrawString(Graphics.Instance.BitmapFont, Input.MousePosition.ToString(), Input.MousePosition, Color.White);
		}

		public void Update() {
			if (Input.LeftMouseButtonPressed) {
				Debug.Log(Input.MousePosition);
				DebugConsole.Instance.Log("Mouse position: {0}", Input.MousePosition);
			}
		}
	}
}
