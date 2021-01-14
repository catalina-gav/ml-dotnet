using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using MLMicroservice.MLModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace MLMicroservice.Controllers
{
    [Route("api/v1/predictions")]
    [ApiController]

    public class DigitController : ControllerBase
    {

        private const int SizeOfImage = 448;
        private const int SizeOfArea =16;

        private readonly PredictionEnginePool<DigitData, DigitPrediction> _predictionEnginePool;
        
        public DigitController(PredictionEnginePool<DigitData,DigitPrediction> predictionEnginePool)
        {
            this._predictionEnginePool = predictionEnginePool;
        }
        private static List<float> GetPixelValuesFromImage(string base64Image)
        {
            
            //base64Image = base64Image.Replace("data:image/png;base64,", "");
            var imageBytes = Convert.FromBase64String(base64Image).ToArray();

            Image image;

            using (var stream = new MemoryStream(imageBytes))
            {
                image = Image.FromStream(stream);
            }
            

            var res = new Bitmap(SizeOfImage, SizeOfImage);
            using (var g = Graphics.FromImage(res))
            {
                g.Clear(Color.White);
                g.DrawImage(image, 0, 0, SizeOfImage, SizeOfImage);
            }

            

            var datasetValue = new List<float>();

            for (int i = 0; i < SizeOfImage; i += SizeOfArea)
            {
                for (int j = 0; j < SizeOfImage; j += SizeOfArea)
                {
                    var sum = 0;
                    for (int k = i; k < i + SizeOfArea; k++)
                    {
                        for (int l = j; l < j + SizeOfArea; l++)
                        {
                            sum += res.GetPixel(l, k).Name == "ffffffff" ? 0 : 1;
                        }
                    }
                    if (sum == 256)
                        sum = sum - 1;
                    datasetValue.Add(sum);
                } /*
                var ibytes = Convert.FromBase64String(base64Image).ToArray();
                var result = new List<float>();
                Image image;
                using (MemoryStream ms = new MemoryStream(ibytes))
                {
                    image = Image.FromStream(ms);
                }
                Bitmap b = new Bitmap(28, 28);
                Graphics g1 = Graphics.FromImage((Image)b);
                g1.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g1.DrawImage(image, 0, 0, 28, 28);
                g1.Dispose();
                image = (Image)b;
                string base64 = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    // Convert Image to byte[]
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    byte[] bytes = ms.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(bytes);
                    base64 = base64String;
                    System.Diagnostics.Debug.WriteLine(base64String);
                }
                var imageBytes= Convert.FromBase64String(base64).ToArray();


                var bitmap = new Bitmap(SizeOfImage, SizeOfImage);

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    using var stream = new MemoryStream(imageBytes);
                    var png = Image.FromStream(stream);
                    g.DrawImage(png, 0, 0, SizeOfImage, SizeOfImage);
                }




                for (var i = 0; i < SizeOfImage; i += SizeOfArea)
                {
                    for (var j = 0; j < SizeOfImage; j += SizeOfArea)
                    {
                        var sum = 0;        
                        for (var k = i; k < i + SizeOfArea; k++)
                        {
                            for (var l = j; l < j + SizeOfArea; l++)
                            {
                                if (bitmap.GetPixel(l, k).Name != "ffffffff") sum++;
                            }
                        }
                        if (sum == 256)
                            result.Add(sum - 1);
                        else
                            result.Add(sum);

                    }
                }
                System.Diagnostics.Debug.WriteLine("Lungime array din imagine");
                System.Diagnostics.Debug.WriteLine(result.ToArray().Length);
            for(var i = 0; i < 28; i++)
            {
                result[i] = 0;
            }
            for(var i =28 ; i < 784; i += 28)
            {
                result[i] = 0;
            }
                return result;*/

            }
            return datasetValue;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Post([FromBody] string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                return BadRequest(new { prediction = "-", dataset = string.Empty });
            }
            var pixelValues = GetPixelValuesFromImage(base64Image);
            DigitData input = new DigitData(pixelValues.ToArray());
            Console.WriteLine(input);
            var result = _predictionEnginePool.Predict(modelName: "DigitAnalysisModel", example: input);
            return Ok(new
            {
                prediction = result.Prediction,
                scores = result.Score,
                pixelValues = string.Join(",", pixelValues)
            });
        }

     
    }
}
