using System;
using System.Collections.Generic;
using System.Text;
using BlockGame.Source;
using BlockGame.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace BlockGame {
	public class BlockGame : Core {

		public BlockGame() : base(windowTitle: Constants.name) { }

		protected override void Initialize() {
			base.Initialize();
			Scene = new SceneCoop();
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}
	}
}
