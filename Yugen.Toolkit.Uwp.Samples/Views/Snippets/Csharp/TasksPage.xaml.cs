using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Yugen.Toolkit.Uwp.Samples.Views.Snippets.Csharp
{
    public sealed partial class TasksPage : Page
    {
        public ObservableCollection<string> MyCollection { get; set; } = new ObservableCollection<string>();

        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly Random _random = new Random();

        public TasksPage()
        {
            this.InitializeComponent();
        }

        private async void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            MyCollection.Clear();
            var tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {                
                tasks.Add(Task.Run(() => DoWork(_random.Next(1, 10), i)));
            }

            //await Task.WhenAll(tasks);
        }

        private async void OnStartButton2Click(object sender, RoutedEventArgs e)
        {
            MyCollection.Clear();
            var tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(DoWork(_random.Next(1, 10), i));
            }

            //await Task.WhenAll(tasks);
        }

        private async void OnStartButton3Click(object sender, RoutedEventArgs e)
        {
            MyCollection.Clear();
            var tasks = new List<Task>();

            Parallel.For(0, 100, (i) =>
            {
                tasks.Add(Task.Run(() => DoWork(_random.Next(1, 10), i)));
            });

            //await Task.WhenAll(tasks);
        }

        private async void OnStartButton4Click(object sender, RoutedEventArgs e)
        {
            MyCollection.Clear();
            var tasks = new List<Task>();

            Parallel.For(0, 100, (i) =>
            {
                tasks.Add(DoWork(_random.Next(1, 10), i));
            });

            //await Task.WhenAll(tasks);
        }

        private async void OnStartButton5Click(object sender, RoutedEventArgs e)
        {
            MyCollection.Clear();
            var tasks = new List<Func<Task>>();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(() => DoWork(_random.Next(1, 10), i));
            }

            Parallel.ForEach(tasks, task =>
            {
                Task.Run(() => task.Invoke());
            });

            //await Task.WhenAll(tasks);
        }

        private async void OnStartButton6Click(object sender, RoutedEventArgs e)
        {
            MyCollection.Clear();
            var tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                var t = new Task(DoWorkAction(_random.Next(1, 10), i));
                tasks.Add(t);
            }

            Parallel.ForEach(tasks, task =>
            {
                Task.Run(() => task.Start());
            });

            //await Task.WhenAll(tasks);
        }

        private async Task DoWork(int delay, int content)
        {
            var start = DateTime.Now.Ticks;
            await Task.Delay(delay);
            var end = DateTime.Now.Ticks;
            _dispatcherQueue.TryEnqueue(() =>
            {
                MyCollection.Add($"{content}: {start}/{end}");
            });
        }

        private Action DoWorkAction(int delay, int content)
        {
            return async () =>
            {
                var start = DateTime.Now.Ticks;
                await Task.Delay(delay);
                var end = DateTime.Now.Ticks;
                _dispatcherQueue.TryEnqueue(() =>
                {
                    MyCollection.Add($"{content}: {start}/{end}");
                });
            };
        }
    }
}
