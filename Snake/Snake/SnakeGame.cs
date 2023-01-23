using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Snake
{
    public partial class SnakeGame : Form
    {
        #region Variables
        private const Keys up = Keys.Up;
        private const Keys right = Keys.Right;
        private const Keys down = Keys.Down;
        private const Keys left = Keys.Left;

        private int posX;
        private int posY;
        private const int xMax = 69;
        private const int xMin = 0;
        private const int yMax = 46;
        private const int yMin = 0;

        private bool lastKeyProcessed = true;
        private int multiplier = 11;
        private int gamePoint = 0;
        private DirectionEnum direction;
        private Point bait;
        private List<Point> snakePosition = new List<Point>();
        #endregion

        #region Constructor And Loader
        public SnakeGame()
        {
            InitializeComponent();
        }

        private void SnakeGame_Load(object sender, EventArgs e)
        {
            speedSelection.SelectedIndex = 2;
        }
        private void startButton_Click(object sender, EventArgs e)
        {
            startGame();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            playGame();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            resetGame();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (gameTimer.Enabled && lastKeyProcessed)
            {
                lastKeyProcessed = false;
                determineDirection(keyData);
            }

            pauseGame(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void startGame()
        {
            speedSelection.Enabled = false;
            startButton.Enabled = false;
            setGameSpeed();
            gameTimer.Enabled = true;
        }

        private void playGame()
        {
            setPositionValues();
            bool isGameEnded = isGameOver();

            if (isGameEnded)
            {
                gameTimer.Enabled = false;
                MessageBox.Show(String.Format("Oyun Bitti!\n\nPuanınız: {0}", gamePoint), "Sevgili yilanyunus", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            snakePosition.Insert(0, new Point(posX, posY));
            snakePosition.RemoveAt(snakePosition.Count - 1);

            if (bait.X == posX * multiplier && bait.Y == posY * multiplier)
            {
                eatBait();
            }

            drawSnake();
            lastKeyProcessed = true;
        }

        private void resetGame()
        {
            gameTimer.Enabled = false;
            startButton.Enabled = true;
            speedSelection.Enabled = true;

            createNewSnake();
            resetVariables();
            createBait();
            drawSnake();
        }

        private void pauseGame(Keys keyData)
        {
            if (keyData == Keys.P)
            {
                gameTimer.Enabled = !gameTimer.Enabled;
            }
        }

        private void setGameSpeed()
        {
            switch (speedSelection.SelectedIndex)
            {
                case 0:
                    gameTimer.Interval = 100;
                    break;
                case 1:
                    gameTimer.Interval = 75;
                    break;
                case 2:
                default:
                    gameTimer.Interval = 50;
                    break;
                case 3:
                    gameTimer.Interval = 40;
                    break;
                case 4:
                    gameTimer.Interval = 25;
                    break;
                case 5:
                    gameTimer.Interval = 10;
                    break;
            }
        }

        private void resetVariables()
        {
            posX = 12;
            posY = 20;
            gamePoint = 0;
            direction = DirectionEnum.Right;
            printStat();
        }

        private void createNewSnake()
        {
            snakePosition.Clear();
            snakePosition.Add(new Point(12, 20));
            snakePosition.Add(new Point(11, 20));
            snakePosition.Add(new Point(10, 20));
        }

        private void setPositionValues()
        {
            switch (direction)
            {
                case DirectionEnum.Down:
                    posY++;
                    break;
                case DirectionEnum.Up:
                    posY--;
                    break;
                case DirectionEnum.Left:
                    posX--;
                    break;
                case DirectionEnum.Right:
                default:
                    posX++;
                    break;
            }
        }

        private bool isGameOver()
        {
            //Limit kontrol ediyorsunuz burdan hocam
            if (posX > xMax || posX < xMin || posY > yMax || posY < yMin)
            {
                return true;
            }

            //Yemek
            if (snakePosition.Any(t => t.X == posX && t.Y == posY))
            {
                return true;
            }

            return false;
        }

        private void createBait()
        {
            Random random = new Random(DateTime.Now.TimeOfDay.Milliseconds);
            int x = 0;
            int y = 0;
            bool contains = true;

            while (contains)
            {
                x = random.Next(xMin, xMax + 1) * multiplier;
                y = random.Next(yMin, yMax + 1) * multiplier;

                contains = snakePosition.Any(t => t.X == x && t.Y == y);
            }

            bait = new Point(x, y);
        }

        private void eatBait()
        {
            Point lastPoint = snakePosition[snakePosition.Count - 1];
            snakePosition.Add(new Point(lastPoint.X, lastPoint.Y));
            gamePoint += (speedSelection.SelectedIndex + 1) * 10;
            printStat();
            createBait();
        }

        private void printStat()
        {
            scoreLabel.Text = gamePoint.ToString();
            baitLabel.Text = (snakePosition.Count - 3).ToString();
        }

        private void determineDirection(Keys keyData)
        {
            switch (keyData)
            {
                case up:
                    if (direction != DirectionEnum.Down)
                        direction = DirectionEnum.Up;
                    break;
                case down:
                    if (direction != DirectionEnum.Up)
                        direction = DirectionEnum.Down;
                    break;
                case left:
                    if (direction != DirectionEnum.Right)
                        direction = DirectionEnum.Left;
                    break;
                case right:
                default:
                    if (direction != DirectionEnum.Left)
                        direction = DirectionEnum.Right;
                    break;
            }
        }

        #endregion

        #region Drawing Methods
        private void drawSnake()
        {
            yilanyunus.Refresh();
            drawBait();
            foreach (Point item in snakePosition)
            {
                int xVal = item.X * multiplier;
                int yVal = item.Y * multiplier;

                drawPoint(xVal, yVal);
            }
        }

        private void drawPoint(int x, int y, bool isBlack = true)
        {
            using (Graphics g = this.yilanyunus.CreateGraphics())
            {
                Color penColor = isBlack ? Color.Black : Color.Red;
                Pen pen = new Pen(penColor, 5);
                g.DrawRectangle(pen, x, y, 5, 5);
                pen.Dispose();
            }
        }

        private void drawBait()
        {
            drawPoint(bait.X, bait.Y, false);
        }
        #endregion

        private void yilanyunus_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Kullanici;

            string sifre;



            Kullanici = textBox1.Text;

            sifre = textBox2.Text;



            if (Kullanici == "yilanyunus" && sifre == "123456")

                MessageBox.Show("Giriş Başarılı yilanyunus!...");

            else

                MessageBox.Show("Hatalı Giriş!...");
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            ToolTip Aciklama = new ToolTip();
            Aciklama.ToolTipTitle = "Dikkat!";
            Aciklama.ToolTipIcon = ToolTipIcon.Warning;
            Aciklama.IsBalloon = true;

            Aciklama.SetToolTip(button1, "yilanyunus");
            Aciklama.SetToolTip(label1, "yilanyunus");
            Aciklama.SetToolTip(textBox1, "yilanyunus");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.BackColor = Color.Azure;
            textBox3.ForeColor = Color.Firebrick;
            textBox3.Text = "Tüm Yemleri YE ! !  !";
            textBox3.Name = "dinamikTxt";
            textBox3.Font = new Font("Georgia", 12);
        }

        private void speedSelection_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public enum DirectionEnum
    {
        Undefined,
        Up,
        Right,
        Down,
        Left
    }
}
