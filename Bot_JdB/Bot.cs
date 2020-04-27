using System;

namespace Ants {
	public abstract class Bot {

		public abstract void DoTurn(GameState state);
		public abstract void Initialize(GameState state);

		protected void IssueOrder(Vector2i loc, Direction direction) {
			
			if (direction == Direction.Halt)
				return;

			System.Console.Out.WriteLine("o {0} {1} {2}", loc.y, loc.x, direction.ToChar());
		}
	}
}