using Kapowey.Core.Entities;
using Kapowey.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Kapowey.Core.Persistance
{
    public partial class KapoweyContext : IdentityDbContext<User, UserRole, int, UserClaim, UserUserRole, UserLogin, UserRoleClaim, UserToken>
    {
        static KapoweyContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<IssueGrade>("e_issue_grade", new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Rating>("e_rating", new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Status>("e_status", new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
        }

        public KapoweyContext()
        {
        }

        public KapoweyContext(DbContextOptions<KapoweyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiApplication> ApiApplication { get; set; }
        public virtual DbSet<Collection> Collection { get; set; }
        public virtual DbSet<CollectionIssue> CollectionIssue { get; set; }
        public virtual DbSet<CollectionIssueGradeTerm> CollectionIssueGradeTerm { get; set; }
        public virtual DbSet<Franchise> Franchise { get; set; }
        public virtual DbSet<FranchiseCategory> FranchiseCategory { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Grade> Grade { get; set; }
        public virtual DbSet<GradeTerm> GradeTerm { get; set; }
        public virtual DbSet<Issue> Issue { get; set; }
        public virtual DbSet<IssueType> IssueType { get; set; }
        public virtual DbSet<Publisher> Publisher { get; set; }
        public virtual DbSet<PublisherCategory> PublisherCategory { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<SeriesCategory> SeriesCategory { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserClaim> UserClaim { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UserRoleClaim> UserRoleClaim { get; set; }
        public virtual DbSet<UserToken> UserToken { get; set; }
        public virtual DbSet<UserUserRole> UserUserRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum(null, "e_issue_grade", new[] { "Not Specified", "NM - Near Mint", "VF - Very Fine", "FN - Fine", "FG - Very Good", "GD - Good", "FR - Fair", "PR - Poor" })
                .HasPostgresEnum(null, "e_rating", new[] { "Not Specified", "Poor", "Fair", "Good", "Very Good", "Excellent" })
                .HasPostgresEnum(null, "e_status", new[] { "New", "Imported", "Ok", "Edited", "Pending Review", "Under Review", "Locked", "Inactive" })
                .HasPostgresExtension("uuid-ossp");
            
            modelBuilder.Entity<ApiApplication>(entity =>
            {
                entity.HasIndex(e => e.ApiKey)
                    .HasDatabaseName("api_api_key_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("api_application_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("api_application_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("api_application_tags")
                    .HasMethod("gin");

                entity.Property(e => e.ApiApplicationId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.ApiApplicationCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("api_application_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.ApiApplicationModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("api_application_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.ApiApplicationReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("api_application_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.HasIndex(e => e.ApiKey)
                    .HasDatabaseName("collection_api_key_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("collection_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("collection_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("collection_tags")
                    .HasMethod("gin");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("collection_user_id_idx")
                    .IsUnique();

                entity.Property(e => e.CollectionId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsPublic).HasDefaultValueSql("false");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Collection)
                    .HasForeignKey<Collection>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("collection_user_id_fkey");
            });

            modelBuilder.Entity<CollectionIssue>(entity =>
            {
                entity.HasIndex(e => e.ApiKey)
                    .HasDatabaseName("collection_issue_api_key_idx")
                    .IsUnique();

                entity.HasIndex(e => e.CollectionId)
                    .HasDatabaseName("collection_issue_collection_id_idx")
                    .IsUnique();

                entity.HasIndex(e => e.IssueId)
                    .HasDatabaseName("collection_issue_issue_id_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("collection_issue_tags")
                    .HasMethod("gin");

                entity.Property(e => e.CollectionIssueId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.HasRead).HasDefaultValueSql("false");

                entity.Property(e => e.IsDigital).HasDefaultValueSql("false");

                entity.Property(e => e.IsForSale).HasDefaultValueSql("false");

                entity.Property(e => e.IsPublic).HasDefaultValueSql("false");

                entity.Property(e => e.IsWanted).HasDefaultValueSql("false");

                entity.HasOne(d => d.Collection)
                    .WithOne(p => p.CollectionIssue)
                    .HasForeignKey<CollectionIssue>(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("collection_issue_collection_id_fkey");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.CollectionIssue)
                    .HasForeignKey(d => d.GradeId)
                    .HasConstraintName("collection_issue_grade_id_fkey");

                entity.HasOne(d => d.Issue)
                    .WithOne(p => p.CollectionIssue)
                    .HasForeignKey<CollectionIssue>(d => d.IssueId)
                    .HasConstraintName("collection_issue_issue_id_fkey");
            });

            modelBuilder.Entity<CollectionIssueGradeTerm>(entity =>
            {
                entity.HasKey(e => new { e.CollectionIssueId, e.GradeTermId })
                    .HasName("pk_collection_issue_grade_term");

                entity.HasIndex(e => new { e.CollectionIssueId, e.GradeTermId })
                    .HasDatabaseName("collection_issue_grade_term_idx")
                    .IsUnique();

                entity.HasOne(d => d.CollectionIssue)
                    .WithMany(p => p.CollectionIssueGradeTerm)
                    .HasForeignKey(d => d.CollectionIssueId)
                    .HasConstraintName("collection_issue_grade_term_collection_issue_id_fkey");

                entity.HasOne(d => d.GradeTerm)
                    .WithMany(p => p.CollectionIssueGradeTerm)
                    .HasForeignKey(d => d.GradeTermId)
                    .HasConstraintName("collection_issue_grade_term_grade_term_id_fkey");
            });

            modelBuilder.Entity<Franchise>(entity =>
            {
                entity.HasIndex(e => e.ApiKey)
                    .HasDatabaseName("franchise_api_key_idx")
                    .IsUnique();

                entity.HasIndex(e => e.FranchiseCategoryId)
                    .HasDatabaseName("franchise_franchise_category_idx");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("franchise_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ParentFranchiseId)
                    .HasDatabaseName("franchise_parent_franchise_idx");

                entity.HasIndex(e => e.PublisherId)
                    .HasDatabaseName("franchise_publisher_idx");

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("franchise_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("franchise_tags")
                    .HasMethod("gin");

                entity.Property(e => e.FranchiseId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.FranchiseCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("franchise_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.FranchiseModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("franchise_modified_user_id_fkey");

                entity.HasOne(d => d.Publisher)
                    .WithMany(p => p.Franchise)
                    .HasForeignKey(d => d.PublisherId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("franchise_publisher_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.FranchiseReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("franchise_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<FranchiseCategory>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("franchise_category_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ParentFranchiseCategoryId)
                    .HasDatabaseName("franchise_category_parent_franchise_category_idx");

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("franchise_category_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("franchise_category_tags")
                    .HasMethod("gin");

                entity.Property(e => e.FranchiseCategoryId).UseIdentityAlwaysColumn();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.FranchiseCategoryCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("franchise_category_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.FranchiseCategoryModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("franchise_category_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.FranchiseCategoryReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("franchise_category_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("genre_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ParentGenreId)
                    .HasDatabaseName("genre_parent_genre_idx");

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("genre_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("genre_tags")
                    .HasMethod("gin");

                entity.Property(e => e.GenreId).UseIdentityAlwaysColumn();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.GenreCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("genre_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.GenreModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("genre_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.GenreReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("genre_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasIndex(e => e.Abbreviation)
                    .HasDatabaseName("grade_abbreviation_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("grade_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Scale)
                    .HasDatabaseName("grade_scale_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("grade_tags")
                    .HasMethod("gin");

                entity.Property(e => e.GradeId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsBasicGrade).HasDefaultValueSql("false");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.GradeCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("grade_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.GradeModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("grade_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.GradeReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("grade_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<GradeTerm>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("grade_term_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("grade_term_ide_tags")
                    .HasMethod("gin");

                entity.Property(e => e.GradeTermId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.GradeTermCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("grade_term_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.GradeTermModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("grade_term_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.GradeTermReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("grade_term_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.HasIndex(e => e.ApiKey)
                    .HasDatabaseName("issue_api_key_idx")
                    .IsUnique();

                entity.HasIndex(e => e.IssueTypeId)
                    .HasDatabaseName("issue_IssueType_idx");

                entity.HasIndex(e => e.SeriesId)
                    .HasDatabaseName("issue_series_idx");

                entity.HasIndex(e => e.ShortTitle)
                    .HasDatabaseName("issue_short_title_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("issue_tags")
                    .HasMethod("gin");

                entity.HasIndex(e => e.Title)
                    .HasDatabaseName("issue_title_idx")
                    .IsUnique();

                entity.Property(e => e.IssueId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CultureCode).HasDefaultValueSql("'en'::character varying");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.IssueCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("issue_created_user_id_fkey");

                entity.HasOne(d => d.IssueType)
                    .WithMany(p => p.Issue)
                    .HasForeignKey(d => d.IssueTypeId)
                    .HasConstraintName("issue_IssueType_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.IssueModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("issue_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.IssueReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("issue_reviewed_user_id_fkey");

                entity.HasOne(d => d.Series)
                    .WithMany(p => p.Issue)
                    .HasForeignKey(d => d.SeriesId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("issue_series_id_fkey");
            });

            modelBuilder.Entity<IssueType>(entity =>
            {
                entity.HasIndex(e => e.Abbreviation)
                    .HasDatabaseName("IssueType_abbreviation_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IssueType_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("IssueType_tags")
                    .HasMethod("gin");

                entity.Property(e => e.IssueTypeId).UseIdentityAlwaysColumn();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.IssueTypeCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("IssueType_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.IssueTypeModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("IssueType_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.IssueTypeReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("IssueType_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasIndex(e => e.ApiKey)
                    .HasDatabaseName("publisher_api_key_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("publisher_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ParentPublisherId)
                    .HasDatabaseName("publisher_parent_publisher_idx");

                entity.HasIndex(e => e.PublisherCategoryId)
                    .HasDatabaseName("publisher_publisher_category_idx");

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("publisher_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("publisher_tags")
                    .HasMethod("gin");

                entity.Property(e => e.PublisherId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CountryCode).HasDefaultValueSql("'USA'::character varying");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.PublisherCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("publisher_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.PublisherModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("publisher_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.PublisherReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("publisher_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<PublisherCategory>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("publisher_category_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ParentPublisherCategoryId)
                    .HasDatabaseName("pc_parent_pc_idx");

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("publisher_category_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("publisher_category_tags")
                    .HasMethod("gin");

                entity.Property(e => e.PublisherCategoryId).UseIdentityAlwaysColumn();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.PublisherCategoryCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("publisher_category_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.PublisherCategoryModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("publisher_category_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.PublisherCategoryReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("publisher_category_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<Series>(entity =>
            {
                entity.HasIndex(e => e.ApiKey)
                    .HasDatabaseName("series_api_key_idx")
                    .IsUnique();

                entity.HasIndex(e => e.FranchiseId)
                    .HasDatabaseName("series_franchise_idx");

                entity.HasIndex(e => e.GenreId)
                    .HasDatabaseName("series_genre_idx");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("series_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.SeriesCategoryId)
                    .HasDatabaseName("series_series_category_idx");

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("series_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("series_tags")
                    .HasMethod("gin");

                entity.Property(e => e.SeriesId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CultureCode).HasDefaultValueSql("'en'::character varying");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.SeriesCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("series_created_user_id_fkey");

                entity.HasOne(d => d.Franchise)
                    .WithMany(p => p.Series)
                    .HasForeignKey(d => d.FranchiseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("series_franchise_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.SeriesModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("series_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.SeriesReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("series_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<SeriesCategory>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("series_category_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ParentSeriesCategoryId)
                    .HasDatabaseName("series_parent_series_category_idx");

                entity.HasIndex(e => e.ShortName)
                    .HasDatabaseName("series_category_short_name_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Tags)
                    .HasDatabaseName("series_category_tags")
                    .HasMethod("gin");

                entity.Property(e => e.SeriesCategoryId).UseIdentityAlwaysColumn();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.SeriesCategoryCreatedUser)
                    .HasForeignKey(d => d.CreatedUserId)
                    .HasConstraintName("series_category_created_user_id_fkey");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.SeriesCategoryModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("series_category_modified_user_id_fkey");

                entity.HasOne(d => d.ReviewedUser)
                    .WithMany(p => p.SeriesCategoryReviewedUser)
                    .HasForeignKey(d => d.ReviewedUserId)
                    .HasConstraintName("series_category_reviewed_user_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("user_id").UseIdentityAlwaysColumn();

                // A concurrency token for use with the optimistic concurrency checking
                entity.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                entity.Property(e => e.ApiKey).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EmailConfirmed).HasDefaultValueSql("false");

                entity.Property(e => e.IsPublic).HasDefaultValueSql("false");

                entity.Property(e => e.LockoutEnabled).HasDefaultValueSql("false");

                entity.Property(e => e.PhoneNumberConfirmed).HasDefaultValueSql("false");

                entity.Property(e => e.TwoFactorEnabled).HasDefaultValueSql("false");

                entity.HasOne(d => d.ModifiedUser)
                    .WithMany(p => p.InverseModifiedUser)
                    .HasForeignKey(d => d.ModifiedUserId)
                    .HasConstraintName("user_modified_user_id_fkey");

                // Each User can have many UserClaims
                entity.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

                // Each User can have many UserClaims
                entity.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                //// Each User can have many UserLogins
                //entity.HasMany<UserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

                //// Each User can have many UserTokens
                //entity.HasMany<UserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                //// Each User can have many entries in the UserRole join table
                //entity.HasMany<UserUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();

            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.ToTable("user_login");

                entity.Property(e => e.UserId).HasColumnName("user_id");
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_login_user_id_fkey");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_role");

                entity.Property(e => e.Id).HasColumnName("user_role_id");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
                
                // A concurrency token for use with the optimistic concurrency checking
                entity.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                // Each Role can have many entries in the UserRole join table
                entity.HasMany<UserUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

                // Each Role can have many associated RoleClaims
                entity.HasMany<UserRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();

                // Each User Role can have many UserClaims
                entity.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.RoleId)
                    .IsRequired();

            });

            modelBuilder.Entity<UserRoleClaim>(entity =>
            {
                entity.ToTable("user_role_claim");

                entity.Property(x => x.Id).HasColumnName("user_role_claim_id").UseIdentityAlwaysColumn();
                entity.Property(x => x.RoleId).HasColumnName("user_role_id");
                entity.Property(x => x.ClaimType).HasColumnName("claim_type");
                entity.Property(x => x.ClaimValue).HasColumnName("claim_value");
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("user_token");

                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("pk_user_tokens");

                entity.Property(e => e.UserId).HasColumnName("user_id");                
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserToken)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_token_user_id_fkey");
            });
            
            modelBuilder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("user_claim");

                entity.HasKey(x => x.Id);
                
                entity.Property(x => x.Id).HasColumnName("user_claim_id").UseIdentityAlwaysColumn();
                entity.Property(x => x.ClaimType).HasColumnName("claim_type");
                entity.Property(x => x.ClaimValue).HasColumnName("claim_value");
                
                entity.Property(e => e.UserId).HasColumnName("user_id");                
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Claims)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_claim_user_id_fkey");  
            });            

            modelBuilder.Entity<UserUserRole>(entity =>
            {
                entity.ToTable("user_user_role");

                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("pk_user_roles");

                entity.Property(x => x.RoleId).HasColumnName("user_role_id");
                entity.Property(x => x.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserUserRole)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_user_role_user_id_fkey");

                entity.HasOne(d => d.UserRole)
                    .WithMany(p => p.UserUserRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("user_user_role_user_role_id_fkey");
            });
            
            modelBuilder.Entity<UserRoleClaim>(entity =>
            {
                entity.ToTable("user_role_claim");

                entity.Property(x => x.Id).HasColumnName("user_role_claim_id").UseIdentityAlwaysColumn();
                entity.Property(x => x.ClaimType).HasColumnName("claim_type");
                entity.Property(x => x.ClaimValue).HasColumnName("claim_value");
                
                entity.Property(e => e.RoleId).HasColumnName("user_role_id");                
                
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Claims)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("user_role_claim_user_role_id_fkey");  
            });            

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}