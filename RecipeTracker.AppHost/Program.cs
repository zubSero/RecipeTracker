var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.RecipeTracker_ApiService>("apiservice");

var postgres = builder.AddPostgres("postgres").WithPgWeb(pgWeb => pgWeb.WithHostPort(8081));
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.RecipeTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.Build().Run();
