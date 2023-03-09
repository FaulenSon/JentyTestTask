using System;
using System.Drawing;
using System.Windows.Forms;
using Tetris.Properties;

namespace Tetris
{
    public partial class MainForm : Form
    {
        private Shape currentShape;
        private Shape nextShape;
        private Timer timer = new Timer() { Interval = 500 };
        private Bitmap canvasBitmap;
        private Graphics canvasGraphics;
        private int canvasWidth = Settings.Default.FieldWidth;
        private int canvasHeight = Settings.Default.FieldHeight;
        private (int value, Brush brush)[,] canvasDotArray;
        public readonly int dotSize = 20;
        private int currentX;
        private int currentY;
        private Bitmap workingBitmap;
        private Graphics workingGraphics;
        private SetFieldSizeForm form2;
        private ConstructorForm constructorForm;

        public MainForm()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;
            this.KeyDown += Form1_KeyDown;

            StartGame();
        }

        private void StartGame()
        {
            loadCanvas();

            currentShape = getRandomShapeWithCenterAligned();
            nextShape = getNextShape();

            timer.Start();
        }

        private void StopGame()
        {
            timer.Stop();
        }

        public void SetFieldSize(Size size)
        {
            StopGame();
                        
            this.canvasWidth = size.Width;
            Settings.Default.FieldWidth = size.Width;

            canvasHeight = size.Height;
            Settings.Default.FieldHeight = size.Height;

            Settings.Default.WindowSize = new Size(size.Width * dotSize, size.Height * dotSize + this.menuStrip1.Height + 30);
            Settings.Default.Save();
            
            this.Size = Settings.Default.WindowSize;

            StartGame();
        }
        
        private void resetGrid()
        {
            canvasGraphics.FillRectangle(Brushes.LightGray, 0, 0, canvasBitmap.Width, canvasBitmap.Height);

            drawGrid();
        }

        private void drawGrid()
        {
            for (int i = 0; i < pictureBox1.Width; i += dotSize)
            {
                for (int j = 0; j < pictureBox1.Height; j += dotSize)
                {
                    canvasGraphics.DrawRectangle(Pens.DarkGray, i, j, dotSize, dotSize);
                }
            }
        }

        private void loadCanvas()
        {
            pictureBox1.Width = canvasWidth * dotSize;
            pictureBox1.Height = canvasHeight * dotSize;

            canvasBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            canvasGraphics = Graphics.FromImage(canvasBitmap);
            resetGrid();
            pictureBox1.Image = canvasBitmap;
            canvasDotArray = new (int, Brush)[canvasWidth, canvasHeight];
        }

        private Shape getRandomShapeWithCenterAligned()
        {
            var shape = ShapesHandler.GetRandomShape(); 
            currentX = canvasWidth / 2;
            currentY = -shape.Height;
            return shape;
        }

        
        private void Timer_Tick(object sender, EventArgs e)
        {
            var isMoveSuccess = moveShapeIfPossible(moveDown: 1);

            if (!isMoveSuccess)
            {
                canvasBitmap = new Bitmap(workingBitmap);

                updateCanvasDotArrayWithCurrentShape();

                currentShape = nextShape;
                nextShape = getNextShape();
                clearFilledRows();
                
            }
        }

