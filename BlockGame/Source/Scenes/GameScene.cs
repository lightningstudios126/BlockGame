using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Source.Blocks;
using BlockGame.Source.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Textures;

namespace BlockGame.Source.Scenes {
	class GameScene : Scene {
		Playfield field;
		Entity controller1;
		Entity controller2;
		Randomizers.Randomizer randomizer;

		public GameScene() : base() {
			AddRenderer(new DefaultRenderer());
		}

		public override void Initialize() {
			base.Initialize();
			randomizer = Randomizers.BagRandomizer;

			field = CreateEntity("grid", new Vector2(400, 620)).AddComponent(new Playfield(15));
			{
				var localNextQueue = CreateEntity("next-queue", new Vector2(230, 30))
					.AddComponent(new NextQueue(randomizer, options: Tetrominos.tetrominos));
				var localHoldQueue = CreateEntity("hold-queue", new Vector2(50, 30)).AddComponent(new HoldQueue());

				controller1 = CreateEntity("group-controller");
				controller1.AddComponent(new KeyboardControls(lMov: Keys.A, rMov: Keys.D, softDrop: Keys.S, hardDrop: Keys.W, lRot: Keys.D1, rRot: Keys.D2, hold: Keys.D3, delayedAutoShift: 170, autoRepeatRate: 50));
				controller1.AddComponent(new PlayerController(field) { nextQueue = localNextQueue, holdQueue = localHoldQueue });
			}

			{
				var localNextQueue = CreateEntity("next-queue", new Vector2(900, 30))
					.AddComponent(new NextQueue(randomizer, options: Tetrominos.tetrominos));
				var localHoldQueue = CreateEntity("hold-queue", new Vector2(1100, 30)).AddComponent(new HoldQueue());

				controller2 = CreateEntity("group-controller");
				controller2.AddComponent(new KeyboardControls(lRot: Keys.OemComma, rRot: Keys.OemPeriod, hold: Keys.OemQuestion, hardDrop: Keys.Up, delayedAutoShift: 170, autoRepeatRate: 50));
				controller2.AddComponent(new PlayerController(field) { nextQueue = localNextQueue, holdQueue = localHoldQueue });
			}

			Camera.AddComponent<MouseLocator>();
		}

		public override void OnStart() {
			base.OnStart();
			Console.WriteLine(string.Join(", ", field.FullRows));
		}

		public override void Update() {
			base.Update();
		}
	}
}
