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

		public Piece piece;
		public Color outlineTint;
		public Point spawnLocation;

		Playfield playfield;
		StateMachine<PlayerController> stateMachine;

		public ControlScheme controls;
		public NextQueue nextQueue;
		public HoldQueue holdQueue;

		event Action<Piece> PieceGenerated;
		event Action<Piece> PieceLocked;
		event Action<Piece> PieceDisplaced;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		public PlayerController(Playfield playfield, float gravity = 1f / 60, float softDropMultiplier = 20, float lockDelay = 0.5f, int maxMoveResets = 15) {
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
			this.playfield = playfield;

			this.gravity = gravity;
			this.softDropMultiplier = softDropMultiplier;
			this.lockDelay = lockDelay;
			this.maxMoveResets = maxMoveResets;

			outlineTint = Color.White;
			spawnLocation = Constants.defaultGenerationLocation;

			PieceGenerated += (p) => { };
			PieceLocked += (p) => { };
			PieceDisplaced += (p) => { };
		}

		public override void OnAddedToEntity() {
			controls ??= Entity.GetComponent<ControlScheme>();
			Insist.IsNotNull(playfield);
			Insist.IsNotNull(controls);
			Insist.IsNotNull(nextQueue);
			//Insist.IsNotNull(holdQueue); 

			stateMachine = new StateMachine<PlayerController>(this, new States.StateGenerate());
			stateMachine.AddState(new States.StateGravitate());
			stateMachine.AddState(new States.StateLock());
			stateMachine.AddState(new States.StatePlayfield());

			if (holdQueue != null) PieceLocked += x => holdQueue.Unlock();

			playfield.StartedProcessing += () => this.SetEnabled(false);
			playfield.FinishedProcessing += () => this.SetEnabled(true);
			playfield.AddPlayer(this);
		}

		public void Update() {
			UsePlayerInput();
			if (doHold) {
				if (holdQueue.Swap(piece.definition, out var swapped)) {
					var state = stateMachine.ChangeState<States.StateGenerate>();
					if (swapped != null) state.ProvidePiece(swapped);
				}
				doHold = false;
			}
			if (piece != null) {
				// if current overlapping terrain, then shift up
				if (!piece.TestOffset(new Point(0, 0))) {
					int upOffset = 1;
					while (!piece.TestOffset(new Point(0, upOffset)))
						upOffset++;
					piece.Shift(new Point(0, upOffset));
					PieceDisplaced(piece);
				}
			}
			stateMachine.Update(Time.DeltaTime);
		}

		void UsePlayerInput() {
			moveSucceeded = false;
			if (controls.MoveAxis.Value < 0) {
				if (piece.MoveLeft()) moveSucceeded = true;
			} else if (controls.MoveAxis.Value > 0) {
				if (piece.MoveRight()) moveSucceeded = true;
			}

			if (controls.LRotate.IsPressed) {
				if (piece.RotateLeft()) moveSucceeded = true;
			} else if (controls.RRotate.IsPressed) {
				if (piece.RotateRight()) moveSucceeded = true;
			}

			doHold = controls.Hold.IsPressed;
			doSoftDrop = controls.SoftDrop;
			if (controls.HardDrop.IsPressed)
				doHardDrop = true;
		}

		private static class States {
			public class StateGenerate : State<PlayerController> {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
				PieceDefinition def;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
				bool pieceProvided;

				public override void Reason() {
					if (!pieceProvided) {
						def = _context.nextQueue.GetNext();
						pieceProvided = false;
					}
					_context.piece = _context.playfield.SpawnTileGroup(def, _context.spawnLocation);
					_machine.ChangeState<StateGravitate>().AddLine();
				}

				public override void Update(float deltaTime) { }
				public override void End() {
					_context.PieceGenerated(_context.piece);
				}

				public void ProvidePiece(PieceDefinition def) {
					this.def = def;
					pieceProvided = true;
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
						if (!_context.piece.MoveDown())
							lineAcc %= 1;
					}

					if (_context.piece.Landed) {
						var next = _machine.ChangeState<StateLock>();
						if (_context.piece.position.Y < next.lowestRow) {
							next.lowestRow = _context.piece.position.Y;
							next.ResetMoveResets();
						}
					}
				}

				public void AddLine() => lineAcc++;
			}

			public class StateLock : State<PlayerController> {
				private float lockTimer;
				private int moveResets;
				public int lowestRow;

				public override void OnInitialized() {
					_context.PieceGenerated += (x) => ResetAll();
					_context.PieceDisplaced += (x) => ResetAll();
				}

				public override void Update(float deltaTime) {
					lockTimer += deltaTime;

					if (_context.moveSucceeded) {
						lockTimer = 0;
						moveResets++;
						if (!_context.piece.Landed) {
							_machine.ChangeState<StateGravitate>();
						}
					} else if (_context.doHardDrop || lockTimer >= _context.lockDelay || moveResets >= _context.maxMoveResets) {
						_context.doHardDrop = false;
						_context.playfield.LockTileGroup(_context.piece);
						_machine.ChangeState<StatePlayfield>();
						_context.PieceLocked(_context.piece);
					}
				}

				public void ResetMoveResets() {
					moveResets = 0;
				}

				public void ResetAll() {
					lockTimer = 0;
					moveResets = 0;
					lowestRow = int.MaxValue;
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
