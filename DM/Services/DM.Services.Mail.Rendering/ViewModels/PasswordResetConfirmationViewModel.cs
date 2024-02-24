namespace DM.Services.Mail.Rendering.ViewModels;

/// <summary>
/// View model for password reset confirmation letter
/// </summary>
/// <paramref name="Login">
/// User login
/// </paramref>
/// <paramref name="ConfirmationLinkUrl">
/// Confirmation link URL
/// </paramref>
public record PasswordResetConfirmationViewModel(
    string Login,
    string ConfirmationLinkUrl);