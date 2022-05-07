using MongoDB.Bson.Serialization.Conventions;
using ServiceLibrary;
using TuwelSolution.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();