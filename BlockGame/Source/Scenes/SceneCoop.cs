using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BlockGame.Source.Blocks;
using BlockGame.Source.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Textures;

namespace BlockGame.Source.Scenes {
	class SceneCoop : Scene {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		Playfield field;
		Entity controller1;
		Entity controller2;
		Randomizers.Randomizer randomizer;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		public override void Initialize() {
			base.Initialize();

			SetDesignResolution(1280, 720, SceneResolutionPolicy.ShowAllPixelPerfect);
			ClearColor = new Color(16, 16, 16);
			AddRenderer(new DefaultRenderer());
			AddPostProcessors();

			randomizer = Randomizers.BagRandomizer;

			field = CreateEntity("grid").AddComponent(new Playfield(15) { OutlineColour = Color.White });
			field.Transform.Position = new Vector2(Screen.Width / 2 - field.Width / 2, 620);

			{
				var outline = Color.OrangeRed;
				var leftEdge = field.Transform.Position.X;
				var localNextQueue = CreateEntity("next-queue1", new Vector2(leftEdge - (4 * Constants.pixelsPerTile + 20) - 20, 30))
					.AddComponent(new NextQueue(randomizer, options: Tetrominos.tetrominos) { OutlineColour = outline });
				var localHoldQueue = CreateEntity("hold-queue1", new Vector2(leftEdge - 2 * (4 * Constants.pixelsPerTile + 20) - 40, 30))
					.AddComponent(new HoldQueue() { OutlineColour = outline });

				controller1 = CreateEntity("group-controller1");
				controller1.AddComponent(new KeyboardControls(lMov: Keys.A, rMov: Keys.D, softDrop: Keys.S, hardDrop: Keys.W, lRot: Keys.D1, rRot: Keys.D2, hold: Keys.D3, delayedAutoShift: 170, autoRepeatRate: 50));
				controller1.AddComponent(new PlayerController(field) { nextQueue = localNextQueue, holdQueue = localHoldQueue, outlineTint = outline });
			}

			{
				var outline = Color.Turquoise;
				var rightEdge = field.Transform.Position.X + field.Width;
				var localNextQueue = CreateEntity("next-queue2", new Vector2(rightEdge + 20, 30))
					.AddComponent(new NextQueue(randomizer, options: Tetrominos.tetrominos) { OutlineColour = outline });
				var localHoldQueue = CreateEntity("hold-queue2", new Vector2(localNextQueue.Width + rightEdge + 40, 30))
					.AddComponent(new HoldQueue() { OutlineColour = outline });

				controller2 = CreateEntity("group-controller2");
				controller2.AddComponent(new KeyboardControls(lRot: Keys.NumPad1, rRot: Keys.NumPad2, hold: Keys.NumPad3, hardDrop: Keys.Up, delayedAutoShift: 170, autoRepeatRate: 50));
				controller2.AddComponent(new PlayerController(field) { nextQueue = localNextQueue, holdQueue = localHoldQueue, outlineTint = outline, spawnLocation = new Point(10, 20) });
			}

			Camera.AddComponent<MouseLocator>();
		}

		private void AddPostProcessors() {
			AddPostProcessor(new ScanlinesPostProcessor(1));
			var glitch = AddPostProcessor(new PixelGlitchPostProcessor(0) { HorizontalOffset = 0, VerticalSize = 4 });

			static IEnumerator DoGlitchEffect(PixelGlitchPostProcessor glitch) {
				while (true) {
					var next = Nez.Random.Range(1f, 3f);
					yield return Coroutine.WaitForSeconds(next);
					var duration = Nez.Random.Range(0.1f, 0.5f);
					while (duration > 0) {
						duration -= Time.DeltaTime;
						glitch.HorizontalOffset = Nez.Random.Range(0.2f, 1f) * ((Nez.Random.NextInt(2) * 2) - 1);
						yield return null;
					}
					glitch.HorizontalOffset = 0;
				}
			}
			Core.StartCoroutine(DoGlitchEffect(glitch));
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
