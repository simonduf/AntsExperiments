using System;

namespace Ants {
	public abstract class Bot {

		public abstract void DoTurn(GameState state);

		public static void IssueOrder(GameState state, Location loc, Direction direction) {
			System.Console.Out.WriteLine("o {0} {1} {2}", loc.Row, loc.Col, direction.ToChar());

            if (state.OccupiedNextRound.At(loc) != true)
                Logger.Log.Error("Location was not occupied...");

            state.OccupiedNextRound.Set(loc, false);
            state.OccupiedNextRound.Set(state.GetDestination(loc, direction), true);

        }
	}
}