﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using trimlink.data;

#nullable disable

namespace trimlink.data.Migrations
{
    [DbContext(typeof(TrimLinkDbContext))]
    [Migration("20230428195635_AddLink")]
    partial class AddLink
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("trimlink.data.Models.Link", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsMarkedForDeletion")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNeverExpires")
                        .HasColumnType("bit");

                    b.Property<string>("RedirectToUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UtcDateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UtcDateExpires")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "UtcDateExpires" }, "IX_Links_UtcDateExpires");

                    b.HasIndex(new[] { "Token" }, "UX_Links_Token")
                        .IsUnique();

                    b.ToTable("Links");
                });
#pragma warning restore 612, 618
        }
    }
}
