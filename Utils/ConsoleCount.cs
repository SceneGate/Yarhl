//-----------------------------------------------------------------------
// <copyright file="ConsoleCount.cs" company="none">
// Copyright (C) 2013
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>20/09/2013</date>
//-----------------------------------------------------------------------
using System;

namespace Libgame.Utils
{
	public class ConsoleCount
	{
		private int index;
		private int x;
		private int y;
		private int updX;
		private int updY;
		private int total;
		private string message;

		public ConsoleCount(string msg, int total)
		{
			this.index = 1;
			this.x = Console.CursorLeft;
			this.y = Console.CursorTop;
			this.updX = this.x;
			this.updY = this.y + 1;
			this.total = total;
			this.message = msg;

			if (this.updY == Console.BufferHeight) {
				Console.WriteLine();
				this.updY--;
				this.y--;
			}
		}

		public void Show()
		{
			Console.SetCursorPosition(this.x, this.y);
			Console.WriteLine(this.message, this.index++, this.total);
			Console.SetCursorPosition(this.updX, this.updY);
		}

		public void UpdateCoordinates()
		{
			this.updX = Console.CursorLeft;
			this.updY = Console.CursorTop;
		}
	}
}

