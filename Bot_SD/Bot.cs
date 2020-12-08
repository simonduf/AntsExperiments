using System;

namespace Ants {
	public abstract class Bot {

		public abstract void DoTurn(GameState state);

		public static void IssueOrder(GameState state, Location loc, Direction direction, string action) {
			System.Console.Out.WriteLine("o {0} {1} {2}", loc.Row, loc.Col, direction.ToChar());

            Logger.Log.Trace("Ant Moved : {0} => {1} ({2}) ", loc, direction,action);

            if (state.OccupiedNextRound.At(loc) != true)
                Logger.Log.Error("Source Location was not occupied (ant moved)...   ANT {0}  {1}", loc, new System.Diagnostics.StackTrace(1, true));

            if (state.OccupiedNextRound.At(state.GetDestination(loc, direction)) == true)
                Logger.Log.Error( "Dest Location is occupied...   ANT {0}  {1}", loc,  new System.Diagnostics.StackTrace(1,true));

            state.OccupiedNextRound.Set(loc, false);
            state.OccupiedNextRound.Set(state.GetDestination(loc, direction), true);

        }
	}
}