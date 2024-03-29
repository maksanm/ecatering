﻿// <auto-generated />
using System;
using ECaterer.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ECaterer.Core.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220612213945_AddOrdersDietsRelation")]
    partial class AddOrdersDietsRelation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DietOrder", b =>
                {
                    b.Property<string>("DietsDietId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OrdersOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("DietsDietId", "OrdersOrderId");

                    b.HasIndex("OrdersOrderId");

                    b.ToTable("DietOrder");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Address", b =>
                {
                    b.Property<string>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ApartmentNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("BuildingNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PostCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("AddressId");

                    b.ToTable("Address");

                    b.HasData(
                        new
                        {
                            AddressId = "f83c4738-ba84-4617-b0e3-6743ce3a39b0",
                            ApartmentNumber = "228",
                            BuildingNumber = "12",
                            City = "Warszawa",
                            PostCode = "00-631",
                            Street = "Waryńskiego"
                        });
                });

            modelBuilder.Entity("ECaterer.Core.Models.Allergent", b =>
                {
                    b.Property<string>("AllergentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MealId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("AllergentId");

                    b.HasIndex("MealId");

                    b.ToTable("Allergent");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Client", b =>
                {
                    b.Property<string>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AddressId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("ClientId");

                    b.HasIndex("AddressId");

                    b.ToTable("Client");

                    b.HasData(
                        new
                        {
                            ClientId = "ffa4ebd8-39fb-46bd-a7bd-c2466c53030c",
                            AddressId = "f83c4738-ba84-4617-b0e3-6743ce3a39b0",
                            Email = "klient@klient.pl",
                            LastName = "Kowalski",
                            Name = "Jan",
                            PhoneNumber = "+48-322-228-444"
                        });
                });

            modelBuilder.Entity("ECaterer.Core.Models.Complaint", b =>
                {
                    b.Property<string>("ComplaintId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Answer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("ComplaintId");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Complaint");
                });

            modelBuilder.Entity("ECaterer.Core.Models.DeliveryDetails", b =>
                {
                    b.Property<string>("DeliveryDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AddressId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CommentForDeliverer")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("DeliveryDetailsId");

                    b.HasIndex("AddressId");

                    b.ToTable("DeliveryDetails");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Diet", b =>
                {
                    b.Property<string>("DietId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Calories")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Vegan")
                        .HasColumnType("bit");

                    b.HasKey("DietId");

                    b.ToTable("Diet");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Ingredient", b =>
                {
                    b.Property<string>("IngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MealId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IngredientId");

                    b.HasIndex("MealId");

                    b.ToTable("Ingredient");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Meal", b =>
                {
                    b.Property<string>("MealId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Calories")
                        .HasColumnType("int");

                    b.Property<string>("DietId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Vegan")
                        .HasColumnType("bit");

                    b.HasKey("MealId");

                    b.HasIndex("DietId");

                    b.ToTable("Meal");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Order", b =>
                {
                    b.Property<string>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DeliveryDetailsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("ClientId");

                    b.HasIndex("DeliveryDetailsId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("ECaterer.Core.Models.OrderDiet", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DietId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("OrderId", "DietId");

                    b.HasIndex("DietId");

                    b.ToTable("OrdersDiets");
                });

            modelBuilder.Entity("DietOrder", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Diet", null)
                        .WithMany()
                        .HasForeignKey("DietsDietId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECaterer.Core.Models.Order", null)
                        .WithMany()
                        .HasForeignKey("OrdersOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ECaterer.Core.Models.Allergent", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Meal", "Meal")
                        .WithMany("AllergentList")
                        .HasForeignKey("MealId");

                    b.Navigation("Meal");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Client", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Complaint", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Order", null)
                        .WithOne("Complaint")
                        .HasForeignKey("ECaterer.Core.Models.Complaint", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ECaterer.Core.Models.DeliveryDetails", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.Navigation("Address");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Ingredient", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Meal", "Meal")
                        .WithMany("IngredientList")
                        .HasForeignKey("MealId");

                    b.Navigation("Meal");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Meal", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Diet", "Diet")
                        .WithMany("Meals")
                        .HasForeignKey("DietId");

                    b.Navigation("Diet");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Order", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Client", "Client")
                        .WithMany("Orders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECaterer.Core.Models.DeliveryDetails", "DeliveryDetails")
                        .WithMany()
                        .HasForeignKey("DeliveryDetailsId");

                    b.Navigation("Client");

                    b.Navigation("DeliveryDetails");
                });

            modelBuilder.Entity("ECaterer.Core.Models.OrderDiet", b =>
                {
                    b.HasOne("ECaterer.Core.Models.Diet", "Diet")
                        .WithMany()
                        .HasForeignKey("DietId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECaterer.Core.Models.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Diet");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Client", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Diet", b =>
                {
                    b.Navigation("Meals");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Meal", b =>
                {
                    b.Navigation("AllergentList");

                    b.Navigation("IngredientList");
                });

            modelBuilder.Entity("ECaterer.Core.Models.Order", b =>
                {
                    b.Navigation("Complaint");
                });
#pragma warning restore 612, 618
        }
    }
}
