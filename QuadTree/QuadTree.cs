using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace QuadTreeSearch
{
    public class QuadTree
    {
        private bool _isSplitted;
        private Rectangle _quadArea;
        private QuadTree[] _quads;
        private uint _quadCapacity;
        private Point[] _points;
        private int _pointIndex;

        public QuadTree(Rectangle pQuadArea, uint pCapacity = 4)
        {
            _quadArea = pQuadArea;
            _quadCapacity = pCapacity;
            _quads = new QuadTree[4];
            _points = new Point[_quadCapacity];
        }

        public void Insert(ref Point pPoint)
        {
            if (_quadArea.Contains(pPoint))
            {
                if (_pointIndex < _quadCapacity && !_isSplitted)
                {
                    _points[_pointIndex] = pPoint;
                    _pointIndex++;
                }
                else
                {
                    if (!_isSplitted)
                    {
                        if (_quadArea.Width == 1 || _quadArea.Height == 1)
                            throw new Exception("The capacity is too low, to much quads are created.");
                        else
                        {
                            int x = _quadArea.X;
                            int y = _quadArea.Y;
                            int w = (int)Math.Ceiling((decimal)_quadArea.Width / 2);
                            int h = (int)Math.Ceiling((decimal)_quadArea.Height / 2);

                            for (int l = 0; l < 2; l++)
                            {
                                for (int c = 0; c < 2; c++)
                                {
                                    _quads[c + l * 2] = new QuadTree(new Rectangle(x + w * c, y + h * l, w, h), _quadCapacity);
                                }
                            }
                            _isSplitted = true;

                            for (int i = 0; i < _pointIndex; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    _quads[j].Insert(ref _points[i]);
                                }
                            }
                            _points = null;
                            _pointIndex = -1;
                        }
                    }
                    for (int j = 0; j < 4; j++)
                    {
                        _quads[j].Insert(ref pPoint);
                    }
                }
            }
        }

        public void GetPoints(Rectangle pArea, List<Point> pOutList)
        {
            if (pOutList == null)
                pOutList = new List<Point>();

            if (_quadArea.Intersects(pArea))
            {
                if (!_isSplitted)
                {
                    for (int i = 0; i < _pointIndex; i++)
                    {
                        pOutList.Add(_points[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < _quads.Length; i++)
                    {
                        _quads[i].GetPoints(pArea, pOutList);
                    }
                }
            }
        }

        private void countSubQuads(ref int pCounter)
        {
            if (!_isSplitted)
            {
                pCounter += _pointIndex;
            }
            else
            {
                for (int i = 0; i < _quads.Length; i++)
                {
                    _quads[i].countSubQuads(ref pCounter);
                }
            }
        }

        public int Count()
        {
            int counter = 0;
            countSubQuads(ref counter);
            return counter;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(_quadArea, Color.White);
            if (!_isSplitted)
            {
                for (int i = 0; i < _pointIndex; i++)
                {
                    spriteBatch.DrawCircle(_points[i].ToVector2(), 2, 5, Color.White);
                }
            }
            else
            {
                for (int i = 0; i < _quads.Length; i++)
                {
                    _quads[i].Draw(spriteBatch);
                }
            }
        }
    }
}