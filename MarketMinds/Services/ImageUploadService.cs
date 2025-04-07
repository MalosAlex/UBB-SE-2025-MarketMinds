using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using DomainLayer.Domain;
using Newtonsoft.Json;
using Windows.Storage.Streams;

namespace MarketMinds.Services
{
    public class ImageUploadService
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<string> UploadImage(Window window)
        {
            var hwnd = WindowNative.GetWindowHandle(window);
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                return await UploadToImgur(file);
            }
            return string.Empty;
        }

        private async Task<string> UploadToImgur(StorageFile file)
        {
            try
            {
                // Log file details
                var properties = await file.GetBasicPropertiesAsync();
                string clientId = App.Configuration.GetSection("ImgurSettings:ClientId").Value;

                // Validate Client ID format
                if (string.IsNullOrEmpty(clientId))
                {
                    // In a service, we should throw exceptions instead of showing dialogs
                    throw new InvalidOperationException("Client ID is not configured. Please check your appsettings.json file.");
                }

                // Typical Imgur Client IDs are around 15 chars
                if (clientId.Length > 20)
                {
                    throw new InvalidOperationException("Client ID format appears invalid. Please ensure you're using the Client ID, not the Client Secret.");
                }

                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    if (stream.Size > 10 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("File size exceeds Imgur's 10MB limit.");
                    }

                    byte[] buffer = new byte[stream.Size];
                    using (var reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(buffer);
                    }

                    int maxRetries = 3;
                    int currentRetry = 0;
                    TimeSpan delay = TimeSpan.FromSeconds(2);

                    while (currentRetry < maxRetries)
                    {
                        try
                        {
                            using (var content = new MultipartFormDataContent())
                            {
                                var imageContent = new ByteArrayContent(buffer);
                                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                                content.Add(imageContent, "image", "image.png");

                                using (var request = new HttpRequestMessage(HttpMethod.Post, "https://api.imgur.com/3/image"))
                                {
                                    request.Headers.Add("Authorization", $"Client-ID {clientId}");
                                    request.Content = content;
                                    var response = await HttpClient.SendAsync(request);
                                    var responseBody = await response.Content.ReadAsStringAsync();

                                    if (response.IsSuccessStatusCode)
                                    {
                                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                                        string link = jsonResponse?.data?.link;
                                        if (!string.IsNullOrEmpty(link))
                                        {
                                            return link;
                                        }
                                    }
                                }
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            // Log exception but continue with retry
                        }

                        if (currentRetry < maxRetries - 1)
                        {
                            await Task.Delay(delay);
                            delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
                        }
                        currentRetry++;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Upload failed: {ex.Message}", ex);
            }
        }

        public Task<bool> UploadImageAsync(string filePath)
        {
            // Simulate image upload
            return Task.FromResult(true);
        }

        public Image CreateImageFromPath(string path)
        {
            // Create an image from a file path
            return new Image(path);
        }

        public string FormatImagesString(List<Image> images)
        {
            return images != null ? string.Join("\n", images.Select(img => img.Url)) : string.Empty;
        }

        public List<Image> ParseImagesString(string imagesString)
        {
            if (string.IsNullOrEmpty(imagesString))
            {
                return new List<Image>();
            }

            return imagesString.Split("\n")
                .Where(url => !string.IsNullOrEmpty(url))
                .Select(url => new Image(url))
                .ToList();
        }
        public async Task<string> AddImageToCollection(Window window, string currentImagesString)
        {
            string imgurLink = await UploadImage(window);
            if (!string.IsNullOrEmpty(imgurLink))
            {
                return string.IsNullOrEmpty(currentImagesString)
                    ? imgurLink
                    : currentImagesString + "\n" + imgurLink;
            }
            return currentImagesString;
        }
    }
}