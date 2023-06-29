using aims_api.Cores.Implementation;
using aims_api.Cores.Interface;
using aims_api.Repositories.Implementation;
using aims_api.Repositories.Interface;
using aims_api.Repositories.Sub;
using aims_api.Utilities;
using aims_api.Utilities.Interface;
using aims_printsvc.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using Serilog;
using System.ComponentModel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial; //EPPlus

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// my custom services starts here...

// allow CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
                .AllowAnyMethod() //allow any http methods
                .SetIsOriginAllowed(isOriginAllowed: _ => true) //no restriction in any domain
                .AllowCredentials();
    });
});

// make appsettings service
var appSettings = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettings);

var appSettingsDB = builder.Configuration.GetSection("AppSettingsDB");
builder.Services.Configure<AppSettingsDB>(appSettingsDB);


// database connection service
//builder.Services.AddTransient(_ => new MySqlDatabase());
//builder.Services.AddTransient(_ => new DBConnection());

// multitenancy references/services required
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ITenantSource, TenantSource>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// configure jwt authentication
var appSett = appSettings.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSett.Key ?? "");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// serilog rolling logger setup
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"logs\\logfile_.log",
                    rollingInterval: RollingInterval.Minute,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 20000000)
                .CreateLogger();

//// serilog rolling logger of all status
//string basedir = AppDomain.CurrentDomain.BaseDirectory;
//builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.File(path: $"{basedir}logs\\skilliam.log",
//                                                    rollingInterval: RollingInterval.Minute,
//                                                    rollOnFileSizeLimit: true,
//                                                    fileSizeLimitBytes: 20000000));

// dependency injection
#region Dependency Injections
builder.Services.AddTransient<ITransactionTypeCore, TransactionTypeCore>();
builder.Services.AddTransient<ITransactionTypeRepository, TransactionTypeRepository>();
builder.Services.AddTransient<IAccessRightCore, AccessRightCore>();
builder.Services.AddTransient<IAccessRightRepository, AccessRightRepository>();
builder.Services.AddTransient<IAccessRightDetailCore, AccessRightDetailCore>();
builder.Services.AddTransient<IAccessRightDetailRepository, AccessRightDetailRepository>();
builder.Services.AddTransient<IAreaCore, AreaCore>();
builder.Services.AddTransient<IAreaRepository, AreaRepository>();
builder.Services.AddTransient<IConfigCore, ConfigCore>();
builder.Services.AddTransient<IConfigRepository, ConfigRepository>();
builder.Services.AddTransient<IAuditTrailCore, AuditTrailCore>();
builder.Services.AddTransient<IAuditTrailRepository, AuditTrailRepository>();
builder.Services.AddTransient<IIdNumberCore, IdNumberCore>();
builder.Services.AddTransient<IIdNumberRepository, IdNumberRepository>();
builder.Services.AddTransient<IInvAdjustLineStatusCore, InvAdjustLineStatusCore>();
builder.Services.AddTransient<IInvAdjustLineStatusRepository, InvAdjustLineStatusRepository>();
builder.Services.AddTransient<IInvAdjustStatusCore, InvAdjustStatusCore>();
builder.Services.AddTransient<IInvAdjustStatusRepository, InvAdjustStatusRepository>();
builder.Services.AddTransient<IInvCountLineStatusCore, InvCountLineStatusCore>();
builder.Services.AddTransient<IInvCountLineStatusRepository, InvCountLineStatusRepository>();
builder.Services.AddTransient<IInvCountStatusCore, InvCountStatusCore>();
builder.Services.AddTransient<IInvCountStatusRepository, InvCountStatusRepository>();
builder.Services.AddTransient<IInventoryCore, InventoryCore>();
builder.Services.AddTransient<IInventoryRepository, InventoryRepository>();
builder.Services.AddTransient<IInventoryHistoryCore, InventoryHistoryCore>();
builder.Services.AddTransient<IInventoryHistoryRepository, InventoryHistoryRepository>();
builder.Services.AddTransient<IInventoryStatusCore, InventoryStatusCore>();
builder.Services.AddTransient<IInventoryStatusRepository, InventoryStatusRepository>();

