using ToDoList.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.interfaces;
using ToDoList.Infrastructure.Repositories;
using ToDoList.Application.Interfaces;
using ToDoList.Application.Services;
using ToDoList.Application.Mappings;
using ToDoList.Domain.Interfaces;

namespace ToDoList.Infrastructure;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IListaRepository, ListaRepository>();
        services.AddScoped<IListaService, ListaService>();
        services.AddScoped<ITarefaRepository, TarefaRepository>();
        services.AddScoped<ITarefaService, TarefaService>();
        services.AddScoped<ICacheRepository, RedisRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISenhaService, SenhaService>();

        services.AddAutoMapper(typeof(DomainToDtoMapping),
            typeof(DtoToDomainMapping));

        services.AddDistributedRedisCache(o =>
        {
            o.InstanceName = "instance";
            o.Configuration = "localhost:6379";
        });

    }
}