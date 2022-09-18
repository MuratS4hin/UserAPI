using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UserApi.Repository
{
    public abstract class BaseServiceRepository<T> where T : class
    {

        private readonly IMongoCollection<T> _collection;

        public BaseServiceRepository(DBContext context, object collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }
        
        public List<T> Find()
        {
            return _collection.Find(new BsonDocument()).ToList();
        }
        
        public T FindById(Expression<Func<T,bool>> expression)
        {
            return _collection.Find(expression).FirstOrDefault();
        }

        public T Create(T document)
        {
            _collection.InsertOne(document);
            return document;
        }
        
        public void Update(Expression<Func<T, bool>> expression, Expression<Func<T, object>> field, object value)
        {
            var filter = Builders<T>.Filter.Where(expression);
            var updateDefinition = Builders<T>.Update.Set(field, value);
            _collection.UpdateOne(filter, updateDefinition);
        }
        
        public T ReplaceOne(T newDocument, Expression<Func<T,bool>> expression)
        {
            var filter = Builders<T>.Filter.Where(expression);
            _collection.DeleteOne(filter);
            _collection.InsertOne(newDocument);
            return newDocument;
        }
        
        public T Delete(Expression<Func<T,bool>> expression)
        {
            var filter = Builders<T>.Filter.Where(expression);
            return _collection.FindOneAndDelete(filter);
        }
    }
}