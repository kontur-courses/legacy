using System;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Samples
{
    [TestFixture]
    public class ApprovalsDemo
    {
        [Test]
        public void Puzzle15_InitialState()
        {
            var puzzle15 = new Puzzle15();
            // TODO: assert
            // HINT: Approvals.Verify
        }

        #region Как это работает

        // DiffReporter - выбирает наилучший имеющийся в наличии способ сравнения
        // Approvals.Verify создает файл *.received.txt с текущим значением и сравнивает его с файлом *.approved.txt 

        #endregion

        [Test]
        public void Puzzle15_MoveRight()
        {
            var puzzle15 = new Puzzle15();
            puzzle15.MoveRight();
            // TODO: assert
        }

        [Test]
        public void ApproveProductData()
        {
            var product = new Product
            {
                Id = Guid.Empty,
                Name = "Name",
                Price = 3.14m,
                UnitsCode = "112"
            };
            //TODO: Verify product
            //TODO: Exclude TemporaryData
            //HINT: stateprinter.Configuration.Project.Exclude
        }

        [Test]
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
            //TODO: Проверить, что сериализуется корректно!
            //HINT: Should().BeEquivalentTo с опциями в FluentAssertions
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