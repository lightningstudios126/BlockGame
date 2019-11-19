using BlockGame.Source.Blocks;
using EO.Internal;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.FSM;
using System;
using System.Collections.Generic;

namespace BlockGame.Source.Components {
	class PlayerController : Component, IUpdatable {
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
		public bool MoveSucceeded { get; private set; }

		Playfield playfield;
		TileGroup activeGroup;
		StateMachine<PlayerController> stateMachine;

		public Controls controls;
		public NextQueue nextQueue;
		public HoldQueue holdQueue;

		public PlayerController(Playfield playfield, float gravity = 1f / 60, float softDropMultiplier = 20, float lockDelay = 0.5f, int maxMoveResets = 16) {
			this.playfield = playfield;

			this.gravity = gravity;
			this.softDropMultiplier = softDropMultiplier;
			this.lockDelay = lockDelay;
			this.maxMoveResets = maxMoveResets;
		}

		public override void OnAddedToEntity() {
			controls ??= Entity.GetComponent<Controls>();
			Insist.IsNotNull(playfield);
			Insist.IsNotNull(controls);
			Insist.IsNotNull(nextQueue);
			//Insist.IsNotNull(holdQueue);

			stateMachine = new StateMachine<PlayerController>(this, new States.StateGenerate());
			stateMachine.AddState(new States.StateGravitate());
			stateMachine.AddState(new States.StateLock());
			stateMachine.AddState(new States.StatePlayfield());
		}

		public void Update() {
			UsePlayerInput();
			stateMachine.Update(Time.DeltaTime);
		}

		void UsePlayerInput() {
			MoveSucceeded = false;
			if (controls.MoveAxis.Value < 0) {
				if (activeGroup.MoveLeft()) MoveSucceeded = true;
			} else if (controls.MoveAxis.Value > 0) {
				if (activeGroup.MoveRight()) MoveSucceeded = true;
			}

			if (controls.LRotate.IsPressed) {
				if (activeGroup.RotateLeft()) MoveSucceeded = true;
			} else if (controls.RRotate.IsPressed) {
				if (activeGroup.RotateRight()) MoveSucceeded = true;
			}

			doSoftDrop = controls.SoftDrop.IsDown;
			if (controls.HardDrop.IsPressed)
				doHardDrop = true;

			if (Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up)) {
				activeGroup.position = new Point(4, 19);
			}
		}

		internal static class States {
			public enum ControllerState {
				Generate, // control is restored, then a new group is instantiated from the next queue and assigned
				Gravitate, // piece is falling and controllable
				Lock, // piece is on the ground, counting down, and controllable
				Playfield, // piece has been locked and control is passed to playfield
			}

			public class StateGenerate : State<PlayerController> {
				public override void Begin() {
					var groupDef = _context.nextQueue.GetNext();
					_context.activeGroup = _context.playfield.AddGroup(groupDef);
				}

				public override void Reason() {
					_machine.ChangeState<StateGravitate>().AddLine();
				}

				public override void Update(float deltaTime) { }
			}

			public class StateGravitate : State<PlayerController> {
				private float timeAcc, lineAcc;
				public override void Begin() {
					timeAcc = 0;
					lineAcc = 0;
				}

				public override void Update(float deltaTime) {
					timeAcc += deltaTime;
					if (_context.doHardDrop) {
						lineAcc += 20;
					} else if (timeAcc >= Constants.sixtieth) {
						timeAcc -= Constants.sixtieth;
						if (_context.doSoftDrop) {
							lineAcc += _context.gravity * _context.softDropMultiplier;
						} else lineAcc += _context.gravity;
					}

					while (lineAcc >= 1) {
						lineAcc--;
						// attempt move down by one
						if (_context.activeGroup.Landed || _context.doHardDrop) {
							// if it has already landed, then transition to the Lock state
							var next = _machine.ChangeState<StateLock>();
							if (_context.activeGroup.position.Y < next.lowestRow) {
								next.lowestRow = _context.activeGroup.position.Y;
								next.ResetMoveResets();
							}
						} else {
							// if active group is not yet landed on a surface
							// then try to move it down by one line
							// if it can, then continue back into the loop
							// otherwise, remove the integer part of lineAcc
							if (!_context.activeGroup.MoveDown())
								lineAcc %= 1;
						}
					}
				}

				internal void AddLine() => lineAcc++;
			}

			public class StateLock : State<PlayerController> {
				private float lockTimer;
				private int moveResets;
				internal int lowestRow;

				public override void Update(float deltaTime) {
					lockTimer += deltaTime;

					if (_context.MoveSucceeded) {
						lockTimer = 0;
						moveResets++;
					}

					if (_context.doHardDrop || lockTimer >= _context.lockDelay || moveResets == _context.maxMoveResets) {
						_context.doHardDrop = false;
						_context.playfield.LockTileGroup(_context.activeGroup);
						_machine.ChangeState<StatePlayfield>();
					}
				}

				public void ResetMoveResets() {
					moveResets = 0;
				}
			}

			public class StatePlayfield : State<PlayerController> {
				bool isLocked;
				public override void OnInitialized() {
					//_context.playfield
				}

				public override void Begin() {
					isLocked = true;
				}

				public override void Update(float deltaTime) {
					throw new NotImplementedException();
				}
			}
		}
	}
}
