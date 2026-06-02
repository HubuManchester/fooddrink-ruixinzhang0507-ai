using System.Diagnostics;

namespace FoodMoment.Services;

public class CameraPhotoService : ICameraPhotoService
{
    public async Task<CameraCaptureResult> CaptureDishPhotoAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return CameraCaptureResult.FailedResult(CameraCaptureStatus.Failed, "Invalid file name.");

        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
                return await CaptureWithPlatformFallbackAsync(fileName);

            var permission = await Permissions.RequestAsync<Permissions.Camera>();
            if (permission != PermissionStatus.Granted)
            {
                return CameraCaptureResult.FailedResult(
                    CameraCaptureStatus.PermissionDenied,
                    "Camera permission was denied. Enable camera access in system settings and try again.");
            }

            FileResult? photo = null;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Capture your dish"
                });
            });

            if (photo is null)
                return CameraCaptureResult.CancelledResult();

            var savedPath = await SavePhotoAsync(photo, fileName);
            return CameraCaptureResult.SuccessResult(savedPath);
        }
        catch (OperationCanceledException)
        {
            return CameraCaptureResult.CancelledResult();
        }
        catch (PermissionException)
        {
            return CameraCaptureResult.FailedResult(
                CameraCaptureStatus.PermissionDenied,
                "Camera permission is required to capture a photo.");
        }
        catch (FeatureNotSupportedException)
        {
            return await CaptureWithPlatformFallbackAsync(fileName);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Camera capture failed: {ex}");
            return await CaptureWithPlatformFallbackAsync(fileName, ex.Message);
        }
    }

    private static async Task<CameraCaptureResult> CaptureWithPlatformFallbackAsync(
        string fileName,
        string? previousError = null)
    {
#if WINDOWS
        var windowsResult = await WindowsCameraHelper.CaptureAsync(fileName);
        if (windowsResult.Status != CameraCaptureStatus.Failed || previousError is null)
            return windowsResult;

        return CameraCaptureResult.FailedResult(
            CameraCaptureStatus.Failed,
            $"{previousError} {windowsResult.Message}".Trim());
#else
        _ = previousError;
        return CameraCaptureResult.FailedResult(
            CameraCaptureStatus.NotSupported,
            previousError ?? "This device does not support taking photos with the camera.");
#endif
    }

    private static async Task<string> SavePhotoAsync(FileResult photo, string fileName)
    {
        var recordsDir = Path.Combine(FileSystem.AppDataDirectory, "records");
        Directory.CreateDirectory(recordsDir);
        var localPath = Path.Combine(recordsDir, fileName);

        await using var source = await photo.OpenReadAsync();
        await using var dest = File.Create(localPath);
        await source.CopyToAsync(dest);

        return localPath;
    }
}
