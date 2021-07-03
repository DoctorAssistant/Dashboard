using API.DTOs.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API{
    public class DataContext : IdentityDbContext{
        public DataContext(DbContextOptions options): base(options){

        }
        public DataContext(){

        }
        public DbSet<API.DTOs.Model.Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientNote> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);
            builder.Entity<API.DTOs.Model.Doctor>().HasMany(x=> x.Patients);
            builder.Entity<Patient>().HasMany(x=> x.Notes);
        }
        
    }
}