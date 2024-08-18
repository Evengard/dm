namespace DM.Services.Mail.Rendering.ViewModels;

/// <summary>
/// View model for registration confirmation letter
/// </summary>
/// <paramref name="Login">
/// Registered user login
/// </paramref>
/// <paramref name="ConfirmationLinkUrl">
/// Link to activate the user
/// </paramref>
public record RegistrationConfirmationViewModel(
    string Login,
    string ConfirmationLinkUrl);