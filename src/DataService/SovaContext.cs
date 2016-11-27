using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DomainModels;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DataService
{
    public class SovaContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RelatedPost> RelatedPosts { get; set; }
        public DbSet<CloudTag> CloudTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CloudTag>().HasKey(t => new {t.Word, t.Count});
       
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Insert db connection information here!");
            base.OnConfiguring(optionsBuilder);
        }
        
    }
}
