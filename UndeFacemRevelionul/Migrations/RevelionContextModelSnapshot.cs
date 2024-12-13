﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UndeFacemRevelionul.ContextModels;

#nullable disable

namespace UndeFacemRevelionul.Migrations
{
    [DbContext(typeof(RevelionContext))]
    partial class RevelionContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("UndeFacemRevelionul.Models.FoodMenuModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MenuFilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<int>("ProviderId")
                        .HasColumnType("int");

                    b.Property<float>("Rating")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("FoodMenus");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.LocationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<int>("ProviderId")
                        .HasColumnType("int");

                    b.Property<float>("Rating")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartierModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<int>("RankId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Partiers");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartyModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FoodMenuId")
                        .HasColumnType("int");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("RemainingBudget")
                        .HasColumnType("real");

                    b.Property<float>("TotalBudget")
                        .HasColumnType("real");

                    b.Property<int>("TotalPoints")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FoodMenuId");

                    b.HasIndex("LocationId");

                    b.ToTable("Parties");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartyPartierModel", b =>
                {
                    b.Property<int>("PartyId")
                        .HasColumnType("int");

                    b.Property<int>("PartierId")
                        .HasColumnType("int");

                    b.HasKey("PartyId", "PartierId");

                    b.HasIndex("PartierId");

                    b.ToTable("PartyPartierModel");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PlaylistModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PartyId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PartyId")
                        .IsUnique();

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PlaylistSongModel", b =>
                {
                    b.Property<int>("PlaylistId")
                        .HasColumnType("int");

                    b.Property<int>("SongId")
                        .HasColumnType("int");

                    b.Property<int>("PartierId")
                        .HasColumnType("int");

                    b.HasKey("PlaylistId", "SongId", "PartierId");

                    b.HasIndex("PartierId");

                    b.HasIndex("SongId");

                    b.ToTable("PlaylistSongs");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.ProviderModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Contact")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Rating")
                        .HasColumnType("real");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.RankModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("MaxPoints")
                        .HasColumnType("int");

                    b.Property<int>("MinPoints")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ranks");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.SongModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Artist")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SongPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.SuperstitionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PartierId")
                        .HasColumnType("int");

                    b.Property<int?>("PartierModelId")
                        .HasColumnType("int");

                    b.Property<int>("PartyId")
                        .HasColumnType("int");

                    b.Property<int?>("PartyModelId")
                        .HasColumnType("int");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PartierId");

                    b.HasIndex("PartierModelId");

                    b.HasIndex("PartyId");

                    b.HasIndex("PartyModelId");

                    b.ToTable("Superstitions");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.TaskModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PartierId")
                        .HasColumnType("int");

                    b.Property<int?>("PartierModelId")
                        .HasColumnType("int");

                    b.Property<int>("PartyId")
                        .HasColumnType("int");

                    b.Property<int?>("PartyModelId")
                        .HasColumnType("int");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PartierId");

                    b.HasIndex("PartierModelId");

                    b.HasIndex("PartyId");

                    b.HasIndex("PartyModelId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicturePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserRole")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.FoodMenuModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.ProviderModel", "Provider")
                        .WithMany("FoodMenus")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.LocationModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.ProviderModel", "Provider")
                        .WithMany("Locations")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartierModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.UserModel", "User")
                        .WithMany("Partiers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartyModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.FoodMenuModel", "FoodMenu")
                        .WithMany("Parties")
                        .HasForeignKey("FoodMenuId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("UndeFacemRevelionul.Models.LocationModel", "Location")
                        .WithMany("Parties")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("FoodMenu");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartyPartierModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.PartierModel", "Partier")
                        .WithMany("PartyUsers")
                        .HasForeignKey("PartierId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UndeFacemRevelionul.Models.PartyModel", "Party")
                        .WithMany("PartyUsers")
                        .HasForeignKey("PartyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Partier");

                    b.Navigation("Party");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PlaylistModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.PartyModel", "Party")
                        .WithOne("Playlist")
                        .HasForeignKey("UndeFacemRevelionul.Models.PlaylistModel", "PartyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Party");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PlaylistSongModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.PartierModel", "Partier")
                        .WithMany("PlaylistSongs")
                        .HasForeignKey("PartierId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UndeFacemRevelionul.Models.PlaylistModel", "Playlist")
                        .WithMany("PlaylistSongs")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UndeFacemRevelionul.Models.SongModel", "Song")
                        .WithMany("PlaylistSongs")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Partier");

                    b.Navigation("Playlist");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.ProviderModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.UserModel", "User")
                        .WithMany("Providers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.SuperstitionModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.PartierModel", "Partier")
                        .WithMany()
                        .HasForeignKey("PartierId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UndeFacemRevelionul.Models.PartierModel", null)
                        .WithMany("Superstitions")
                        .HasForeignKey("PartierModelId");

                    b.HasOne("UndeFacemRevelionul.Models.PartyModel", "Party")
                        .WithMany()
                        .HasForeignKey("PartyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UndeFacemRevelionul.Models.PartyModel", null)
                        .WithMany("Superstitions")
                        .HasForeignKey("PartyModelId");

                    b.Navigation("Partier");

                    b.Navigation("Party");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.TaskModel", b =>
                {
                    b.HasOne("UndeFacemRevelionul.Models.PartierModel", "Partier")
                        .WithMany()
                        .HasForeignKey("PartierId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UndeFacemRevelionul.Models.PartierModel", null)
                        .WithMany("Tasks")
                        .HasForeignKey("PartierModelId");

                    b.HasOne("UndeFacemRevelionul.Models.PartyModel", "Party")
                        .WithMany()
                        .HasForeignKey("PartyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UndeFacemRevelionul.Models.PartyModel", null)
                        .WithMany("Tasks")
                        .HasForeignKey("PartyModelId");

                    b.Navigation("Partier");

                    b.Navigation("Party");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.FoodMenuModel", b =>
                {
                    b.Navigation("Parties");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.LocationModel", b =>
                {
                    b.Navigation("Parties");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartierModel", b =>
                {
                    b.Navigation("PartyUsers");

                    b.Navigation("PlaylistSongs");

                    b.Navigation("Superstitions");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PartyModel", b =>
                {
                    b.Navigation("PartyUsers");

                    b.Navigation("Playlist")
                        .IsRequired();

                    b.Navigation("Superstitions");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.PlaylistModel", b =>
                {
                    b.Navigation("PlaylistSongs");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.ProviderModel", b =>
                {
                    b.Navigation("FoodMenus");

                    b.Navigation("Locations");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.SongModel", b =>
                {
                    b.Navigation("PlaylistSongs");
                });

            modelBuilder.Entity("UndeFacemRevelionul.Models.UserModel", b =>
                {
                    b.Navigation("Partiers");

                    b.Navigation("Providers");
                });
#pragma warning restore 612, 618
        }
    }
}
