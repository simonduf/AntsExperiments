using System;

namespace Ants {

	public enum Direction {
		North,
		South,
		East,
		West


}

	public static class DirectionExtensions {

        public static Direction FromChar(char c)
        {
            switch (c)
            {
                case 'e':
                    return Direction.East;

                case 'n':
                    return Direction.North;

                case 's':
                    return Direction.South;

                case 'w':
                    return Direction.West;

                default:
                    throw new ArgumentException("Unknown direction", "self");
            }
        }

        public static char ToChar (this Direction self) {
			switch (self)
			{
				case Direction.East:
					return 'e';

				case Direction.North:
					return 'n';

				case Direction.South:
					return 's';

				case Direction.West:
					return 'w';

				default:
					throw new ArgumentException ("Unknown direction", "self");
			}
		}

        public static Direction Opposite(this Direction self)
        {
            switch (self)
            {
                case Direction.East:
                    return Direction.West;

                case Direction.North:
                    return Direction.South;

                case Direction.South:
                    return Direction.North;

                case Direction.West:
                    return Direction.East;

                default:
                    throw new ArgumentException("Unknown direction", "self");
            }
        }


    }
}