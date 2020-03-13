using BlockGame.Source;
using BlockGame.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using System.Diagnostics;

namespace BlockGame {
	public class BlockGame : Core {

		public BlockGame() : base(windowTitle: Constants.name) {
			System.Environment.SetEnvironmentVariable("FNA_OPENGL_BACKBUFFER_SCALE_NEAREST", "1");
		}

		protected override void Initialize() {
			base.Initialize();
			Scene = new SceneCoop();

#if DEBUG
			//Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
			var imGuiManager = new ImGuiManager();
			RegisterGlobalManager(imGuiManager);
#endif
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}
	}
}
