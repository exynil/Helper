using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;

namespace Helper
{
    public partial class MainWindow
    {
        public bool CursorInWindow;
        public DispatcherTimer Timer;
        public double WindowOpacity;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Инициализация
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadInternalDocument();
            // LoadExternalDocument();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            ProgramNameTextBlock.Text = $"Помощник {version.Major}.{version.Minor} ({version.Build})";

            WindowOpacity = 0.6;
            Timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(5)};
            Timer.Tick += TimerOnTick;
            Timer.Start();
        }

        // Загрузка внутреннего документа
        private void LoadInternalDocument()
        {
            const string documentPath = "Helper.doc.xps";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(documentPath);
            if (stream == null) return;
            var length = (int) stream.Length;
            var ms = new MemoryStream {Capacity = length};
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            ms.Write(buffer, 0, length);
            var pkg = Package.Open(ms);
            var strMemoryPackageName = $"memorystream://{documentPath}";
            var packageUri = new Uri(strMemoryPackageName);
            PackageStore.AddPackage(packageUri, pkg);
            var doc = new XpsDocument(pkg, CompressionOption.Maximum, strMemoryPackageName);

            TextDocumentViewer.Document = doc.GetFixedDocumentSequence();
        }

        // Загрузка внешнего документа
        private void LoadExternalDocument()
        {
            const string filename = @"doc.xps";

            if (File.Exists(filename))
            {
                TextDocumentViewer.Document = new XpsDocument(filename, FileAccess.Read).GetFixedDocumentSequence();
            }
            else
            {
                MessageBox.Show("Файл doc.xps не найден.");
            }
        }

        // Вывод окна программы на передний план, если курсор попадает в область расположения окна на экране
        private void TimerOnTick(object sender, EventArgs e)
        {
            if (IsCursorInWindowArea() && !CursorInWindow)
            {
                WindowController.BringWindowToFront();
                CursorInWindow = true;
            }

            if (!IsCursorInWindowArea() && CursorInWindow) CursorInWindow = false;
        }

        // Определяет находится ли курсор внутри окна глобально
        private bool IsCursorInWindowArea()
        {
            var gcp = CursorController.GetCursorPosition();

            return gcp.X > Wn.Left && gcp.X < Wn.Left + Wn.Width && gcp.Y > Wn.Top && gcp.Y < Wn.Top + Wn.Height;
        }

        // Изменение прозрачности окна когда курсор покидает окно
        private void MainWindow_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Opacity = 0.02;
        }

        // Изменение прозрачности окна когда курсор находится над окном
        private void MainWindow_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Opacity = WindowOpacity;
        }

        // Самоуничтожение программы через 5 секунд после нажатии комбинации клавиш CTRL + DEL
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control) return;

            if (e.Key != Key.Delete) return;

            var name = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath).Name;

            Process.Start(new ProcessStartInfo
            {
                Arguments = $"/C choice /C Y /N /D Y /T 5 & Del \"doc.xps\" & Del \"{name}\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });

            Close();
        }

        // Действие на закрытие программы
        private void ExitTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var messageBoxResult = MessageBox.Show("Выйти из программы?", "Помощник", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) Close();
        }

        // Перетаскивание окна
        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // Изменение надписи
        private void ShowMessage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((TextBlock) sender).Text = ((TextBlock) sender).Text == "exynil" ? "Ким Максим" : "exynil";
        }

        // Изменение прозрачности окна при скроле колесика над заголовком окна
        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && Opacity < 1)
            {
                Opacity = WindowOpacity += 0.05;
            }
            else if (e.Delta < 0 && Opacity > 0.1)
            {
                Opacity = WindowOpacity -= 0.05;
            }
        }
    }
}