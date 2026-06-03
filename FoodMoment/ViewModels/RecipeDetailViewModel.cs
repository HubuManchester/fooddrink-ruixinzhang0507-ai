using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodMoment.Models;
using FoodMoment.Services;

namespace FoodMoment.ViewModels;

public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeRepository _repository;
    private readonly ICameraPhotoService _cameraPhotoService;
    private CancellationTokenSource? _speechCts;

    [ObservableProperty]
    private Recipe? _recipe;

    [ObservableProperty]
    private int _currentStepIndex;

    [ObservableProperty]
    private string _currentStepText = string.Empty;

    [ObservableProperty]
    private bool _showIngredients = true;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private bool _isSpeaking;

    [ObservableProperty]
    private bool _isCapturing;

    [ObservableProperty]
    private string? _lastCapturedPhotoPath;

    [ObservableProperty]
    private bool _hasCapturedPhoto;

    public int RecipeId { get; set; }

    public RecipeDetailViewModel()
    {
        _repository = ServiceHelper.GetService<IRecipeRepository>();
        _cameraPhotoService = ServiceHelper.GetService<ICameraPhotoService>();
        Title = "Recipe";
    }

    public async Task LoadRecipeAsync()
    {
        if (RecipeId <= 0)
            return;

        IsBusy = true;
        try
        {
            LastCapturedPhotoPath = null;
            HasCapturedPhoto = false;

            Recipe = await _repository.GetByIdAsync(RecipeId);
            if (Recipe is not null)
            {
                Title = Recipe.Title;
                IsFavorite = Recipe.IsFavorite;
                UpdateCurrentStepText();
            }
        }
        finally
        {
            IsBusy = false;
            CapturePhotoCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void ShowIngredientsTab() => ShowIngredients = true;

    [RelayCommand]
    private void ShowStepsTab() => ShowIngredients = false;

    [RelayCommand]
    private async Task ReadAloudAsync()
    {
        if (Recipe is null || Recipe.Steps.Count == 0)
            return;

        if (IsSpeaking)
            return;

        _speechCts = new CancellationTokenSource();
        IsSpeaking = true;

        try
        {
            var stepText = Recipe.Steps[CurrentStepIndex];
            var text = $"Step {CurrentStepIndex + 1}. {stepText}";
            await TextToSpeech.Default.SpeakAsync(text, cancelToken: _speechCts.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsSpeaking = false;
            _speechCts?.Dispose();
            _speechCts = null;
        }
    }

    [RelayCommand]
    private void StopSpeech()
    {
        if (!IsSpeaking)
            return;

        _speechCts?.Cancel();
        IsSpeaking = false;
    }

    [RelayCommand(CanExecute = nameof(CanCapturePhoto))]
    private async Task CapturePhotoAsync()
    {
        if (Recipe is null || IsCapturing)
            return;

        IsCapturing = true;
        CapturePhotoCommand.NotifyCanExecuteChanged();

        try
        {
            var fileName = $"cook_{Recipe.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var result = await _cameraPhotoService.CaptureDishPhotoAsync(fileName);

            switch (result.Status)
            {
                case CameraCaptureStatus.Success when result.SavedPath is not null:
                    LastCapturedPhotoPath = result.SavedPath;
                    HasCapturedPhoto = true;
                    await _repository.AddCookRecordAsync(Recipe.Id, Recipe.Title, result.SavedPath);
                    TryHapticFeedback();
                    break;

                case CameraCaptureStatus.Cancelled:
                    break;

                default:
                    await ShowAlertAsync("Capture photo", result.Message ?? "Unable to capture a photo.");
                    break;
            }
        }
        finally
        {
            IsCapturing = false;
            CapturePhotoCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanCapturePhoto() => !IsCapturing && !IsBusy;

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (Recipe is null)
            return;

        await _repository.ToggleFavoriteAsync(Recipe.Id);
        Recipe.IsFavorite = !Recipe.IsFavorite;
        IsFavorite = Recipe.IsFavorite;
        TryHapticFeedback();
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (Recipe is null || CurrentStepIndex <= 0)
            return;

        CurrentStepIndex--;
    }

    [RelayCommand]
    private void NextStep()
    {
        if (Recipe is null || CurrentStepIndex >= Recipe.Steps.Count - 1)
            return;

        CurrentStepIndex++;
    }

    public int CurrentStepNumber => CurrentStepIndex + 1;

    partial void OnCurrentStepIndexChanged(int value)
    {
        UpdateCurrentStepText();
        OnPropertyChanged(nameof(CurrentStepNumber));
    }

    private void UpdateCurrentStepText()
    {
        if (Recipe is null || Recipe.Steps.Count == 0)
        {
            CurrentStepText = string.Empty;
            return;
        }

        var index = Math.Clamp(CurrentStepIndex, 0, Recipe.Steps.Count - 1);
        CurrentStepIndex = index;
        CurrentStepText = Recipe.Steps[index];
    }

    private static void TryHapticFeedback()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
        }
    }

    private static async Task ShowAlertAsync(string title, string message)
    {
        var page = Shell.Current?.CurrentPage;
        if (page is not null)
            await page.DisplayAlert(title, message, "OK");
    }
}
