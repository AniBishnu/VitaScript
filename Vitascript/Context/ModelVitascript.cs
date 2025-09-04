namespace Vitascript.Context
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;
    using Vitascript.Models;

    public class ModelVitascript : DbContext
    {
        // Your context has been configured to use a 'ModelVitascript' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Vitascript.Context.ModelVitascript' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'ModelVitascript' 
        // connection string in the application configuration file.
        public ModelVitascript()
            : base("name=ModelVitascript")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<DoctorType> DoctorTypes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<GenericMedicine> GenericMedicines { get; set; }
        public DbSet<BrandedMedicine> BrandedMedicines { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescribedMedicine> PrescribedMedicines { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<PharmacyInventory> PharmacyInventories { get; set; }
        public DbSet<MedicineTransaction> MedicineTransactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Patient> Patients { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove plural table naming
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Auto-generate PrescriptionCode 
            modelBuilder.Entity<Prescription>()
                .Property(p => p.PrescriptionCode)
                .HasMaxLength(20)
                .IsRequired();

            // Handle optional FK relationships
            modelBuilder.Entity<MedicineTransaction>()
                .HasOptional(mt => mt.Payment)
                .WithMany(p => p.MedicineTransactions)
                .HasForeignKey(mt => mt.PaymentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MedicineTransaction>()
                .HasRequired(mt => mt.Patient)
                .WithMany(u => u.MedicineTransactions)
                .HasForeignKey(mt => mt.PatientId)
                .WillCascadeOnDelete(false);

            // Pharmacy Inventory relationship
            modelBuilder.Entity<PharmacyInventory>()
                .HasRequired(pi => pi.Pharmacy)
                .WithMany(p => p.Inventories)
                .HasForeignKey(pi => pi.PharmacyId);

            modelBuilder.Entity<PharmacyInventory>()
                .HasRequired(pi => pi.BrandedMedicine)
                .WithMany(bm => bm.PharmacyInventories)
                .HasForeignKey(pi => pi.BrandedMedicineId);

            // Required fields
            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<GenericMedicine>()
                .Property(g => g.Name)
                .IsRequired();

            modelBuilder.Entity<BrandedMedicine>()
                .Property(b => b.MedicineName)
                .IsRequired();

            modelBuilder.Entity<Prescription>()
                .HasRequired(p => p.Doctor)
                .WithMany(u => u.PrescriptionsGiven)
                .HasForeignKey(p => p.DoctorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prescription>()
                .HasRequired(p => p.Patient)
                .WithMany(u => u.PrescriptionsReceived)
                .HasForeignKey(p => p.PatientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patient>()
                .HasRequired(p => p.PatientUser)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patient>()
                .HasOptional(p => p.AssignedDoctor)
                .WithMany()
                .HasForeignKey(p => p.AssignedDoctorId)
                .WillCascadeOnDelete(false);
        }
    }


    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}