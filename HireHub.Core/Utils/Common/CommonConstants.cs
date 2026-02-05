using HireHub.Core.Data.Models;

namespace HireHub.Core.Utils.Common;

public static class LogMessage
{
    public const string StartMethod = "Start - {Method}";
    public const string EndMethod = "End - {Method}";
    public const string EndMethodException = "End - {Method} - {Exception}";
    public const string UserNotFoundOnLogin = "Login failed: user not found: {Username}";
    public const string InvalidPassword = "Login failed: invalid password for {EmpId}";
    public const string NotActiveUser = "Login failed: {EmpId} is not a Active User";
    public const string UserNotFound = "User not found: {Username}";
    public const string OtpValidationFailed = "Otp validation failed for user: {Username}";
}

public static class ExceptionMessage
{
    public const string JwtNotConfigured = "JWT settings not configured";
    public const string ConnectionStringNotConfigured = "ConnectionString not configured";
    public const string AzureLogicAppNotConfigured = "Azure Logic App not configured";
    public const string UserIdNotSetOnMiddleware = "Middlerware failed: User Id not set";
    public const string UserRoleNotSetOnMiddleware = "Middlerware failed: User Role not set";
}

public static class ResponseMessage
{
    public const string NotActiveUser = "You are not a Active User";
    public const string PasswordSetRequire = "Password require to be set";
    public const string PasswordReSetRequire = "Password require to be re-set";
    public const string InvalidExperienceLevel = "Provided Candidate Experience Level is Invalid";
    public const string EmailOrPhoneAlreadyExist = "Email or Phone number already exist";
    public const string InvalidRole = "Provided Role is Invalid";
    public const string InvalidEmail = "Invalid Email";
    public const string NameShouldNotNull = "Name should not be Null";
    public const string IsActiveShouldNotNull = "IsActive should not be Null";
    public const string EmailShouldNotNull = "Email should not be Null";
    public const string PhoneShouldNotNull = "Phone should not be Null";
    public const string ExcelFileEmpty = "Excel file is empty";
    public const string CellValueConvertionFailed = "Cell value convertion failed";
    public const string UpdatedSuccessfully = "Updated Successfully";
    public const string EmailNotFound = "Email not found";
    public const string PasswordChangedSuccessfully = "Password Changed Successfully";
    public const string OtpValidationFailed = "Otp validation failed";
    public const string DuplicateRecordsFound = "Duplicate records found for email or phone";
    public const string NoHrs = "Minimum one HR should be add";
    public const string NoPanelMembers = "Minimum one Panel member should be add";
    public const string NoMentors = "Minimum one Mentor should be add";
    public const string SomeUserNotFound = "Some user not found";
    public const string SomeInactiveUsersFound = "Some users are Inactive";
    public const string InactiveUser = "Not a Active User";
    public const string SomeUserNotInSpecifiedRole = "Some user not in a Specified Role";
    public const string SomeUsersAssignedToAnotherActiveDriveOnSameDate = "Some user were assigned to another active Drive on same Date";
    public const string UsersAssignedToAnotherActiveDriveOnSameDate = "User was assigned to another active Drive on same Date";
    public const string SomeDuplicateUsersFoundIn = "Some duplicate users found in {0} list";
    public const string UserNotFound = "User not found";
    public const string DriveNotFound = "Drive not found";
    public const string DriveMemberNotFound = "Drive Member not found";
    public const string DriveCandidateNotFound = "Drive Candidate not found";
    public const string CannotAddMemberOnClosedDrive = "Cannot add Member on a closed Drive";
    public const string CannotAddCandidatesOnClosedDrive = "Cannot add candidates on a closed Drive";
    public const string ClosedDriveCannotBeEdit = "Closed Drive cannot be edit";
    public const string PausedDriveCannotBeEdit = "Paused Drive cannot be edit";
    public const string SomeDriveNotFound = "Some Drive not found";
    public const string CandidateNotFound = "Candidate not found";
    public const string UserNotInSpecifiedRole = "User not in a Specified Role";
    public const string UserIdRequired = "UserId is required";
    public const string InvalidUserId = "Invalid UserId";
    public const string PasswordCannotBeUpdated = "Password cannot be updated";
    public const string CreatedDateCannotBeUpdated = "CreatedDate cannot be updated";
    public const string UpdatedDateCannotBeUpdated = "UpdatedDate cannot be updated";
    public const string TechStackShouldNotBeNull = "Tech stack is should not be null";
    public const string TechStackMustBeList = "Tech stack must be Array of string";
    public const string InvalidCandidateId = "Invalid CandidateId";
    public const string DriveIdRequired = "DriveId is required";
    public const string InvalidDriveId = "Invalid DriveId";
    public const string CreatedByCannotBeUpdated = "CreatedBy cannot be updated";
    public const string AdminOrDriveOwnerCanEdit = "Admin or Owner of the Drive can able to edit";
    public const string AdminOrDriveOwnerCanAdd = "Admin or Owner of the Drive can able to add";
    public const string AdminOrDriveOwnerCanRemove = "Admin or Owner of the Drive can able to remove";
    public const string AdminOrDriveOwnerOrHrInterviewerCanEdit = "Admin or Drive Owner or Interviewer(Hr) can able to edit";
    public const string AdminOrDriveOwnerOrInterviewerCanEdit = "Admin or Drive Owner or Interviewer can able to edit";
    public const string DriveNameAlreadyExist = "Drive Name Already Exist";
    public const string DriveNameCannotBeChange = "Drive Name cannot be change";
    public const string DriveDateCannotBeChange = "Drive Date cannot be change";
    public const string DriveTechnicalRoundsCannotBeChange = "Drive Technical Rounds cannot be change";
    public const string FutureDateOnlyAllowed = "Future Date only allowed";
    public const string TechRoundsShouldBe = "Technical Rounds should be 1 or 2";
    public const string InvalidDriveStatus = "Invalid Drive Status";
    public const string DriveStatusCannotChangeToInproposal = "DriveStatus cannot be change to InProposal";
    public const string CannotRemoveMembersOnStartedDrive = "Cannot remove Members on a started Drive";
    public const string InvalidCandidateStatus = "Invalid Candidate Status";
    public const string CandidateIdRequired = "CandidateId is required";
    public const string CandidateStatusCannotChangeToPending = "CandidateStatus cannot be change to Pending";
    public const string DriveNeedToStartFirst = "Drive need to be start first";
    public const string InvalidRoundType = "Invalid Round Type";
    public const string InvalidRoundStatus = "Invalid Round Status";
    public const string InvalidRoundResult = "Invalid Round Result";
    public const string InterviewRoundNotFound = "Interview Round not found";
    public const string AlreadyMemberOfDrive = "Already a member of the Drive";
    public const string SomeCandidateAlreadyAddedToDrive = "Some Candidate already added to the Drive";
    public const string FeedbackNotFound = "Feedback not found";
    public const string RoundIdRequired = "RoundId is required";
    public const string InvalidRoundId = "Invalid RoundId";
    public const string InterviewRoundClosed = "Interview Round was closed";
    public const string NeedToSetRoundResultBeforeCloseRound = "Need to set Round Result before closing Round";
    public const string NeedToStartRoundBeforeSetRoundResult = "Need to Start Round before set Round Result";
    public const string NoFeedbackProvidedForRound = "No Feedback was provided for the Interview Round";
    public const string InvalidCandidateRecommendation = "Invalid Candidate Recommendation";
    public const string InvalidRatingNumber = "Invalid Rating Number";
    public const string FeedbackAlreadyProvided = "Feedback already provided";
}

