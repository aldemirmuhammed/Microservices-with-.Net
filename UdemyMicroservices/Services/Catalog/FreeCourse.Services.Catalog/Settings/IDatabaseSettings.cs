namespace FreeCourse.Services.Catalog.Settings
{
    public interface IDatabaseSettings
    {

        public string CourseCatologName { get; set; }
        public string CategoryCatologName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabseName { get; set; }
    }
}
