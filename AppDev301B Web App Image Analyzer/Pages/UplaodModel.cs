using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

namespace AppDev301B_Web_App_Image_Analyzer.Pages
{
    public class Class
    {
    }
}
public class UploadModel : PageModel
{
    [BindProperty]
    public IFormFile Image { get; set; }

    public string Description { get; set; }

    // Add properties for other extracted information (optional)
    public List<string> Tags { get; set; } = new List<string>();
    public List<string> Categories { get; set; } = new List<string>();
    private readonly string _connectionString =
        "DefaultEndpointsProtocol=https;AccountName=appdevimageanalyzerappst;AccountKey=7ZODOfO0gF4QRDaBSHZ6xfjPnH2LywjNEll9Z/iOze/mdI2DVHmAMrPJjLOs6zYGqFNhd4o4LtT8+AStiOchBg==;EndpointSuffix=core.windows.net";
    private async Task UploadImageToBlobAsync(IFormFile image)
    {
        // Create a BlobServiceClient object
        var blobServiceClient = new BlobServiceClient(_connectionString);

        // Get the container reference (create if it doesn't exist)
        var containerClient = blobServiceClient.GetBlobContainerClient("appdevimageanalyzerappst");
        await containerClient.CreateIfNotExistsAsync();

        // Generate a unique name for the uploaded image
        var fileName = Path.GetFileName(image.FileName);
        var blobClient = containerClient.GetBlobClient(fileName);

        // Upload the image data to Blob Storage
        await blobClient.UploadBlobAsync(image.OpenReadStream());
    }
    private readonly string _computerVisionEndpoint = "https://image-analyzer-web-app.cognitiveservices.azure.com/";
    private readonly string _computerVisionApiKey = "3825ff8e98414450992e4b172ef23e8d";

    private async Task AnalyzeImageAsync(string imageUrl)
    {
        using (var httpClient = new HttpClient())
        {
            // Prepare the request body
            string requestBody = "{ \"url\": \"" + imageUrl + "\" }";

            // Set headers for authorization and content type
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_computerVisionApiKey}");
            httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            // Send the request to the Computer Vision API
            var response = await httpClient.PostAsync(_computerVisionEndpoint + "/vision/v3.2/analyze", new StringContent(requestBody));

            // Check for successful response
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                // Parse the response to extract image description, tags, and categories
                // (implementation details based on Computer Vision API response format)
                Description = "Extracted description from Computer Vision API";
                Tags = new List<string> { "tag1", "tag2" }; // Example
                Categories = new List<string> { "category1", "category2" }; // Example
            }
            else
            {
                ModelState.AddModelError("Analysis", "Failed to analyze image using Computer Vision API.");
            }
        }
    }
}