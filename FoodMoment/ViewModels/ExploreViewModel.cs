using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodMoment.Models;

namespace FoodMoment.ViewModels;

public partial class ExploreViewModel : BaseViewModel
{
    public ObservableCollection<PlaceOfInterest> NearbyPlaces { get; } = [];

    [ObservableProperty]
    private Location? _currentLocation;

    [ObservableProperty]
    private string _locationStatus = "Tap the button to read GPS coordinates.";

    [ObservableProperty]
    private string _latitudeText = "Latitude: —";

    [ObservableProperty]
    private string _longitudeText = "Longitude: —";

    [ObservableProperty]
    private string _addressText = string.Empty;

    public ExploreViewModel()
    {
        Title = "Explore";
    }

    [RelayCommand]
    private async Task GetLocationAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        LocationStatus = "Requesting location permission…";

        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            LocationStatus = "Location permission denied.";
            IsBusy = false;
            return;
        }

        LocationStatus = "Reading GPS…";
        CurrentLocation = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
        {
            DesiredAccuracy = GeolocationAccuracy.Medium,
            Timeout = TimeSpan.FromSeconds(12)
        });

        if (CurrentLocation is null)
        {
            LocationStatus = "Unable to get location.";
            IsBusy = false;
            return;
        }

        LatitudeText = $"Latitude: {CurrentLocation.Latitude:F6}";
        LongitudeText = $"Longitude: {CurrentLocation.Longitude:F6}";
        LocationStatus = "Location updated.";
        await LoadAddressAsync(CurrentLocation);
        LoadMockNearbyPlaces(CurrentLocation);

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
        }

        IsBusy = false;
    }

    private async Task LoadAddressAsync(Location location)
    {
        try
        {
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);
            var placemark = placemarks?.FirstOrDefault();
            if (placemark is null)
            {
                AddressText = string.Empty;
                return;
            }

            AddressText = $"{placemark.Thoroughfare} {placemark.Locality} {placemark.AdminArea}".Trim();
            if (string.IsNullOrWhiteSpace(AddressText))
                AddressText = placemark.FeatureName ?? string.Empty;
        }
        catch
        {
            AddressText = string.Empty;
        }
    }

    private void LoadMockNearbyPlaces(Location location)
    {
        NearbyPlaces.Clear();

        var offsets = new (string name, string type, double latOffset, double lonOffset)[]
        {
            ("Fresh Mart", "Grocery", 0.002, 0.001),
            ("Juice Bar", "Drinks", -0.001, 0.002),
            ("Taste Kitchen", "Restaurant", 0.001, -0.002),
            ("Bake House", "Bakery", -0.002, -0.001)
        };

        foreach (var (name, type, latOffset, lonOffset) in offsets)
        {
            NearbyPlaces.Add(new PlaceOfInterest
            {
                Name = name,
                Type = type,
                Latitude = location.Latitude + latOffset,
                Longitude = location.Longitude + lonOffset,
                Address = AddressText
            });
        }
    }
}