builder.Services.AddTransient<IInvMoveCore, InvMoveCore>();
builder.Services.AddTransient<IInvMoveRepository, InvMoveRepository>();
builder.Services.AddTransient<IInvMoveDetailCore, InvMoveDetailCore>();
builder.Services.AddTransient<IInvMoveDetailRepository, InvMoveDetailRepository>();
builder.Services.AddTransient<IInvMoveLineStatusCore, InvMoveLineStatusCore>();
builder.Services.AddTransient<IInvMoveLineStatusRepository, InvMoveLineStatusRepository>();
builder.Services.AddTransient<IInvMoveStatusCore, InvMoveStatusCore>();
builder.Services.AddTransient<IInvMoveStatusRepository, InvMoveStatusRepository>();
builder.Services.AddTransient<IInvMoveUserFieldCore, InvMoveUserFieldCore>();
builder.Services.AddTransient<IInvMoveUserFieldRepository, InvMoveUserFieldRepository>();

builder.Services.AddTransient<IInvAdjustmentCore, InvAdjustmentCore>();
builder.Services.AddTransient<IInvAdjustmentRepository, InvAdjustmentRepository>();

builder.Services.AddTransient<ILocationCore, LocationCore>();
builder.Services.AddTransient<ILocationRepository, LocationRepository>();
builder.Services.AddTransient<ILocationGroupCore, LocationGroupCore>();
builder.Services.AddTransient<ILocationGroupRepository, LocationGroupRepository>();
builder.Services.AddTransient<ILocationTypeCore, LocationTypeCore>();
builder.Services.AddTransient<ILocationTypeRepository, LocationTypeRepository>();
builder.Services.AddTransient<ILotAttributeDetailCore, LotAttributeDetailCore>();
builder.Services.AddTransient<ILotAttributeDetailRepository, LotAttributeDetailRepository>();
builder.Services.AddTransient<IModuleCore, ModuleCore>();
builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
builder.Services.AddTransient<IMovementTypeCore, MovementTypeCore>();
builder.Services.AddTransient<IMovementTypeRepository, MovementTypeRepository>();
builder.Services.AddTransient<IOrganizationCore, OrganizationCore>();
builder.Services.AddTransient<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddTransient<IOrganizationTypeCore, OrganizationTypeCore>();
builder.Services.AddTransient<IOrganizationTypeRepository, OrganizationTypeRepository>();
builder.Services.AddTransient<IPackageTypeCore, PackageTypeCore>();
builder.Services.AddTransient<IPackageTypeRepository, PackageTypeRepository>();
builder.Services.AddTransient<IPagingRepository, PagingRepository>();

builder.Services.AddTransient<IPOCore, POCore>();
builder.Services.AddTransient<IPORepository, PORepository>();
builder.Services.AddTransient<IPODetailCore, PODetailCore>();
builder.Services.AddTransient<IPODetailRepository, PODetailRepository>();
builder.Services.AddTransient<IPOLineStatusCore, POLineStatusCore>();
builder.Services.AddTransient<IPOLineStatusRepository, POLineStatusRepository>();
builder.Services.AddTransient<IPOStatusCore, POStatusCore>();
builder.Services.AddTransient<IPOStatusRepository, POStatusRepository>();
builder.Services.AddTransient<IPOUserFieldCore, POUserFieldCore>();
builder.Services.AddTransient<IPOUserFieldRepository, POUserFieldRepository>();

