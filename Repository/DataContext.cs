﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;

namespace Shopping_Tutorial.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<ProductModel> Products { get; set; }

        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }

        public DbSet<OrderModel> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<ContactModel> Contact { get; set; }

        public DbSet<WishlistModel> Wishlists { get; set; }

        public DbSet<CompareModel> Compares { get; set; }

        public DbSet<ProductQuantityModel> ProductQuantities { get; set; }

        public DbSet<ProductWarrantyModel> ProductWarranties { get; set; }

        public DbSet<ShippingModel> Shippings { get; set; }

        public DbSet<CouponModel> Coupons { get; set; }

        public DbSet<StatisticalModel> Statisticals { get; set; }

        public DbSet<MomoInfoModel> MomoInfos { get; set; }

        public DbSet<VnpayModel> VnInfos { get; set; }

		public DbSet<ThongSoKyThuatModel> ThongSoKyThuats { get; set; }
		public DbSet<UsageNeedModel> UsageNeeds { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Thiết lập quan hệ giữa Product và UsageNeed qua bảng trung gian
			modelBuilder.Entity<ProductUsageNeed>()
				.HasKey(pu => new { pu.ProductId, pu.UsageNeedId });

			modelBuilder.Entity<ProductUsageNeed>()
				.HasOne(pu => pu.Product)
				.WithMany(p => p.ProductUsageNeeds)
				.HasForeignKey(pu => pu.ProductId);

			modelBuilder.Entity<ProductUsageNeed>()
				.HasOne(pu => pu.UsageNeed)
				.WithMany(u => u.ProductUsageNeeds)
				.HasForeignKey(pu => pu.UsageNeedId);
		}
	}
}
