using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int theme = 1;
        int velocity = 50;
        MediaPlayer player = new MediaPlayer();
        Jogo game;
        Timer t;
        
        SolidColorBrush borderColor = new SolidColorBrush();

        //Construtor
        public MainWindow()
        {
            InitializeComponent();
            run();

            //Pega a musica do arquivo e carrega no player
            Uri uri = new Uri("Resources\\mus_zz_megalovania.ogg", UriKind.Relative);
            player.Open(uri);

            player.Play();
            player.MediaEnded += Player_MediaEnded;

            canvas.LayoutTransform = new ScaleTransform();    
        }

        //Quando acaba a musica, recomeça
        private void Player_MediaEnded(object sender, EventArgs e)
        {
            player.Play();
        }
       
        //Muda o tema atual do jogo
        public void setTheme()
        {
            if (theme == 1)
            {
                //Black Theme
                canvas.Background = new SolidColorBrush(Colors.Black);
                Background = new SolidColorBrush(Colors.Black);
                borderColor = new SolidColorBrush(Colors.White);
                border.BorderBrush = borderColor;
                game.rectColor = new SolidColorBrush(Colors.White);
                game.rectStrokeColor = new SolidColorBrush(Colors.LightGray);
                game.foodStrokeColor = new SolidColorBrush(Colors.DarkGray);
                game.food.Stroke = game.foodStrokeColor;
                BorderBrush = new SolidColorBrush(Colors.White);
                paredeCheck();
                theme = 0;
            }
            else
            {
                //White theme
                canvas.Background = new SolidColorBrush(Colors.White);
                Background = new SolidColorBrush(Colors.White);
                borderColor = new SolidColorBrush(Colors.Black);
                border.BorderBrush = borderColor;
                game.rectColor = new SolidColorBrush(Colors.Black);
                game.rectStrokeColor = new SolidColorBrush(Colors.LightGray);
                game.foodStrokeColor = new SolidColorBrush(Colors.DarkGray);
                game.food.Stroke = game.foodStrokeColor;
                BorderBrush = new SolidColorBrush(Colors.Black);
                paredeCheck();
                theme = 1;
            }
        }

        //Quando o timer é disposed
        private void T_Disposed(object sender, EventArgs e)
        {
            canvas.Children.Clear();
            run();
        }
       
        //Tick do jogo 
        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                game.tick2();              
            });                      
        }

        //Inicia o jogo
        public void run()
        {      
            t = new System.Timers.Timer(velocity);
            game = new Jogo(canvas, t, parede_CBox);

            setTheme();
            game.createBaseRects();

            t.Elapsed += T_Elapsed;
            t.Disposed += T_Disposed;
            t.Start();                   
        }

        //Pega a input e processa
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if ((e.Key == Key.W || e.Key == Key.Up))
            {
                if (game.subDir1 != Jogo.Direcao.Nenhuma && game.subDir1 != Jogo.Direcao.Down && game.subDir1 != Jogo.Direcao.Up)
                    game.subDir2 = Jogo.Direcao.Up;
                else if (game.direction != Jogo.Direcao.Down && game.subDir1 != Jogo.Direcao.Down && game.direction != Jogo.Direcao.Up)
                     game.subDir1 = Jogo.Direcao.Up;
            }
            if ((e.Key == Key.A || e.Key == Key.Left))
            {
                if (game.subDir1 != Jogo.Direcao.Nenhuma && game.subDir1 != Jogo.Direcao.Right && game.subDir1 != Jogo.Direcao.Left)
                    game.subDir2 = Jogo.Direcao.Left;
                else if (game.direction != Jogo.Direcao.Right && game.subDir1 != Jogo.Direcao.Right && game.direction != Jogo.Direcao.Left)
                    game.subDir1 = Jogo.Direcao.Left;
            }
            if ((e.Key == Key.D || e.Key == Key.Right))
            {
                if (game.subDir1 != Jogo.Direcao.Nenhuma && game.subDir1 != Jogo.Direcao.Left && game.subDir1 != Jogo.Direcao.Right)
                    game.subDir2 = Jogo.Direcao.Right;
                else if (game.direction != Jogo.Direcao.Left && game.subDir1 != Jogo.Direcao.Left && game.direction != Jogo.Direcao.Right)
                    game.subDir1 = Jogo.Direcao.Right;
            }
            if ((e.Key == Key.S || e.Key == Key.Down))
            {
                if (game.subDir1 != Jogo.Direcao.Nenhuma && game.subDir1 != Jogo.Direcao.Up && game.subDir1 != Jogo.Direcao.Down)
                    game.subDir2 = Jogo.Direcao.Down;
                else if (game.direction != Jogo.Direcao.Up && game.subDir1 != Jogo.Direcao.Up && game.direction != Jogo.Direcao.Down)
                    game.subDir1 = Jogo.Direcao.Down;
            }

            if (e.Key == Key.Escape)            
                Close();
            
        }

        //Aumenta e diminui a velocidade
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            Button snd = sender as Button;

            if (snd.Content.ToString() == "-")
            {
                if (velocity <= 10)
                    velocity -= 1;
                else
                    velocity -= 10;

                if (velocity <= 0)
                    velocity = 1;

                velocidade.Content = velocity;
                t.Interval = velocity;
            }
            if (snd.Content.ToString() == "+")
            {
                if (velocity + 1 <= 10)
                    velocity += 1;
                else
                    velocity += 10;

                velocidade.Content = velocity;
                t.Interval = velocity;
            }
        }

        //Muda a propriedade da parede
        private void parede_CBox_Changed(object sender, RoutedEventArgs e)
        {
            paredeCheck();
        }
        void paredeCheck()
        {
            if (parede_CBox.IsChecked.Value)
            {
                border.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                border.BorderBrush = borderColor;
            }
        }  

        //Muda o tema
        private void button_Click(object sender, RoutedEventArgs e)
        {
            setTheme();
        }

        //Para e starta a musica ao mudar a checkbox
        private void musica_CBox_Changed(object sender, RoutedEventArgs e)
        {
            if (musica_CBox.IsChecked.Value)
                player.Play();          
            else 
                player.Pause();   
            //  player.Stop();        
        }

        #region Meh
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            // if (sizeInfo.WidthChanged && ActualWidth / aspect != ActualHeight)
            //      this.Width = sizeInfo.NewSize.Height * aspect;
            //  else this.Height = sizeInfo.NewSize.Width / aspect;


            // ((ScaleTransform)canvas.LayoutTransform).ScaleX = this.Width / 350;
            // ((ScaleTransform)canvas.LayoutTransform).ScaleY = this.Width / 350;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
        const double aspect = 350 / 400.0;
        //Inside dispatcher method run
        /* if (game.rects.Count() % 10 == 0)
                {
                    bmpCout++;
                    var image = rsr[bmpCout];
                    var bitmap = new System.Drawing.Bitmap(image);
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    bitmap.Dispose();
                    var brush = new ImageBrush(bitmapSource);

                    canvas.Background = brush;
                }*/

        /*Bitmap[] rsr = new[] {
           Properties.Resources.p17,
           Properties.Resources.p16,
           Properties.Resources.p15,
           Properties.Resources.p14,
           Properties.Resources.p13,
           Properties.Resources.p12,
           Properties.Resources.p11,
                  Properties.Resources.p10,
                   Properties.Resources.p9,
                    Properties.Resources.p8,
                     Properties.Resources.p7,
                      Properties.Resources.p6,
                       Properties.Resources.p5,
                        Properties.Resources.p4,
                         Properties.Resources.p3,
                          Properties.Resources.p2,
                           Properties.Resources.p1,
                            Properties.Resources.p0,
       };*/

        //Inside constructor
        /* var image = Properties.Resources.p17;
            var bitmap = new System.Drawing.Bitmap(image);
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),   IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions() );
            bitmap.Dispose();
            var brush = new ImageBrush(bitmapSource);

            canvas.Background = brush;*/
        #endregion

    }
}
