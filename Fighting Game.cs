using System;
using System.Diagnostics;
using System.Threading;

class ByDenisRafi
{
	static readonly int height = Console.WindowHeight;
	static readonly int width = Console.WindowWidth;
	static readonly int barWidth = (width - 1) / 2;
	const int Y = 7;
	static readonly TimeSpan sleep = TimeSpan.FromMilliseconds(10);
	static readonly TimeSpan timeSpanIdle = TimeSpan.FromMilliseconds(400);
	static readonly TimeSpan timeSpanPunch = TimeSpan.FromMilliseconds(100);
	static readonly TimeSpan timeSpanBlock = TimeSpan.FromMilliseconds(800);
	static readonly TimeSpan timeSpanJumpKick = TimeSpan.FromMilliseconds(100);
	static readonly TimeSpan timeSpanOwned = TimeSpan.FromMilliseconds(30);
	static readonly TimeSpan timeSpanGround = TimeSpan.FromMilliseconds(600);
	static readonly TimeSpan timeSpanGetUp = TimeSpan.FromMilliseconds(80);
	static readonly Random random = new Random();
	enum DR
	{
		Idle = 0,
		Punch = 1,
		Block = 2,
		JumpKick = 3,
		Owned = 4,
		Ground = 5,
		GetUp = 6,
	}
	class F
	{
		public DR DR = DR.Idle;
		public int Frame = 0;
		public int Position;
		public Stopwatch Stopwatch = new Stopwatch();
		public int MaxEnergy = 60;
		public int Energy = 40;
		public int MaxHealth = 10;
		public int Health = 10;
		public string[] IdleAnimation;
		public string[] PunchAnimation;
		public string[] BlockAnimation;
		public string[] JumpKickAnimation;
		public string[] OwnedAnimation;
		public string[] GroundAnimation;
		public string[] GetUpAnimation;
	}

