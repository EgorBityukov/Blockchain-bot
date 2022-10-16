using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using mm_bot;
using mm_bot.Mapper;
using mm_bot.Models;
using mm_bot.Services;
using mm_bot.Services.Interfaces;
using mmTransactionDB.DataAccess;
using mmTransactionDB.Repository;
using mmTransactionDB.Repository.Interfaces;

var config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .Build();

string connectionString = config.GetConnectionString("SqlConnectionString");
string cryptoApiUrl = config.GetConnectionString("CryptoApiUrl");

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddAutoMapper(typeof(WalletProfile));

        services.AddDbContext<mmTransactionDBContext>(
        options => options.UseNpgsql(connectionString));

        services.AddHttpClient("CryptoClient", config =>
        {
            config.BaseAddress = new Uri(cryptoApiUrl);
            config.Timeout = new TimeSpan(0, 0, 30);
            config.DefaultRequestHeaders.Clear();
        });

        services.AddTransient<ICommandService, CommandService>();
        services.AddTransient<IWalletRepository, WalletRepository>();
        services.AddTransient<IWalletService, WalletService>();
        services.AddTransient<ICryptoService, CryptoService>();
        services.AddTransient<ITransactionService, TransactionService>();

        services.Configure<ConfigSettings>(config.GetSection("Settings"));
        services.AddTransient<ConfigSettings>(_ => _.GetRequiredService<IOptions<ConfigSettings>>().Value);
    })
    .Build();

await host.RunAsync();
