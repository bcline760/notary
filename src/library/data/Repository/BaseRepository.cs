﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

using Notary.Contract;
using Notary.Model;
using Notary.Interface.Repository;
using MongoDB.Driver;

namespace Notary.Data.Repository
{
    public abstract class BaseRepository<TC, TM> : IRepository<TC>
        where TC : Entity, new()
        where TM : BaseModel, new()
    {
        public IMongoCollection<TM> Collection { get; private set; }

        public IndexKeysDefinitionBuilder<TM> IndexKeys { get => Builders<TM>.IndexKeys; } 

        protected BaseRepository(IMongoDatabase db)
        {
            var type = typeof(TM);
            var attribute = type.GetCustomAttribute<CollectionAttribute>();
            if (attribute != null)
            {
                Collection = db.GetCollection<TM>(attribute.Name);
            }
            else
            {
                string collectionName = GetCollectionName();
                Collection = db.GetCollection<TM>(collectionName);
            }

            var slugIndex = new CreateIndexModel<TM>(IndexKeys.Ascending(t => t.Slug));
            Task.Run(async () => await Collection.Indexes.CreateOneAsync(slugIndex));
        }

        public virtual async Task DeleteAsync(string slug, string updatedBySlug)
        {
            var record = await GetAsync(slug);

            if (record != null)
            {
                record.Active = false;
                record.Updated = DateTime.Now;
                record.UpdatedBySlug = updatedBySlug;

                await SaveAsync(record);
            }
        }

        public virtual async Task DeleteByIdAsync(string id, string updatedBySlug)
        {
            var contract = await GetAsync(id);
            contract.Active = false;
            contract.UpdatedBySlug = updatedBySlug;
            contract.Updated = DateTime.UtcNow;

            await SaveAsync(contract);
        }

        public virtual async Task<List<TC>> GetAllAsync()
        {
            var filter = Builders<TM>.Filter.Empty;

            var contract = new List<TC>();
            using (var collection = await Collection.FindAsync(filter))
            {
                var models = await collection.ToListAsync();
                
                contract.AddRange(models.Select(m => (TC)Activator.CreateInstance(typeof(TC), m)));
            }
            return contract;
        }

        public virtual async Task<TC> GetAsync(string slug)
        {
            var filter = Builders<TM>.Filter.Eq("slug", slug);

            return await RunQuery(filter);
        }

        public virtual async Task<TC> GetByIdAsync(string id)
        {
            var filter = Builders<TM>.Filter.Eq("id", id);
            return await RunQuery(filter);
        }

        public virtual async Task SaveAsync(TC entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrEmpty(entity.Slug))
            {
                entity.Slug = entity.Slugify();
            }

            var model = (TM)Activator.CreateInstance(typeof(TM), entity);
            var filter = Builders<TM>.Filter.Eq("slug", model.Slug);
            var result = await Collection.ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = true });

            if (!result.IsAcknowledged)
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Get the name of the collection based off the model. Will appropriatly pluralize
        /// </summary>
        /// <typeparam name="TModel">The type of the model to use</typeparam>
        /// <returns>The name of the collection</returns>
        protected string GetCollectionName()
        {
            Type modelType = typeof(TM);
            string modelName = modelType.Name.Replace("Model", string.Empty);
            string collectionName = string.Empty;
            char endingCharacter = modelName[modelName.Length - 1];

            //Be smart about English
            if (char.ToLowerInvariant(endingCharacter) == 'y')
            {
                char nextChar = modelName[modelName.Length - 2];
                char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
                if (vowels.Any(v => v == nextChar))
                    collectionName = string.Concat(modelName, 's'); //bays, toys, keys
                else
                    collectionName = string.Concat(modelName[0..^1], "ies"); //histories, flies, countries, etc.
            }
            else if (char.ToLowerInvariant(endingCharacter) == 'o') //Gonna have the odd case here...pianoes
            {
                collectionName = string.Concat(modelName, "es");
            }
            else
                collectionName = string.Concat(modelName, 's'); //Bows, Arrows

            Regex s_seperateWordRegex =
                            new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
            return collectionName.ToLowerInvariant();
        }

        protected async Task<TC> RunQuery(FilterDefinition<TM> filter)
        {
            var result = await Collection.FindAsync(filter);
            var doc = await result.FirstOrDefaultAsync();
            if (doc == null)
                return null;

            var obj = (TC)Activator.CreateInstance(typeof(TC), doc);
            return obj;
        }
    }
}
