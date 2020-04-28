using System;

namespace Ants {

	public enum Direction {
		North,
		East,
        South,
        West
    }

    public enum Direction8
    {
        North = Direction.North,
        South = Direction.South,
        East = Direction.East,
        West = Direction.West,
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest
    }

	public static class DirectionExtensions {

		public static char ToChar (this Direction self) {
            return self switch
            {
                Direction.East => 'e',
                Direction.North => 'n',
                Direction.South => 's',
                Direction.West => 'w',
                _ => throw new ArgumentException("Unknown direction", "self"),
            };
        }

        public static Direction Clockwise(this Direction self)
        {
            return self switch
            {
                Direction.East => Direction.South,
                Direction.North => Direction.East,
                Direction.South => Direction.West,
                Direction.West => Direction.North,
                _ => throw new ArgumentException("Unknown direction", "self"),
            };
        }

        public static Direction CounterClockwise(this Direction self)
        {
            return self.Clockwise().Flip();
        }

        public static Direction Flip (this Direction self)
        {
            return self switch
            {
                Direction.East => Direction.West,
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                _ => throw new ArgumentException("Unknown direction", "self"),
            };
        }
	}

    public static class Direction8Extensions
    {
		public static Direction8 Clockwise45(this Direction8 self)
		{
            return self switch
            {
                Direction8.East => Direction8.SouthEast,
                Direction8.SouthEast => Direction8.South,
                Direction8.South => Direction8.SouthWest,
                Direction8.SouthWest => Direction8.West,
                Direction8.West => Direction8.NorthWest,
                Direction8.NorthWest => Direction8.North,
                Direction8.North => Direction8.NorthEast,
                Direction8.NorthEast => Direction8.East,
                _ => throw new ArgumentException("Unknown direction", "self"),
            };
        }

        public static Direction Clockwise45IfDiagonal(this Direction8 self)
        {
            return self switch
            {
                Direction8.NorthEast => Direction.East,
                Direction8.East => Direction.East,
                Direction8.SouthEast => Direction.South,
                Direction8.South => Direction.South,
                Direction8.SouthWest => Direction.West,
                Direction8.West => Direction.West,
                Direction8.NorthWest => Direction.North,
                Direction8.North => Direction.North,
                _ => throw new ArgumentException("Unknown direction", "self"),
            };
        }
	}
}