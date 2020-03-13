using BlockGame.Source.Blocks;
using BlockGame.Source.Components;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockGame.Source.UI {
	class PlayfieldWidget : Element {

		readonly Playfield playfield;
		Color BackgroundColour;
		Color OutlineColour;

		Vector2 Position => new Vector2(this.GetX(), this.GetY());

		public PlayfieldWidget(Playfield playfield) {
			this.playfield = playfield;
			this.BackgroundColour = new Color(40, 40, 40);
			this.OutlineColour = Color.White;
		}

		public override float PreferredHeight => Constants.pixelsPerTile * playfield.Height;
		public override float PreferredWidth => Constants.pixelsPerTile * playfield.Width;
		private Point CorrectPosition => (Position + new Vector2(0, GetHeight() - Constants.pixelsPerTile)).ToPoint();

		public override void Draw(Batcher batcher, float parentAlpha) {
			base.Draw(batcher, parentAlpha);
			batcher.DrawCircle(GetX(), GetY(), 2, Color.Red);
			batcher.DrawHollowRect(GetX(), GetY(), GetWidth(), GetHeight(), OutlineColour, 6);
			batcher.DrawRect(GetX(), GetY(), GetWidth(), GetHeight(), BackgroundColour);

			for (int y = 0; y < playfield.Height; y++) {
				for (int x = 0; x < playfield.Width; x++) {
					DrawGridTile(batcher, new Point(x, y));
				}
			}

			foreach (var player in playfield.players) {
				var piece = player.piece;
				if (piece != null) {
					var ghost = piece.GetLandedOffset();
					foreach (Point point in piece.Shape) {
						Utilities.DrawTileOutline(batcher, point + piece.position, CorrectPosition, player.outlineTint, 6, playfield.Height);
						Utilities.DrawTileOutline(batcher, point + piece.position + ghost, CorrectPosition, player.outlineTint, 6, playfield.Height);
					}

					foreach (Point point in piece.Shape) {
						Utilities.DrawTile(batcher, point + piece.position + ghost, CorrectPosition, piece.definition.type, piece.definition.type.ghostColor, playfield.Height);
						Utilities.DrawTile(batcher, point + piece.position, CorrectPosition, piece.definition.type, playfield.Height);
					}

					int temp = 0;
					foreach (Point point in piece.outline) {
						Point gridOffset = point + piece.position;
						Point offset = new Point(Constants.pixelsPerTile * gridOffset.X, -Constants.pixelsPerTile * gridOffset.Y);
						batcher.DrawCircle(Position + offset.ToVector2(), 2, Color.Green);
						batcher.DrawString(Graphics.Instance.BitmapFont, temp + "", CorrectPosition.ToVector2() + offset.ToVector2(), Color.White);
						temp++;
					}
				}
			}
		}

		public void DrawGridTile(Batcher batcher, Point gridLocation) {
			Tile? tile = playfield.grid[gridLocation.X, gridLocation.Y];
			if (tile != null)
				Utilities.DrawTile(batcher, gridLocation, CorrectPosition, tile);
		}
	}
}