public static class InnerExceptionMessage
{
    public const string DriveCandidateAlreadyExistViolation = "Violation of UNIQUE KEY constraint 'UQ_CandidateId_DriveId'.";
}

public static class PropertyName
{
    public const string Main = "Main";
}

public static class JOPropertyName
{
    public const string UserId = "userId";
    public const string PasswordHash = "passwordHash";
    public const string CreatedDate = "createdDate";
    public const string UpdatedDate = "updatedDate";
    public const string Email = "email";
    public const string Phone = "phone";
    public const string FullName = "fullName";
    public const string IsActive = "isActive";
    public const string RoleName = "roleName";
    public const string CandidateId = "candidateId";
    public const string Address= "address";
    public const string College = "college";
    public const string PreviousCompany = "previousCompany";
    public const string ExperienceLevel = "experienceLevel";
    public const string TechStack = "techStack";
    public const string ResumeUrl = "resumeUrl";
    public const string LinkedInUrl = "linkedInUrl";
    public const string GitHubUrl = "gitHubUrl";
    public const string DriveId = "driveId";
    public const string DriveName = "driveName";
    public const string DriveDate = "driveDate";
    public const string TechnicalRounds = "technicalRounds";
    public const string DriveStatus = "driveStatus";
    public const string CreatedBy = "createdBy";
    public const string CandidateStatus = "candidateStatus";
    public const string RoundId = "roundId";
    public const string RoundStatus = "roundStatus";
    public const string RoundResult = "roundResult";
    public const string FeedbackId = "feedbackId";
    public const string OverallRating = "overallRating";
    public const string TechnicalSkill = "technicalSkill";
    public const string Communication = "communication";
    public const string ProblemSolving = "problemSolving";
    public const string OverallFeedback = "overallFeedback";
    public const string Recommendation = "recommendation";

