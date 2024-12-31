using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.PhotoStocks;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FreeCourse.Web.Services
{
    public class PhotoStockService : IPhotoStockService
    {

        private readonly HttpClient _httpClient;

        public PhotoStockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeletePhoto(string photoUrl)
        {
            var response = await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");

            return response.IsSuccessStatusCode;
        }

        public async Task<PhotoViewModel> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length <= 0)
                return null;
            var randomFilename = $"{Guid.NewGuid().ToString()}{Path.GetExtension(photo.FileName)}";

            //using var ms = new MemoryStream();
            //await photo.CopyToAsync(ms);

            var multipartContent = new MultipartFormDataContent();
            using (var memoryStream = new MemoryStream())
            {
                await photo.CopyToAsync(memoryStream);
                using (var img = System.Drawing.Image.FromStream(memoryStream))
                {
                    var image = Resize(img,300,168);
                    image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                multipartContent.Add(new ByteArrayContent(memoryStream.ToArray()), "photo", randomFilename);
            }

            var response = await _httpClient.PostAsync("photos", multipartContent);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<PhotoViewModel>>();
            return responseSuccess.Data;
        }

        private System.Drawing.Image Resize(System.Drawing.Image image, int width, int height)
        {
            var res = new Bitmap(width, height);
            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, width, height);
            }
            return res;
        }

    }
}
