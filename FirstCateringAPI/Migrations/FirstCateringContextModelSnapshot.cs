﻿// <auto-generated />
using System;
using FirstCateringAPI.Core.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FirstCateringAPI.Migrations
{
    [DbContext(typeof(FirstCateringContext))]
    partial class FirstCateringContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FirstCateringAPI.Core.Entities.Employee", b =>
                {
                    b.Property<int>("pkAutoId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("CardId");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("EmployeeId");

                    b.Property<string>("Forename")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("PINNumber")
                        .HasMaxLength(4);

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("pkAutoId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("FirstCateringAPI.Core.Entities.MembershipCard", b =>
                {
                    b.Property<int>("pkCardAutoId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("CardId");

                    b.Property<decimal>("CurrentBalance")
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("pkCardAutoId");

                    b.HasIndex("CardId")
                        .IsUnique();

                    b.ToTable("MembershipCards");
                });

            modelBuilder.Entity("FirstCateringAPI.Core.Entities.MembershipCard", b =>
                {
                    b.HasOne("FirstCateringAPI.Core.Entities.Employee", "Employee")
                        .WithOne("MembershipCard")
                        .HasForeignKey("FirstCateringAPI.Core.Entities.MembershipCard", "CardId")
                        .HasPrincipalKey("FirstCateringAPI.Core.Entities.Employee", "CardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
