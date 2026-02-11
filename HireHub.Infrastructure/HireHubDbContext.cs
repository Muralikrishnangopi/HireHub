using HireHub.Core.Data.Models;
using HireHub.Core.Utils.Common;
using Microsoft.EntityFrameworkCore;

namespace HireHub.Infrastructure;

public class HireHubDbContext : DbContext
{
    public HireHubDbContext(DbContextOptions<HireHubDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Drive> Drives => Set<Drive>();
    public DbSet<DriveMember> DriveMembers => Set<DriveMember>();
    public DbSet<DriveRoleConfiguration> DriveRoleConfigurations => Set<DriveRoleConfiguration>();
    public DbSet<PanelVisibilitySettings> PanelVisibilitySettings => Set<PanelVisibilitySettings>();
    public DbSet<NotificationSettings> NotificationSettings => Set<NotificationSettings>();
    public DbSet<FeedbackConfiguration> FeedbackConfigurations => Set<FeedbackConfiguration>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<DriveCandidate> DriveCandidates => Set<DriveCandidate>();
    public DbSet<Round> Rounds => Set<Round>();
    public DbSet<CandidateReassignment> CandidateReassignments => Set<CandidateReassignment>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Availability> Availabilities => Set<Availability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.UserId);

            b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.FullName)
            .HasColumnName("full_name")
            .HasColumnType("VARCHAR(100)")
            .HasMaxLength(100)
            .IsRequired();

            b.Property(x => x.Email)
            .HasColumnName("email")
            .HasColumnType("VARCHAR(150)")
            .HasMaxLength(150)
            .IsRequired();

            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasColumnType("VARCHAR(32)")
            .HasMaxLength(32)
            .IsRequired();

            b.HasIndex(x => x.Phone).IsUnique();

            b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

            b.Property(x => x.UpdatedDate)
            .HasColumnName("updated_date")
            .HasColumnType("DATETIME")
            .IsRequired(false);

            b.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("VARCHAR(MAX)")
            .IsRequired(false);

            b.HasOne(x => x.Role).WithMany(x => x.Users)
            .HasPrincipalKey(x => x.RoleId).HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // Roles
        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("roles");
            b.HasKey(x => x.RoleId);

            b.Property(x => x.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.RoleName)
            .HasColumnName("role_name")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasConversion(Helper.EnumConverter<UserRole>())
            .IsRequired();

            b.HasIndex(x => x.RoleName).IsUnique();
        });

        // UserPermissions
        modelBuilder.Entity<UserPermission>(b =>
        {
            b.ToTable("user_permissions");
            b.HasKey(x => new { x.UserId, x.Action });

            b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.Action)
            .HasColumnName("action")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .IsRequired();

            b.Property(x => x.View)
            .HasColumnName("view")
            .HasColumnType("BIT")
            .IsRequired();

            b.Property(x => x.Add)
            .HasColumnName("add")
            .HasColumnType("BIT")
            .IsRequired();

            b.Property(x => x.Update)
            .HasColumnName("update")
            .HasColumnType("BIT")
            .IsRequired();

            b.Property(x => x.Delete)
            .HasColumnName("delete")
            .HasColumnType("BIT")
            .IsRequired();

            b.HasOne(x => x.User).WithMany(x => x.UserPermissions)
            .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // Requests
        modelBuilder.Entity<Request>(b =>
        {
            b.ToTable("requests");
            b.HasKey(x => x.RequestId);

            b.Property(x => x.RequestId)
            .HasColumnName("request_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.RequestType)
            .HasColumnName("request_type")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasConversion(Helper.EnumConverter<RequestType>())
            .IsRequired();

            b.Property(x => x.SubType)
             .HasColumnName("sub_type")
             .HasColumnType("VARCHAR(30)")
             .HasMaxLength(30)
             .HasConversion(Helper.EnumConverter<RequestSubType>())
             .IsRequired();

            b.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(RequestStatus.Pending)
            .HasConversion(Helper.EnumConverter<RequestStatus>())
            .IsRequired();

            b.Property(x => x.ApprovedBy)
            .HasColumnName("approved_by")
            .HasColumnType("INT")
            .IsRequired(false);

            b.Property(x => x.ApprovedDate)
            .HasColumnName("approved_date")
            .HasColumnType("DATETIME")
            .IsRequired(false);

            b.Property(x => x.RequestedBy)
            .HasColumnName("requested_by")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.RequestedDate)
            .HasColumnName("requested_date")
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

            b.HasOne(x => x.Approver).WithMany(e => e.ApprovedRequests)
            .HasPrincipalKey(e => e.UserId).HasForeignKey(x => x.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.Requester).WithMany(e => e.RaisedRequests)
            .HasPrincipalKey(e => e.UserId).HasForeignKey(x => x.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // Drives
        modelBuilder.Entity<Drive>(b =>
        {
            b.ToTable("drives");
            b.HasKey(x => x.DriveId);

            b.Property(x => x.DriveId)
            .HasColumnName("drive_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveName)
            .HasColumnName("drive_name")
            .HasColumnType("VARCHAR(200)")
            .HasMaxLength(200)
            .IsRequired();

            b.HasIndex(x => x.DriveName).IsUnique();

            b.Property(x => x.DriveDate)
            .HasColumnName("drive_date")
            .HasColumnType("DATETIME")
            .IsRequired();

            b.Property(x => x.TechnicalRounds)
            .HasColumnName("technical_rounds")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(DriveStatus.InProposal)
            .HasConversion(Helper.EnumConverter<DriveStatus>())
            .IsRequired();

            b.Property(x => x.CreatedBy)
            .HasColumnName("created_by")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

            b.HasOne(x => x.Creator).WithMany(x => x.CreatedDrives)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // DriveTeam
        modelBuilder.Entity<DriveMember>(b =>
        {
            b.ToTable("drive_members");
            b.HasKey(x => x.DriveMemberId);

            b.Property(x => x.DriveMemberId)
            .HasColumnName("drive_member_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveId)
            .HasColumnName("drive_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasIndex(x => new { x.DriveId, x.UserId }).IsUnique().HasDatabaseName("UQ_DriveId_UserId");

            b.Property(x => x.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasOne(x => x.Drive).WithMany(x => x.DriveMembers)
            .HasPrincipalKey(x => x.DriveId).HasForeignKey(x => x.DriveId)
            .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.User).WithMany(x => x.DriveMembers)
            .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Role).WithMany(x => x.DriveMembers)
            .HasPrincipalKey(x => x.RoleId).HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // DriveRoleConfiguration
        modelBuilder.Entity<DriveRoleConfiguration>(b =>
        {
            b.ToTable("drive_role_configuration");
            b.HasKey(x => x.ConfigId);

            b.Property(x => x.ConfigId)
            .HasColumnName("config_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveId)
            .HasColumnName("drive_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasIndex(x => new { x.DriveId, x.RoleId }).IsUnique().HasDatabaseName("UQ_DriveId_RoleId");

            b.Property(x => x.AllowPanelReassign)
            .HasColumnName("allow_panel_reassign")
            .HasColumnType("BIT")
            .HasDefaultValue(false)
            .IsRequired();

            b.Property(x => x.CanViewFeedback)
            .HasColumnName("can_view_feedback")
            .HasColumnType("BIT")
            .HasDefaultValue(false)
            .IsRequired();

            b.Property(x => x.AllowBulkUpload)
            .HasColumnName("allow_bulk_upload")
            .HasColumnType("BIT")
            .HasDefaultValue(false)
            .IsRequired();

            b.Property(x => x.CanEditSubmittedFeedback)
            .HasColumnName("can_edit_submitted_feedback")
            .HasColumnType("BIT")
            .HasDefaultValue(false)
            .IsRequired();

            b.Property(x => x.RequireApprovalForReassignment)
            .HasColumnName("require_approval_for_reassignment")
            .HasColumnType("BIT")
            .HasDefaultValue(false)
            .IsRequired();

            b.HasOne(x => x.Drive).WithMany(x => x.DriveRoleConfigurations)
            .HasPrincipalKey(x => x.DriveId).HasForeignKey(x => x.DriveId)
            .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Role).WithMany(x => x.DriveRoleConfigurations)
            .HasPrincipalKey(x => x.RoleId).HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // PanelVisibilitySettings
        modelBuilder.Entity<PanelVisibilitySettings>(b =>
        {
            b.ToTable("panel_visibility_settings");
            b.HasKey(x => x.VisibilityId);

            b.Property(x => x.VisibilityId)
            .HasColumnName("visibility_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveId)
            .HasColumnName("drive_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasIndex(x => x.DriveId).IsUnique();

            b.Property(x => x.ShowPhone)
            .HasColumnName("show_phone")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ShowEmail)
            .HasColumnName("show_email")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ShowPreviousCompany)
            .HasColumnName("show_previous_company")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ShowResume)
            .HasColumnName("show_resume")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ShowCollege)
            .HasColumnName("show_college")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ShowAddress)
            .HasColumnName("show_address")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ShowLinkedIn)
            .HasColumnName("show_linkedin")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ShowGitHub)
            .HasColumnName("show_github")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.HasOne(x => x.Drive).WithOne(x => x.PanelVisibilitySettings)
            .HasPrincipalKey<Drive>(x => x.DriveId)
            .HasForeignKey<PanelVisibilitySettings>(x => x.DriveId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // NotificationSettings
        modelBuilder.Entity<NotificationSettings>(b =>
        {
            b.ToTable("notification_settings");
            b.HasKey(x => x.NotificationId);

            b.Property(x => x.NotificationId)
            .HasColumnName("notification_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveId)
            .HasColumnName("drive_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasIndex(x => x.DriveId).IsUnique();

            b.Property(x => x.EmailNotificationEnabled)
            .HasColumnName("email_notification_enabled")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.HasOne(x => x.Drive).WithOne(x => x.NotificationSettings)
            .HasPrincipalKey<Drive>(x => x.DriveId).HasForeignKey<NotificationSettings>(x => x.DriveId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // FeedbackConfiguration
        modelBuilder.Entity<FeedbackConfiguration>(b =>
        {
            b.ToTable("feedback_configuration");
            b.HasKey(x => x.FeedbackConfigId);

            b.Property(x => x.FeedbackConfigId)
            .HasColumnName("feedback_config_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveId)
            .HasColumnName("drive_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasIndex(x => x.DriveId).IsUnique();

            b.Property(x => x.OverallRatingRequired)
            .HasColumnName("overall_rating_required")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.TechnicalSkillRequired)
            .HasColumnName("technical_skill_required")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.CommunicationRequired)
            .HasColumnName("communication_required")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.ProblemSolvingRequired)
            .HasColumnName("problem_solving_required")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.RecommendationRequired)
            .HasColumnName("recommendation_required")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.Property(x => x.OverallFeedbackRequired)
            .HasColumnName("overall_feedback_required")
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

            b.HasOne(x => x.Drive).WithOne(x => x.FeedbackConfiguration)
            .HasPrincipalKey<Drive>(x => x.DriveId).HasForeignKey<FeedbackConfiguration>(x => x.DriveId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // Candidates
        modelBuilder.Entity<Candidate>(b =>
        {
            b.ToTable("Candidates");
            b.HasKey(x => x.CandidateId);

            b.Property(x => x.CandidateId)
            .HasColumnName("candidate_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.FullName)
            .HasColumnName("full_name")
            .HasColumnType("VARCHAR(100)")
            .HasMaxLength(100)
            .IsRequired();

            b.Property(x => x.Email)
            .HasColumnName("email")
            .HasColumnType("VARCHAR(150)")
            .HasMaxLength(150)
            .IsRequired();

            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasColumnType("VARCHAR(32)")
            .HasMaxLength(32)
            .IsRequired();

            b.HasIndex(x => x.Phone).IsUnique();

            b.Property(x => x.Address)
            .HasColumnName("address")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);

            b.Property(x => x.College)
            .HasColumnName("college")
            .HasColumnType("VARCHAR(200)")
            .HasMaxLength(200)
            .IsRequired(false);

            b.Property(x => x.PreviousCompany)
            .HasColumnName("previous_company")
            .HasColumnType("VARCHAR(200)")
            .HasMaxLength(200)
            .IsRequired(false);

            b.Property(x => x.ExperienceLevel)
            .HasColumnName("experience_level")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(CandidateExperienceLevel.Fresher)
            .HasConversion(Helper.EnumConverter<CandidateExperienceLevel>())
            .IsRequired();

            b.Property(x => x.TechStack)
            .HasColumnName("tech_stack")
            .HasColumnType("VARCHAR(MAX)")
            .HasConversion(Helper.ListConverter)
            .IsRequired(false);

            b.Property(x => x.ResumeUrl)
            .HasColumnName("resume_url")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);

            b.Property(x => x.LinkedInUrl)
            .HasColumnName("linkedin_url")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);

            b.Property(x => x.GitHubUrl)
            .HasColumnName("github_url")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);

            b.Property(x => x.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
        });

        // CandidateDrive
        modelBuilder.Entity<DriveCandidate>(b =>
        {
            b.ToTable("drive_candidates");
            b.HasKey(x => x.DriveCandidateId);

            b.Property(x => x.DriveCandidateId)
            .HasColumnName("drive_candidate_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.CandidateId)
            .HasColumnName("candidate_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveId)
            .HasColumnName("drive_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasIndex(x => new { x.CandidateId, x.DriveId }).IsUnique().HasDatabaseName("UQ_CandidateId_DriveId");

            b.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(CandidateStatus.Pending)
            .HasConversion(Helper.EnumConverter<CandidateStatus>())
            .IsRequired();

            b.Property(x => x.StatusSetBy)
            .HasColumnName("status_set_by")
            .HasColumnType("INT")
            .IsRequired(false);

            b.Property(x => x.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

            b.HasOne(x => x.Candidate).WithMany(x => x.DriveCandidates)
            .HasPrincipalKey(x => x.CandidateId).HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Drive).WithMany(x => x.DriveCandidates)
            .HasPrincipalKey(x => x.DriveId).HasForeignKey(x => x.DriveId)
            .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Recruiter).WithMany(x => x.RecruitedCandidates)
            .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.StatusSetBy)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // Rounds
        modelBuilder.Entity<Round>(b =>
        {
            b.ToTable("Rounds");
            b.HasKey(x => x.RoundId);

            b.Property(x => x.RoundId)
            .HasColumnName("round_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveCandidateId)
            .HasColumnName("drive_candidate_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.InterviewerId)
            .HasColumnName("interviewer_id")
            .HasColumnType("INT")
            .IsRequired();

            b.HasIndex(x => new { x.DriveCandidateId, x.InterviewerId }).IsUnique().HasDatabaseName("UQ_DriveCandidateId_PanelId");

            b.Property(x => x.RoundType)
            .HasColumnName("round_type")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(RoundType.Tech1)
            .HasConversion(Helper.EnumConverter<RoundType>())
            .IsRequired();

            b.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(RoundStatus.Scheduled)
            .HasConversion(Helper.EnumConverter<RoundStatus>())
            .IsRequired();

            b.Property(x => x.Result)
            .HasColumnName("result")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(RoundResult.Pending)
            .HasConversion(Helper.EnumConverter<RoundResult>())
            .IsRequired();

            b.Property(x => x.FeedbackId)
            .HasColumnName("feedback_id")
            .HasColumnType("INT")
            .IsRequired(false);

            b.HasOne(x => x.DriveCandidate).WithMany(x => x.Rounds)
            .HasPrincipalKey(x => x.DriveCandidateId).HasForeignKey(x => x.DriveCandidateId)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Interviewer).WithMany(x => x.InterviewedPanels)
            .HasPrincipalKey(x => x.DriveMemberId).HasForeignKey(x => x.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Feedback).WithMany()
            .HasPrincipalKey(x => x.FeedbackId).HasForeignKey(x => x.FeedbackId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // CandidateReassignments
        modelBuilder.Entity<CandidateReassignment>(b =>
        {
            b.ToTable("candidate_reassignments");
            b.HasKey(x => x.ReassignId);

            b.Property(x => x.ReassignId)
            .HasColumnName("reassign_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.DriveCandidateId)
            .HasColumnName("drive_candidate_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.PreviousUserId)
            .HasColumnName("previous_user_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.NewUserId)
            .HasColumnName("new_user_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.RequireApproval)
            .HasColumnName("require_approval")
            .HasColumnType("BIT")
            .IsRequired();

            b.Property(x => x.ApprovedBy)
            .HasColumnName("approved_by")
            .HasColumnType("INT")
            .IsRequired(false);

            b.Property(x => x.ApprovedDate)
            .HasColumnName("approved_date")
            .HasColumnType("DATETIME")
            .IsRequired(false);

            b.Property(x => x.RequestedBy)
            .HasColumnName("requested_by")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.RequestedDate)
            .HasColumnName("requested_date")
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

            b.HasOne(x => x.DriveCandidate).WithMany(x => x.CandidateReassignments)
            .HasPrincipalKey(x => x.DriveCandidateId).HasForeignKey(x => x.DriveCandidateId)
            .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.PreviousUser).WithMany()
            .HasPrincipalKey(x => x.DriveMemberId).HasForeignKey(x => x.PreviousUserId)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.NewUser).WithMany()
            .HasPrincipalKey(x => x.DriveMemberId).HasForeignKey(x => x.NewUserId)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Requester).WithMany(x => x.RequestedReassignments)
            .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Approver).WithMany(x => x.ApprovedReassignments)
            .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // Feedbacks
        modelBuilder.Entity<Feedback>(b =>
        {
            b.ToTable("feedbacks");
            b.HasKey(x => x.FeedbackId);

            b.Property(x => x.FeedbackId)
            .HasColumnName("feedback_id")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.OverallRating)
            .HasColumnName("overall_rating")
            .HasColumnType("INT")
            .IsRequired();

            b.Property(x => x.TechnicalSkill)
            .HasColumnName("technical_skill")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);

            b.Property(x => x.Communication)
            .HasColumnName("communication")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);

            b.Property(x => x.ProblemSolving)
            .HasColumnName("problem_solving")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);

            b.Property(x => x.OverallFeedback)
            .HasColumnName("overall_feedback")
            .HasColumnType("TEXT")
            .IsRequired(false);

            b.Property(x => x.Recommendation)
            .HasColumnName("recommendation")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasDefaultValue(Recommendation.NA)
            .HasConversion(Helper.EnumConverter<Recommendation>())
            .IsRequired();

            b.Property(x => x.SubmittedDate)
            .HasColumnName("submitted_date")
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.ToTable("availability");
            entity.HasKey(e => e.AvailabilityId);

            entity.Property(x => x.AvailabilityId)
                .HasColumnName("availability_id")
                .HasColumnType("INT")
                .IsRequired();

            entity.Property(e => e.AvailabilityDate)
                  .HasColumnName("availability_date")
                  .HasColumnType("date")
                  .IsRequired();

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id")
                  .HasColumnType("INT")
                  .IsRequired();

            // FK relationship (if User table exists)
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Availabilities)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate availability per user per day
            entity.HasIndex(e => new { e.UserId, e.AvailabilityDate })
                  .IsUnique();
        });
    }
}
