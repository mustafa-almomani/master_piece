using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace startupCompany.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ClientMeeting> ClientMeetings { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<CustomerRequest> CustomerRequests { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeePerformanceReport> EmployeePerformanceReports { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<ProjectManagement> ProjectManagements { get; set; }

    public virtual DbSet<ScheduledMeeting> ScheduledMeetings { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }

    public virtual DbSet<TeamMeeting> TeamMeetings { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ORANGE;Database=startupcompany;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientMeeting>(entity =>
        {
            entity.HasKey(e => e.MeetingId).HasName("PK__ClientMe__E9F9E94CB6821805");

            entity.Property(e => e.ClientEmail).HasMaxLength(255);
            entity.Property(e => e.ClientName).HasMaxLength(255);
            entity.Property(e => e.ClientPhone).HasMaxLength(20);
            entity.Property(e => e.MeetingDateTime).HasColumnType("datetime");
            entity.Property(e => e.MeetingLink).HasMaxLength(255);
            entity.Property(e => e.MeetingPlatform).HasMaxLength(255);
            entity.Property(e => e.Subjectmeet).HasColumnName("subjectmeet");

            entity.HasOne(d => d.Admin).WithMany(p => p.ClientMeetings)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__ClientMee__Admin__49C3F6B7");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__ContactM__C87C0C9C5BA9254A");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.MessageDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<CustomerRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Customer__33A8517AA951DE2B");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.MeetingDate)
                .HasColumnType("datetime")
                .HasColumnName("meetingDate");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Service).WithMany(p => p.CustomerRequests)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Services_CustomerRequests");

            entity.HasOne(d => d.User).WithMany(p => p.CustomerRequests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__CustomerR__UserI__3C69FB99");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F114548A9FD");

            entity.HasIndex(e => e.Email, "UQ__Employee__A9D1053496BFE6F1").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.JobTitle).HasMaxLength(255);
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
        });

        modelBuilder.Entity<EmployeePerformanceReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Employee__D5BD4805B3230C1C");

            entity.Property(e => e.Completedtask).HasColumnName("completedtask");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.ReportDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePerformanceReports)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeP__Emplo__45F365D3");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC07473FBA1C");

            entity.ToTable("Payment");

            entity.Property(e => e.AmountPaid)
                .HasMaxLength(255)
                .HasDefaultValue("0");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payment__UserId__05D8E0BE");
        });

        modelBuilder.Entity<ProjectManagement>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__ProjectM__761ABEF07934E3A7");

            entity.ToTable("ProjectManagement");

            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectStatus).HasMaxLength(50);

            entity.HasOne(d => d.Admin).WithMany(p => p.ProjectManagements)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__ProjectMa__Admin__5629CD9C");

            entity.HasOne(d => d.AdminNavigation).WithMany(p => p.ProjectManagements)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProjectManagement_Users");

            entity.HasOne(d => d.Request).WithMany(p => p.ProjectManagements)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_ProjectManagement_RequestId");
        });

        modelBuilder.Entity<ScheduledMeeting>(entity =>
        {
            entity.HasKey(e => e.ScheduledMeetingId).HasName("PK__Schedule__0443B3175DEF6181");

            entity.Property(e => e.MeetingDateTime).HasColumnType("datetime");
            entity.Property(e => e.MeetingType).HasMaxLength(255);
            entity.Property(e => e.Platform).HasMaxLength(255);

            entity.HasOne(d => d.Admin).WithMany(p => p.ScheduledMeetings)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__Scheduled__Admin__4F7CD00D");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00A58377DC0");

            entity.Property(e => e.ServiceName).HasMaxLength(255);
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TaskAssi__7C6949B1285E02BC");

            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.TaskStatus).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__TaskAssig__Emplo__5AEE82B9");

            entity.HasOne(d => d.Project).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__TaskAssig__Proje__59FA5E80");
        });

        modelBuilder.Entity<TeamMeeting>(entity =>
        {
            entity.HasKey(e => e.MeetingId).HasName("PK__TeamMeet__E9F9E94CA0616997");

            entity.Property(e => e.Linkmeeting).HasColumnName("linkmeeting");
            entity.Property(e => e.MeetingDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.TeamMeetings)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__TeamMeeti__Emplo__4CA06362");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Testimon__3213E83FB5FD7151");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Firstname).HasMaxLength(255);
            entity.Property(e => e.Isaccepted).HasColumnName("isaccepted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(255)
                .HasColumnName("lastname");
            entity.Property(e => e.TheTestimonials)
                .HasMaxLength(255)
                .HasColumnName("theTestimonials");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Testimoni__UserI__02FC7413");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC7827D28");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D67C5A90").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Passwordsult).HasColumnName("passwordsult");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
