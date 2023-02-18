using System;
using System.Drawing.Imaging;
using Timer = System.Threading.Timer;

namespace _7Task
{
    public partial class Form1 : Form
    {
        private Color _emplyColor = Color.White;
        private Color[,] _colors = new Color[10, 10];
        private int _currentBlockPositionX;
        private int _currentBlockPositionY;
        Timer _timer;
        Random _random = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            InitializeGame();

            //Запускаем таймер
            TimerCallback timerCallback = new TimerCallback(OnTimerTicked);
            _timer = new Timer(timerCallback, 0, 0, 200);
        }

        private void OnTimerTicked(object? state)
        {
            //Если блок находится на нижней линии
            if (_currentBlockPositionY == 9)
            {
                //Генерируем новый цветной блок сверху
                GenerateNewBlock();
            }
            //Если под падающим блоком находится другой цветной блок
            else if (_colors[_currentBlockPositionX, _currentBlockPositionY + 1] != _emplyColor)
            {
                //Генерируем новый цветной блок сверху
                GenerateNewBlock();
            }
            //...иначе под блоком пусто и можно дальше упасть
            else
            {
                //Переносим цвет вниз
                _colors[_currentBlockPositionX, _currentBlockPositionY + 1] = _colors[_currentBlockPositionX, _currentBlockPositionY];
                //Для прежней позиции цвет убираем (делаем белым)
                _colors[_currentBlockPositionX, _currentBlockPositionY] = _emplyColor;
                //Перемещаем текущую позицию падающего блока вниз
                _currentBlockPositionY++;
            }

            //Перерисовываем сцену
            Redraw();
        }

        private void Redraw()
        {
            //Очищаем сцену
            var graphics = Panel.CreateGraphics();
            graphics.Clear(Color.White);

            //Отрисовываем все квадраты
            var rectangleHeight = Panel.Size.Height / 10;
            var rectangleWidth = Panel.Size.Width / 10;
            for (int i = 0; i < 10; i++)
            {
                var rectangleXLocation = Panel.Size.Height * i / 10;
                for (int j = 0; j < 10; j++)
                {
                    var rectangleYLocation = Panel.Size.Width * j / 10;
                    var color = _colors[i, j];
                    var brush = new SolidBrush(color);
                    graphics.FillRectangle(brush, rectangleXLocation, rectangleYLocation, rectangleHeight, rectangleWidth);
                }
            }
        }

        private void GenerateNewBlock()
        {
            _currentBlockPositionX = _random.Next(0, 10);
            _currentBlockPositionY = 0;
            _colors[_currentBlockPositionX, _currentBlockPositionY] = Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255));
        }

        private void InitializeGame()
        {
            //Заполняем всё пустыми белыми полями
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _colors[i, j] = _emplyColor;
                }
            }

            //Генерируем новый цветной блок сверху
            GenerateNewBlock();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            _timer.Dispose();
            int width = Panel.ClientSize.Width;
            int height = Panel.ClientSize.Height;

            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Images (*.png,*.jpeg)|*.png;*.jpeg";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                DialogResult dialogResult = MessageBox.Show("Are you really want to save file?", "Save your plot", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Bitmap bmp = new Bitmap(width, height);
                    Panel.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    bmp.Save(saveDialog.FileName, ImageFormat.Png);
                }
            }
        }
    }
    }
