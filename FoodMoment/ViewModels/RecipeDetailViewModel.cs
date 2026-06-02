using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodMoment.Models;
using FoodMoment.Services;

namespace FoodMoment.ViewModels;

public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeRepository _repository;
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
    private string? _lastCapturedPhotoPath;

    [ObservableProperty]
    private bool _hasCapturedPhoto;

    public int RecipeId { get; set; }

    public RecipeDetailViewModel()
    {
        _repository = ServiceHelper.GetService<IRecipeRepository>();
        Title = "Recipe";
    }

    public async Task LoadRecipeAsync()
    {
        if (RecipeId <= 0)
            return;

        IsBusy = true;
        Recipe = await _repository.GetByIdAsync(RecipeId);
        if (Recipe is not null)
        {
            Title = Recipe.Title;
            IsFavorite = Recipe.IsFavorite;
            UpdateCurrentStepText();
        }
        IsBusy = false;
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

    [RelayCommand]
    private async Task CapturePhotoAsync()
    {
        if (Recipe is null)
            return;

        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            return;

        var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = "Capture your dish"
        });

        if (photo is null)
            return;

        var recordsDir = Path.Combine(FileSystem.AppDataDirectory, "records");
        Directory.CreateDirectory(recordsDir);
        var fileName = $"cook_{Recipe.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
        var localPath = Path.Combine(recordsDir, fileName);

        await using var source = await photo.OpenReadAsync();
        await using var dest = File.OpenWrite(localPath);
        await source.CopyToAsync(dest);

        LastCapturedPhotoPath = localPath;
        HasCapturedPhoto = true;

        await _repository.AddCookRecordAsync(Recipe.Id, Recipe.Title, localPath);

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (Recipe is null)
            return;

        await _repository.ToggleFavoriteAsync(Recipe.Id);
        Recipe.IsFavorite = !Recipe.IsFavorite;
        IsFavorite = Recipe.IsFavorite;

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
        }
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
}