builder.Services.AddTransient<IPrintHelperRepository, PrintHelperRepository>();
builder.Services.AddTransient<IPrintHelperCore, PrintHelperCore>();
builder.Services.AddTransient<IProductCore, ProductCore>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IProductCategoryCore, ProductCategoryCore>();
builder.Services.AddTransient<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddTransient<IProductConditionCore, ProductConditionCore>();
builder.Services.AddTransient<IProductConditionRepository, ProductConditionRepository>();
builder.Services.AddTransient<IProductPricingRepository, ProductPricingRepository>();
builder.Services.AddTransient<IProductUserFieldCore, ProductUserFieldCore>();
builder.Services.AddTransient<IProductUserFieldRepository, ProductUserFieldRepository>();
builder.Services.AddTransient<IPutawayTaskCore, PutawayTaskCore>();
builder.Services.AddTransient<IPutawayTaskRepository, PutawayTaskRepository>();
builder.Services.AddTransient<IReceivingCore, ReceivingCore>();
builder.Services.AddTransient<IReceivingRepository, ReceivingRepository>();
builder.Services.AddTransient<IReferredTagsCore, ReferredTagsCore>();
builder.Services.AddTransient<IReferredTagsRepository, ReferredTagsRepository>();
builder.Services.AddTransient<IRetReceivingCore, RetReceivingCore>();
builder.Services.AddTransient<IRetReceivingRepository, RetReceivingRepository>();
builder.Services.AddTransient<IReturnsCore, ReturnsCore>();
builder.Services.AddTransient<IReturnsRepository, ReturnsRepository>();
builder.Services.AddTransient<IReturnsDetailCore, ReturnsDetailCore>();
builder.Services.AddTransient<IReturnsDetailRepository, ReturnsDetailRepository>();
builder.Services.AddTransient<IReturnsUserFieldCore, ReturnsUserFieldCore>();
builder.Services.AddTransient<IReturnsUserFieldRepository, ReturnsUserFieldRepository>();
builder.Services.AddTransient<IRunningBalanceCore, RunningBalanceCore>();
builder.Services.AddTransient<IRunningBalanceRepository, RunningBalanceRepository>();
builder.Services.AddTransient<IShippingLoadStatusCore, ShippingLoadStatusCore>();
builder.Services.AddTransient<IShippingLoadStatusRepository, ShippingLoadStatusRepository>();
builder.Services.AddTransient<ISOCore, SOCore>();
builder.Services.AddTransient<ISORepository, SORepository>();
builder.Services.AddTransient<ISODetailCore, SODetailCore>();
builder.Services.AddTransient<ISODetailRepository, SODetailRepository>();
builder.Services.AddTransient<ISOLineStatusCore, SOLineStatusCore>();
builder.Services.AddTransient<ISOLineStatusRepository, SOLineStatusRepository>();
builder.Services.AddTransient<ISOStatusCore, SOStatusCore>();
builder.Services.AddTransient<ISOStatusRepository, SOStatusRepository>();
builder.Services.AddTransient<ISOTypeCore, SOTypeCore>();
builder.Services.AddTransient<ISOTypeRepository, SOTypeRepository>();
builder.Services.AddTransient<ISOUserFieldCore, SOUserFieldCore>();
builder.Services.AddTransient<ISOUserFieldRepository, SOUserFieldRepository>();
builder.Services.AddTransient<IUniqueTagsCore, UniqueTagsCore>();
builder.Services.AddTransient<IUniqueTagsRepository, UniqueTagsRepository>();
builder.Services.AddTransient<IUserAccountCore, UserAccountCore>();
builder.Services.AddTransient<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddTransient<IUsrAccUserFieldRepository, UsrAccUserFieldRepository>();
builder.Services.AddTransient<IUtilitiesCore, UtilitiesCore>();
builder.Services.AddTransient<IUtilitiesRepository, UtilitiesRepository>();
builder.Services.AddTransient<IVehicleTypeCore, VehicleTypeCore>();
builder.Services.AddTransient<IVehicleTypeRepository, VehicleTypeRepository>();
builder.Services.AddTransient<IWarehouseInfoCore, WarehouseInfoCore>();
builder.Services.AddTransient<IWarehouseInfoRepository, WarehouseInfoRepository>();
builder.Services.AddTransient<IWhTransferCore, WhTransferCore>();
builder.Services.AddTransient<IWhTransferRepository, WhTransferRepository>();
builder.Services.AddTransient<IWhTransferDetailCore, WhTransferDetailCore>();
builder.Services.AddTransient<IWhTransferDetailRepository, WhTransferDetailRepository>();
builder.Services.AddTransient<IWhTransReceivingCore, WhTransReceivingCore>();
builder.Services.AddTransient<IWhTransReceivingRepository, WhTransReceivingRepository>();
builder.Services.AddTransient<IWhTransUserFieldCore, WhTransUserFieldCore>();
builder.Services.AddTransient<IWhTransUserFieldRepository, WhTransUserFieldRepository>();

// enum description helper
builder.Services.AddSingleton<EnumHelper>(new EnumHelper());
builder.Services.AddTransient<PrintHelper>();
builder.Services.AddSingleton<PasswordHash>(new PasswordHash());
builder.Services.AddSingleton<EPCConverter>(new EPCConverter());


// declaration of subtitute classes services
builder.Services.AddTransient<PORepoSub>();
builder.Services.AddTransient<ReceivingRepoSub>();
builder.Services.AddTransient<ReturnsRepoSub>();
builder.Services.AddTransient<WhTransferRepoSub>();
builder.Services.AddTransient<PutawayTaskRepoSub>();

// mycustom services end here...
#endregion

var app = builder.Build();

// implement CORS policy
app.UseCors("ClientPermission");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// for jwt token auth
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

// replaced with code below to allow jwt auth
//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// disable on production builds - JADC
app.UseDeveloperExceptionPage();

app.Run();
