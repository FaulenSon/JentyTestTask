using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tetris
{
    static class ShapesHandler
    {
        private static List<Shape> shapesList;
        private static Random random = new Random();

        // static constructor : No need to manually initialize
        static ShapesHandler()
        {
            // Create shapes add into the array.
            shapesList = new List<Shape>()
                {
                    new Shape {
                        Width = 2,
                        Height = 2,
                        Dots = new int[,]
                        {
                            { 1, 1 },
                            { 1, 1 }
                        },
                        Color = Brushes.Yellow
                    },
                    new Shape {
                        Width = 1,
                        Height = 4,
                        Dots = new int[,]
                        {
                            { 1 },
                            { 1 },
                            { 1 },
                            { 1 }
                        },
                        Color = Brushes.Turquoise
                    },
                    new Shape {
                        Width = 3,
                        Height = 2,
                        Dots = new int[,]
                        {
                            { 0, 1, 0 },
                            { 1, 1, 1 }
                        },
                        Color = Brushes.Purple
                    },
                    new Shape {
                        Width = 3,
                        Height = 2,
                        Dots = new int[,]
                        {
                            { 0, 0, 1 },
                            { 1, 1, 1 }
                        },
                        Color = Brushes.Orange
                    },
                    new Shape {
                        Width = 3,
                        Height = 2,
                        Dots = new int[,]
                        {
                            { 1, 0, 0 },
                            { 1, 1, 1 }
                        },
                        Color = Brushes.Blue
                    },
                    new Shape {
                        Width = 3,
                        Height = 2,
                        Dots = new int[,]
                        {
                            { 1, 1, 0 },
                            { 0, 1, 1 }
                        },
                        Color = Brushes.Red
                    },
                    new Shape {
                        Width = 3,
                        Height = 2,
                        Dots = new int[,]
                        {
                            { 0, 1, 1 },
                            { 1, 1, 0 }
                        },
                        Color = Brushes.Green
                    }
                };
        }
        
        public static void AddShape(Shape shape)
        {
            shapesList.Add(shape);
        }

        // Get a shape form the array in a random basis
        public static Shape GetRandomShape()
        {
            var shape = shapesList[random.Next(shapesList.Count)];

            return shape;
        }
    }
}
