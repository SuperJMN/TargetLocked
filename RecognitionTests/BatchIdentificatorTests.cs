namespace RecognitionTests
{
    using System;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Identification;
    using Microsoft.ProjectOxford.Face;
    using Xunit;

    public class BatchIdentificatorTests
    {
        [Fact]
        public async Task RealTest()
        {
            var sut = new BatchIdentificator(new FaceServiceClient("c789130948bb409a801eabd78e136071"));
            var identifyOptions = new CaptureOptions
            {
                Skip = 9000,
                Take = 400,
                CaptureEvery = 60,
            };

            var identifications = await sut.Identify(new Uri(@"D:\SuperJMN\OneDrive\Exchange\Bravent\AISGE\El Príncipe 3.mp4"), identifyOptions).ToList();
            Assert.NotEmpty(identifications);
        }
    }
}
