using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Character {
		public Role Role;
		public CharacterState State;
		public ImmutableQueue<Command> Commands;
		public float Timeout;
		public Dictionary<int, float> Cooldowns;
		public bool StopRepeating;

		public bool IsIdle => State == CharacterState.Standby && Commands.IsEmpty;
		public bool HasCommand => !Commands.IsEmpty;
		public bool IsActive => State != CharacterState.Standby && State != CharacterState.Cooldown;

		public Character(Role role) {
			Role = role;
			State = CharacterState.Standby;
			Commands = ImmutableQueue.Create<Command>();
			Timeout = 0;
			Cooldowns = new Dictionary<int, float>();
			StopRepeating = false;
		}

		public void Update(float dt) {
			foreach (var pair in Cooldowns) {
				var value = Math.Max(pair.Value - dt, 0);
				if (value > 0) {
					Cooldowns[pair.Key] = value;
				} else {
					Cooldowns.Remove(pair.Key);
				}
			}
		}

		public Command? GetCurrentCommand() {
			return Commands.IsEmpty ? null : Commands.Peek();
		}

		public void EnqueueNext(Entity entity, Command command) {
			if (Commands.IsEmpty || !IsActive) {
				if (!IsCool(command)) return;
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
					} else if (skill.Timeout > 0) {
						CompleteCommand(entity, skill.Timeout);
					} else {
						State = CharacterState.Standby;
					}
					break;

				case CharacterState.Follow:
					if (skill.Timeout > 0) {
						CompleteCommand(entity, skill.Timeout);
					} else {
						State = CharacterState.Standby;
					}
					break;

				case CharacterState.Cooldown:
					State = CharacterState.Standby;
					break;
			}
		}

		public void CompleteCommand(Entity entity, float timeout) {
			State = CharacterState.Cooldown;
			Timeout = timeout;

			var command = Commands.Peek();
			if (!StopRepeating && command.IsRepeating && command.IsValid) return;

			NextCommand(entity);
		}

		public void RepeatCommand(float pause) {
			State = CharacterState.Cooldown;
			Timeout = pause;
		}

		public void CancelCommand(Entity entity) {
			if (State != CharacterState.Cooldown) {
				State = CharacterState.Standby;
			}
			NextCommand(entity);
		}

		public float GetCooldown(int skillId) {
			return Cooldowns.GetValueOrDefault(skillId);
		}

		public bool IsCool(int skillId) {
			return GetCooldown(skillId) <= 0;
		}

		public bool IsCool(Command command) {
			return !command.HasSkill || IsCool(command.Skill.Id);
		}

		public void StartCooldown(Entity entity, Skill skill) {
			Cooldowns[skill.Id] = skill.Cooldown;
			if (!entity.Has<Player>()) return;
			entity.World.Publish(new CooldownMessage { Entity = entity, SkillId = skill.Id });
		}

		void NextCommand(Entity entity) {
			var previous = Commands.Peek();
			if (previous.HasSkill) {
				StartCooldown(entity, previous.Skill);
			}

			Commands = Commands.Dequeue();
			StopRepeating = false;

			while (!Commands.IsEmpty) {
				var next = Commands.Peek();
				if (IsCool(next)) break;
				Commands = Commands.Dequeue();
			}

			entity.NotifyChanged<Character>();
		}

	}

}
