using System.Numerics;
using Raylib_cs;

static class Constants
{
    public const int MAX_SUBDIVISIONS = 8;
}

namespace QuadtreeImplementation
{
    public class Point
    {
        public int Radius;
        public Color Color;
        public int X;
        public int Y;
        public Vector2 Position;
        public static int Id;
        public Point() { }


        public Point(int radius, Color color, int x, int y)
        {
            Radius = radius;
            Color = color;
            X = x;
            Y = y;
            Position = new Vector2(X, Y);
            Id++;
            Console.WriteLine("Id " + Id);
        }
    }

    public class QuadTree
    {
        private Rectangle _bounds;

        private int _capacity;

        private List<Point> _points;

        private bool _divided;

        private QuadTree? _topLeft;
        private QuadTree? _topRight;
        private QuadTree? _bottomRight;
        private QuadTree? _bottomLeft;

        private Color _color;

        private static int subdivisionCount = 0;

        public List<Point> Points {  get { return _points; } }

        public QuadTree(Rectangle shape, Color col)
        {
            _capacity = 4;
            _divided = false;
            _bounds = shape;
            _points = new List<Point>();
            _topLeft = null;
            _topRight = null;
            _bottomRight = null;
            _bottomLeft = null;

            _color = col;
        }

        // Add the node to the quad tree
        public void Insert(Point p)
        {
            if (!ContainsPoint(p)) return;


            if (_points.Count < _capacity)
            {
                _points.Add(p);
                Console.WriteLine($"Points count: {_points.Count}");
            }
            else
            {
                if (_topLeft == null)
                {
                    Subdivide();
                }
                _topLeft?.Insert(p);
                _topRight?.Insert(p);
                _bottomLeft?.Insert(p);
                _bottomRight?.Insert(p);
            }
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)_bounds.X, (int)_bounds.Y, (int)_bounds.Width, (int)_bounds.Height, _color);

            if (_topLeft == null) return;


            _topLeft?.Draw();
            _topRight?.Draw();
            _bottomRight?.Draw();
            _bottomLeft?.Draw();
        }

        /* 
         *             -> +x
         * min
         *  ._________
         * |          |   | +y
         * |          |   v 
         * |          |
         * |__________.   max
         * 
         */
        public bool ContainsPoint(Point p)
        {
            //int pX = p.Radius + p.X;
            //int pY = p.Radius + p.Y;

            //Vector2 min = new(_bounds.X, _bounds.Y);
            //Vector2 max = new(_bounds.X + _bounds.Width, _bounds.Y + _bounds.Height);

            //if (pX > min.X && pX < max.X)
            //{
            //    if (pY > min.Y && pY < max.Y)
            //    {
            //        return true;
            //    }
            //}

            //return false;

            Console.WriteLine($"Point location: {p.X}, {p.Y}");

            Console.WriteLine($"Checking in boundaries \n" +
                $"Lower bounds: {_bounds.X}, {_bounds.Y}\n" +
                $"Upper bounds: {_bounds.X + _bounds.Width}, {_bounds.Y + _bounds.Height}");


            return Raylib.CheckCollisionPointRec(new(p.X, p.Y), _bounds);
        }

        // Get the new width and height by dividing both by 2
        // The new X and Y coords are based on the current X and Y coords, plus the new width and new height respectively
        // Create new Quadtrees, passing in rectangles with X, Y, Width, Height values from the calculated ones
        /*
         * 
         *                      
         * *----------------------------
         * | x, y       |  x + nw, y     |
         * |            |                |
         * |            |                |
         * |------------|----------------- 
         * | x, y+nh    | x+nw, y+nh     |
         * |            |                |
         * *----------------------------*
         * 
         */
        private void Subdivide()
        {
            if (subdivisionCount < Constants.MAX_SUBDIVISIONS)
            {
                int newWidth = (int)_bounds.Width / 2;
                int newHeight = (int)_bounds.Height / 2;

                int currX = (int)_bounds.X;
                int currY = (int)_bounds.Y;
                int newX = currX + newWidth;
                int newY = currY + newHeight;


                _topLeft = new QuadTree(
                    new Rectangle(currX, currY, new(newWidth, newHeight)),
                    new(new Random().Next(0, 256), new Random().Next(0, 256), new Random().Next(0, 256), 255));


                _topRight = new QuadTree(new Rectangle(newX, currY, new(newWidth, newHeight)),
                    new(new Random().Next(0, 256), new Random().Next(0, 256), new Random().Next(0, 256), 255));


                _bottomRight = new QuadTree(new Rectangle(currX, newY, new(newWidth, newHeight)),
                    new(new Random().Next(0, 256), new Random().Next(0, 256), new Random().Next(0, 256), 255));


                _bottomLeft = new QuadTree(new Rectangle(newX, newY, new(newWidth, newHeight)),
                    new(new Random().Next(0, 256), new Random().Next(0, 256), new Random().Next(0, 256), 255));

               // ++subdivisionCount;
               // Console.WriteLine($"Subdivision count: {subdivisionCount}");
            } else
            {
                Console.WriteLine("Max subdivisions!");
            }
        }

        private bool IntersectAABB(Rectangle other)
        {
            return Raylib.CheckCollisionRecs(_bounds, other);
        }

        // Check to make sure that the node is inside a given range 
        public List<Point> Search(Rectangle range)
        {
            List<Point> pointsInRange = [];

            if (!IntersectAABB(range))
            {
                return pointsInRange;
            }

            foreach (Point point in _points)
            {
                if (Raylib.CheckCollisionPointRec(new(point.X, point.Y), range))
                {
                    pointsInRange.Add(point);
                }
            }

            if (_topLeft == null) return pointsInRange;

            pointsInRange.AddRange(_topLeft.Search(range));
            pointsInRange.AddRange(_topRight.Search(range));
            pointsInRange.AddRange(_bottomRight.Search(range));
            pointsInRange.AddRange(_bottomLeft.Search(range));

            return pointsInRange;
        }
    }
}