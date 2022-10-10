using Domain.Setting.Interface;

namespace Domain.Setting
{
    public class ProjectSettings : IProjectSettings
    {
        public string DbConnectionString { get; set; }
    }
}
