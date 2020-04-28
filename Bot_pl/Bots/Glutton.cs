using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Ants.Logger;


namespace Ants {

	class Glutton : Bot {

        public const int MIN_ANTS_PER_HILL_BEFORE_GUARDING_HILLS = 10;

        int turn = 0;
        IGameState gameState;
        // Contains locations where my ants are moving this turn to avoid collisions
        List<Location> antDestinations = new List<Location>();
        // Contains list of ants that are guarding hills
        Dictionary<AntHill, List<Ant>> guards = new Dictionary<AntHill, List<Ant>>();

        public override void DoTurn (IGameState state) {

            // Match Init
            if (++turn == 1)
            {
            }

            try
            {
                // Turn init
                gameState = state;
                antDestinations.Clear();
                guards.Clear();
                foreach (var hill in gameState.MyHills)
                {
                    guards.Add(hill, gameState.MyAnts.FindAll(ant => gameState.GetRealDistance2(hill, ant) <= 2));
                }

                // Once we have enough ants, protect hills agains wild Jonathans
                if (gameState.MyAnts.Count > (gameState.MyHills.Count * MIN_ANTS_PER_HILL_BEFORE_GUARDING_HILLS))
                {
                    foreach (var hill in gameState.MyHills)
                    {
                        // Is there an ant on the hill?
                        var antOnHill = gameState.MyAnts.Find(x => x.IsAt(hill));
                        if (antOnHill != null)
                        {
                            // First, try to fill an empty adjacent spot
                            if (!HasImmediateGuard(hill, out Direction emptyImmediateSpot))
                            {
                                IssueOrderOrThrow(antOnHill, emptyImmediateSpot);
                            }
                            // Second, try to push a guard to fill all 8 spots
                            else if (!HasFullGuard(hill, out Direction8 emptyDiagonalSpot))
                            {
                                gameState.FindClosest(gameState.GetDestination(hill, emptyDiagonalSpot), guards[hill], out Location guardPostToReplace, out List<Direction> path);

                                IssueOrderOrThrow(guards[hill].Find(x => x.IsAt(guardPostToReplace)), path.Single().Flip());
                                IssueOrderOrThrow(antOnHill, gameState.GetDirections(antOnHill, guardPostToReplace).Single());
                            }
                            // Else, push one guard away
                            else
                            {
                                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                                {
                                    var guard = gameState.MyAnts.Find(x => x.IsAt(gameState.GetDestination(antOnHill, direction)));
                                    if (guard == null) continue;
                                    var hasMoved = TryIssueOrder(guard, direction);
                                    if (hasMoved)
                                    {
                                        IssueOrderOrThrow(antOnHill, direction);
                                        guards[hill].Remove(guard);
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (var guard in guards[hill])
                        {
                            DontMove(guard);
                        }
                    }
                }

                // Food is #1 priority. Make sure all food is assigned to an ant
                foreach (var food in gameState.FoodTiles)
                {
                    // check if we have time left to calculate more orders
                    if (gameState.TimeRemaining < 10) break;

                    if (gameState.FindClosest(food, gameState.MyAnts.Select(x => x as Location), out var closestAnt, out var path))
                    {
                        var assignee = gameState.MyAnts.Find(x => x.IsAt(closestAnt));
                        // Path is from food to ant, need to reverse direction
                        TryIssueOrder(assignee, path.Last().Flip());
                    }
                }

                // If no food in sight, move along
                foreach (Ant ant in gameState.MyAnts.Where(x => !x.HasOrder))
                {
                    // check if we have time left to calculate more orders
                    if (gameState.TimeRemaining < 10) break;

                    MoveAlong(ant);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error at turn {turn}: {e}");
            }
			
		}

        private bool HasImmediateGuard(AntHill hill, out Direction emptySpot)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (gameState.GetIsUnoccupied(gameState.GetDestination(hill, direction)))
                {
                    emptySpot = direction;
                    return false;
                }
            }
            emptySpot = Direction.North;
            return true;
        }

        private bool HasFullGuard(AntHill hill, out Direction8 emptySpot)
        {
            foreach (Direction8 direction in Enum.GetValues(typeof(Direction8)))
            {
                if (gameState.GetIsUnoccupied(gameState.GetDestination(hill, direction)))
                {
                    emptySpot = direction;
                    return false;
                }
            }
            emptySpot = Direction8.North;
            return true;
        }

        private void MoveAlong(Ant ant)
        {
            // Look around
            foreach (Direction8 direction in Enum.GetValues(typeof(Direction8)))
            {
                // You see any water? (guards are also water lol)
                if (gameState[gameState.GetDestination(ant, direction)] == Tile.Water
                    || guards.Any(x => x.Value.Contains(gameState.GetDestination(ant, direction))))
                {
                    var shoreRadar = direction;

                    // While there's water in front of us and 45 deg to the right
                    while (gameState[gameState.GetDestination(ant, shoreRadar)] == Tile.Water
                        || gameState[gameState.GetDestination(ant, shoreRadar.Clockwise45())] == Tile.Water
                        || guards.Any(x => x.Value.Contains(gameState.GetDestination(ant, shoreRadar))))
                    {
                        // Rotate right
                        shoreRadar = shoreRadar.Clockwise45();
                        if (shoreRadar.Clockwise45() == direction)
                        {
                            // Can't go anywhere, stand your ground
                            ant.HasOrder = true;
                            antDestinations.Add(ant);
                            break;
                        }
                    }
                    // Snap to grid
                    TryIssueOrder(ant, shoreRadar.Clockwise45IfDiagonal());
                }
            }

            // No water.
            // Go anywhere you can.
            MoveAnywhere(ant);
        }

        private bool MoveAnywhere(Ant ant)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (TryIssueOrder(ant, direction))
                {
                    return true;
                }
            }
            return false;
        }

        private void DontMove(Ant ant)
        {
            ant.HasOrder = true;
            antDestinations.Add(ant);
        }

        private void IssueOrderOrThrow(Ant ant, Direction direction)
        {
            if (!TryIssueOrder(ant, direction))
            {
                throw new Exception($"Moving ant at {ant} in direction {direction} failed.");
            }
        }

        private bool TryIssueOrder(Ant ant, Direction direction)
        {
            var destination = gameState.GetDestination(ant, direction);
            if (!antDestinations.Contains(destination) && gameState.GetIsPassable(destination))
            {
                IssueOrder(ant, direction);
                antDestinations.Add(destination);
                ant.HasOrder = true;
                return true;
            }
            else
            {
                return false;
            }
        }

    public static void Main (string[] args) {
			new Ants().PlayGame(new Glutton());
		}

	}
	
}