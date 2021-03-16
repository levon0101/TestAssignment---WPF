using Autofac;
using ProgramB.Repositories;
using ProgramB.Services;
using ProgramB.ViewModel;

namespace ProgramB.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AppDbContext>().AsSelf();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();


            builder.RegisterType<HandRepository>().As<IHandRepository>();
            builder.RegisterType<HandFromFileService>().As<IHandFromFileService>();

            return builder.Build();
        }
    }
}
