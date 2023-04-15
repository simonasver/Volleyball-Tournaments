﻿// <auto-generated />
using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Backend.Auth.Model.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ProfilePictureUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("RefreshTokenExpiration")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("FinishDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("FirstTeamScore")
                        .HasColumnType("int");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastEditDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("MaxSets")
                        .HasColumnType("int");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("longtext");

                    b.Property<int>("PlayersPerTeam")
                        .HasColumnType("int");

                    b.Property<int>("PointDifferenceToWin")
                        .HasColumnType("int");

                    b.Property<int>("PointsToWin")
                        .HasColumnType("int");

                    b.Property<int>("SecondTeamScore")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("TournamentMatchId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("WinnerId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("TournamentMatchId")
                        .IsUnique();

                    b.HasIndex("WinnerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.GameTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<Guid?>("GameWhereFirstId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("GameWhereSecondId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("TournamentId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("GameWhereFirstId")
                        .IsUnique();

                    b.HasIndex("GameWhereSecondId")
                        .IsUnique();

                    b.HasIndex("TournamentId");

                    b.ToTable("GameTeams");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.GameTeamPlayer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GameTeamId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("GameTeamId");

                    b.ToTable("GameTeamPlayers");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.Set", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("FinishDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("FirstTeamId")
                        .HasColumnType("char(36)");

                    b.Property<int>("FirstTeamScore")
                        .HasColumnType("int");

                    b.Property<Guid>("GameId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<Guid>("SecondTeamId")
                        .HasColumnType("char(36)");

                    b.Property<int>("SecondTeamScore")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid?>("WinnerId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("FirstTeamId");

                    b.HasIndex("GameId");

                    b.HasIndex("SecondTeamId");

                    b.HasIndex("WinnerId");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.SetPlayer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<Guid>("SetId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Team")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("SetId");

                    b.ToTable("SetPlayers");
                });

            modelBuilder.Entity("Backend.Data.Entities.Team.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<Guid?>("GameId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("LastEditDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("TournamentId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("TournamentId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Backend.Data.Entities.Team.TeamPlayer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamPlayers");
                });

            modelBuilder.Entity("Backend.Data.Entities.Tournament.Tournament", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastEditDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("MaxSets")
                        .HasColumnType("int");

                    b.Property<int>("MaxTeams")
                        .HasColumnType("int");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("longtext");

                    b.Property<int>("PlayersPerTeam")
                        .HasColumnType("int");

                    b.Property<int>("PointDifferenceToWin")
                        .HasColumnType("int");

                    b.Property<int>("PointsToWin")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("Backend.Data.Entities.Tournament.TournamentMatch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("FirstParentId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Round")
                        .HasColumnType("int");

                    b.Property<Guid?>("SecondParentId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TournamentId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("FirstParentId");

                    b.HasIndex("SecondParentId");

                    b.HasIndex("TournamentId");

                    b.ToTable("TournamentMatches");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.Game", b =>
                {
                    b.HasOne("Backend.Auth.Model.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Data.Entities.Tournament.TournamentMatch", "TournamentMatch")
                        .WithOne("Game")
                        .HasForeignKey("Backend.Data.Entities.Game.Game", "TournamentMatchId")
                        .OnDelete(DeleteBehavior.ClientCascade);

                    b.HasOne("Backend.Data.Entities.Game.GameTeam", "Winner")
                        .WithMany()
                        .HasForeignKey("WinnerId");

                    b.Navigation("Owner");

                    b.Navigation("TournamentMatch");

                    b.Navigation("Winner");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.GameTeam", b =>
                {
                    b.HasOne("Backend.Data.Entities.Game.Game", "GameWhereFirst")
                        .WithOne("FirstTeam")
                        .HasForeignKey("Backend.Data.Entities.Game.GameTeam", "GameWhereFirstId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Backend.Data.Entities.Game.Game", "GameWhereSecond")
                        .WithOne("SecondTeam")
                        .HasForeignKey("Backend.Data.Entities.Game.GameTeam", "GameWhereSecondId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Backend.Data.Entities.Tournament.Tournament", "Tournament")
                        .WithMany("AcceptedTeams")
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.ClientCascade);

                    b.Navigation("GameWhereFirst");

                    b.Navigation("GameWhereSecond");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.GameTeamPlayer", b =>
                {
                    b.HasOne("Backend.Data.Entities.Game.GameTeam", "GameTeam")
                        .WithMany("Players")
                        .HasForeignKey("GameTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameTeam");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.Set", b =>
                {
                    b.HasOne("Backend.Data.Entities.Game.GameTeam", "FirstTeam")
                        .WithMany()
                        .HasForeignKey("FirstTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Data.Entities.Game.Game", "Game")
                        .WithMany("Sets")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("Backend.Data.Entities.Game.GameTeam", "SecondTeam")
                        .WithMany()
                        .HasForeignKey("SecondTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Data.Entities.Game.GameTeam", "Winner")
                        .WithMany()
                        .HasForeignKey("WinnerId");

                    b.Navigation("FirstTeam");

                    b.Navigation("Game");

                    b.Navigation("SecondTeam");

                    b.Navigation("Winner");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.SetPlayer", b =>
                {
                    b.HasOne("Backend.Data.Entities.Game.Set", "Set")
                        .WithMany("Players")
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Set");
                });

            modelBuilder.Entity("Backend.Data.Entities.Team.Team", b =>
                {
                    b.HasOne("Backend.Data.Entities.Game.Game", null)
                        .WithMany("RequestedTeams")
                        .HasForeignKey("GameId");

                    b.HasOne("Backend.Auth.Model.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Data.Entities.Tournament.Tournament", null)
                        .WithMany("RequestedTeams")
                        .HasForeignKey("TournamentId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Backend.Data.Entities.Team.TeamPlayer", b =>
                {
                    b.HasOne("Backend.Data.Entities.Team.Team", "Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Backend.Data.Entities.Tournament.Tournament", b =>
                {
                    b.HasOne("Backend.Auth.Model.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Backend.Data.Entities.Tournament.TournamentMatch", b =>
                {
                    b.HasOne("Backend.Data.Entities.Tournament.TournamentMatch", "FirstParent")
                        .WithMany()
                        .HasForeignKey("FirstParentId");

                    b.HasOne("Backend.Data.Entities.Tournament.TournamentMatch", "SecondParent")
                        .WithMany()
                        .HasForeignKey("SecondParentId");

                    b.HasOne("Backend.Data.Entities.Tournament.Tournament", "Tournament")
                        .WithMany("Matches")
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("FirstParent");

                    b.Navigation("SecondParent");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Backend.Auth.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Backend.Auth.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Auth.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Backend.Auth.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.Game", b =>
                {
                    b.Navigation("FirstTeam");

                    b.Navigation("RequestedTeams");

                    b.Navigation("SecondTeam");

                    b.Navigation("Sets");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.GameTeam", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Backend.Data.Entities.Game.Set", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Backend.Data.Entities.Team.Team", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Backend.Data.Entities.Tournament.Tournament", b =>
                {
                    b.Navigation("AcceptedTeams");

                    b.Navigation("Matches");

                    b.Navigation("RequestedTeams");
                });

            modelBuilder.Entity("Backend.Data.Entities.Tournament.TournamentMatch", b =>
                {
                    b.Navigation("Game")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
