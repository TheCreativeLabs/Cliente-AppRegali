namespace AppRegaliApi.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=DbConnectionData")
        {
        }

        public virtual DbSet<Evento> Eventoes { get; set; }
        public virtual DbSet<EventoCategoria> EventoCategorias { get; set; }
        public virtual DbSet<ImmagineEvento> ImmagineEventoes { get; set; } 
        public virtual DbSet<ImmagineRegalo> ImmagineRegaloes { get; set; }
        public virtual DbSet<ImmagineUser> ImmagineUsers { get; set; }
        public virtual DbSet<Regalo> Regaloes { get; set; }
        public virtual DbSet<RegaloUserPartecipazione> RegaloUserPartecipaziones { get; set; }
        public virtual DbSet<UserAmicizia> UserAmicizias { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>()
                .HasMany(e => e.ImmagineEventoes)
                .WithRequired(e => e.Evento)
                .HasForeignKey(e => e.IdEvento)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EventoCategoria>()
                .HasMany(e => e.Eventoes)
                .WithRequired(e => e.EventoCategoria)
                .HasForeignKey(e => e.IdCategoriaEvento)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ImmagineEvento>()
                .HasMany(e => e.Eventoes)
                .WithOptional(e => e.ImmagineEvento)
                .HasForeignKey(e => e.IdImmagineEvento);

            modelBuilder.Entity<ImmagineRegalo>()
                .HasMany(e => e.Regaloes)
                .WithOptional(e => e.ImmagineRegalo)
                .HasForeignKey(e => e.IdImmagineRegalo);

            modelBuilder.Entity<ImmagineRegalo>()
                .HasMany(e => e.Regaloes1)
                .WithOptional(e => e.ImmagineRegalo1)
                .HasForeignKey(e => e.IdImmagineRegalo);

            modelBuilder.Entity<Regalo>()
                .HasMany(e => e.ImmagineRegaloes)
                .WithRequired(e => e.Regalo)
                .HasForeignKey(e => e.IdRegalo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Regalo>()
                .HasMany(e => e.RegaloUserPartecipaziones)
                .WithRequired(e => e.Regalo)
                .HasForeignKey(e => e.IdRegalo)
                .WillCascadeOnDelete(false);
        }
    }
}
