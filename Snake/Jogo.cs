using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake {
    public class Jogo
    {

        public enum Direcao
        {
            Left, Up, Right, Down, Nenhuma
        }
        public int tamanho;
        public List<Rectangle> rects = new List<Rectangle>();
        public Direcao direction;
        public Direcao subDir1 = Direcao.Nenhuma;
        public Direcao subDir2 = Direcao.Nenhuma;
        const int dimension = 10;
        Point posição;
        public Rectangle food;
        Canvas canvas;
        CheckBox parede;
        Timer t;

        public SolidColorBrush foodColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00ff00"));
        public SolidColorBrush foodStrokeColor;
        public SolidColorBrush blackColor;

        public SolidColorBrush rectColor;
        public SolidColorBrush rectStrokeColor;
        public SolidColorBrush hitColor = new SolidColorBrush(Colors.Red);

        //Pega as variaveis da MainWindow e armazena nesta classe
        public Jogo(Canvas canvas, Timer t, CheckBox parede)
        {
            this.parede = parede;
            this.t = t;
            this.canvas = canvas;

            food = foodRect();

            waveOutSetVolume(IntPtr.Zero, uint.MaxValue);
        }

        //Metodo que adiciona os retangulos da cobra e diz qual é a frente e a direção
        public void createBaseRects()
        {
            rects.Add(refRect(2, 2));
            rects.Add(refRect(3, 2));
            rects.Add(refRect(4, 2));
            rects.Add(refRect(5, 2));
            rects.Add(refRect(6, 2));
            rects.Add(refRect(7, 2));
            rects.Add(refRect(8, 2));
            rects.Add(refRect(9, 2));          

            posição = new Point(9 * dimension, 2 * dimension);

            canvas.Children.Add(food);
            foreach (var x in rects)
            {
                canvas.Children.Add(x);
            }

            direction = Direcao.Right;
        }

        //Metodo que adiciona os retangulos da cobra
        public Rectangle refRect(double x, double y)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = rectColor;
            rect.Stroke = rectStrokeColor;
            rect.Width = dimension;
            rect.Height = dimension;
            Canvas.SetLeft(rect, x * dimension);
            Canvas.SetTop(rect, y * dimension);
            return rect;
        }

        //Metodo que adiciona o retangulo da comida
        public Rectangle foodRect()
        {
            Random x = new Random();
            Rectangle rect = new Rectangle();
            rect.Fill = foodColor;
            rect.Stroke = foodStrokeColor;
            rect.StrokeThickness = 1;
            rect.Width = dimension;
            rect.Height = dimension;
            Canvas.SetLeft(rect, x.Next(30) * dimension);
            Canvas.SetTop(rect, x.Next(30) * dimension);
            return rect;
        }

        //Tick do jogo
        public void tick2()
        {
            //Stopwatch para testar performance
            Stopwatch st1 = new Stopwatch();
            st1.Start();

            //Pega as direções armazenadas e joga pra direção atual
            if (subDir1 != Direcao.Nenhuma)
            {
                direction = subDir1;
                subDir1 = Direcao.Nenhuma;
            }
            if (subDir2 != Direcao.Nenhuma)
            {
                subDir1 = subDir2;
                subDir2 = Direcao.Nenhuma;
            }

            //Move snake para a proxima posição e executa o metodo
            if (direction == Direcao.Left)
            {
                posição.X -= dimension;
                moveRect();
            }
            else
            if (direction == Direcao.Right)
            {
                posição.X += dimension;
                moveRect();
            }
            else
            if (direction == Direcao.Up)
            {
                posição.Y -= dimension;
                moveRect();
            }
            else
            if (direction == Direcao.Down)
            {
                posição.Y += dimension;
                moveRect();
            }

            //Passa por todos os retangulos para mudar sua cor caso mude de tema e mudar o tamanho da Border
            PointCollection d = new PointCollection();
            for (int i = 0; i < rects.Count; i++)
            {
                d.Add(new Point(Canvas.GetLeft(rects[i]), Canvas.GetTop(rects[i])));
                rects[i].StrokeThickness = i / (double)rects.Count;
                if (i == rects.Count - 1)
                {
                    rects[i].StrokeThickness = 1.2;
                }
                if (rects[i].Fill != rectColor)
                {
                    rects[i].Fill = rectColor;
                }
                if (rects[i].Stroke != rectStrokeColor)
                {
                    rects[i].Stroke = rectStrokeColor;
                }
            }

            //Caso a cobra bata em si mesma
            if (d.Where(l => l == posição).Count() > 1)
            {
                Canvas.SetZIndex(rects.Last(), int.MaxValue);
                rects.Last().Fill = hitColor;
                t.Stop();
                MessageBox.Show("Snake is kill");
                t.Dispose();
                return;
            }

            //Caso saia do quadrado
            if (posição.X > 29 * dimension || posição.Y > 29 * dimension || posição.X < 0 || posição.Y < 0)
            {
                rects.Last().Fill = hitColor;
                t.Stop();
                MessageBox.Show("Snake is kill");
                t.Dispose();                
            }

            //Teste de performance
            st1.Stop();
            Debug.WriteLine(st1.Elapsed);
        }

        //Metodo que move o ultimo retangulo para a proxima casa
        public void moveRect()
        {
            //Move snake para a posição oposta do canvas caso saia do proprio e a checkbox não esteja checada
            if (!parede.IsChecked.Value)
            {
                if (posição.X > 29 * dimension)
                {
                    posição.X = 0;
                }
                if (posição.X < 0)
                {
                    posição.X = 29 * dimension;
                }
                if (posição.Y > 29 * dimension)
                {
                    posição.Y = 0;
                }
                if (posição.Y < 0)
                {
                    posição.Y = 29 * dimension;
                }
            }

            
            //Se não for comida a proxima posição
            if (!gotFood())
            {
                Rectangle temp = rects.First();
                rects.Remove(temp);
                Canvas.SetTop(temp, posição.Y);
                Canvas.SetLeft(temp, posição.X);
                rects.Add(temp);
            }
            else
            {
                rects.Add(refRect(posição.X / dimension, posição.Y / dimension));
                canvas.Children.Add(rects.Last());
            }
        }

        //Metodo que testa se a proxmia posição for comida
        public bool gotFood()
        {
            if (posição.X == Canvas.GetLeft(food) && posição.Y == Canvas.GetTop(food))
            {
                Task.Run(() => notificationSound.Play());

                Random rnd = new Random();
                Console.WriteLine("Pegou a comida");

                PointCollection d = new PointCollection();
                for (int i = 0; i < rects.Count; i++)
                {
                    d.Add(new Point(Canvas.GetLeft(rects[i]), Canvas.GetTop(rects[i])));
                }
                int rnd1, rnd2;
                //Muda a posição da comida caso não seja dentro da snake
                do
                {
                    rnd1 = rnd.Next(30);
                    rnd2 = rnd.Next(30);
                    Canvas.SetLeft(food, rnd1 * dimension);
                    Canvas.SetTop(food, rnd2 * dimension);
                }
                while (d.Contains(new Point(rnd1 * dimension, rnd2 * dimension)));

                return true;               
            }
            return false;
        }

        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        //MediaPlayer mp = new MediaPlayer();
        SoundPlayer notificationSound = new SoundPlayer("Resources\\buh.wav");
    }
}
