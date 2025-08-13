using System.Data;
using System.Reflection;
using ApprovalSystem.Extensions;
using ApprovalSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, long, UserClaim,
        UserRole, UserLogin, RoleClaim, UserToken>
    {
        private readonly IHttpContextAccessor _contextAccessor;

#if DEBUG
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }
#else
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor contextAccessor) 
            : base(options)
        {
            _contextAccessor = contextAccessor;
        }
#endif

        #region Fluent Api Configuration

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // get all the models
            var definedTypes = GetApplicatonDefinedTypes();
            SetConcurrencyAndDeleteNulls(builder, definedTypes);

            builder.Entity<User>().ToTable("Approval.Users");
            builder.Entity<Role>().ToTable("Approval.Roles");
            builder.Entity<UserRole>().ToTable("Approval.UserRoles");
            builder.Entity<UserToken>().ToTable("Approval.UserTokens");
            builder.Entity<UserLogin>().ToTable("Approval.UserLogins");
            builder.Entity<UserClaim>().ToTable("Approval.UserClaims");
            builder.Entity<RoleClaim>().ToTable("Approval.RoleClaims");
        }

        #endregion

        /// <summary>
        /// Get all the entities inheriting from BaseModel 
        /// </summary>
        /// <returns>A <![CDATA[List<Type>]]></returns>
        private IEnumerable<Type> GetApplicatonDefinedTypes()
        {
            HashSet<Type> models =
            [
                ..Assembly.GetAssembly(GetType()).DefinedTypes.Where(t => t.IsAssignableTo(typeof(IModelBase<long>)) && !t.IsAbstract),
                ..Assembly.GetAssembly(GetType()).DefinedTypes.Where(t => t.IsAssignableTo(typeof(IModelBase<string>)) && !t.IsAbstract)
            ];

            return models;
        }


        #region DbContext Overrides

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        public override int SaveChanges()
        {
            UpdateAuditColumns();
            ValidateConcurrency();
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException($"An error occured saving the current changes. Another user might have updated the record " +
                        $"since you last retrieved it. Try retrieve the record and attempt the operation again.");
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditColumns();
            ValidateConcurrency();
            try
            {
                return base.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException($"An error occured saving the current changes. Another user might have updated the record " +
                        $"since you last retrieved it. Try retrieve the record and attempt the operation again.");
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditColumns();
            ValidateConcurrency();
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException($"An error occured saving the current changes. Another user might have updated the record " +
                        $"since you last retrieved it. Try retrieve the record and attempt the operation again.");
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateAuditColumns();
            ValidateConcurrency();
            try
            {
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException($"An error occured saving the current changes. Another user might have updated the record " +
                        $"since you last retrieved it. Try retrieve the record and attempt the operation again.");
            }
        }

        #endregion

        private void SetConcurrencyAndDeleteNulls(ModelBuilder builder, IEnumerable<Type> typesToRegister)
        {
            foreach (var item in typesToRegister)
            {
                Console.WriteLine($"Registering entity: {item.Name} with the database context.");
                builder.Entity(item).Property(nameof(BaseModel.ConcurrencyStamp)).IsConcurrencyToken(true);
                

                builder.Entity(item)
                    .HasOne(nameof(BaseModel.CreatedBy))
                    .WithOne()
                    .OnDelete(DeleteBehavior.NoAction);

                builder.Entity(item)
                    .HasOne(nameof(BaseModel.UpdatedBy))
                    .WithOne()
                    .OnDelete(DeleteBehavior.NoAction);

                if(item.IsAssignableTo(typeof(IApprovableEntity<long>)) || item.IsAssignableTo(typeof(IApprovableEntity<string>)))
                {
                    builder.Entity(item)
                    .ToTable(t => t.HasCheckConstraint($"CK_{item.Name}_ApprovalRequired",
                    $"({nameof(ApprovalBaseModel.ResourceState)} = '${(int)ResourceState.Active}' AND {nameof(ApprovalBaseModel.ApprovalHashId)} IS NOT NULL) " +
                    $"OR ({nameof(ApprovalBaseModel.ResourceState)} <> '${(int)ResourceState.Active}')"));

                    builder.Entity(item)
                        .HasOne(typeof(ApprovalHash))
                        .WithOne()
                        .HasForeignKey(item.Name, nameof(ApprovalBaseModel.ApprovalHashId))
                        .OnDelete(DeleteBehavior.NoAction);
                }
            }
        }

        private void UpdateAuditColumns()
        {
            User user = null;
            if (_contextAccessor?.HttpContext != null && _contextAccessor.HttpContext.User != null && _contextAccessor.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                user = _contextAccessor.GetCurrentUser().GetAwaiter().GetResult();
            }

            var entries = ChangeTracker.Entries().Where(n => n.State == EntityState.Added || n.State == EntityState.Modified);
            foreach (var item in entries)
            {
                var lastPropInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.LastUpdated));
                var lastPropUser = item.Entity.GetType().GetProperty(nameof(BaseModel.UpdatedById));

                lastPropInfo?.SetValue(item.Entity, new DateTimeOffset(new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
                lastPropUser?.SetValue(item.Entity, user?.Id);

                if (item.State == EntityState.Added)
                {
                    // enforce that every new entity of type IApprovableEntity is created with a PendingApproval state
                    // even though the creator might have set it different.
                    // if the property is not found probably its not an IApprovableEntity
                    var rsState = item.Entity.GetType().GetProperty(nameof(ApprovalBaseModel.ResourceState));
                    rsState?.SetValue(item.Entity, ResourceState.Modified);

                    var newPropUser = item.Entity.GetType().GetProperty(nameof(BaseModel.CreatedById));
                    var newPropInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.DateCreated));
                    var idPropInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.Id));
                    //var concurrencyInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.ConcurrencyStamp));

                    newPropInfo?.SetValue(item.Entity, new DateTimeOffset(new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
                    //concurrencyInfo?.SetValue(item.Entity, Guid.NewGuid().ToString("N").ToUpper());
                    newPropUser?.SetValue(item.Entity, user?.Id);
                }
                // should we let the approver create the ApprovalHash for it or be automatically handled here
                //else
                //{
                //    var state = item.Entity.GetType().GetProperty(nameof(ApprovalBaseModel.ResourceState));
                //    if (state != null)
                //    {
                //        var newState = (ResourceState)state.GetValue(item.Entity);
                //        if (newState == ResourceState.Active)
                //        {
                //            // create a hash for the entity anytime its value has been modified and it is approved
                //            ApprovalHash hash = new ()
                //            {
                //                CreatedById = user == null? default : user!.Id,
                //                DateCreated = new DateTimeOffset(new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))
                //            };

                //            var hashProp = item.Entity.GetType().GetProperty(nameof(ApprovalBaseModel.ApprovalHashId));
                //            hashProp?.SetValue(item.Entity, hash.Id);
                //        }
                //    }
                //}
            }
        }

        private void ValidateConcurrency()
        {
            //var entries = ChangeTracker.Entries().Where(n => n.State == EntityState.Modified);
            //foreach (var item in entries)
            //{

            //    var prop = item.Entity.GetType().GetProperty(nameof(BaseModel.ConcurrencyStamp));
            //    var oldCurrency = prop?.GetValue(item.OriginalValues);
            //    var transitCurrency = prop?.GetValue(item.Entity);
            //    if (oldCurrency == null || transitCurrency == null || oldCurrency.ToString() != transitCurrency.ToString())
            //    {
            //        throw new DbUpdateConcurrencyException($"The record you are trying to update has been modified by another user. " +
            //            $"Please retrieve the record and try the operation again.");
            //    }

            //    // generate a new concurrency stamp for the entity
            //    prop?.SetValue(item.Entity, $"{Guid.NewGuid().ToString("N").ToUpper()}");
            //}
        }
    }
}
