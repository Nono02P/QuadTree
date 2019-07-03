using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace QuadTreeSearch
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        private const bool STATIC_TREE = true;
        private const int NUMBER_OF_POINTS = 10000;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random rnd;

        private Viewport _viewport;
        private QuadTree _quadTree;
        private MouseState _oldMouse;
        private Rectangle _cursor;
        private List<Point> _points;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _viewport = GraphicsDevice.Viewport;
            _cursor = new Rectangle(0, 0, 100, 100);
            _points = new List<Point>();
            rnd = new Random();
            if (STATIC_TREE)
                GenerateTree();
        }

        private void GenerateTree()
        {
            _quadTree = new QuadTree(_viewport.Bounds, 5);

            for (int i = 0; i < NUMBER_OF_POINTS; i++)
            {
                Point p = new Point((int)(rnd.NextDouble() * _viewport.Width), (int)(rnd.NextDouble() * _viewport.Height));
                _quadTree.Insert(ref p);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouse = Mouse.GetState();
            if (!STATIC_TREE)
                GenerateTree();
            else
            {
                if (mouse.LeftButton == ButtonState.Pressed && _oldMouse.LeftButton == ButtonState.Released)
                {
                    Point p = mouse.Position;
                    _quadTree.Insert(ref p);
                }
            }

            _cursor.Location = mouse.Position;

            _points.Clear();
            _quadTree.GetPoints(_cursor, _points);

            _oldMouse = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Console.WriteLine(gameTime.IsRunningSlowly + " : " + gameTime.ElapsedGameTime.ToString());

            spriteBatch.Begin();
            _quadTree.Draw(spriteBatch);
            spriteBatch.DrawRectangle(_cursor, Color.Green);

            int total = _points.Count;
            int collide = 0;
            int notCollide = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                Point p = _points[i];
                Color color = Color.Yellow;
                if (_cursor.Contains(p))
                {
                    color = Color.Green;
                    collide++;
                }
                else
                {
                    notCollide++;
                }
                spriteBatch.DrawCircle(p.ToVector2(), 2, 5, color);
            }
            //Console.WriteLine("Total : " + total + " Collide : " + collide + " Not Collide : " + notCollide);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}