using eCommerceDomain.Entities;
using eCommerceInfarsructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceInfarsructure.AppDbContext
{
    public class ECommerceDbContext : IdentityDbContext<ApplicationUser>
    {

        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> dbContextOptions) : base(dbContextOptions) { }



        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Product>()
                  .HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Cart>()
                .HasOne<ApplicationUser>()
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);



            builder.Entity<CartItem>()
                .HasOne(c => c.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(c => c.CartId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany(p => p.cartItems)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
                
                
                
                

            


        }


    }
}