    public const string PanelVisibilitySettings = "panelVisibilitySettings";
    public const string PVS_ShowPhone = "panelVisibilitySettings.showPhone";
    public const string PVS_ShowEmail = "panelVisibilitySettings.showEmail";
    public const string PVS_ShowPreviousCompany = "panelVisibilitySettings.showPreviousCompany";
    public const string PVS_ShowResume = "panelVisibilitySettings.showResume";
    public const string PVS_ShowCollege = "panelVisibilitySettings.showCollege";
    public const string PVS_ShowAddress = "panelVisibilitySettings.showAddress";
    public const string PVS_ShowLinkedIn = "panelVisibilitySettings.showLinkedIn";
    public const string PVS_ShowGitHub = "panelVisibilitySettings.showGitHub";

    public const string NotificationSettings = "notificationSettings";
    public const string NS_EmailNotificationEnabled = "notificationSettings.emailNotificationEnabled";

    public const string FeedbackConfiguration = "feedbackConfiguration";
    public const string FC_OverallRatingRequired = "feedbackConfiguration.overallRatingRequired";
    public const string FC_TechnicalSkillRequired = "feedbackConfiguration.technicalSkillRequired";
    public const string FC_CommunicationRequired = "feedbackConfiguration.communicationRequired";
    public const string FC_ProblemSolvingRequired = "feedbackConfiguration.problemSolvingRequired";
    public const string FC_RecommendationRequired = "feedbackConfiguration.recommendationRequired";
    public const string FC_OverallFeedbackRequired = "feedbackConfiguration.overallFeedbackRequired";

    public const string HrConfiguration = "hrConfiguration";
    public const string HC_AllowBulkUpload = "hrConfiguration.allowBulkUpload";
    public const string HC_CanEditSubmittedFeedback = "hrConfiguration.canEditSubmittedFeedback";
    public const string HC_AllowPanelReassign = "hrConfiguration.allowPanelReassign";
    public const string HC_RequireApprovalForReassignment = "hrConfiguration.requireApprovalForReassignment";

    public const string PanelConfiguration = "panelConfiguration";
    public const string PC_CanEditSubmittedFeedback = "panelConfiguration.canEditSubmittedFeedback";
    public const string PC_AllowPanelReassign = "panelConfiguration.allowPanelReassign";
    public const string PC_RequireApprovalForReassignment = "panelConfiguration.requireApprovalForReassignment";

