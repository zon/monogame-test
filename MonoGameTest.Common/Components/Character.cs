using System;
using System.Collections.Immutable;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Character {
		public Role Role;
		public CharacterState State;
		public ImmutableQueue<Command> Commands;
		public float Timeout;
		public bool StopRepeating;

		public bool IsIdle => State == CharacterState.Standby && Commands.IsEmpty;
		public bool HasCommand => !Commands.IsEmpty;
		public bool IsActive => State != CharacterState.Standby && State != CharacterState.Cooldown;

		public Character(Role role) {
			Role = role;
			State = CharacterState.Standby;
			Commands = ImmutableQueue.Create<Command>();
			Timeout = 0;
			StopRepeating = false;
		}

		public Command? GetCurrentCommand() {
			return Commands.IsEmpty ? null : Commands.Peek();
		}

		public void EnqueueNext(Entity entity, Command command) {
			if (Commands.IsEmpty || !IsActive) {
				Commands = ImmutableQueue.Create(command);
				StopRepeating = false;
				entity.NotifyChanged<Character>();

			} else {
				Commands = ImmutableQueue.Create(Commands.Peek(), command);
				StopRepeating = true;
			}
		}

		public void Enqueue(Command command) {
			Commands = Commands.Enqueue(command);
		}

		public void NextState(Entity entity, Skill skill) {
			switch (State) {

				case CharacterState.Standby:
					if (skill.Lead > 0) {
						State = CharacterState.Lead;
						Timeout = skill.Lead;
					} else {
						State = CharacterState.Active;
					}
					break;

				case CharacterState.Lead:
					State = CharacterState.Active;
					break;

				case CharacterState.Active:
					if (skill.Follow > 0) {
						State = CharacterState.Follow;
						Timeout = skill.Follow;
					} else if (skill.Cooldown > 0) {
						CompleteCommand(entity, skill.Cooldown);
					} else {
						State = CharacterState.Standby;
					}
					break;

				case CharacterState.Follow:
					if (skill.Cooldown > 0) {
						CompleteCommand(entity, skill.Cooldown);
					} else {
						State = CharacterState.Standby;
					}
					break;

				case CharacterState.Cooldown:
					State = CharacterState.Standby;
					break;
			}
		}

		public void CompleteCommand(Entity entity, float cooldown) {
			State = CharacterState.Cooldown;
			Timeout = cooldown;

			var command = Commands.Peek();
			if (!StopRepeating && command.IsRepeating && command.IsValid) return;

			NextCommand(entity);
		}

		public void RepeatCommand(float cooldown) {
			State = CharacterState.Cooldown;
			Timeout = cooldown;
		}

		public void CancelCommand(Entity entity) {
			if (State != CharacterState.Cooldown) {
				State = CharacterState.Standby;
			}
			NextCommand(entity);
		}

		void NextCommand(Entity entity) {
			Commands = Commands.Dequeue();
			StopRepeating = false;
			entity.NotifyChanged<Character>();
		}

	}

}
