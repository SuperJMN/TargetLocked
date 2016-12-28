namespace Identification
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using DotImaging;
    using Microsoft.ProjectOxford.Face;

    public class BatchIdentificator : IBatchIdentificator
    {
        private readonly IFaceServiceClient faceServiceClient;

        public BatchIdentificator(IFaceServiceClient faceServiceClient)
        {
            this.faceServiceClient = faceServiceClient;
        }
        public IObservable<Identification> Identify(Uri origin, CaptureOptions options)
        {
            return CreateObservableFromCapture(new FileCapture(origin.OriginalString), options);
        }

        private IObservable<Identification> CreateObservableFromCapture(FileCapture capture, CaptureOptions options)
        {
            return capture
                .ExtractIndexedImages(options)
                .ToObservable(new EventLoopScheduler())
                .Select(indexedImg => new IdentificationRequest(indexedImg.Image.ToBgr().EncodeAsJpeg(), indexedImg.Index))
                .Select(IdentifyImage)
                .Merge(3);
        }

        private IObservable<Identification> IdentifyImage(IdentificationRequest request)
        {
            return Observable.Defer(() => Observable.FromAsync(() => IdentifyImageAsync(request))).RetryWithBackoffStrategy(retryCount: 6);
        }

        private async Task<Identification> IdentifyImageAsync(IdentificationRequest request)
        {
            using (var stream = new MemoryStream(request.Image))
            {
                var faces = await IdentifyFaces(stream);
                return new Identification(request.Index, faces);
            }
        }

        private async Task<IEnumerable<Face>> IdentifyFaces(Stream stream)
        {
            var detectedFaces = await faceServiceClient.DetectAsync(stream);
            return detectedFaces.Select(face => new Face(face.FaceId, face.FaceRectangle.ToRect(), null)).ToList();
        }

        private async Task<IEnumerable<Face>> IdentifyFaces2(Stream stream)
        {
            var detectedFaces = await faceServiceClient.DetectAsync(stream);
            var identifications = await faceServiceClient.IdentifyAsync("38", detectedFaces.Select(face => face.FaceId).ToArray());

            var myFaces = from face in detectedFaces
                          join ident in identifications on face.FaceId equals ident.FaceId into identifyResults
                          from identifyResult in identifyResults.DefaultIfEmpty()
                          select new Face(identifyResult.FaceId, face.FaceRectangle.ToRect(), identifyResult.Candidates.FirstOrDefault()?.PersonId);

            return myFaces;
        }
    }
}