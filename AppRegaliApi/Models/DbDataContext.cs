namespace AppRegaliApi.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbDataContext : DbContext
    {
        public DbDataContext()
            : base("name=DbConnectionData")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<Evento> Evento { get; set; }
        public virtual DbSet<EventoCategoria> EventoCategoria { get; set; }
        public virtual DbSet<ImmagineEvento> ImmagineEvento { get; set; }
        public virtual DbSet<ImmagineRegalo> ImmagineRegalo { get; set; }
        public virtual DbSet<Regalo> Regalo { get; set; }
        public virtual DbSet<RegaloUserPartecipazione> RegaloUserPartecipazione { get; set; }
        public virtual DbSet<UserAmicizia> UserAmicizia { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Regalo)
                .WithRequired(e => e.Evento)
                .HasForeignKey(e => e.IdEvento);

            modelBuilder.Entity<EventoCategoria>()
                .HasMany(e => e.Evento)
                .WithRequired(e => e.EventoCategoria)
                .HasForeignKey(e => e.IdCategoriaEvento)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ImmagineEvento>()
                .HasMany(e => e.Evento)
                .WithOptional(e => e.ImmagineEvento)
                .HasForeignKey(e => e.IdImmagineEvento);

            modelBuilder.Entity<ImmagineRegalo>()
                .HasMany(e => e.Regalo)
                .WithOptional(e => e.ImmagineRegalo)
                .HasForeignKey(e => e.IdImmagineRegalo);

            modelBuilder.Entity<Regalo>()
                .HasMany(e => e.RegaloUserPartecipazione)
                .WithRequired(e => e.Regalo)
                .HasForeignKey(e => e.IdRegalo)
                .WillCascadeOnDelete(false);
        }
    }
}
