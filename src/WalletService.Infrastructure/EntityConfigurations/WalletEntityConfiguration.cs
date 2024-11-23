using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Domain.Models;

namespace WalletService.Infrastructure.EntityConfigurations;

internal class WalletEntityConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable(nameof(Wallet)).HasIndex(w=> w.Owner);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
        builder.HasKey(p => p.Id);

        builder.Property(p => p.WalletName)
            .HasField("_walletName")
            .IsRequired();

        builder.Property(p => p.WalletType)
            .HasField("_walletType")
            .IsRequired();

        builder.Property(p => p.AccountScheme)
            .HasField("_accountScheme")
            .IsRequired();

        builder.Property(p => p.AccountNumber)
            .HasField("_accountNumber")
            .IsRequired();

        builder.Property(p => p.Owner)
            .HasField("_walletOwner")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasField("_createdAt")
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasField("_isActive")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasField("_updatedAt");
    }
}
