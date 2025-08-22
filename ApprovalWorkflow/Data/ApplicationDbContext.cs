using System.Collections;
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
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// A list of ids of items of type IApprovableEntity. These ids are used to ignore
        /// cloning modified IApprovalbleEntity(ies) the we want to actually persist the change
        /// on the actual item
        /// </summary>
        //public ArrayList ShadowIds { get; set; } = new(10);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider)
            : base(options)
        {
            _serviceProvider = serviceProvider;
        }


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
                    var c = $"({nameof(ApprovalBaseModel.ApprovalStatus)} = '{(int)ApprovalStatus.New}' AND ({nameof(ApprovalBaseModel.ApprovalHashId)} IS NULL OR {nameof(ApprovalBaseModel.ApprovalHashId)} = '')) OR " +
                        $"({nameof(ApprovalBaseModel.ApprovalStatus)} <> '{(int)ApprovalStatus.New}' AND ({nameof(ApprovalBaseModel.ApprovalHashId)} IS NOT NULL AND {nameof(ApprovalBaseModel.ApprovalHashId)} <> ''))";

                    builder.Entity(item)
                    .ToTable(t => t.HasCheckConstraint($"CK_{item.Name}_ApprovalRequired", c));

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
            IHttpContextAccessor contextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            User user = null;
            if (contextAccessor?.HttpContext != null && contextAccessor.HttpContext.User != null && contextAccessor.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                user = contextAccessor.GetCurrentUser().GetAwaiter().GetResult();
            }

            var entries = ChangeTracker.Entries().Where(n => n.State == EntityState.Added || n.State == EntityState.Modified);
            foreach (var item in entries)
            {
                if (item.State == EntityState.Modified)
                {
                    var aprStatProp = item.Entity.GetType().GetProperty(nameof(ApprovalBaseModel.ApprovalStatus));
                    // if not null then the entity is an approvable entity
                    // also check if the entity is changing from one state to another other than New
                    // as we need to associate every change with a new hash Id
                    if (aprStatProp != null && (int)aprStatProp.GetValue(item.Entity) != (int)ApprovalStatus.New)
                    {
                        var currValue = aprStatProp.GetValue(item.Entity);
                        // if the old status is not same as the new status, ensure it has a hasdId

                        var hashIdInfo = item.Entity.GetType().GetProperty(nameof(ApprovalBaseModel.ApprovalHashId));
                        var idVal = hashIdInfo.GetValue(item.Entity);

                        var hash = Set<ApprovalHash>().FirstOrDefault(n => n.Id == idVal.ToString());
                        if (hash == null)
                        {
                            throw new InvalidOperationException($"Unable to persist the entity: " +
                                $"{item.Entity.GetType().Name} to backing store because it contains no " +
                                $"approval hash Id");
                        }

                        // if the status changed then ensure the hasdId was created not more than 1 min ago or less
                        if ((int)currValue != (int)item.OriginalValues.GetValue<ApprovalStatus>(nameof(ApprovalBaseModel.ApprovalStatus)))
                        {
                            if (hash.DateCreated > DateTime.Now.Subtract(TimeSpan.FromMinutes(1)))
                            {
                                throw new InvalidOperationException($"Unable to persist the entity: " +
                                    $"{item.Entity.GetType().Name} to backing store because the approval " +
                                    $"hash Id is issued older than expected.");
                            }
                        }
                    }

                    /**
                     * Goal:
                     * If an entity changed then, we should determine if its an IApprovableEntity
                     * If it is then it needs to go for approval again. We do that by cloning the 
                     * item, creating a new one and sending the new one for approval
                     * 
                     * if we don't do this here, it means every consumer who wants to modify an entity
                     * will have to create the shadow manually. (This could be useful for complex cases)
                     * 
                     * Algorithm
                     * - clone the entity and create new one
                     * - set the shadowId to the id of the old one
                     * - detach the old one from the context
                     * - 
                     * - Issues
                     * - 1. what if the target was to change the original because its approved?
                     * - 2. How do we know the ApprovalType for this item so we start the approval process
                     * 
                     * - Solution
                     * - if the id is in shadowIds list above, ignore cloning
                     * - For this to work, the user needs to call IRepository.Update even though
                     * - the entity is being tracked
                     * - for the new one which was created as a clone to be reviewed,
                     * - the user can delete it by themselves
                     * 
                     * 
                     * Currently we haven't found a way to know the approval type for the entity from reflection
                     * so we would let consumers create shadows and start approval manually
                    */
                    //var oIdInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.Id));
                    //if(!ShadowIds.Contains(oIdInfo?.GetValue(item.Entity)))
                    //{
                    //    item.State = EntityState.Detached;
                    //    var newEntity = item.Entity;

                    //    var shadowInfo = newEntity.GetType().GetProperty(nameof(ApprovalBaseModel.ShadowId));
                    //    shadowInfo?.SetValue(newEntity, oIdInfo?.GetValue(item.Entity));

                    //    var idInfo = newEntity.GetType().GetProperty(nameof(ApprovalBaseModel.Id));
                    //    idInfo?.SetValue(newEntity, 0);

                    //    var statusInfo = newEntity.GetType().GetProperty(nameof(ApprovalBaseModel.ApprovalStatus));
                    //    statusInfo?.SetValue(newEntity, ApprovalStatus.Modified);

                    //    var newPropInfo = newEntity.GetType().GetProperty(nameof(BaseModel.DateCreated));
                    //    newPropInfo?.SetValue(newEntity, new DateTimeOffset(new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

                    //    var newPropUser = newEntity.GetType().GetProperty(nameof(BaseModel.CreatedById));
                    //    newPropUser?.SetValue(newEntity, user?.Id);

                    //    Add(newEntity);
                    //}

                    var lastPropInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.LastUpdated));
                    var lastPropUser = item.Entity.GetType().GetProperty(nameof(BaseModel.UpdatedById));

                    lastPropInfo?.SetValue(item.Entity, new DateTimeOffset(new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
                    lastPropUser?.SetValue(item.Entity, user?.Id);

                    aprStatProp?.SetValue(item.Entity, ApprovalStatus.Modified);
                }
                else if (item.State == EntityState.Added)
                {
                    // enforce that every new entity of type IApprovableEntity is created with a PendingApproval state
                    // even though the creator might have set it different.
                    // if the property is not found probably its not an IApprovableEntity
                    var apState = item.Entity.GetType().GetProperty(nameof(ApprovalBaseModel.ApprovalStatus));
                    apState?.SetValue(item.Entity, ApprovalStatus.New);

                    // update audit fields

                    var newPropInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.DateCreated));
                    newPropInfo?.SetValue(item.Entity, new DateTimeOffset(new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

                    //var concurrencyInfo = item.Entity.GetType().GetProperty(nameof(BaseModel.ConcurrencyStamp));
                    //concurrencyInfo?.SetValue(item.Entity, Guid.NewGuid().ToString("N").ToUpper());

                    var newPropUser = item.Entity.GetType().GetProperty(nameof(BaseModel.CreatedById));
                    newPropUser?.SetValue(item.Entity, user?.Id);
                }
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
