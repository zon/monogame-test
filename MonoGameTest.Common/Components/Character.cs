using System.Collections.Immutable;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Character {
		public Role Role;
		public CharacterState State;
		public ImmutableQueue<Command> Commands;
		public float Timeout;
		public bool IsCanceled;

		public bool IsIdle => State == CharacterState.Standby && Commands.IsEmpty;
		public bool HasCommand => !Commands.IsEmpty;

		public Character(Role role) {
			Role = role;
			State = CharacterState.Standby;
			Commands = ImmutableQueue.Create<Command>();
			Timeout = 0;
			IsCanceled = false;
		}

		public Command? GetCurrentCommand() {
			return Commands.IsEmpty ? null : Commands.Peek();
		}

		public void EnqueueNext(Command command) {
			if (Commands.IsEmpty || State == CharacterState.Standby) {
				Commands = ImmutableQueue.Create(command);
				IsCanceled = false;
			} else {
				Commands = ImmutableQueue.Create(Commands.Peek(), command);
				IsCanceled = true;
			}
		}

		public void Enqueue(Command command) {
			Commands = Commands.Enqueue(command);
		}

		public void NextState(Skill skill) {
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
						BeginCooldown(skill.Cooldown);
					} else {
						NextCommand();
					}
					break;

				case CharacterState.Follow:
					if (skill.Cooldown > 0) {
						BeginCooldown(skill.Cooldown);
					} else {
						NextCommand();
					}
					break;

				case CharacterState.Cooldown:
					NextCommand();
					break;
			}
		}

		public void BeginCooldown(float time) {
			State = CharacterState.Cooldown;
			Timeout = time;
		}

		public void NextCommand() {
			State = CharacterState.Standby;
			var current = GetCurrentCommand();
			if (current == null) return;
			var command = current.Value;
			if (!IsCanceled && command.IsRepeating && command.Target?.IsValid == true) return;
			Commands = Commands.Dequeue();
			IsCanceled = false;
		}

		public void CancelCommand() {
			IsCanceled = true;
		}

	}

}