    public const string MentorConfiguration = "mentorConfiguration";
    public const string MC_CanViewFeedback = "mentorConfiguration.canViewFeedback";
    public const string MC_AllowPanelReassign = "mentorConfiguration.allowPanelReassign";
    public const string MC_RequireApprovalForReassignment = "mentorConfiguration.requireApprovalForReassignment";
}

public static class AppSettingKey
{
    public const string DefaultConnection = "DefaultConnection";
    public const string JwtSettings = "JwtSettings";
    public const string AzureLogicApp = "AzureLogicApp";
}

public static class Key
{
    public const string UserId = "UserId";
    public const string Role = "Role";
}

public static class RoleName
{
    public const string Mentor = nameof(UserRole.Mentor);
    public const string Hr = nameof(UserRole.HR);
    public const string Panel = nameof(UserRole.Panel);
    public const string Admin = nameof(UserRole.Admin);
}

public static class UserAction
{
    public const string Drive = "Drive";
}

public static class ActionType
{
    public const string View = "View";
    public const string Add = "Add";
    public const string Update = "Update";
    public const string Delete = "Delete";
}

public static class Options
{
    public static int[] RatingNumbers => [1, 2, 3, 4, 5];
    public static string[] RoundResults => [
        nameof(RoundResult.Pending),
        nameof(RoundResult.Selected),
        nameof(RoundResult.Rejected)
    ];
    public static string[] RoundStatuses => [
        nameof(RoundStatus.OnProcess),
        nameof(RoundStatus.Scheduled),
        nameof(RoundStatus.Completed),
        nameof(RoundStatus.Skipped),
    ];
    public static string[] CandidateStatuses => [
        nameof(CandidateStatus.Pending),
        nameof(CandidateStatus.Selected),
        nameof(CandidateStatus.Rejected)
    ];
    public static string[] DriveStatuses => [
        nameof(DriveStatus.InProposal),
        nameof(DriveStatus.Started),
        nameof(DriveStatus.Halted),
        nameof(DriveStatus.Completed),
        nameof(DriveStatus.Cancelled)
    ];
    public static string[] Recommendations => [
        nameof(Recommendation.NA),
        nameof(Recommendation.NoHire),
        nameof(Recommendation.Maybe),
        nameof(Recommendation.Hire)
    ];
    public static string[] ExperienceLevels => [
        nameof(CandidateExperienceLevel.Fresher),
        nameof(CandidateExperienceLevel.Intermediate),
        nameof(CandidateExperienceLevel.Experienced)
    ];
    public static string[] RoleNames => [
        nameof(UserRole.Mentor),
        nameof(UserRole.HR),
        nameof(UserRole.Panel),
        nameof(UserRole.Admin)
    ];
}

public static class Otp
{
    public const string Prefix = "OTP_";
}

public static class EmailSubject
{
    public const string ForgotPasswordOTP = "Forgot Password OTP";
    public const string NewUserWelcome = "Welcome to HireHub – Your Account Is Ready";
    public const string EmailChangedNotification = "HireHub Email Address Changed";
}

public static class EmailBody
{
    public const string ForgotPasswordOTP =
@"Hello {0},

We received a request to reset your password.

Your One-Time Password (OTP) is: {1}

This OTP is valid for 5 minutes.
Please do not share this OTP with anyone.

If you did not request a password reset, please ignore this email.

Regards,
Your Application Team";

    public const string NewUserWelcome =
@"Hello {0},

You have been added to the HireHub system.

Your account has been successfully created with the following credentials:

Username: {1}
Password: {2}

You can access HireHub using the link below:
https://localhost:5000/login

For security reasons, we strongly recommend that you change your password after your first login.

If you believe this account was created in error, please contact the system administrator.

Regards,
HireHub Team";

    public const string EmailChangedNotification =
@"Hello {0},

This is to inform you that the email address associated with your HireHub account has been changed successfully.

Previous Email Address:
{1}

New Email Address:
{2}

If you made this change, no further action is required.

If you did NOT make this change, please contact our support team immediately or reset your password to secure your account.

Regards,
HireHub Team";


}