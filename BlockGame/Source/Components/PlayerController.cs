using BlockGame.Source.Blocks;
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

		private bool doSoftDrop, doHardDrop, doHold;
		private bool moveSucceeded;

		Playfield playfield;
		TileGroup activeGroup;
		StateMachine<PlayerController> stateMachine;

		public Controls controls;
		public NextQueue nextQueue;
		public HoldQueue holdQueue;

		event Action<TileGroup> GeneratedNewPiece;
		event Action<TileGroup> LockedPiece;

		public PlayerController(Playfield playfield, float gravity = 1f / 60, float softDropMultiplier = 20, float lockDelay = 0.5f, int maxMoveResets = 15) {
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

			if (holdQueue != null) LockedPiece += (x => holdQueue.Unlock());

			playfield.StartedProcessing += () => this.SetEnabled(false);
			playfield.FinishedProcessing += () => this.SetEnabled(true);
		}

		public void Update() {
			UsePlayerInput();
			if (doHold) {
				if (holdQueue.Swap(activeGroup.groupDef, out var swapped)) {
					playfield.RemoveGroup(activeGroup);
					var nextState = stateMachine.ChangeState<States.StateGenerate>();
					if (swapped != null) nextState.ProvidePiece(swapped);
				}
				doHold = false;
			}
			stateMachine.Update(Time.DeltaTime);
		}

		void UsePlayerInput() {
			moveSucceeded = false;
			if (controls.MoveAxis.Value < 0) {
				if (activeGroup.MoveLeft()) moveSucceeded = true;
			} else if (controls.MoveAxis.Value > 0) {
				if (activeGroup.MoveRight()) moveSucceeded = true;
			}

			if (controls.LRotate.IsPressed) {
				if (activeGroup.RotateLeft()) moveSucceeded = true;
			} else if (controls.RRotate.IsPressed) {
				if (activeGroup.RotateRight()) moveSucceeded = true;
			}

			doHold = controls.Hold.IsPressed;
			doSoftDrop = controls.SoftDrop;
			if (controls.HardDrop.IsPressed)
				doHardDrop = true;
		}

		private static class States {
			public class StateGenerate : State<PlayerController> {
				TileGroupDefinition def;

				public override void Reason() {
					def ??= _context.nextQueue.GetNext();
					_context.activeGroup = _context.playfield.AddGroup(def);
					def = null;
					_machine.ChangeState<StateGravitate>().AddLine();
				}

				public override void Update(float deltaTime) { }
				public override void End() {
					_context.GeneratedNewPiece(_context.activeGroup);
				}

				public void ProvidePiece(TileGroupDefinition def) {
					this.def = def;
				}
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
						if (!_context.activeGroup.MoveDown())
							lineAcc %= 1;
					}

					if (_context.activeGroup.Landed) {
						var next = _machine.ChangeState<StateLock>();
						if (_context.activeGroup.position.Y < next.lowestRow) {
							next.lowestRow = _context.activeGroup.position.Y;
							next.ResetMoveResets();
						}
					}
				}

				internal void AddLine() => lineAcc++;
			}

			public class StateLock : State<PlayerController> {
				private float lockTimer;
				private int moveResets;
				public int lowestRow;

				public override void OnInitialized() {
					_context.GeneratedNewPiece += (x) => {
						lockTimer = 0;
						moveResets = 0;
						lowestRow = int.MaxValue;
					};
				}

				public override void Update(float deltaTime) {
					lockTimer += deltaTime;

					if (_context.moveSucceeded) {
						lockTimer = 0;
						moveResets++;
						if (!_context.activeGroup.Landed) {
							_machine.ChangeState<StateGravitate>();
						}
					} else if (_context.doHardDrop || lockTimer >= _context.lockDelay || moveResets >= _context.maxMoveResets) {
						_context.doHardDrop = false;
						_context.playfield.LockTileGroup(_context.activeGroup);
						_machine.ChangeState<StatePlayfield>();
						_context.LockedPiece(_context.activeGroup);
					}
				}

				public void ResetMoveResets() {
					moveResets = 0;
				}
			}

			public class StatePlayfield : State<PlayerController> {
				public override void Begin() {
					_context.playfield.StartSequence();
				}

				public override void Reason() {
					_machine.ChangeState<StateGenerate>();
				}

				public override void Update(float deltaTime) { }
			}
		}
	}
}
