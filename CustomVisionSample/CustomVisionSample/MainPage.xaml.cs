using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Plugin.Media;
using Plugin.Media.Abstractions;


namespace CustomVisionSample
{
    public partial class MainPage : ContentPage
    {
        private string ProjectID = "ProjectID";
        private string PredictionKey = "PredictionKey";
        private string IterationID = "IterationID";
        private string Endpoint = @"https://southcentralus.api.cognitive.microsoft.com";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            // 写真撮影
            // Cameraを起動して写真を撮影（Plugin任せ）
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                DefaultCamera = CameraDevice.Front,
                AllowCropping = false,
                PhotoSize = PhotoSize.Medium,
            });

            // 撮影しなかった場合は何もしない
            if (file == null) return;

            // 画像を表示
            image.Source = ImageSource.FromStream(() => file.GetStream());

            try
            {
                // CustomVision用クライアント準備
                // ApiKeyにはPredictionKeyを指定
                // Endpointにはhttps://southcentralus.api.cognitive.microsoft.comを指定
                var client = new CustomVisionPredictionClient()
                {
                    ApiKey = PredictionKey,
                    Endpoint = Endpoint
                };

                // 画像を解析(IterationIDを指定しないとうまくいかない)
                var resul = await client.PredictImageAsync(Guid.Parse(ProjectID), file.GetStream(), iterationId: Guid.Parse(IterationID));
                // 結果取得
                var prediction = resul.Predictions.FirstOrDefault();

                await DisplayAlert("判定結果", $"タグ名: {prediction.TagName} Probability: {prediction.Probability}", "OK");
            }
            catch
            {
                await DisplayAlert("判定結果", $"エラー", "OK");
            }

        }
    }
}
