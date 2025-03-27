using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceScopeMediator.Sample.Components;
using ServiceScopeMediator.Sample.Data;
using ServiceScopeMediator.Sample.Model;
using ServiceScopeMediator.Sample.PipelineBehaviors;
using ServiceScopeMediator.Sample.ScopedMediator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ScopedMediator>(sp => (ScopedMediator)sp.GetRequiredService<IMediator>());
builder.Services.AddMediatR(options =>
{
    options.MediatorImplementationType = typeof(ScopedMediator);
    options.Lifetime = ServiceLifetime.Scoped;
    options.RegisterServicesFromAssemblyContaining<ScopedMediator>();
    options.AddOpenBehavior(typeof(TransactionBehavior<,>));
});
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=app.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

if (builder.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var stuffCount = await dbContext.Stuffs.CountAsync();
    if (stuffCount == 0)
    {
        var stuff1 = new Stuff
        {
            Name = "Stuff 1"
        };
        var stuff2 = new Stuff()
        {
            Name = "Stuff 2"
        };

        await dbContext.Stuffs.AddRangeAsync(stuff1, stuff2);
        await dbContext.SaveChangesAsync();
    }
}

app.Run();
