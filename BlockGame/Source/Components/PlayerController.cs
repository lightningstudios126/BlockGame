using BlockGame.Source.Blocks;
using Nez;
using System;
using System.Collections.Generic;

namespace BlockGame.Source.Components {
	class PlayerController : Component, IUpdatable {
		Playfield playfield;
		TileGroup activeGroup;
		Controls controls;

		public NextQueue nextQueue;

		/// <summary>
		/// Automatic drop rate, measured in cells per 1/60th of a second
		/// </summary>
		public float gravity;
		/// <summary>
		/// The gravity multiplier to be applied when performing a soft drop
		/// </summary>
		public float softDropMultiplier;

		/// <summary>
		/// Length of time until a group locks on the playfield, measured in seconds
		/// </summary>
		public float lockDelay;
		/// <summary>
		/// Maximum number of times lock delay can be reset until the piece locks on with no delay
		/// </summary>
		public int maxMoveResets;

		private bool doSoftDrop, doHardDrop;
		private float timeAcc, lineAcc;

		private bool inFreefall;

		float lockTimer;
		int moveResets;
		bool resetInputReceived;
		int lowestRow;

		public enum ControllerState {
			Generate,
			Control,
			Lock,
			Match, 
			Animate,
			Clear,
			Complete
		}

		public PlayerController(Playfield playfield, float gravity = 1f/60, float softDropMultiplier = 20, float lockDelay = 0.5f, int maxMoveResets = 16) {
			this.playfield = playfield;

			this.gravity = gravity;
			this.softDropMultiplier = softDropMultiplier;
			this.lockDelay = lockDelay;
			this.maxMoveResets = maxMoveResets;
		}

		public override void OnAddedToEntity() {
			controls = Entity.GetComponent<Controls>();
			nextQueue ??= Entity.GetComponent<NextQueue>();
		}

		public override void OnEnabled() {
			SetActiveGroup(Dequeue());
		}

		public void Update() { 
			UsePlayerInput();

			if (inFreefall) {
				Gravitate();
				if (activeGroup.position.Y < lowestRow) {
					lowestRow = activeGroup.position.Y;
					moveResets = 0;
				}
			} else {
				// move accumulator reset stuff to the enter-freefall phase of the state machine
				timeAcc = 0;
				lineAcc = 0;
				Lock();
			}

			if (doHardDrop) {
				LockActiveGroup();
				doHardDrop = false;
			}

			inFreefall = activeGroup.MoveDown(true);
		}

		private void Gravitate() {
			timeAcc += Time.DeltaTime;
			if (doHardDrop) {
				lineAcc += 20;
			} else if (timeAcc >= Constants.sixtieth) {
				timeAcc -= Constants.sixtieth;
				if (doSoftDrop) {
					lineAcc += gravity * softDropMultiplier;
				} else lineAcc += gravity;
			}

			while (lineAcc >= 1) {
				lineAcc--;
				if (!activeGroup.MoveDown())
					lineAcc %= 1;
			}
		}

		private void Lock() {
			lockTimer += Time.DeltaTime;

			if (resetInputReceived) {
				lockTimer = 0;
				moveResets++;
				resetInputReceived = false;
			}

			if (lockTimer >= lockDelay || moveResets == maxMoveResets) {
				LockActiveGroup();
			}
		}

		void UsePlayerInput() {
			if (controls.MoveAxis.Value < 0) {
				if (activeGroup.MoveLeft()) resetInputReceived = true;
			} else if (controls.MoveAxis.Value > 0) {
				if (activeGroup.MoveRight()) resetInputReceived = true;
			}

			if (controls.LRotate.IsPressed) {
				if (activeGroup.RotateLeft()) resetInputReceived = true;
			} else if (controls.RRotate.IsPressed) {
				if (activeGroup.RotateRight()) resetInputReceived = true;
			}

			doSoftDrop = controls.SoftDrop.IsDown;
			if (controls.HardDrop.IsPressed)
				doHardDrop = true;

			if (Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up)) {
				activeGroup.position = new Microsoft.Xna.Framework.Point(4, 19);
			}
		}

		void SetActiveGroup(TileGroupDefinition groupDef) {
			activeGroup = playfield.AddGroup(groupDef);
			inFreefall = true;

			timeAcc = 0;
			lineAcc = 1;

			lockTimer = 0;
			lowestRow = activeGroup.position.Y;
		}

		void LockActiveGroup() {
			playfield.LockTileGroup(activeGroup);
			SetActiveGroup(Dequeue());
		}

		// TODO: move next queue to a separate object so the controller doesn't need to handle it (i.e. more decoupling)
		// TODO: maybe make the hold queue similar?
		TileGroupDefinition Dequeue() {
			return nextQueue.GetNext();
		}
	}
}
