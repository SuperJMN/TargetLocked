namespace Desktop.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using Avalonia.Threading;
    using DynamicData;
    using Identification;
    using ReactiveUI;

    public class MainViewModel : ReactiveObject, IDisposable
    {
        private readonly IBatchIdentificator faceServiceClient;
        readonly ReadOnlyObservableCollection<Job> jobs;
        private readonly SourceList<Job> jobsSource;
        private readonly IDisposable jobsSubscription;

        public MainViewModel(IBatchIdentificator faceServiceClient)
        {
            AddJobCommand = ReactiveCommand.Create();
            AddJobCommand.Subscribe(o => jobsSource.Add(new Job(faceServiceClient)));
            
            this.faceServiceClient = faceServiceClient;
            //OpenCommand = ReactiveCommand.CreateAsyncTask(o => new OpenFileDialog().ShowAsync());
            //OpenCommand.Subscribe(o =>
            //{
            //    var path = o?.FirstOrDefault();
            //    if (path == null)
            //    {
            //        return;

            //    }
            //    ImagePath = path;
            //});   


            jobsSource = new SourceList<Job>();

            jobsSubscription = jobsSource
                .Connect()
                .ObserveOn(AvaloniaScheduler.Instance)
                .Bind(out jobs)
                .DisposeMany()
                .Subscribe();

            //isBusyObs = OpenCommand.IsExecuting.ToProperty(this, model => model.IsBusy);
        }

        public ReactiveCommand<object> AddJobCommand { get; set; }

        //public bool IsBusy => isBusyObs.Value;      
               
        public ReactiveCommand<string[]> OpenCommand { get; set; }

        public ReadOnlyObservableCollection<Job> Jobs => jobs;
        public void Dispose()
        {
            jobsSubscription.Dispose();
        }
    }
}
