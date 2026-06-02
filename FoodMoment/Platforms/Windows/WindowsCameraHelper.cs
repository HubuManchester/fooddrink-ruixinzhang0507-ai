#if WINDOWS

using Windows.Media.Capture;

namespace FoodMoment.Services;

internal static class WindowsCameraHelper
{
    public static async Task<CameraCaptureResult> CaptureAsync(string fileName)
    {
        try
        {
            var cameraResult = await CaptureWithSystemCameraUiAsync(fileName);
            if (cameraResult.Status != CameraCaptureStatus.Failed)
                return cameraResult;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Windows CameraCaptureUI failed: {ex.Message}");
        }

        return await PickPhotoFallbackAsync(fileName);
    }

    private static async Task<CameraCaptureResult> CaptureWithSystemCameraUiAsync(string fileName)
    {
        var captureUi = new CameraCaptureUI();

        var captured = await MainThread.InvokeOnMainThreadAsync(async () =>
            await captureUi.CaptureFileAsync(CameraCaptureUIMode.Photo));

        if (captured is null)
            return CameraCaptureResult.CancelledResult();

        var recordsDir = Path.Combine(FileSystem.AppDataDirectory, "records");
        Directory.CreateDirectory(recordsDir);
        var localPath = Path.Combine(recordsDir, fileName);

        await using var source = await captured.OpenStreamForReadAsync();
        await using var dest = File.Create(localPath);
        await source.CopyToAsync(dest);

        return CameraCaptureResult.SuccessResult(localPath);
    }

    private static async Task<CameraCaptureResult> PickPhotoFallbackAsync(string fileName)
    {
        try
        {
            var photo = await MainThread.InvokeOnMainThreadAsync(async () =>
                await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a dish photo"
                }));

            if (photo is null)
                return CameraCaptureResult.CancelledResult();

            var recordsDir = Path.Combine(FileSystem.AppDataDirectory, "records");
            Directory.CreateDirectory(recordsDir);
            var localPath = Path.Combine(recordsDir, fileName);

            await using var source = await photo.OpenReadAsync();
            await using var dest = File.Create(localPath);
            await source.CopyToAsync(dest);

            return CameraCaptureResult.SuccessResult(localPath);
        }
        catch (OperationCanceledException)
        {
            return CameraCaptureResult.CancelledResult();
        }
        catch (Exception ex)
        {
            return CameraCaptureResult.FailedResult(
                CameraCaptureStatus.Failed,
                "Camera is not available. Enable webcam in Windows Settings → Privacy, then try again.");
        }
    }
}

#endif
