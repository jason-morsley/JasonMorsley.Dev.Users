using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using Users.API.Entities;

namespace Users.API.Migrations
{
    [DbContext(typeof(User))]
    partial class LibraryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Library.API.Entities.Author", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 50);

                //b.Property<DateTimeOffset>("DateOfBirth");

                //b.HasKey("Id"); //Not used at the moment
            });
        }
    }
}
