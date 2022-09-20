using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;
using ImageProcessorCore;

namespace ComputerVisionQuickstart
{
    class Program
    {
        static string subscriptionKey = "************************";
        static string endpoint = "************************";
        const string imagePath = @"************************";
        const string destinationImage = @"************************";


        static void Main(string[] args)
        {
            Console.WriteLine("Azure Cognitive Services Computer Vision - .NET example");
            Console.WriteLine();

            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            AnalyzeImageStream(client, imagePath).Wait();
        }
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }
        public static async Task AnalyzeImageStream(ComputerVisionClient client, string imageStream)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();
            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Faces
            };

            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageStream)}...");
            Console.WriteLine();

            using (FileStream input = File.OpenRead(imageStream))
            {
                // Analyze the local image.
                ImageAnalysis results = await client.AnalyzeImageInStreamAsync(input, visualFeatures: features);
                //Blur Image
                using (FileStream stream = File.OpenRead(imagePath))
                using (FileStream output = File.OpenWrite(destinationImage))
                {
                    var image = new Image<Color, uint>(stream);
                    foreach (var face in results.Faces)
                    {
                        Console.WriteLine($"{face.FaceRectangle.Top} {face.FaceRectangle.Left} {face.FaceRectangle.Width} {face.FaceRectangle.Height}");
                        var rectangle = new Rectangle(
                            face.FaceRectangle.Left,
                            face.FaceRectangle.Top,
                            face.FaceRectangle.Width,
                            face.FaceRectangle.Height
                        );
                        image = image.BoxBlur(20, rectangle);
                    }
                    image.SaveAsJpeg(output);
                }
            }
        }
    }
}