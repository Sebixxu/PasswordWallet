﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PasswordWallet.Data;

namespace PasswordWallet.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20201208192134_Alter_DoNullable_ForSharedPasswordId_Column")]
    partial class Alter_DoNullable_ForSharedPasswordId_Column
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PasswordWallet.Data.DbModels.IpAttemptsDb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IdUser")
                        .HasColumnName("IdUser")
                        .HasColumnType("int");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnName("IpAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsStale")
                        .HasColumnName("IsStale")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LoginAttemptDate")
                        .HasColumnName("LoginAttemptDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("WasSuccess")
                        .HasColumnName("WasSuccess")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("IpAttempts");
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.LoginAttemptsDb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IdUser")
                        .HasColumnName("IdUser")
                        .HasColumnType("int");

                    b.Property<bool>("IsStale")
                        .HasColumnName("IsStale")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LoginAttemptDate")
                        .HasColumnName("LoginAttemptDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("WasSuccess")
                        .HasColumnName("WasSuccess")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("LoginAttempts");
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.PasswordDb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdUser")
                        .HasColumnName("IdUser")
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnName("Login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnName("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("WebAddress")
                        .HasColumnName("WebAddress")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("Passwords");
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.PendingPasswordSharesDb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DestinationUserId")
                        .HasColumnName("DestinationUserId")
                        .HasColumnType("int");

                    b.Property<bool>("IsStale")
                        .HasColumnName("IsStale")
                        .HasColumnType("bit");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnName("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("PasswordId")
                        .HasColumnName("PasswordId")
                        .HasColumnType("int");

                    b.Property<int?>("SharedPasswordId")
                        .HasColumnName("SharedPasswordId")
                        .HasColumnType("int");

                    b.Property<int>("SourceUserId")
                        .HasColumnName("SourceUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DestinationUserId");

                    b.HasIndex("PasswordId");

                    b.HasIndex("SourceUserId");

                    b.ToTable("PendingPasswordShares");
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.UserDb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsHMAC")
                        .HasColumnName("IsHMAC")
                        .HasColumnType("bit");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnName("Login")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnName("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnName("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.IpAttemptsDb", b =>
                {
                    b.HasOne("PasswordWallet.Data.DbModels.UserDb", "User")
                        .WithMany("IpAttempts")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.LoginAttemptsDb", b =>
                {
                    b.HasOne("PasswordWallet.Data.DbModels.UserDb", "User")
                        .WithMany("LoginAttempts")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.PasswordDb", b =>
                {
                    b.HasOne("PasswordWallet.Data.DbModels.UserDb", "User")
                        .WithMany("Passwords")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PasswordWallet.Data.DbModels.PendingPasswordSharesDb", b =>
                {
                    b.HasOne("PasswordWallet.Data.DbModels.UserDb", "DestinationUser")
                        .WithMany("DestinationPendingPasswordShares")
                        .HasForeignKey("DestinationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PasswordWallet.Data.DbModels.PasswordDb", "Password")
                        .WithMany("PendingPasswordShares")
                        .HasForeignKey("PasswordId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PasswordWallet.Data.DbModels.UserDb", "SourceUser")
                        .WithMany("SourcePendingPasswordShares")
                        .HasForeignKey("SourceUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
