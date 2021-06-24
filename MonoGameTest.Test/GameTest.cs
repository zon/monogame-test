using System;
using DefaultEcs;
using MonoGameTest.Common;
using MonoGameTest.Server;
using Xunit;
using Xunit.Sdk;

namespace MonoGameTest.Test {

	public class GameTest : Game {

		public const float TIMEOUT = 10;
		public const int PRECISION = 6;
		
		public GameTest() : base("../../../../MonoGameTest.Client/Content/empty.tmx") {}

		public void Simulate(float duration = Time.FRAME, float tick = Time.FRAME) {
			var remaining = duration;
			while (remaining > 0) {
				var dt = tick;
				if (remaining < tick) {
					dt = remaining;
				}
				remaining -= dt;
				Update(dt);
			}
		}

		public float Simulate(Func<bool> until, float tick = Time.FRAME, float timeout = TIMEOUT) {
			var elasped = 0f;
			do {
				if (until()) return elasped;
				Update(tick);
				elasped += tick;
			} while (elasped < timeout);
			throw new TestTimeoutException(Calc.Floor(elasped * 1000));
		}

		public float Simulate<C>(
			in Entity entity,
			Func<C, bool> until,
			float tick = Time.FRAME,
			float timeout = TIMEOUT
		) {
			var elasped = 0f;
			do {
				ref var c = ref entity.Get<C>();
				if (until(c)) return elasped;
				Update(tick);
				elasped += tick;
			} while (elasped < timeout);
			throw new TestTimeoutException(Calc.Floor(elasped * 1000));
		}

		public float SimulateCommand(in Entity entity, Skill skill, in Entity target) {
			ref var c = ref entity.Get<Character>();
			var command = Command.Targeting(target, skill);
			c.EnqueueNext(entity, command);
			var elapsed = SimulateCommandActivation(entity, command);
			return elapsed + SimulateCommandCompletion(entity, command);
		}

		public float SimulateCommandActivation(in Entity entity, Command command) {
			var skill = command.Skill;
			ref var c = ref entity.Get<Character>();
			Assert.Equal(CharacterState.Standby, c.State);
			Assert.Equal(0, c.Timeout);
			AssertCommand(c, command);
			var elapsed = 0f;
			var expectedElapsed = Time.FRAME;
			var previousState = c.State;
			var elapsedTotal = 0f;

			if (skill.Charge > 0) {
				elapsed = Simulate<Character>(entity, c => c.State == CharacterState.Charge);
				elapsedTotal += elapsed;
				Assert.Equal(expectedElapsed, elapsed, PRECISION);
				c = ref entity.Get<Character>();
				Assert.Equal(skill.Charge, c.Timeout);
				AssertCommand(c, command);
				expectedElapsed = skill.Charge;
				previousState = c.State;
			}

			if (skill.Lead > 0) {
				elapsed = Simulate<Character>(entity, c => c.State == CharacterState.Lead);
				elapsedTotal += elapsed;
				Assert.Equal(expectedElapsed, elapsed, PRECISION);
				c = ref entity.Get<Character>();
				Assert.Equal(skill.Lead, c.Timeout);
				AssertCommand(c, command);
				expectedElapsed = skill.Lead;
				previousState = c.State;
			}

			elapsed = Simulate<Character>(entity, c => c.State != previousState);
			elapsedTotal += elapsed;
			Assert.Equal(expectedElapsed, elapsed, PRECISION);

			return elapsedTotal;
		}

		public float SimulateCommandCompletion(in Entity entity, Command command) {
			var skill = command.Skill;
			ref var c = ref entity.Get<Character>();
			AssertCommand(c, command);
			var expectedElapsed = 0f;

			if (skill.Follow > 0) {
				Assert.Equal(CharacterState.Follow, c.State);
				Assert.Equal(skill.Follow, c.Timeout);
				expectedElapsed = skill.Follow;
			}

			var elapsed = Simulate<Character>(entity, c => c.State == CharacterState.Cooldown);
			Assert.Equal(expectedElapsed, elapsed, PRECISION);
			c = ref entity.Get<Character>();
			Assert.Equal(skill.Timeout, c.Timeout);

			if (skill.Repeating) {
				AssertCommand(c, command);
			}

			return elapsed;
		}

		public void AssertDamage(
			in Entity entity,
			int damage,
			int previousDamage = 0
		) {
			ref var h = ref entity.Get<Health>();
			Assert.Equal(h.Maximum - previousDamage - damage, h.Amount);
		}

		public int AssertDamage(
			in Entity entity,
			Skill skill,
			int previousDamage = 0
		) {
			ref var attributes = ref entity.Get<Attributes>();
			var damage = skill.GetDamage(attributes);
			Assert.NotEqual(0, damage);
			AssertDamage(entity, damage, previousDamage);
			return damage;
		}

		public void AssertCommand(in Character character, Command command) {
			Assert.True(character.HasCommand);
			Assert.Equal(command.Id, character.Commands.Peek().Id);
		}

	}

}
