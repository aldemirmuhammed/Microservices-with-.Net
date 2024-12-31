namespace FreeCourse.Services.Catalog.Settings.Database
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string CourseCollectionName { get; set; }
        public string CategoryCollectionName { get; set; }
        public string FavoriteCourseCollectionName { get; set; }
        public string CourseCommentCollectionName { get; set; }
        public string CourseQuestionsCollectionName { get; set; }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
