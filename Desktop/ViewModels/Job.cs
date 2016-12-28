using DotImaging;

namespace Desktop.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.Caching;
    using Avalonia.Controls;
    using Avalonia.Threading;
    using DynamicData;
    using Identification;
    using ReactiveUI;
    using Serilog;
    using Views;

    public class Job : ReactiveObject, IDisposable
    {
        private readonly ObservableAsPropertyHelper<bool> canProcessHelper;
        private readonly ObservableAsPropertyHelper<int> framesToTake;
        private readonly ObservableAsPropertyHelper<int> identificationProp;
        private readonly ReadOnlyObservableCollection<IdentificationWithSource> identifications;
        private readonly SourceList<IdentificationWithSource> identificationsSource;
        private readonly IDisposable identificationsSuscription;
        private readonly ObservableAsPropertyHelper<bool> isProcessingHelper;
        private readonly ObservableAsPropertyHelper<VideoMetadata> metadataHelper;
        private readonly ObservableAsPropertyHelper<int> processedHelper;
        private int captureEnd;
        private int captureRate;
        private int captureStart;

        private string videoPath;

        public Job(IBatchIdentificator identificator)
        {
            identificationsSource = new SourceList<IdentificationWithSource>();
            BrowseCommand = ReactiveCommand.CreateAsyncTask(o => new OpenFileDialog().ShowAsync());
            BrowseCommand.Subscribe(OnNewPathsSelected);

            metadataHelper = this
                .WhenAnyValue(job => job.VideoPath)
                .Where(path => path != null)
                .Select(GetMetadata)
                .ToProperty(this, job => job.Metadata);

            var canProcess = this.WhenAnyValue(job => job.VideoPath).Select(path => path != null);
            canProcessHelper = canProcess.ToProperty(this, job => job.CanProcess);

            ProcessJobCommand = ReactiveCommand.CreateAsyncObservable(
                canProcess,
                o =>
                {
                    identificationsSource.Clear();
                    return identificator.Identify(new Uri(VideoPath), CaptureOptions);
                });

            ProcessJobCommand.Subscribe(
                identification =>
                {
                    AddToCache(identification);
                    identificationsSource.Add(new IdentificationWithSource(VideoPath, identification));
                });

            identificationsSuscription = identificationsSource
                .Connect()
                .ObserveOn(AvaloniaScheduler.Instance)
                .Bind(out identifications)
                .DisposeMany()
                .Subscribe();

            var canReview = this.WhenAnyValue(job => job.Identifications).Select(list => list != null);
            ReviewCommand = ReactiveCommand.Create(canReview);
            ReviewCommand.Subscribe(o => StartReview(this));

            ProcessJobCommand.ThrownExceptions.Subscribe(exception => Log.Error(exception, "Cannot process the job {Job}", VideoPath));

            processedHelper = ProcessJobCommand
                .IsExecuting
                .Where(isExecuting => isExecuting)
                .Select(b => ProcessJobCommand.Scan(0, (acc, _) => ++acc))
                .Switch()
                .ToProperty(this, job => job.ProcessedCount);

            identificationProp = this
                .WhenAnyValue(job => job.Identifications)
                .Where(list => list != null)
                .Select(list => list.Count)
                .ToProperty(this, job => job.IdentificationsCount);

            framesToTake =
                this.WhenAnyValue(
                        x => x.CaptureRate,
                        x => x.Metadata,
                        x => x.CaptureStart,
                        x => x.CaptureEnd,
                        (r, m, s, e) =>
                        {
                            if (m == null)
                            {
                                return 0;
                            }

                            return (int) ((e - s) / (r * m.Fps));
                        })
                    .ToProperty(this, x => x.FramesToTake);

            isProcessingHelper = ProcessJobCommand.IsExecuting.ToProperty(this, x => x.IsProcessing);
        }

        public bool IsProcessing => isProcessingHelper.Value;

        public ReadOnlyObservableCollection<IdentificationWithSource> Identifications => identifications;

        public int FramesToTake => framesToTake.Value;

        public int ProcessedCount => processedHelper.Value;

        private CaptureOptions CaptureOptions
        {
            get
            {
                var captureEvery = (int) (Metadata.Fps * CaptureRate);
                var identifyOptions = new CaptureOptions
                {
                    Skip = CaptureStart,
                    Take = FramesToTake,
                    CaptureEvery = captureEvery
                };

                return identifyOptions;
            }
        }

        public bool CanProcess => canProcessHelper.Value;


        public VideoMetadata Metadata => metadataHelper.Value;

        public int CaptureStart
        {
            get { return captureStart; }
            set { this.RaiseAndSetIfChanged(ref captureStart, value); }
        }

        public int CaptureEnd
        {
            get { return captureEnd; }
            set { this.RaiseAndSetIfChanged(ref captureEnd, value); }
        }

        public int CaptureRate
        {
            get { return captureRate; }
            set { this.RaiseAndSetIfChanged(ref captureRate, value); }
        }

        public ReactiveCommand<object> ReviewCommand { get; set; }

        public int IdentificationsCount => identificationProp.Value;

        public ReactiveCommand<string[]> BrowseCommand { get; set; }

        public ReactiveCommand<Identification> ProcessJobCommand { get; }

        public string VideoPath
        {
            get { return videoPath; }
            set { this.RaiseAndSetIfChanged(ref videoPath, value); }
        }

        public void Dispose()
        {
            identificationsSuscription?.Dispose();
        }

        private void AddToCache(Identification identification)
        {
            var uri = new Uri(VideoPath);
            var cache = CacheStore.GetCache(uri);

            using (var fileCapture = new FileCapture(uri.OriginalString))
            {
                var frameId = identification.Ordinal * Metadata.Fps * CaptureRate;
                fileCapture.Seek((long) frameId, SeekOrigin.Begin);

                var encodeAsJpeg = fileCapture.Read().ToBgr().EncodeAsJpeg();
                cache.Add(identification.Ordinal.ToString(), encodeAsJpeg, new CacheItemPolicy());
            }
        }

        private void OnNewPathsSelected(string[] paths)
        {
            if (paths == null)
            {
                return;
            }

            VideoPath = paths.First();
        }

        private static VideoMetadata GetMetadata(string jobSource)
        {
            using (var videoCapture = new FileCapture(jobSource))
            {
                return new VideoMetadata
                {
                    Duration = TimeSpan.FromSeconds(videoCapture.Length / videoCapture.FrameRate),
                    Fps = videoCapture.FrameRate,
                    FrameHeight = videoCapture.FrameSize.Height,
                    FrameWidth = videoCapture.FrameSize.Width,
                    TotalFrames = videoCapture.Length
                };
            }
        }

        private void FillCache(Uri uri)
        {
            var cache = CacheStore.GetCache(uri);

            using (var fileCapture = new FileCapture(uri.OriginalString))
            {
                var indexedImages = fileCapture.ExtractIndexedImages(CaptureOptions);

                foreach (var indexedImage in indexedImages)
                {
                    var key = indexedImage.Index.ToString();
                    var encodeAsJpeg = indexedImage.Image.ToBgr().EncodeAsJpeg();
                    cache.Add(key, encodeAsJpeg, new CacheItemPolicy());
                }
            }
        }

        private void StartReview(Job job)
        {
            var window = new ReviewWindow();
            window.DataContext = job;
            window.Show();
        }
    }
}