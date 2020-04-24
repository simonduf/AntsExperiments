using System;
using System.Collections.Generic;
using static Ants.Logger;
using StateTile = Ants.Tile;

namespace Ants {
	
	public class GameState
	{
		
		public int Width { get; private set; }
		public int Height { get; private set; }
		
		public int LoadTime { get; private set; }
		public int TurnTime { get; private set; }
		
		private DateTime turnStart;
		public int TimeRemaining {
			get {
				TimeSpan timeSpent = DateTime.Now - turnStart;
				return TurnTime - (int)timeSpent.TotalMilliseconds;
			}
		}

		public int ViewRadius2 { get; private set; }
		public int AttackRadius2 { get; private set; }
		public int SpawnRadius2 { get; private set; }

		public List<Ant> MyAnts { get; private set; }
		public List<AntHill> MyHills { get; private set; }
		public List<Ant> EnemyAnts { get; private set; }
		public List<AntHill> EnemyHills { get; private set; }
		public List<Location> DeadTiles { get; private set; }
		public List<Location> FoodTiles { get; private set; }

		
		
		public enum Terrain
		{
			Land, Water, Unknown
		}

		public struct Tile
		{
			public Terrain terrain;
			public bool isEnemyAnt;
			public bool isEnemyHill;
			public bool isMyAnt;
			public bool isMyHill;
			public bool isDeadAnt;
			public bool isVisible;
			public bool isFood;
			public bool isActiveHill;
		}

		private readonly Tile defaultTile = new Tile()
		{
			terrain = Terrain.Unknown,
			isEnemyAnt = false,
			isEnemyHill = false,
			isVisible = false,
			isDeadAnt = false,
			isMyHill = false,
			isMyAnt = false,
			isFood = false,
			isActiveHill = false,
		};



		private static void ClearDynamic(ref Tile tile)
		{
			tile.isVisible = false;
			tile.isEnemyAnt = false;
			tile.isDeadAnt = false;
			tile.isMyAnt = false;
			tile.isFood = false;
			tile.isActiveHill = false;
		}


		public Vector2i[] coords;
		public Tile[,] map;
		
		public GameState (int width, int height, 
		                  int turntime, int loadtime, 
		                  int viewradius2, int attackradius2, int spawnradius2) {
			
			Width = width;
			Height = height;
			
			LoadTime = loadtime;
			TurnTime = turntime;
			
			ViewRadius2 = viewradius2;
			AttackRadius2 = attackradius2;
			SpawnRadius2 = spawnradius2;
			
			MyAnts = new List<Ant>();
			MyHills = new List<AntHill>();
			EnemyAnts = new List<Ant>();
			EnemyHills = new List<AntHill>();
			DeadTiles = new List<Location>();
			FoodTiles = new List<Location>();
			


			map = new Tile[width, height];
			coords = new Vector2i[width * height];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					coords[i + j * width] = new Vector2i(i, j);
					map[i, j] = defaultTile;
				}
			}

		}

		#region State mutators
		public void StartNewTurn () {
			// start timer
			turnStart = DateTime.Now;

			foreach (var coord in coords)
				ClearDynamic(ref map[coord.x, coord.y]);
			

			MyHills.Clear();
			MyAnts.Clear();
			EnemyHills.Clear();
			EnemyAnts.Clear();
			DeadTiles.Clear();
			FoodTiles.Clear();
		}

		public void AddAnt (int row, int col, int team) 
		{
			
			Ant ant = new Ant(row, col, team);
			if (team == 0) {
				MyAnts.Add(ant);
				map[col, row].isMyAnt = true;
			} else {
				EnemyAnts.Add(ant);
				map[col, row].isEnemyAnt = true;
			}

		}

		public void AddFood (int row, int col) {
			map[col, row].isFood = true;
			FoodTiles.Add(new Location(row, col));
		}

		public void RemoveFood (int row, int col) {
			map[col, row].isFood = false;
			FoodTiles.Remove(new Location(row, col));
		}

		public void AddWater (int row, int col) {
			map[col, row].terrain = Terrain.Water;
		}

		public void DeadAnt (int row, int col) {

			map[col, row].isDeadAnt = true;

			// but always add to the dead list
			DeadTiles.Add(new Location(row, col));
		}

		public void AntHill (int row, int col, int team) {


			AntHill hill = new AntHill (row, col, team);
			map[col, row].isActiveHill = true;
			if (team == 0)
			{
				MyHills.Add(hill);
				map[col, row].isMyHill = true;
			}
			else
			{
				EnemyHills.Add(hill);
				map[col, row].isEnemyHill = true;
			}
		}
		#endregion

		
		private void WrapAround(ref int row, ref int col)
		{
			row = (row) % Height;
			if (row < 0) row += Height; // because the modulo of a negative number is negative

			col = (col) % Width;
			if (col < 0) col += Width;
		}

		public void CalculateVisibility()
		{
			
				

			foreach (Ant ant in this.MyAnts)
			{
				foreach (Location offset in Offsets)
				{
					int col = ant.Col + offset.Col;
					int row = ant.Row + offset.Row;

					WrapAround(ref row, ref col);

					map[col, row].isVisible = true;

					if (map[col, row].terrain == Terrain.Unknown)
						map[col, row].terrain = Terrain.Land;
				}
			}

		}

		public void ClearHills()
		{
			foreach(var coord in coords)
			{
				var tile = map[coord.x, coord.y];

				bool isHill = tile.isEnemyHill || tile.isMyHill;

				if (isHill && !tile.isActiveHill && tile.isVisible)
				{
					tile.isEnemyHill = false;
					tile.isMyHill = false;
					map[coord.x, coord.y] = tile;
				}
			}
		}

		

		

		public void Set(StateTile[,] state)
		{
			StartNewTurn();

			foreach(var coord in coords)
			{
				map[coord.x, coord.y].isVisible = true;

				switch(state[coord.y, coord.x])
				{
					case StateTile.Land:
						break;

					case StateTile.Water:
						AddWater(coord.y, coord.x);
						break;

					case StateTile.Food:
						AddFood(coord.y, coord.x);
						break;

					case StateTile.MyAnt:
						AddAnt(coord.y, coord.x, 0);
						break;

					case StateTile.TheirAnt:
						AddAnt(coord.y, coord.x, 1);
						break;

					case StateTile.MyHill:
						AntHill(coord.y, coord.x, 0);
						break;

					case StateTile.TheirHill:
						AntHill(coord.y, coord.x, 1);
						break;

					case StateTile.Unseen:
						map[coord.x, coord.y].isVisible = false;
						break;

					default:
						break;
				}

				//
				//	Set visibility
				//
				if (map[coord.x, coord.y].terrain == Terrain.Unknown && map[coord.x, coord.y].isVisible)
					map[coord.x, coord.y].terrain = Terrain.Land;

				ClearHills();

			}

		}

		private List<Location> offsets;
		public List<Location> Offsets
		{
			get
			{
				if (offsets == null)
				{
					offsets = new List<Location>();
					int squares = (int)Math.Floor(Math.Sqrt(this.ViewRadius2));
					for (int r = -1 * squares; r <= squares; ++r)
					{
						for (int c = -1 * squares; c <= squares; ++c)
						{
							int square = r * r + c * c;
							if (square < this.ViewRadius2)
							{
								offsets.Add(new Location(r, c));
							}
						}
					}
				}
				return offsets;
			}
		}

	}
}

