using GestaoLoja3D.Dominio.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoLoja3D.Dominio.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Filamento> Filamentos { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MaterialUtilizado> MateriaisUtilizados { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "server=localhost;port=3306;database=gestaoloja3d_db;user=root;password=root;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}
