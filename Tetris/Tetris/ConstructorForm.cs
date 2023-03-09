using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    enum Type
    {
        Row = 1,
        Col = 2
    }

    public partial class ConstructorForm : Form
    {
        private Bitmap canvasBitmap;
        private Graphics canvasGraphics;
        private readonly int dotSize = 50;

        private const int constructorSide = 4;

        private int[,] newBlockMatrix = new int[constructorSide, constructorSide];
        private bool hasOnlyZeros = true;
        private int countOfOnes = 0;

        private MainForm MainForm { get; set; }

        public ConstructorForm()
        {
            InitializeComponent();
        }

        public ConstructorForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
        }

        private void ConstructorForm_Load(object sender, EventArgs e)
        {
            DrawConstructor();
        }

        private void DrawConstructor()
        {
            pictureBox1.Width = dotSize * 4 + 1;
            pictureBox1.Height = dotSize * 4 + 1;

            canvasBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            canvasGraphics = Graphics.FromImage(canvasBitmap);

            for (int i = 0; i < pictureBox1.Width; i += dotSize)
            {
                for (int j = 0; j < pictureBox1.Height; j += dotSize)
                {
                    canvasGraphics.DrawRectangle(new Pen(Color.Red), i, j, dotSize, dotSize);
                }
            }

            // Load bitmap into picture box
            pictureBox1.Image = canvasBitmap;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            hasOnlyZeros = false;
            countOfOnes++;

            int x = getTopLeftCellCoordinate(e.Location.X);
            int y = getTopLeftCellCoordinate(e.Location.Y);

            FillCell(x, y);

            var indexX = getIndex(x);
            var indexY = getIndex(y);

            newBlockMatrix[indexX, indexY] = newBlockMatrix[indexX, indexY] == 0 ? 1 : 0;
        }

        private void FillCell(int x, int y)
        {
            canvasGraphics.FillRectangle(Brushes.Yellow, x + 1, y + 1, dotSize - 2, dotSize - 2);

            pictureBox1.Image = canvasBitmap;
        }

        private int getTopLeftCellCoordinate (int coordinate)
        {
            return coordinate - coordinate % dotSize;
        }

        private int getIndex(int coordinate)
        {
            coordinate = getTopLeftCellCoordinate(coordinate);

            return coordinate == 0 ? 0 : coordinate / dotSize;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (hasOnlyZeros)
            {
                MessageBox.Show("Размер новой фигуры должен быть не меньше 1х1");
                return;
            }

            var newBlockCuttedMatrix = getWithoutZeroRows();

            if (!IsNewFigureCorrect(newBlockCuttedMatrix))
            {
                MessageBox.Show("Введенная фигура некорректна");
                return;
            }

            if (countOfOnes > 8)
            {
                MessageBox.Show("Размер новой фигуры должен быть не больше 8х8");
                return;
            }

            ShapesHandler.AddShape(new Shape
            {
                Width = getNewBlockWidth(),
                Height = getNewBlockHeight(),
                Color = new SolidBrush(colorDialog1.Color),
                Dots = newBlockCuttedMatrix
            });
        }

        private int getNewBlockWidth()
        {
            var max = 0;

            for (int i = 0; i < constructorSide; i++)
            {
                var sum = 0;
                for (int j = 0; j < constructorSide; j++)
                {
                    if (newBlockMatrix[j, i] == 1) ++sum;
                }

                max = sum > max ? sum : max;
            }

            return max;
        }

        private int getNewBlockHeight()
        {
            var max = 0;

            for (int i = 0; i < constructorSide; i++)
            {
                var sum = 0;
                for (int j = 0; j < constructorSide; j++)
                {
                    if (newBlockMatrix[i, j] == 1) ++sum;
                }

                max = sum > max ? sum : max;
            }

            return max;
        }

        private int[,] getWithoutZeroRows()
        {
            (int fromRow, int toRow) fromToRowIndexes = getFromToIndexes(0, constructorSide, 0, constructorSide, Type.Row);

            (int fromCol, int toCol) fromToColIndexes = getFromToIndexes(0, constructorSide, fromToRowIndexes.fromRow, fromToRowIndexes.toRow + 1, Type.Col);

            var resultArray = new int[fromToRowIndexes.toRow - fromToRowIndexes.fromRow + 1, fromToColIndexes.toCol - fromToColIndexes.fromCol + 1];

            var newI = 0;
            var newJ = 0;

            for (int i = fromToColIndexes.fromCol; i <= fromToColIndexes.toCol; i++)
            {
                for (int j = fromToRowIndexes.fromRow; j <= fromToRowIndexes.toRow; j++)
                {
                    resultArray[newJ, newI] = newBlockMatrix[i, j];
                    newJ++;
                }

                newI++;
                newJ = 0;
            }

            return resultArray;
        }

        private (int from, int to) getFromToIndexes(int from_i, int to_i, int from_j, int to_j, Type type)
        {
            int from = -1;
            int to = -1;

            for (int i = from_i; i < to_i; i++)
            {
                bool isEmpty = true;

                for (int j = from_j; j < to_j; j++)
                {
                    if (newBlockMatrix[i, j] == 1 && type == Type.Col)
                    {
                        isEmpty = false;
                        break;
                    }
                    else
                    if (newBlockMatrix[j, i] == 1 && type == Type.Row)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty) continue;

                if (from < 0) from = i;
                else to = Math.Max(to, i);
            }

            if (to < 0) to = from;

            return (from, to);
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            colorButton.BackColor = colorDialog1.Color;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            Array.Clear(newBlockMatrix, 0, newBlockMatrix.Length);
            hasOnlyZeros = true;
            countOfOnes = 0;
            DrawConstructor();
        }

        private bool IsNewFigureCorrect(int[,] newBlockMatrix)
        {
            if (countOfOnes == 1) return true;

            int rowCount = newBlockMatrix.GetLength(0);
            int colCount = newBlockMatrix.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if (newBlockMatrix[i, j] != 1)
                        continue;

                    var leftX = Math.Max(j - 1, 0);
                    var rightX = Math.Min(j + 1, colCount - 1);

                    var leftY = Math.Max(i - 1, 0);
                    var rightY = Math.Min(i + 1, rowCount - 1);

                    if ((newBlockMatrix[i, leftX] != 1 || i == leftX) &&
                        (newBlockMatrix[i, rightX] != 1 || i == rightX) &&
                        (newBlockMatrix[leftY, j] != 1 || j == leftY) &&
                        (newBlockMatrix[rightY, j] != 1 || j == rightY))
                        return false;                
                }
            }

            return true;
        }
    }
}
