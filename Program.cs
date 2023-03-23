using Microsoft.EntityFrameworkCore;
using ZetaTrading.Data;
using ZetaTrading.Models;
using ZetaTrading.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<TreeService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    CheckDB();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void CheckDB()
{
    using (var context = new ApplicationDbContext())
    {
        if (!context.Database.CanConnect())
        {
            Console.WriteLine($"Database with name TestJobBD11 does not exist. Creating database...");
            context.Database.EnsureCreated();
        }
        else if (!context.Nodes.Any())
        {
            var nodeConfigurations = new List<NodeConfiguration>
            {
                new NodeConfiguration { Name = "Node 1.1", Description = "Description 1", Tree = new Tree { Id = 1, Name = "Tree 1" } }, //1
                new NodeConfiguration { Name = "Node 1.2", Description = "Description 2", ParentId = 1, Tree = new Tree { Id = 1, Name = "Tree 1" } }, //2
                new NodeConfiguration { Name = "Node 1.3", Description = "Description 3", ParentId = 2, Tree = new Tree { Id = 1, Name = "Tree 1" } },//3
                new NodeConfiguration { Name = "Node 1.4", Description = "Description 4", ParentId = 3, Tree = new Tree { Id = 1, Name = "Tree 1" } },//4
                new NodeConfiguration { Name = "Node 1.5", Description = "Description 5", ParentId = 4, Tree = new Tree { Id = 1, Name = "Tree 1" } },//5
                new NodeConfiguration { Name = "Node 1.2.1", Description = "Description 6", ParentId = 3, Tree = new Tree { Id = 2, Name = "Tree 2" } },//6
                new NodeConfiguration { Name = "Node 1.2.2", Description = "Description 7", ParentId = 6, Tree = new Tree { Id = 2, Name = "Tree 2" } },//7
                new NodeConfiguration { Name = "Node 1.2.3", Description = "Description 8", ParentId = 7, Tree = new Tree { Id = 2, Name = "Tree 2" } },//8
                new NodeConfiguration { Name = "Node 1.5.1", Description = "Description 9", ParentId = 5, Tree = new Tree { Id = 3, Name = "Tree 3" } },//9
                new NodeConfiguration { Name = "Node 2.1", Description = "Description 10", Tree = new Tree { Id = 4, Name = "Tree 4" } },//10
                new NodeConfiguration { Name = "Node 2.2", Description = "Description 11", ParentId = 10, Tree = new Tree { Id = 4, Name = "Tree 4" } },//9
                // add more configurations
            };

            var treeService = new TreeService(context);
            treeService.CreateNodes(nodeConfigurations);

            Console.WriteLine("Database created and initial data added successfully.");
        }
    }
}

