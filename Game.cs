using Raylib_cs;
using System.Numerics;
using System;
using QuadtreeImplementation;

class QuadtreeImp
{
    public static void GetUserInput(Point user)
    {
        int moveSpeed = 5;

        if (Raylib.IsKeyDown(KeyboardKey.Up))
        {
            user.Y -= moveSpeed;
        }

        if (Raylib.IsKeyDown(KeyboardKey.Down))
        {
            user.Y += moveSpeed;
        }

        if (Raylib.IsKeyDown(KeyboardKey.Right))
        {
            user.X += moveSpeed;
        }

        if (Raylib.IsKeyDown(KeyboardKey.Left))
        {
            user.X -= moveSpeed;
        }
    }

    public static Rectangle[] CreateBorder()
    {
        Rectangle[] border =
        [
            new(350, 50, new(600, 15)),
            new(350, 600, new(600, 15)),
            new(350, 50, new(15, 550)),
            new(950, 50, new(15, 565)),
        ];

        return border;
    }

    public static void Main()
    {
        Raylib.InitWindow(1920, 1080, "Quadtree");


        int randomRVal = new Random().Next(0, 256);
        int randomGVal = new Random().Next(0, 256);
        int randomBVal = new Random().Next(0, 256);

        QuadTree qTree = new (new Rectangle( 0, 0, new Vector2( Raylib.GetScreenWidth(), Raylib.GetScreenHeight())), new(randomRVal, randomGVal, randomBVal, 255));
       

        Raylib.SetTargetFPS(60);

        List<Point> points = new List<Point>();

        //Point user = new Point(10, Color.Black, Raylib.GetScreenWidth()/2, Raylib.GetScreenHeight()/2);
        //points.Add(user);

        foreach (Point p in points)
            qTree.Insert(p);

        int prevPointsCount = points.Count;

        while (!Raylib.WindowShouldClose())
        {
            //GetUserInput(user);

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Point p = new Point(5, Color.Black, Raylib.GetMouseX(), Raylib.GetMouseY());
                points.Add(p);
            }

            int currPointsCount = points.Count;

            if (prevPointsCount != currPointsCount)
            {
                foreach (Point p in points)
                {
                    if (!qTree.Points.Contains(p))
                        qTree.Insert(p);
                }
            }

            prevPointsCount = currPointsCount;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            qTree.Draw();

            foreach (Point p in points)
            {
                Raylib.DrawCircleV(new(p.X, p.Y), p.Radius, p.Color);
            }

            Raylib.DrawFPS(10, 10);

            Raylib.EndDrawing();
        }
    }
}