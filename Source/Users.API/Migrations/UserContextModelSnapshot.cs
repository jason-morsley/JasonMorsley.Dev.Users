using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using Users.API.Entities;

namespace Users.API.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("User.API.Entities.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("FirstName")
                    .IsRequired();
                    //.HasAnnotation("MaxLength", 50);

                b.Property<string>("LastName")
                    .IsRequired();
                    //.HasAnnotation("MaxLength", 50);

                //b.Property<DateTimeOffset>("DateOfBirth");

                //b.HasKey("Id"); //Not used at the moment, used with more than 1 table
            });
        }
    }
}
