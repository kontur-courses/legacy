using System;
using System.Drawing;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using StatePrinter;

namespace Legacy.Demos.Solved
{
	[TestFixture]
	public class ToolsDemo
	{
		[Test]
        [UseReporter(typeof(DiffReporter))]
        public void Puzzle15_InitialState()
		{
			var puzzle15 = new Puzzle15();
            Approvals.Verify(puzzle15);
        }

		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void Puzzle15_MoveRight()
		{
			var puzzle15 = new Puzzle15();
            var state = puzzle15 + "\r\n";
            puzzle15.MoveRight();
		    state += "MOVE RIGHT\r\n";
            state += puzzle15;
            Approvals.Verify(state);
        }

		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void ApproveProductDataWithStateprinter()
		{
			var product = new Product
			{
				Id = Guid.Empty,
				Name = "Name",
				Price = 3.14m,
				UnitsCode = "112"
			};

            var stateprinter = new Stateprinter();
            stateprinter.Configuration.Project
                .Exclude<Product>(p => p.TemporaryData);
            var s = stateprinter.PrintObject(product);
            Approvals.Verify(s);
		}

		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void ProductData_IsJsonSerializable()
		{
			Product original = new Product
			{
				Id = Guid.Empty,
				Name = "Name",
				Price = 3.14m,
				UnitsCode = "112",
				TemporaryData = "qwe"
			};
			string serialized = JsonConvert.SerializeObject(original);
			Product deserialized = JsonConvert.DeserializeObject<Product>(serialized);
            deserialized.ShouldBeEquivalentTo(original,
                opt => opt.Excluding(p => p.TemporaryData));
        }
	}

	public class Product
	{
		public Guid Id { get; set; }
		[JsonIgnore]
		public string TemporaryData { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string UnitsCode { get; set; }
	}

    public class Puzzle15
    {
        private readonly int[,] map = new int[4, 4];
        private Point empty;

        public Puzzle15(int[,] map)
        {
            if (map.GetLength(0) != 4 || map.GetLength(1) != 4)
                throw new ArgumentException("should be 4x4", nameof(map));
            this.map = (int[,])map.Clone();
        }

        public Puzzle15()
        {
            var i = 0;
            empty = new Point(0, 0);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    map[y, x] = i++;
        }

        public int this[Point pos]
        {
            get { return map[pos.Y, pos.X]; }
            set { map[pos.Y, pos.X] = value; }
        }

        public override string ToString() =>
            string.Join("\r\n", Enumerable.Range(0, 4).Select(FormatLine));

        private string FormatLine(int y)
        {
            var cells = Enumerable.Range(0, 4).Select(x => map[y, x].ToString().PadLeft(2));
            return string.Join(" ", cells);
        }

        public void MoveLeft() => Move(-1, 0);
        public void MoveRight() => Move(1, 0);
        public void MoveUp() => Move(0, -1);
        public void MoveDown() => Move(0, 1);

        public void Move(int dx, int dy)
        {
            var newEmpty = empty + new Size(dx, dy);
            if (newEmpty.X >= 0 && newEmpty.X < 4 &&
                newEmpty.Y >= 0 && newEmpty.Y < 4)
            {
                var t = this[empty];
                this[empty] = this[newEmpty];
                this[newEmpty] = t;
                empty = newEmpty;
            }
        }
    }
}