	static void Main()
	{
		Console.Title = "Fighting Game. Keys F, D, S";
		Console.Clear();
		Console.CursorVisible = false;

		F player = new F()
		{
			Position = width / 3,
			IdleAnimation = DR2.DR3.IdleAnimation,
			PunchAnimation = DR2.DR3.PunchAnimation,
			BlockAnimation = DR2.DR3.BlockAnimation,
			JumpKickAnimation = DR2.DR3.JumpKickAnimation,
			OwnedAnimation = DR2.DR3.OwnedAnimation,
			GroundAnimation = DR2.DR3.GroundAnimation,
			GetUpAnimation = DR2.DR3.GetUpAnimation,
		};
		F enemy = new F()
		{
			Position = (width / 3) * 2,
			IdleAnimation = DR2.DR1.IdleAnimation,
			PunchAnimation = DR2.DR1.PunchAnimation,
			BlockAnimation = DR2.DR1.BlockAnimation,
			JumpKickAnimation = DR2.DR1.JumpKickAnimation,
			OwnedAnimation = DR2.DR1.OwnedAnimation,
			GroundAnimation = DR2.DR1.GroundAnimation,
			GetUpAnimation = DR2.DR1.GetUpAnimation,
		};
		player.Stopwatch.Restart();
		enemy.Stopwatch.Restart();
		Console.SetCursorPosition(player.Position, Y);
		Render(DR2.DR3.IdleAnimation[player.Frame]);
		Console.SetCursorPosition(enemy.Position, Y);
		Render(DR2.DR1.IdleAnimation[enemy.Frame]);
		Console.SetCursorPosition(0, Y + 6);
		for (int i = 0; i < width; i++)
		{
			Console.Write('`');
		}
		while (true)
		{
			#region 
			if (Console.WindowHeight != height || Console.WindowWidth != width)
			{
				Console.Clear();
				Console.Write("Closed.");
				return;
			}
			#endregion
			bool skipPlayerUpdate = false;
			bool skipEnemyUpdate = false;
			#region 
			static void Trigger(F fighter, DR action)
			{
				if (!(fighter.Energy >= action switch
				{
					DR.Punch => 10,
					DR.JumpKick => 20,
					DR.Block => 0,
					_ => throw new NotImplementedException(),
				})) return;
				Console.SetCursorPosition(fighter.Position, Y);
				Erase(fighter.IdleAnimation[fighter.Frame]);
				fighter.DR = action;
				fighter.Frame = 0;
				fighter.Energy = Math.Max(action switch
				{
					DR.Punch => fighter.Energy - 10,
					DR.JumpKick => fighter.Energy - 20,
					DR.Block => fighter.Energy,
					_ => throw new NotImplementedException(),
				}, 0);
				Console.SetCursorPosition(fighter.Position, Y);
				Render(action switch
				{
					DR.Idle => fighter.IdleAnimation[fighter.Frame],
					DR.Punch => fighter.PunchAnimation[fighter.Frame],
					DR.Block => fighter.BlockAnimation[fighter.Frame],
					DR.JumpKick => fighter.JumpKickAnimation[fighter.Frame],
					DR.Owned => fighter.OwnedAnimation[fighter.Frame],
					DR.GetUp => fighter.GetUpAnimation[fighter.Frame],
					_ => throw new NotImplementedException(),
				});
				fighter.Stopwatch.Restart();
			}
			static void Move(F fighter, int location)
			{
				Console.SetCursorPosition(fighter.Position, Y);
				Erase(fighter.IdleAnimation[fighter.Frame]);
				fighter.Position = location;
				Console.SetCursorPosition(fighter.Position, Y);
				Render(fighter.IdleAnimation[fighter.Frame]);
			}
			#endregion
			#region 
			if (Console.KeyAvailable)
			{
				switch (Console.ReadKey(true).Key)
				{
					case ConsoleKey.F:
						if (player.DR == DR.Idle)
						{
							Trigger(player, DR.Punch);
							skipPlayerUpdate = true;
						}
						break;
					case ConsoleKey.D:
						if (player.DR == DR.Idle)
						{
							Trigger(player, DR.Block);
							skipPlayerUpdate = true;
						}
						break;
					case ConsoleKey.S:
						if (player.DR == DR.Idle)
						{
							Trigger(player, DR.JumpKick);
							skipPlayerUpdate = true;
						}
						break;
					case ConsoleKey.LeftArrow:
						if (player.DR == DR.Idle)
						{
							int newPosition = Math.Min(Math.Max(player.Position - 1, 0), enemy.Position - 4);
							if (newPosition != player.Position && player.Energy >= 2)
							{
								Move(player, newPosition);
								skipPlayerUpdate = true;
								player.Energy = Math.Max(player.Energy - 1, 0);
							}
						}
						break;
					case ConsoleKey.RightArrow:
						if (player.DR == DR.Idle)
						{
							int newPosition = Math.Min(Math.Max(player.Position + 1, 0), enemy.Position - 4);
							if (newPosition != player.Position && player.Energy >= 2)
							{
								Move(player, newPosition);
								skipPlayerUpdate = true;
								player.Energy = Math.Max(player.Energy - 1, 0);
							}
						}
						break;
					case ConsoleKey.Escape:
						Console.Clear();
						return;
				}
			}
			while (Console.KeyAvailable)
			{
				Console.ReadKey(true);
			}
			#endregion
			#region
			if (enemy.DR == DR.Idle)
			{
				if (enemy.Position - player.Position <= 5 && random.Next(10) == 0)
				{
					Trigger(enemy, DR.Punch);
					skipEnemyUpdate = true;
				}
				else if (enemy.Position - player.Position <= 5 && random.Next(10) == 0)
				{
					Trigger(enemy, DR.JumpKick);
					skipEnemyUpdate = true;
				}
				else if (enemy.Position - player.Position <= 5 && random.Next(7) == 0 && player.Energy >= 9)
				{
					Trigger(enemy, DR.Block);
					skipEnemyUpdate = true;
				}
				else if (random.Next(10) == 0 && enemy.Energy >= 2 && (enemy.Energy == enemy.MaxEnergy || random.Next(enemy.MaxEnergy - enemy.Energy + 3) == 0))
				{
					int newPosition = Math.Min(Math.Max(enemy.Position - 1, player.Position + 4), width - 9);
					if (enemy.Position != newPosition)
					{
						Move(enemy, newPosition);
						skipEnemyUpdate = true;
						enemy.Energy = Math.Max(enemy.Energy - 1, 0);
					}
				}
				else if (random.Next(13) == 0 && enemy.Energy >= 2 && (enemy.Energy == enemy.MaxEnergy || random.Next(enemy.MaxEnergy - enemy.Energy + 3) == 0))
				{
					int newPosition = Math.Min(Math.Max(enemy.Position + 1, player.Position + 4), width - 9);
					if (enemy.Position != newPosition)
					{
						Move(enemy, newPosition);
						skipEnemyUpdate = true;
						enemy.Energy = Math.Max(enemy.Energy - 1, 0);
					}
				}
			}
			#endregion
			#region 
			if (!skipPlayerUpdate)
			{
				Update(player);
			}
			if (!skipEnemyUpdate)
			{
				Update(enemy);
			}
			void Update(F fighter)
			{
				if (fighter.DR == DR.Idle && fighter.Stopwatch.Elapsed > timeSpanIdle)
				{
					Console.SetCursorPosition(fighter.Position, Y);
					Erase(fighter.IdleAnimation[fighter.Frame]);
					fighter.Frame = fighter.Frame is 0 ? 1 : 0;
					Console.SetCursorPosition(fighter.Position, Y);
					Render(fighter.IdleAnimation[fighter.Frame]);
					fighter.Stopwatch.Restart();
					fighter.Energy = Math.Min(fighter.Energy + 1, fighter.MaxEnergy);
				}
				else if (fighter.DR == DR.Punch && fighter.Stopwatch.Elapsed > timeSpanPunch)
				{
					Console.SetCursorPosition(fighter.Position, Y);
					Erase(fighter.PunchAnimation[fighter.Frame]);
					fighter.Frame++;
					F opponent = fighter == player ? enemy : player;
					if (Math.Abs(opponent.Position - fighter.Position) <= 5 &&
						2 >= fighter.Frame && fighter.Frame <= 3 &&
						opponent.DR != DR.Block &&
						opponent.DR != DR.GetUp &&
						opponent.DR != DR.Ground &&
						opponent.DR != DR.Owned)
					{
						opponent.Health -= 4;
						Console.SetCursorPosition(opponent.Position, Y);
						Erase(opponent.DR switch
						{
							DR.Punch => opponent.PunchAnimation[opponent.Frame],
							DR.Idle => opponent.IdleAnimation[opponent.Frame],
							DR.JumpKick => opponent.JumpKickAnimation[opponent.Frame],
							_ => throw new NotImplementedException(),
						});
						opponent.DR = DR.Owned;
						opponent.Frame = 0;
						Console.SetCursorPosition(opponent.Position, Y);
						Render(opponent.OwnedAnimation[opponent.Frame]);
						opponent.Stopwatch.Restart();
					}
					if (fighter.Frame >= fighter.PunchAnimation.Length)
					{
						fighter.DR = DR.Idle;
						fighter.Frame = 0;
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.IdleAnimation[fighter.Frame]);
					}
					else
					{
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.PunchAnimation[fighter.Frame]);
					}
					fighter.Stopwatch.Restart();
				}
				else if (fighter.DR == DR.Block && fighter.Stopwatch.Elapsed > timeSpanBlock)
				{
					Console.SetCursorPosition(fighter.Position, Y);
					Erase(fighter.BlockAnimation[fighter.Frame]);
					fighter.DR = DR.Idle;
					fighter.Frame = 0;
					Console.SetCursorPosition(fighter.Position, Y);
					Render(fighter.IdleAnimation[fighter.Frame]);
					fighter.Stopwatch.Restart();
				}
				else if (fighter.DR ==DR.JumpKick && fighter.Stopwatch.Elapsed > timeSpanJumpKick)
				{
					Console.SetCursorPosition(fighter.Position, Y);
					Erase(fighter.JumpKickAnimation[fighter.Frame]);
					fighter.Frame++;
					F opponent = fighter == player ? enemy : player;
					if (Math.Abs(opponent.Position - fighter.Position) <= 5 &&
						fighter.Frame == 5 &&
						opponent.DR != DR.GetUp &&
						opponent.DR != DR.Ground &&
						opponent.DR != DR.Owned)
					{
						opponent.Health -= opponent.DR == DR.Block ? 4 : 8;
						Console.SetCursorPosition(opponent.Position, Y);
						Erase(opponent.DR switch
						{
							DR.Punch => opponent.PunchAnimation[opponent.Frame],
							DR.Idle => opponent.IdleAnimation[opponent.Frame],
							DR.JumpKick => opponent.JumpKickAnimation[opponent.Frame],
							DR.Block => opponent.BlockAnimation[opponent.Frame],
							_ => throw new NotImplementedException(),
						});
						opponent.DR = DR.Owned;
						opponent.Frame = 0;
						Console.SetCursorPosition(opponent.Position, Y);
						Render(opponent.OwnedAnimation[opponent.Frame]);
						opponent.Stopwatch.Restart();
					}
					if (fighter.Frame >= fighter.JumpKickAnimation.Length)
					{
						fighter.DR = DR.Idle;
						fighter.Frame = 0;
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.IdleAnimation[fighter.Frame]);
					}
					else
					{
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.JumpKickAnimation[fighter.Frame]);
					}
					fighter.Stopwatch.Restart();
				}
				else if (fighter.DR == DR.Owned && fighter.Stopwatch.Elapsed > timeSpanOwned)
				{
					Console.SetCursorPosition(fighter.Position, Y);
					Erase(fighter.OwnedAnimation[fighter.Frame]);
					fighter.Frame++;
					if (fighter.Frame >= fighter.OwnedAnimation.Length)
					{
						fighter.DR = DR.Ground;
						fighter.Frame = 0;
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.GroundAnimation[fighter.Frame]);
					}
					else
					{
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.OwnedAnimation[fighter.Frame]);
					}
					fighter.Stopwatch.Restart();
				}
				else if (fighter.DR == DR.Ground && fighter.Stopwatch.Elapsed > timeSpanGround)
				{
					Console.SetCursorPosition(fighter.Position, Y);
					Erase(fighter.GroundAnimation[fighter.Frame]);
					fighter.Frame++;
					if (fighter.Frame >= fighter.GroundAnimation.Length)
					{
						fighter.DR = DR.GetUp;
						fighter.Frame = 0;
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.GetUpAnimation[fighter.Frame]);
					}
					else
					{
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.GroundAnimation[fighter.Frame]);
					}
					fighter.Stopwatch.Restart();
				}
				else if (fighter.DR == DR.GetUp && fighter.Stopwatch.Elapsed > timeSpanGetUp)
				{
					Console.SetCursorPosition(fighter.Position, Y);
					Erase(fighter.GetUpAnimation[fighter.Frame]);
					fighter.Frame++;
					if (fighter.Frame >= fighter.GetUpAnimation.Length)
					{
						fighter.DR = DR.Idle;
						fighter.Frame = 0;
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.IdleAnimation[fighter.Frame]);
					}
					else
					{
						Console.SetCursorPosition(fighter.Position, Y);
						Render(fighter.GetUpAnimation[fighter.Frame]);
					}
					fighter.Stopwatch.Restart();
				}
			}
			#endregion
			#region Render Player (to make sure player is always on top)
			Console.SetCursorPosition(player.Position, Y);
			Render(player.DR switch
			{
				DR.Idle => player.IdleAnimation[player.Frame],
				DR.Punch => player.PunchAnimation[player.Frame],
				DR.Block => player.BlockAnimation[player.Frame],
				DR.JumpKick => player.JumpKickAnimation[player.Frame],
				DR.Owned => player.OwnedAnimation[player.Frame],
				DR.Ground => player.GroundAnimation[player.Frame],
				DR.GetUp => player.GetUpAnimation[player.Frame],
				_ => throw new NotImplementedException(),
			});
			#endregion
			#region
			{
				char[] playerHealthBar = new char[barWidth];
				int playerHealthBarLevel = (int)((player.Health / (float)player.MaxHealth) * barWidth);
				for (int i = 0; i < barWidth; i++)
				{
					playerHealthBar[i] = i <= playerHealthBarLevel ? '█' : ' ';
				}
				char[] enemyHealthBar = new char[barWidth];
				int enemyHealthBarLevel = (int)((enemy.Health / (float)enemy.MaxHealth) * barWidth);
				for (int i = 0; i < barWidth; i++)
				{
					enemyHealthBar[barWidth - i - 1] = i <= enemyHealthBarLevel ? '█' : ' ';
				}
				string healthBars = "" + new string(playerHealthBar) + " " + new string(enemyHealthBar) + "";
				Console.SetCursorPosition(0, 1);
				Console.Write(healthBars);
			}
			{
				char[] playerEnergyBar = new char[barWidth];
				int playerEnergyBarLevel = (int)((player.Energy / (float)player.MaxEnergy) * barWidth);
				for (int i = 0; i < barWidth; i++)
				{
					playerEnergyBar[i] = i <= playerEnergyBarLevel ? '█' : ' ';
				}
				char[] enemyEnergyBar = new char[barWidth];
				int enemyEnergyBarLevel = (int)((enemy.Energy / (float)enemy.MaxEnergy) * barWidth);
				for (int i = 0; i < barWidth; i++)
				{
					enemyEnergyBar[barWidth - i - 1] = i <= enemyEnergyBarLevel ? '█' : ' ';
				}
				string energyBars = "" + new string(playerEnergyBar) + " " + new string(enemyEnergyBar) + "";
				Console.SetCursorPosition(0, 3);
				Console.Write(energyBars);
			}
			#endregion
			if (player.Health <= 0 && player.DR == DR.Ground)
			{
				Console.SetCursorPosition(0, Y + 8);
				Console.Write("You Lose!");
				break;
			}
			if (enemy.Health <= 0 && enemy.DR == DR.Ground)
			{
				Console.SetCursorPosition(0, Y + 8);
				Console.Write("You Win!");
				break;
			}
			Thread.Sleep(sleep);
		}
		Console.ReadLine();
	}
	#region 
	static void Render(string @string, bool renderSpace = false)
	{
		int x = Console.CursorLeft;
		int y = Console.CursorTop;
		foreach (char c in @string)
			if (c is '\n')
				Console.SetCursorPosition(x, ++y);
			else if (Console.CursorLeft < width - 1 && (!(c is ' ') || renderSpace))
				Console.Write(c);
			else if (Console.CursorLeft < width - 1 && Console.CursorTop < height - 1)
				Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
	}
	static void Erase(string @string)
	{
		int x = Console.CursorLeft;
		int y = Console.CursorTop;
		foreach (char c in @string)
			if (c is '\n')
				Console.SetCursorPosition(x, ++y);
			else if (Console.CursorLeft < width - 1 && !(c is ' '))
				Console.Write(' ');
			else if (Console.CursorLeft < width - 1 && Console.CursorTop < height - 1)
				Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
	}
	#endregion
	static class DR2
	{
		#region 
		public static class DR3
		{
			public static readonly string[] IdleAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"    O    " + '\n' +
				@"   L|(   " + '\n' +
				@"    |    " + '\n' +
				@"   ( \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   ((L   " + '\n' +
				@"    |    " + '\n' +
				@"   / )   ",
			};
			public static readonly string[] BlockAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o_|  " + '\n' +
				@"    |-'  " + '\n' +
				@"    |    " + '\n' +
				@"   / /   ",
			};
			public static readonly string[] PunchAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"   _o_.  " + '\n' +
				@"   (|    " + '\n' +
				@"    |    " + '\n' +
				@"   > \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o__. " + '\n' +
				@"   (|    " + '\n' +
				@"    |    " + '\n' +
				@"   / >   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    O___." + '\n' +
				@"   L(    " + '\n' +
				@"    |    " + '\n' +
				@"   / >   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o_   " + '\n' +
				@"   L( \  " + '\n' +
				@"    |    " + '\n' +
				@"   > \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o_   " + '\n' +
				@"   L( >  " + '\n' +
				@"    |    " + '\n' +
				@"   > \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   (|)   " + '\n' +
				@"    |    " + '\n' +
				@"   / \   ",
			};
			public static readonly string[] JumpKickAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"    _O   " + '\n' +
				@"   |/|_  " + '\n' +
				@"   /\    " + '\n' +
				@"  /  |   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"     o   " + '\n' +
				@"   </<   " + '\n' +
				@"    >>   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    O    " + '\n' +
				@"   <|<   " + '\n' +
				@"    |    " + '\n' +
				@"   /|    ",
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   L|<   " + '\n' +
				@"    >    " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"   _o_   " + '\n' +
				@"   L|_   " + '\n' +
				@"    |/   " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"   _o_   " + '\n' +
				@"   <|___." + '\n' +
				@"    |    " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   (<_   " + '\n' +
				@"    |/   " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"     o   " + '\n' +
				@"   </<   " + '\n' +
				@"    >>   ",
			};
			public static readonly string[] OwnedAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"   O___  " + '\n' +
				@"    \`-  " + '\n' +
				@"    /\   " + '\n' +
				@"   / /   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"   //    " + '\n' +
				@"  O/__   " + '\n' +
				@"   __/\  " + '\n' +
				@"      /  ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"   //    " + '\n' +
				@"  O/__/\ " + '\n' +
				@"       \  ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"  o___/\ ",
			};
			public static readonly string[] GroundAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"  o___/\ ",
			};
			public static readonly string[] GetUpAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"     __  " + '\n' +
				@"  o__\   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"     /   " + '\n' +
				@"  o__\   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"     |   " + '\n' +
				@"     |   " + '\n' +
				@"  o_/    ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"     |\  " + '\n' +
				@"  o_/    " + '\n' +
				@"  /\     ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"   /-\   " + '\n' +
				@" /o/ //  " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"  /o|\   " + '\n' +
				@"      \  " + '\n' +
				@"     //  ",
				@"         " + '\n' +
				@"    _    " + '\n' +
				@"  __O\   " + '\n' +
				@"     \   " + '\n' +
				@"     /\  " + '\n' +
				@"    / /  ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"     o   " + '\n' +
				@"   </<   " + '\n' +
				@"    >>   ",
			};
		}
		public static class DR1
		{
			public static readonly string[] IdleAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"    O    " + '\n' +
				@"   )|J   " + '\n' +
				@"    |    " + '\n' +
				@"   / )   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   J))   " + '\n' +
				@"    |    " + '\n' +
				@"   ( \   ",
			};
			public static readonly string[] BlockAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"  |_o    " + '\n' +
				@"  '-|    " + '\n' +
				@"    |    " + '\n' +
				@"   \ \   ",
			};
			public static readonly string[] PunchAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"  ._o_   " + '\n' +
				@"    |)   " + '\n' +
				@"    |    " + '\n' +
				@"   / <   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@" .__o    " + '\n' +
				@"    |)   " + '\n' +
				@"    |    " + '\n' +
				@"   < \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@".___O    " + '\n' +
				@"    )J   " + '\n' +
				@"    |    " + '\n' +
				@"   < \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"   _o    " + '\n' +
				@"  / )J   " + '\n' +
				@"    |    " + '\n' +
				@"   / <   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"   _o    " + '\n' +
				@"  < )J   " + '\n' +
				@"    |    " + '\n' +
				@"   / <   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   (|)   " + '\n' +
				@"    |    " + '\n' +
				@"   / \   ",
			};
			public static readonly string[] JumpKickAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"  O_     " + '\n' +
				@" _|\|    " + '\n' +
				@"   /\    " + '\n' +
				@"  |  \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"    >\>  " + '\n' +
				@"    <<   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    O    " + '\n' +
				@"   >|>   " + '\n' +
				@"    |    " + '\n' +
				@"    |\   ",
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   >|J   " + '\n' +
				@"    <    " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"   _o_   " + '\n' +
				@"   _|J   " + '\n' +
				@"   \|    " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"   _o_   " + '\n' +
				@".___|>   " + '\n' +
				@"    |    " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"   _>)   " + '\n' +
				@"   \|    " + '\n' +
				@"    |    " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"    o    " + '\n' +
				@"    >\>   " + '\n' +
				@"    <<   ",
			};
			public static readonly string[] OwnedAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"  ___O   " + '\n' +
				@"  -'/    " + '\n' +
				@"   /\    " + '\n' +
				@"   \ \   ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"    \\   " + '\n' +
				@"   __\O  " + '\n' +
				@"  /\__   " + '\n' +
				@"  \      ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"    \\   " + '\n' +
				@"  /\__O  " + '\n' +
				@"  /      ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@" /\___o ",
			};
			public static readonly string[] GroundAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@" /\___o  ",
			};
			public static readonly string[] GetUpAnimation = new string[]
			{
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"  __     " + '\n' +
				@"   /__o  ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"   \     " + '\n' +
				@"   /__o  ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"   |     " + '\n' +
				@"   |     " + '\n' +
				@"    \_o  ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"   /|    " + '\n' +
				@"    \_o  " + '\n' +
				@"      /\ ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"    /-\  " + '\n' +
				@"  // /o/ " + '\n' +
				@"         ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"   /|o\  " + '\n' +
				@"  /      " + '\n' +
				@"  \\     ",
				@"         " + '\n' +
				@"    _    " + '\n' +
				@"   /O__  " + '\n' +
				@"   /     " + '\n' +
				@"  /\     " + '\n' +
				@"  \ \    ",
				@"         " + '\n' +
				@"         " + '\n' +
				@"         " + '\n' +
				@"   o     " + '\n' +
				@"   >\>   " + '\n' +
				@"   <<    ",
			};
		}
		#endregion
	}
}