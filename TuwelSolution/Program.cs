using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using ServiceLibrary;
using TuwelSolution.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter();
    opts.JsonSerializerOptions.Converters.Add(enumConverter);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

//MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDatabase"));
ConventionRegistry.Register("Ignore", 
    new ConventionPack 
    { 
        new IgnoreIfNullConvention(true) 
    }, 
    _ => true);

builder.Services.AddScoped<ISolutionService, SolutionService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();