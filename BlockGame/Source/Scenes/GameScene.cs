﻿using System;
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
		Entity controller;
		Randomizers.Randomizer randomizer;

		public GameScene() : base() {
			AddRenderer(new DefaultRenderer());
		}

		public override void Initialize() {
			base.Initialize();
			randomizer = Randomizers.BagRandomizer;

			field = CreateEntity("grid", new Vector2(500, 620))
				.AddComponent(new Playfield());
			var nextQueue = CreateEntity("next-queue", new Vector2(850, 30))
				.AddComponent(new NextQueue(randomizer, options: Tetrominos.tetrominos));

			controller = CreateEntity("group-controller");
			controller.AddComponent(new KeyboardControls());
			controller.AddComponent(new PlayerController(field)).nextQueue = nextQueue;

		}

		public override void OnStart() {
			base.OnStart();
			Console.WriteLine(string.Join(", ", field.FullRows));
		}

		public override void Update() {
			base.Update();
			if (Input.LeftMouseButtonPressed) Console.WriteLine(Input.MousePosition);
		}
	}
}
