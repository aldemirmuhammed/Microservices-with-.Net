using AutoMapper;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    internal class CategoryService
    {

        private readonly IMongoCollection<Category> _categoryCollection;

        private readonly IMapper _maper;

        public CategoryService(IMongoCollection<Category> categoryCollection, IMapper maper,IDatabaseSettings databaseSettings)
        {


            var client = new MongoClient(databaseSettings.ConnectionString);

            var database = client.GetDatabase(databaseSettings.DatabseName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCatologName);
             

            this.categoryCollection = categoryCollection;
            this.maper = maper;
        }
    }
}
