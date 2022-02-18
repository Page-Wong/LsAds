using LsAdmin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace LsAdmin.EntityFrameworkCore
{
    public class LsAdminDbContext : DbContext
    {
        public LsAdminDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderEquipment> OrderEquipments { get; set; }
        public DbSet<OrderMaterial> OrderMaterials { get; set; }
        public DbSet<OrderPlace> OrderPlaces { get; set; }
        public DbSet<OrderTime> OrderTimes { get; set; }
        public DbSet<PlayPrice> PlayPrices { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<PlayHistory> PlayHistorys { get; set; }
		public DbSet<Androidapk> Androidapks { get; set; }
        public DbSet<EnterpriseConfirm> EnterpriseConfirm { get; set; }
        public DbSet<EquipmentManagement> EquipmentManagement { get; set; }
        public DbSet<PersonConfirm> PersonConfirm { get; set; }
        public DbSet<PlaceType> PlaceTypes { get; set; }
        public DbSet<ForwardHistory> ForwardHistorys { get; set; }
        public DbSet<PlaceMaterial> PlaceMaterials { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeBusinessType> TradeBusinessTypes { get; set; }
        public DbSet<AdministrativeArea> AdministrativeAreas { get; set; }
        public DbSet<Notify> Notifies { get; set; }
        public DbSet<NotifyType> NotifyTypes { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<EquipmentModel> EquipmentModel { get; set; }
        public DbSet<EquipmentApplication> EquipmentApplications { get; set; }
        public DbSet<RoleApply> RoleApplies { get; set; }
        public DbSet<EquipmentRepair> EquipmentRepair { get; set; }
        public DbSet<EquipmentReplace> EquipmentReplace { get; set; }
        public DbSet<OrderTrade> OrderTrades { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<EquipmentIncome> EquipmentIncome { get; set; }
        public DbSet<PlaceIncome> PlaceIncome { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<InstructionLog> InstructionLogs { get; set; }
        public DbSet<InstructionMethod> InstructionMethods { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<ProgramMaterial> ProgramMaterials { get; set; }
        public DbSet<PlayerProgram> PlayerPrograms { get; set; }
        public DbSet<OrderPlayer> OrderPlayers { get; set; }
        public DbSet<OrderPlayerProgram> OrderPlayerPrograms { get; set; }
        public DbSet<CollectionsBlacklists> CollectionsBlacklists { get; set; }
        public DbSet<ProgramReview> ProgramReviews { get; set; }
        public DbSet<EquipmentLogFile> EquipmentLogFiles { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            //UserRole关联配置
            builder.Entity<UserRole>()
              .HasKey(ur => new { ur.UserId, ur.RoleId });

            //RoleMenu关联配置
            builder.Entity<RoleMenu>()
              .HasKey(rm => new { rm.RoleId, rm.MenuId });

            builder.Entity<OrderTrade>()
              .HasKey(item => new { item.TradeId, item.OrderId });

            builder.Entity<Trade>().HasIndex(item => item.TradeNo).IsUnique();
            builder.Entity<Order>().HasIndex(item => item.OrderNo).IsUnique();
            builder.Entity<NotifyType>().HasIndex(item => item.Name).IsUnique();
            builder.Entity<TradeBusinessType>().HasIndex(item => item.Name).IsUnique();

            builder.Entity<ProgramMaterial>()
              .HasKey(it => new { it.ProgramId, it.MaterialId });
            builder.Entity<OrderPlayer>()
              .HasKey(it => new { it.OrderId, it.PlayerId });
            builder.Entity<OrderPlayerProgram>()
              .HasKey(it => new { it.OrderId, it.PlayerProgramId });

            builder.Entity<EquipmentLogFile>()
              .HasKey(it => new { it.EquipmentId, it.LogFileId });

            base.OnModelCreating(builder);
        }
    }
}

