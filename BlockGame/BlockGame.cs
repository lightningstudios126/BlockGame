using System;
using System.Collections.Generic;
using System.Text;
using BlockGame.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace BlockGame {
	public class BlockGame : Core {

		public BlockGame() : base() {
		}

		protected override void Initialize() {
			base.Initialize();
			Scene = new GameScene();
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			base.Draw(gameTime);
		}
	}
}
