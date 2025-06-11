using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Base.Service.MongoDB;
using Play.Catalog.Service;
using Play.Catalog.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

//Configure GuidRepresentation
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Add services to the container.
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddMongo().AddMongoRepository<Category>("Categories");
builder.Services.AddMongo().AddMongoRepository<Product>("Products");
builder.Services.AddHostedService<ConsumeRabbitMQHostedServices>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
