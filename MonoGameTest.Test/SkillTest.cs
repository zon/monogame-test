using System;
using DefaultEcs;
using MonoGameTest.Common;
using MonoGameTest.Server;
using Xunit;

namespace MonoGameTest.Test {

	public class SkillTest : GameTest {
		public Entity A;
		public Entity B;

		public SkillTest() : base() {
			A = Factory.SpawnCharacter(Context.World, Role.Get(1), new Coord(1, 1));
			B = Factory.SpawnCharacter(Context.World, Role.Get(1), new Coord(2, 1));
		}
		
		[Fact]
		public void Primary() {
			ref var ac = ref A.Get<Character>();
			var skill = ac.Role.PrimarySkill;
			SimulateCommand(A, skill, B);
			AssertDamage(B, skill);
		}

		[Fact]
		public void Fireball() {
			var skill = Skill.GetByIcon("fireball");
			var c = Factory.SpawnCharacter(Context.World, Role.Get(1), new Coord(3, 1));
			var d = Factory.SpawnCharacter(Context.World, Role.Get(1), new Coord(4, 1));
			var e = Factory.SpawnCharacter(Context.World, Role.Get(1), new Coord(3, 0));
			var f = Factory.SpawnCharacter(Context.World, Role.Get(1), new Coord(3, 2));
			SimulateCommand(A, skill, c);
			AssertDamage(B, skill);
			AssertDamage(c, skill);
			AssertDamage(d, skill);
			AssertDamage(e, skill);
			AssertDamage(f, skill);
		}

		[Fact]
		public void Poison() {
			var skill = Skill.GetByIcon("poison");
			ref var c = ref A.Get<Character>();
			var command = Command.Targeting(B, skill);
			c.EnqueueNext(A, command);
			SimulateCommandActivation(A, command);
			AssertDamage(B, skill);
			
			var allBuffs = Context.World.GetEntities().With<BuffEffect>().AsMultiMap<CharacterId>();
			var bid = B.Get<CharacterId>();
			ReadOnlySpan<Entity> buffed;
			Assert.True(allBuffs.TryGetEntities(bid, out buffed));
			Assert.Equal(1, buffed.Length);
			var be = buffed[0];
			Assert.True(be.Has<BuffEffect>());
			ref var effect = ref be.Get<BuffEffect>();
			Assert.Equal(skill.Buff.Id, effect.Buff.Id);
			Assert.Equal(skill.Buff.Duration, effect.Timeout);

			var elapsed = SimulateCommandCompletion(A, command);
			elapsed += Simulate(() => !allBuffs.ContainsKey(bid), timeout: skill.Buff.Duration + 2);
			Assert.Equal(skill.Buff.Duration, elapsed, 0);
			AssertDamage(B, skill.Buff.HealthPerSecond * skill.Buff.Duration, skill.Damage);

			allBuffs.Dispose();
		}

	}

}