        private void updateCanvasDotArrayWithCurrentShape()
        {
            bool isBreak = false;

            for (int i = 0; i < currentShape.Width; i++)
            {
                if (isBreak) break;

                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.Dots[j, i] == 1)
                    {
                        if (IsGameOver())
                        {
                            isBreak = true;
                            break;
                        }

                        canvasDotArray[currentX + i, currentY + j] = (1, currentShape.Color);
                    }
                }
            }
        }

        private bool IsGameOver()
        {
            if (currentY < 0)
            {
                StopGame();
                MessageBox.Show("Game Over");
                StartGame();

                return true;
            }

            return false;
        }

        private bool moveShapeIfPossible(int moveDown = 0, int moveSide = 0)
        {
            var newX = currentX + moveSide;
            var newY = currentY + moveDown;
            if (newX < 0 || newX + currentShape.Width > canvasWidth
                || newY + currentShape.Height > canvasHeight)
                return false;

            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (newY + j > 0 && canvasDotArray[newX + i, newY + j].value == 1 && currentShape.Dots[j, i] == 1)
                        return false;
                }
            }

            currentX = newX;
            currentY = newY;

            drawShape();

            return true;
        }

        private void drawShape()
        {
            workingBitmap = new Bitmap(canvasBitmap);
            workingGraphics = Graphics.FromImage(workingBitmap);

            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.Dots[j, i] == 1)
                    {
                        workingGraphics.FillRectangle(currentShape.Color, (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                        workingGraphics.DrawRectangle(new Pen(Color.Black), (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                    }
                }
            }

            pictureBox1.Image = workingBitmap;
        }
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var verticalMove = 0;
            var horizontalMove = 0;

            switch (e.KeyCode)
            {

                case Keys.Left:
                    verticalMove--;
                    break;
                
                case Keys.Right:
                    verticalMove++;
                    break;

                case Keys.Down:
                    horizontalMove++;
                    break;

                case Keys.Up:
                    currentShape.turn();
                    break;
                default:
                    return;
            }

            var isMoveSuccess = moveShapeIfPossible(horizontalMove, verticalMove);
            if (!isMoveSuccess && e.KeyCode == Keys.Up)
                currentShape.rollback();
        }

        Bitmap nextShapeBitmap;
        Graphics nextShapeGraphics;
        private Shape getNextShape()
        {
            var shape = getRandomShapeWithCenterAligned();
            nextShapeBitmap = new Bitmap(6 * dotSize, 6 * dotSize);
            nextShapeGraphics = Graphics.FromImage(nextShapeBitmap);

            nextShapeGraphics.FillRectangle(Brushes.LightGray, 0, 0, nextShapeBitmap.Width, nextShapeBitmap.Height);

            var startX = (6 - shape.Width) / 2;
            var startY = (6 - shape.Height) / 2;

            for (int i = 0; i < shape.Height; i++)
            {
                for (int j = 0; j < shape.Width; j++)
                {
                    nextShapeGraphics.FillRectangle(
                        shape.Dots[i, j] == 1 ? Brushes.Black : Brushes.LightGray,
                        (startX + j) * dotSize, (startY + i) * dotSize, dotSize, dotSize);
                }
            }

            return shape;
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form2 = new SetFieldSizeForm(this);
            form2.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Settings.Default.WindowLocation != null)
            {
                this.Location = Settings.Default.WindowLocation;
            }

            if (Settings.Default.WindowSize != null)
            {
                this.Size = Settings.Default.WindowSize;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowLocation = this.Location;

            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Default.WindowSize = this.Size;
            }
            else
            {
                Settings.Default.WindowSize = this.RestoreBounds.Size;
            }

            Settings.Default.Save();
        }

        public void clearFilledRows()
        {
            for (int i = 0; i < canvasHeight; i++)
            {
                int j;
                for (j = canvasWidth - 1; j >= 0; j--)
                {
                    if (canvasDotArray[j, i].value == 0)
                        break;
                }

                if (j == -1)
                {
                    timer.Interval -= 10;

                    for (j = 0; j < canvasWidth; j++)
                    {
                        for (int k = i; k > 0; k--)
                        {
                            canvasDotArray[j, k] = canvasDotArray[j, k - 1];
                        }

                        canvasDotArray[j, 0].value = 0;
                    }
                }
            }
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    
                    canvasGraphics.FillRectangle(
                        canvasDotArray[i, j].value == 1 ? canvasDotArray[i, j].brush : Brushes.LightGray,
                        i * dotSize, j * dotSize, dotSize, dotSize);

                    canvasGraphics.DrawRectangle(Pens.DarkGray, i * dotSize, j * dotSize, dotSize, dotSize);
                }
            }

            //для избавления от бага, когда граница цветной фигуры окрашивается в серый цвет
            //да, я понимаю, что это увеличивает сложность алгоритма, но решения красивее не нашел из-за нехватки времени
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    if(canvasDotArray[i, j].value == 1)
                        canvasGraphics.DrawRectangle(Pens.Black, i * dotSize, j * dotSize, dotSize, dotSize);
                }
            }

            pictureBox1.Image = canvasBitmap;
        }

        private void конструкторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            constructorForm = new ConstructorForm(this);
            constructorForm.Show();
        }
    }
}
