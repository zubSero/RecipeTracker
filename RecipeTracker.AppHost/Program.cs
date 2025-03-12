var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres").WithPgAdmin();
var postgresdb = postgres.WithDataVolume().AddDatabase("postgresdb");

var apiService = builder.AddProject<Projects.RecipeTracker_ApiService>("apiservice");

builder.AddProject<Projects.RecipeTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.Build().Run();
