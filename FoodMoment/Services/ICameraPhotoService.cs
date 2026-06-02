namespace FoodMoment.Services;

public enum CameraCaptureStatus
{
    Success,
    Cancelled,
    NotSupported,
    PermissionDenied,
    Failed
}

public sealed class CameraCaptureResult
{
    public CameraCaptureStatus Status { get; init; }
    public string? SavedPath { get; init; }
    public string? Message { get; init; }

    public static CameraCaptureResult CancelledResult() =>
        new() { Status = CameraCaptureStatus.Cancelled };

    public static CameraCaptureResult SuccessResult(string path) =>
        new() { Status = CameraCaptureStatus.Success, SavedPath = path };

    public static CameraCaptureResult FailedResult(CameraCaptureStatus status, string message) =>
        new() { Status = status, Message = message };
}

public interface ICameraPhotoService
{
    Task<CameraCaptureResult> CaptureDishPhotoAsync(string fileName);
}
