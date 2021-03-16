using Autofac;
using ProgramA.Services;
using ProgramA.ViewModel;

namespace ProgramA.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

          
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();

            builder.RegisterType<EmbeddedFileService>().As<IEmbeddedFileService>();

            return builder.Build();
        }
    }
}
